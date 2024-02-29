using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Matchmaker.Authoring.Core.ConfigApi;
using Unity.Services.Matchmaker.Authoring.Core.Model;
using PoolConfig = Unity.Services.Matchmaker.Authoring.Core.Model.QueueConfig.PoolConfig;
using Filter = Unity.Services.Matchmaker.Authoring.Core.Model.QueueConfig.FilteredPoolConfig.Filter;
using BasePoolConfig = Unity.Services.Matchmaker.Authoring.Core.Model.QueueConfig.BasePoolConfig;

namespace Unity.Services.Matchmaker.Authoring.Core.Deploy
{
    class MatchmakerQueueDeployer
    {
        readonly IConfigApiClient m_configApiClient;
        readonly CancellationToken m_ct;

        public MatchmakerQueueDeployer(IConfigApiClient configApiClient, CancellationToken ct)
        {
            m_configApiClient = configApiClient;
            m_ct = ct;
        }

        public async Task<QueueId> DeployQueue(QueueConfig targetQueue, QueueConfig originalQueue)
        {
            QueueId queueId;

            if (originalQueue != null)
            {
                queueId = originalQueue.Id;
                await m_configApiClient.UpdateQueue(queueId, targetQueue, m_ct);

                // Reconcile before deploying any of this queue's pool because pool name is unique for the whole queue
                await ReconcileQueuePools(queueId, targetQueue, originalQueue);
            }
            else
                queueId = await m_configApiClient.CreateQueue(targetQueue, m_ct);

            // Queue's Enabled is not actually part of Queue resource so we update it here
            await m_configApiClient.SetQueueEnabled(queueId, targetQueue.Enabled, m_ct);

            await DeployQueuePools(queueId, targetQueue, originalQueue);

            return queueId;
        }

        async Task ReconcileQueuePools(QueueId queueId, QueueConfig targetQueue, QueueConfig originalQueue)
        {
            // Default pool can only get created or updated, no need for reconcile
            // But still need to reconcile default pool's variants
            if (originalQueue.DefaultPool?.Variants != null)
            {
                foreach (var variantPool in originalQueue.DefaultPool.Variants)
                {
                    if (!targetQueue.DefaultPool?.Variants.Exists(p => p.Name.Equals(variantPool.Name)) ?? false)
                    {
                        await m_configApiClient.DeletePool(queueId, variantPool.Id, m_ct);
                    }
                }
            }

            // Reconcile filtered pools
            foreach (var originalFilteredPool in originalQueue.FilteredPools)
            {
                var targetFilteredPool =
                    targetQueue.FilteredPools.FirstOrDefault(p => p.Name.Equals(originalFilteredPool.Name));
                if (targetFilteredPool == null)
                    await m_configApiClient.DeletePool(queueId, originalFilteredPool.Id, m_ct);
                // Reconcile non-deleted filtered pool's variants
                else
                {
                    foreach (var originalVariantPool in originalFilteredPool.Variants)
                    {
                        if (!targetFilteredPool.Variants.Exists(p => p.Name.Equals(originalVariantPool.Name)))
                            await m_configApiClient.DeletePool(queueId, originalVariantPool.Id, m_ct);
                    }
                }
            }
        }

        async Task DeployQueuePools(QueueId queueId, QueueConfig targetQueue, QueueConfig originalQueue)
        {
            // Deploy default pool
            PoolId defaultPoolId = default;
            if (targetQueue.DefaultPool != null)
            {
                defaultPoolId = await DeployPool(
                    queueId, targetQueue.DefaultPool, originalQueue?.DefaultPool, default, default);

                // Deploy default pool's variants
                await DeployPoolVariants(queueId, defaultPoolId, targetQueue.DefaultPool, originalQueue?.DefaultPool);
            }

            // Deploy filtered pools
            var targetFilteredPoolIdsRanked = new List<PoolId>();
            foreach (var targetFilteredPool in targetQueue.FilteredPools)
            {
                var originalFilteredPool = originalQueue?.FilteredPools.FirstOrDefault(
                    poolDef => poolDef.Name.Equals(targetFilteredPool.Name));

                var filteredPoolId = await DeployPool(
                    queueId, targetFilteredPool, originalFilteredPool, targetFilteredPool.Filters, default);

                targetFilteredPoolIdsRanked.Add(filteredPoolId);

                // Deploy filtered pools variants
                await DeployPoolVariants(
                    queueId, filteredPoolId, targetFilteredPool, originalFilteredPool);
            }

            if (targetQueue.FilteredPools.Any())
            {
                targetFilteredPoolIdsRanked.Add(defaultPoolId);
                await m_configApiClient.UpdatePoolsRank(queueId, targetFilteredPoolIdsRanked, m_ct);
            }
        }

        async Task DeployPoolVariants(QueueId queueId, PoolId basePoolId, BasePoolConfig targetPool,
            BasePoolConfig originalPool)
        {
            foreach (var targetVariantPool in targetPool.Variants)
            {
                var originalVariantPool =
                    originalPool?.Variants.FirstOrDefault(poolDef => poolDef.Name.Equals(targetVariantPool.Name));
                await DeployPool(queueId, targetVariantPool, originalVariantPool, default, basePoolId);
            }
        }

        async Task<PoolId> DeployPool(
            QueueId queueId, PoolConfig targetPool, PoolConfig originalPool, List<Filter> filters, PoolId basePoolId)
        {
            PoolId poolId;

            if (originalPool != null)
            {
                poolId = originalPool.Id;
                await m_configApiClient.UpdatePool(queueId, poolId, targetPool, filters, m_ct);
            }
            else
                poolId = await m_configApiClient.CreatePool(queueId, targetPool, filters, basePoolId, m_ct);

            // Pool's Enabled is not actually part of Pool resource so we update it here
            await m_configApiClient.SetPoolEnabled(queueId, poolId, targetPool.Enabled, m_ct);
            return poolId;
        }
    }
}
