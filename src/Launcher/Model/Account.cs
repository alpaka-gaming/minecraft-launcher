using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Launcher.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class MinecraftProfile
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class UserProperite
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class Account
    {
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("eligibleForMigration")]
        public bool EligibleForMigration { get; set; }

        [JsonProperty("hasMultipleProfiles")]
        public bool HasMultipleProfiles { get; set; }

        [JsonProperty("legacy")]
        public bool Legacy { get; set; }

        [JsonProperty("localId")]
        public string LocalId { get; set; }

        [JsonProperty("minecraftProfile")]
        public MinecraftProfile MinecraftProfile { get; set; }

        [JsonProperty("persistent")]
        public bool Persistent { get; set; }

        [JsonProperty("remoteId")]
        public string RemoteId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("userProperites")]
        public List<UserProperite> UserProperites { get; set; }

        [JsonProperty("username")]
        public string Username { get; set; }
    }



}