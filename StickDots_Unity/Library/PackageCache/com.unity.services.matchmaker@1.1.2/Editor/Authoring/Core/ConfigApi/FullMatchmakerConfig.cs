using System.Collections.Generic;
using Unity.Services.Matchmaker.Authoring.Core.Model;

namespace Unity.Services.Matchmaker.Authoring.Core.ConfigApi
{
    class FullMatchmakerConfig
    {
        public EnvironmentConfig EnvironmentConfig { get; set; }
        public List<QueueConfig> QueueConfigs { get; set; } = new();
    }
}
