using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Matchmaker.Authoring.Core.Model;
using PoolConfig = Unity.Services.Matchmaker.Authoring.Core.Model.QueueConfig.PoolConfig;
using Filter = Unity.Services.Matchmaker.Authoring.Core.Model.QueueConfig.FilteredPoolConfig.Filter;

namespace Unity.Services.Matchmaker.Authoring.Core.ConfigApi
{
    interface IConfigApiClient
    {
        Task<string> Initialize(string projectId, string environmentId, CancellationToken ct = default);

        Task<FullMatchmakerConfig> GetConfig(CancellationToken ct = default);

        Task<ConfigRestrictions> GetConfigSizeRestrictions(CancellationToken ct = default);

        Task UpdateConfig(EnvironmentConfig environmentConfig, CancellationToken ct = default);

        Task<QueueId> CreateQueue(QueueConfig queueConfig, CancellationToken ct = default);

        Task UpdateQueue(QueueId queueId, QueueConfig queueConfig, CancellationToken ct = default);

        Task DeleteQueue(QueueId queueId, CancellationToken ct = default);

        Task SetQueueDefault(QueueId queueId, bool isDefault, CancellationToken ct = default);

        Task SetQueueEnabled(QueueId queueId, bool isEnabled, CancellationToken ct = default);

        Task<PoolId> CreatePool(QueueId queueId, PoolConfig poolConfig, List<Filter> filters = default,
            PoolId basePoolId = default, CancellationToken ct = default);

        Task UpdatePool(QueueId queueId, PoolId poolId, PoolConfig poolConfig, List<Filter> filters = default,
            CancellationToken ct = default);

        Task DeletePool(QueueId queueId, PoolId poolId, CancellationToken ct = default);

        Task SetPoolEnabled(QueueId queueId, PoolId poolId, bool isEnabled, CancellationToken ct = default);

        Task UpdatePoolsRank(QueueId queueId, List<PoolId> poolIds, CancellationToken ct = default);
    }
}
