using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Unity.Services.Matchmaker.Authoring.Core.Deploy
{
    interface IMatchmakerDeployHandler
    {
        Task<DeployResult> DeployAsync(IReadOnlyList<string> filePaths, bool reconcile, bool dryRun, string projectId,
            string environmentId, CancellationToken ct = default);
    }
}
