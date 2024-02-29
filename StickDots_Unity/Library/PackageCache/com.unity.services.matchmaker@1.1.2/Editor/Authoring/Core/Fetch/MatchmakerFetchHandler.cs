using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Matchmaker.Authoring.Core.ConfigApi;
using Unity.Services.Matchmaker.Authoring.Core.IO;
using Unity.Services.Matchmaker.Authoring.Core.Model;
using Unity.Services.Matchmaker.Authoring.Core.Parser;

namespace Unity.Services.Matchmaker.Authoring.Core.Fetch
{
    class MatchmakerFetchHandler : IMatchmakerFetchHandler
    {
        readonly IConfigApiClient m_configApiClient;
        readonly IFileSystem m_fileSystem;
        readonly IMatchmakerConfigParser m_configParser;
        readonly IDeepEqualityComparer m_deepEqualityComparer;

        public MatchmakerFetchHandler(IConfigApiClient configApiClient,
            IMatchmakerConfigParser configParser,
            IFileSystem fileSystem,
            IDeepEqualityComparer deepEqualityComparer)
        {
            m_configApiClient = configApiClient;
            m_configParser = configParser;
            m_fileSystem = fileSystem;
            m_deepEqualityComparer = deepEqualityComparer;
        }

        public async Task<FetchResult> FetchAsync(string rootDir,
            string projectId,
            string environmentId,
            bool reconcile,
            bool dryRun,
            CancellationToken ct = default)
        {
            var result = new FetchResult();

            if (string.IsNullOrEmpty(rootDir))
            {
                result.AbortMessage = "Root directory is empty";
                return result;
            }

            List<string> filePaths;
            try
            {
                filePaths = m_fileSystem.GetFiles(rootDir, $"*{IMatchmakerConfigParser.Extension}", true);
            }
            catch (IOException ioException)
            {
                result.AbortMessage =
                    new FileSystemException(rootDir, FileSystemException.Action.ListFiles, ioException.Message)
                        .ToString();
                return result;
            }
            catch (SystemException ex)
            {
                result.AbortMessage = $"Could not access {rootDir}: {ex.Message}";
                return result;
            }

            var parseResult = await m_configParser.Parse(filePaths, ct);

            result.Failed.AddRange(parseResult.Failed);
            if (!parseResult.Parsed.Any() && !reconcile)
                return result;

            await m_configApiClient.Initialize(projectId, environmentId, ct);

            FullMatchmakerConfig remoteConfig;
            try
            {
                remoteConfig = await m_configApiClient.GetConfig(ct);
            }
            // Anything but 404 for GetConfig
            catch (ConfigApiException ex)
            {
                result.AbortMessage = $"Failed to reach matchmaker config api: {ex.Message}.";
                return result;
            }

            if (remoteConfig == null)
            {
                result.AbortMessage = "No matchmaker config found for this environment.";
                return result;
            }

            // Updating/Deleting existing files
            foreach (var configFile in parseResult.Parsed)
            {
                IMatchmakerConfig remoteConfigFile = null;
                switch (configFile.Content)
                {
                    case QueueConfig localQueueConfig:
                        remoteConfigFile =
                            remoteConfig.QueueConfigs.FirstOrDefault(q => q.Name.Equals(localQueueConfig.Name));
                        break;
                    case EnvironmentConfig:
                        remoteConfigFile = remoteConfig.EnvironmentConfig;
                        break;
                }

                if (remoteConfigFile == null)
                {
                    if (!dryRun)
                    {
                        try
                        {
                            m_fileSystem.Delete(configFile.Path);
                        }
                        catch (IOException ex)
                        {
                            var fsException = new FileSystemException(configFile.Path,
                                FileSystemException.Action.Delete, ex.Message);
                            result.Failed.Add((configFile, fsException.ToString()));
                        }
                        catch (SystemException ex)
                        {
                            result.Failed.Add((configFile, ex.ToString()));
                        }
                    }

                    result.Deleted.Add(configFile);
                }
                else
                {
                    if (!m_deepEqualityComparer.IsDeepEqual(configFile.Content, remoteConfigFile))
                    {
                        configFile.Content = remoteConfigFile;
                        result.Updated.Add(configFile);
                    }

                    if (!dryRun)
                    {
                        var (authored, error) =
                            await m_configParser.SerializeToFile(configFile.Content, configFile.Path, ct);
                        if (!string.IsNullOrEmpty(error))
                            result.Failed.Add((configFile, error));
                        else if (authored)
                            result.Authored.Add(configFile);
                    }
                }
            }

            if (!reconcile)
                return result;

            MatchmakerConfigResource newConfigFile;
            foreach (var remoteQueueConfigFile in remoteConfig.QueueConfigs)
            {
                var localQueueConfigFile = parseResult.Parsed.Where(
                    localConfigFile => localConfigFile.Content is QueueConfig localQueueConfig &&
                                       localQueueConfig.Name.Equals(remoteQueueConfigFile.Name));

                // Create missing queue config files
                if (!localQueueConfigFile.Any())
                {
                    newConfigFile = new MatchmakerConfigResource
                    {
                        Name = remoteQueueConfigFile.Name.ToString(),
                        Path = $"{rootDir}/{remoteQueueConfigFile.Name}{IMatchmakerConfigParser.Extension}",
                        Content = remoteQueueConfigFile
                    };
                    if (!dryRun)
                    {
                        var (authored, error) =
                            await m_configParser.SerializeToFile(remoteQueueConfigFile, newConfigFile.Path, ct);
                        if (!string.IsNullOrEmpty(error))
                            result.Failed.Add((newConfigFile, error));
                        else if (authored)
                            result.Authored.Add(newConfigFile);
                    }

                    result.Created.Add(newConfigFile);
                }
            }

            // Creating new env config file if none exists
            var localEnvConfigFile =
                parseResult.Parsed.FirstOrDefault(localConfigFile => localConfigFile.Content is EnvironmentConfig);
            if (localEnvConfigFile != null)
                return result;

            newConfigFile = new MatchmakerConfigResource
            {
                Name = "EnvironmentConfig",
                Path = $"{rootDir}/EnvironmentConfig{IMatchmakerConfigParser.Extension}",
                Content = remoteConfig.EnvironmentConfig
            };
            if (!dryRun)
            {
                var (authored, error) =
                    await m_configParser.SerializeToFile(remoteConfig.EnvironmentConfig, newConfigFile.Path, ct);
                if (!string.IsNullOrEmpty(error))
                    result.Failed.Add((newConfigFile, error));
                else if (authored)
                    result.Authored.Add(newConfigFile);
            }

            result.Created.Add(newConfigFile);
            return result;
        }
    }
}
