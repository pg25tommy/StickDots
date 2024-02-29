using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Unity.Services.DeploymentApi.Editor;
using Unity.Services.Leaderboards.Authoring.Core.Model;

namespace Unity.Services.Leaderboards.Editor.Authoring.Model
{
    class EditorLeaderboardConfig : LeaderboardConfig
    {
        [JsonProperty("$schema")]
        public string Value = "https://ugs-config-schemas.unity3d.com/v1/leaderboards.schema.json";

        public EditorLeaderboardConfig(string name) : base(name) {}

        // FromJsonFile will populate the values of the Data property and also handle a few override
        // cases for the Id and Name sub-fields
        public void FromJsonFile(string path)
        {
            if (path is not null)
            {
                var content = File.ReadAllText(path);

                var id = System.IO.Path.GetFileNameWithoutExtension(path);

                // PopulateObject() will append already existing tiers if FromJson() called more than once
                // see https://www.newtonsoft.com/json/help/html/PopulateObject.htm
                TieringConfig?.Tiers?.Clear();
                try
                {
                    JsonConvert.PopulateObject(content, this, new JsonSerializerSettings()
                    {
                        // this option is necessary to prevent default values to be set instead of
                        // leaving them to null when absent from the json
                        DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate
                    });
                }
                catch (Exception e) when (e is SerializationException or JsonSerializationException
                                              or JsonReaderException)
                {
                    Status = new DeploymentStatus("Parsing error", e.Message, SeverityLevel.Error);
                }
                catch (Exception e)
                {
                    Status = new DeploymentStatus("Error", e.Message, SeverityLevel.Error);
                }

                // Overriding logic:
                // 1. if Name is not set in JSON, name is same as Id
                // 2. if Id is not set in JSON, Id is same as filename without extension
                Id ??= id;
                Name ??= Id;
            }
        }
    }
}
