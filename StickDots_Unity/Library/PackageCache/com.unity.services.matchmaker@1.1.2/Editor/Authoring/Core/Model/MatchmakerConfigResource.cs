namespace Unity.Services.Matchmaker.Authoring.Core.Model
{
    class MatchmakerConfigResource
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public IMatchmakerConfig Content { get; set; }
    }
}
