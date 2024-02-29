using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Matchmaker.Authoring.Core.ConfigApi;
using Unity.Services.Matchmaker.Authoring.Core.Model;

namespace Unity.Services.Matchmaker.Authoring.Core.Validator
{
    interface IMatchmakerConfigValidator
    {
        class ValidatorResult
        {
            public List<(MatchmakerConfigResource resource, string detail)> Failed { get; } = new();
            public MatchmakerConfigResource EnvironmentConfig { get; set; } = new();
            public List<MatchmakerConfigResource> QueueConfigs { get; } = new();
        }

        Task<ValidatorResult> Validate(List<MatchmakerConfigResource> resources,
            FullMatchmakerConfig liveConfig,
            ConfigRestrictions restrictions,
            bool reconcile,
            CancellationToken ct);
    }
}
