using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Matchmaker.Authoring.Core.Model;
using Unity.Services.Matchmaker.Authoring.Core.ConfigApi;
using Unity.Services.Matchmaker.Authoring.Core.Fetch;
using Unity.Services.Matchmaker.Authoring.Core.Validator;
using Unity.Services.Matchmaker.Authoring.Core.Parser;

namespace Unity.Services.Matchmaker.Authoring.Core.Deploy
{
    class MatchmakerDeployHandler : IMatchmakerDeployHandler
    {
        readonly IConfigApiClient m_configApiClient;
        readonly IMatchmakerConfigParser m_configParser;
        readonly IMatchmakerConfigValidator m_configValidator;
        readonly IDeepEqualityComparer m_deepEqualityComparer;

        public MatchmakerDeployHandler(IConfigApiClient configApiClient,
            IMatchmakerConfigParser configParser,
            IMatchmakerConfigValidator configConfigValidator,
            IDeepEqualityComparer deepEqualityComparer)
        {
            m_configApiClient = configApiClient;
            m_configParser = configParser;
            m_configValidator = configConfigValidator;
            m_deepEqualityComparer = deepEqualityComparer;
        }

        public async Task<DeployResult> DeployAsync(IReadOnlyList<string> filePaths,
            bool reconcile,
            bool dryRun,
            string projectId,
            string environmentId,
            CancellationToken ct = default)
        {
            var result = new DeployResult();

            // Parsing
            var parseResult = await m_configParser.Parse(filePaths, ct);
            result.Failed.AddRange(parseResult.Failed);
            if (!parseResult.Parsed.Any() && !reconcile)
                return result;

            // Validation
            FullMatchmakerConfig originalConfig;
            var restrictions = new ConfigRestrictions();

            result.AbortMessage = await m_configApiClient.Initialize(projectId, environmentId, ct);
            if (!string.IsNullOrWhiteSpace(result.AbortMessage))
                return result;

            try
            {
                originalConfig = await m_configApiClient.GetConfig(ct);
                if (originalConfig != null)
                    restrictions = await m_configApiClient.GetConfigSizeRestrictions(ct);
            }
            // Anything but 404 for GetConfig
            catch (ConfigApiException ex)
            {
                result.AbortMessage = $"Failed to reach matchmaker config api: {ex.Message}.";
                return result;
            }

            var validatedResult =
                await m_configValidator.Validate(parseResult.Parsed, originalConfig, restrictions, reconcile, ct);
            result.Failed.AddRange(validatedResult.Failed);

            // If (no config exists or we are not reconciling) and we have nothing to deploy, return
            if ((originalConfig == null || !reconcile)
                && validatedResult.EnvironmentConfig == null && !validatedResult.QueueConfigs.Any())
                return result;

            var queueConfigFiles = validatedResult.QueueConfigs;
            var envConfigFile = validatedResult.EnvironmentConfig;

            // envConfigFile.Content should be EnvironmentConfig (handled by parser)
            // If no envConfig to deploy, take a default envConfig
            var targetEnvConfig = envConfigFile?.Content as EnvironmentConfig ?? new EnvironmentConfig();

            var envConfigCreated = false;
            // Deploy envConfig first if it's the first deployment
            if (originalConfig == null)
            {
                if (!dryRun)
                {
                    try
                    {
                        await m_configApiClient.UpdateConfig(targetEnvConfig, ct);
                        envConfigCreated = true;
                    }
                    catch (ConfigApiException ex)
                    {
                        result.Failed.Add((envConfigFile, ex.Message));
                        result.AbortMessage = "Failed to create matchmaker config.";

                        return result;
                    }
                }

                if (envConfigFile != null)
                    result.Created.Add(envConfigFile);

                // DefaultQueueName is not deployed as part of EnvironmentConfig resource so don't set it
                originalConfig = new FullMatchmakerConfig();
            }

            // Queues reconcile
            if (reconcile)
            {
                var targetQueueNames = queueConfigFiles.Select(q =>
                {
                    // q.Content should be QueueConfig (handled by parser)
                    var queue = q.Content as QueueConfig;
                    return queue?.Name.ToString();
                }).ToList();

                foreach (var originalQueue in originalConfig.QueueConfigs)
                {
                    if (targetQueueNames.Contains(originalQueue.Name.ToString()))
                        continue;

                    var reconcileQueueResult = new MatchmakerConfigResource { Name = originalQueue.Name.ToString() };
                    try
                    {
                        if (!dryRun)
                            await m_configApiClient.DeleteQueue(originalQueue.Id, ct);
                        result.Deleted.Add(reconcileQueueResult);
                    }
                    catch (ConfigApiException ex)
                    {
                        result.Failed.Add((reconcileQueueResult,
                            $"Failed to delete queue {originalQueue.Name} as part of the reconcile: {ex.Message}"));
                    }
                }
            }

            // Queues deployment
            var mmQueueDeployer = new MatchmakerQueueDeployer(m_configApiClient, ct);

            // Try to get original default queue id
            var defaultQueueId = originalConfig.QueueConfigs.FirstOrDefault(queueConfig =>
                queueConfig.Name.Equals(originalConfig.EnvironmentConfig?.DefaultQueueName))?.Id ?? new QueueId();

            foreach (var queueConfigFile in queueConfigFiles)
            {
                // queueConfigFile.Content should be QueueConfig (handled by parser)
                var queueConfig = queueConfigFile.Content as QueueConfig;

                var originalQueue = originalConfig.QueueConfigs.FirstOrDefault(q => q.Name.Equals(queueConfig?.Name));
                try
                {
                    var queueId = new QueueId();
                    if (originalQueue != null)
                    {
                        if (m_deepEqualityComparer.IsDeepEqual(queueConfig, originalQueue))
                            continue;
                        if (!dryRun)
                            queueId = await mmQueueDeployer.DeployQueue(queueConfig, originalQueue);
                        result.Updated.Add(queueConfigFile);
                    }
                    else
                    {
                        if (!dryRun)
                            queueId = await mmQueueDeployer.DeployQueue(queueConfig, null);
                        result.Created.Add(queueConfigFile);
                    }

                    if (queueConfig.Name.Equals(targetEnvConfig.DefaultQueueName))
                        defaultQueueId = queueId;

                    if (!dryRun)
                        result.Authored.Add(queueConfigFile);
                }
                catch (ConfigApiException ex)
                {
                    result.Failed.Add((queueConfigFile, ex.Message));
                }
            }

            if (envConfigFile == null)
                return result;

            // EnvConfig update
            try
            {
                if (!m_deepEqualityComparer.IsDeepEqual(targetEnvConfig, originalConfig.EnvironmentConfig))
                {
                    if (envConfigCreated)
                        result.Created.Add(envConfigFile);
                    else
                        result.Updated.Add(envConfigFile);
                    if (!dryRun)
                    {
                        await m_configApiClient.UpdateConfig(targetEnvConfig, ct);

                        if (!defaultQueueId.IsEmpty())
                        {
                            if (targetEnvConfig.DefaultQueueName == null)
                                await m_configApiClient.SetQueueDefault(defaultQueueId, false, ct);
                            else
                                await m_configApiClient.SetQueueDefault(defaultQueueId, true, ct);
                        }

                        result.Authored.Add(envConfigFile);
                    }
                }
            }
            catch (ConfigApiException ex)
            {
                result.Failed.Add((envConfigFile, ex.Message));
            }

            return result;
        }
    }
}
