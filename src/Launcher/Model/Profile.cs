using Newtonsoft.Json;
using System;

namespace Launcher.Models
{
    public class Profile
    {
        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("gameDir")]
        public string GameDir { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("lastUsed")]
        public DateTime LastUsed { get; set; }

        [JsonProperty("lastVersionId")]
        public string LastVersionId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }
}