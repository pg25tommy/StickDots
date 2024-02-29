using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Core.Editor.Environments;
using Unity.Services.DeploymentApi.Editor;
using Unity.Services.Leaderboards.Authoring.Core.Deploy;
using Unity.Services.Leaderboards.Authoring.Core.Model;
using Unity.Services.Leaderboards.Authoring.Core.Service;
using Unity.Services.Leaderboards.Editor.Authoring.Analytics;
using Unity.Services.Leaderboards.Editor.Authoring.Model;
using Unity.Services.Leaderboards.Logging;
using UnityEditor;

namespace Unity.Services.Leaderboards.Editor.Authoring.Deployment
{
    class DeployCommand : Command<EditorLeaderboardConfig>
    {
        const string k_AnalyticsEventDeploy = "leaderboards_file_deployed";

        readonly ILeaderboardsDeploymentHandler m_DeploymentHandler;
        readonly ILeaderboardsClient m_Client;
        readonly IEnvironmentsApi m_EnvironmentsApi;
        readonly ILeaderboardsEditorAnalytics m_EditorAnalytics;

        public override string Name => L10n.Tr("Deploy");

        public DeployCommand(
            ILeaderboardsDeploymentHandler moduleDeploymentHandler,
            ILeaderboardsClient client,
            IEnvironmentsApi environmentsApi,
            ILeaderboardsEditorAnalytics analytics)
        {
            m_DeploymentHandler = moduleDeploymentHandler;
            m_Client = client;
            m_EnvironmentsApi = environmentsApi;
            m_EditorAnalytics = analytics;
        }

        public override async Task ExecuteAsync(IEnumerable<EditorLeaderboardConfig> items, CancellationToken cancellationToken = default)
        {
            var itemList = items.ToList();
            string message = default;
            try
            {
                OnPreDeploy(itemList);
                itemList.ForEach(item => item.FromJsonFile(item.Path));
                itemList = itemList.Where(i => i.Status.MessageSeverity != SeverityLevel.Error).ToList();
                m_Client.Initialize(m_EnvironmentsApi.ActiveEnvironmentId.ToString(), CloudProjectSettings.projectId, cancellationToken);
                await m_DeploymentHandler.DeployAsync(itemList, false, false, cancellationToken);
            }
            catch (Exception e)
            {
                message = e?.InnerException?.Message ?? e?.Message;
                throw;
            }
            finally
            {
                itemList.ForEach(i =>
                {
                    m_EditorAnalytics.SendEvent(
                        k_AnalyticsEventDeploy,
                        i.Type,
                        default,
                        message ?? (i.Status.MessageSeverity == SeverityLevel.Success
                            ? default
                            : i.Status.Message)
                    );
                });
            }
        }

        static void OnPreDeploy(IReadOnlyList<LeaderboardConfig> items)
        {
            foreach (var i in items)
            {
                i.Progress = 0f;
                i.Status = new DeploymentStatus();
                i.States.Clear();
            }
        }
    }
}
