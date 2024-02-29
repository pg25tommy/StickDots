namespace Unity.Services.Matchmaker.Authoring.Core.ConfigApi
{
    /// <summary>
    /// Config size restrictions.
    /// Keep default values updated since they are used to validate first time deployment.
    /// </summary>
    class ConfigRestrictions
    {
        internal int MaxQueueSize { get; set; } = 20;

        internal int MaxPoolSize { get; set; } = 20;

        internal int MaxVariantPoolSize { get; set; } = 10;

        internal int MaxTeamCount { get; set; } = 100;

        internal int MaxRuleCountPerMatch { get; set; } = 25;

        internal int MaxRuleCountPerTeam { get; set; } = 25;

        internal int MaxRelaxationCount { get; set; } = 25;

        internal int MaxPlayersPerTicket { get; set; } = 20;

        internal int MaxTimeoutMs { get; set; } = 30 * 60 * 1000;
    }
}
