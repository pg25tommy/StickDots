using System;
using System.Collections.ObjectModel;
using Unity.Services.Core.Editor;
using Unity.Services.Core.Editor.Environments;
using UnityEditor;
using UnityEngine;
using Unity.Services.DeploymentApi.Editor;
using Unity.Services.Leaderboards.Authoring.Client;
using Unity.Services.Leaderboards.Authoring.Client.Apis.Default;
using Unity.Services.Leaderboards.Authoring.Client.ErrorMitigation;
using Unity.Services.Leaderboards.Authoring.Client.Http;
using Unity.Services.Leaderboards.DependencyInversion;
using Unity.Services.Leaderboards.Editor.Authoring.AdminApi;
using Unity.Services.Leaderboards.Authoring.Core.Deploy;
using Unity.Services.Leaderboards.Authoring.Core.Service;
using Unity.Services.Leaderboards.Editor.Authoring.Analytics;
using Unity.Services.Leaderboards.Editor.Authoring.Deployment;
using Unity.Services.Leaderboards.Editor.Authoring.Model;
using Unity.Services.Leaderboards.Editor.Authoring.Shared.Analytics;
using static Unity.Services.Leaderboards.DependencyInversion.Factories;

namespace Unity.Services.Leaderboards.Editor.Authoring
{
    class LeaderboardAuthoringServices : AbstractRuntimeServices<LeaderboardAuthoringServices>
    {
        [InitializeOnLoadMethod]
        static void Initialize()
        {
            Instance.Initialize(new ServiceCollection());
            var deploymentItemProvider = Instance.GetService<DeploymentProvider>();
            Deployments.Instance.DeploymentProviders.Add(deploymentItemProvider);
        }

        public override void Register(ServiceCollection collection)
        {
            collection.Register(Default<ICommonAnalytics, CommonAnalytics>);
#if UNITY_2023_2_OR_NEWER
            collection.Register(Default<ICommonAnalyticProvider, CommonAnalyticProvider>);
#endif
            collection.Register(Default<ILeaderboardsEditorAnalytics, LeaderboardsEditorAnalytics>);
            collection.Register(_ => new Configuration(null, null, null, null));
            collection.Register(Default<IRetryPolicyProvider, RetryPolicyProvider>);
            collection.Register(Default<IHttpClient, HttpClient>);
            collection.Register(Default<IAccessTokens, AccessTokens>);
            collection.RegisterSingleton(Default<ObservableCollection<IDeploymentItem>, ObservableLeaderboardConfigAssets>);
            collection.Register(col => (ObservableLeaderboardConfigAssets)col.GetService(typeof(ObservableCollection<IDeploymentItem>)));
            collection.Register(Default<DeployCommand>);
            collection.Register(Default<ILeaderboardsDeploymentHandler, LeaderboardsDeploymentHandler>);
            collection.Register(Default<ILeaderboardsClient, LeaderboardsClient>);
            collection.Register(Default<IDefaultApiClient, DefaultApiClient>);
            collection.Register(_ => EnvironmentsApi.Instance);
            collection.RegisterStartupSingleton(Default<DeploymentProvider, LeaderboardDeploymentProvider>);
        }
    }
}
