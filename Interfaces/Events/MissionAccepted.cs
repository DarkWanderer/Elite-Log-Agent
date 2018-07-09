using DW.ELA.Interfaces;
using Newtonsoft.Json;
using System;

namespace DW.ELA.Interfaces.Events
{
    public class MissionAccepted : LogEvent
    {
        [JsonProperty("Faction")]
        public string Faction { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("LocalisedName")]
        public string LocalisedName { get; set; }

        [JsonProperty("TargetType")]
        public string TargetType { get; set; }

        [JsonProperty("TargetType_Localised")]
        public string TargetTypeLocalised { get; set; }

        [JsonProperty("TargetFaction")]
        public string TargetFaction { get; set; }

        [JsonProperty("DestinationSystem")]
        public string DestinationSystem { get; set; }

        [JsonProperty("DestinationStation")]
        public string DestinationStation { get; set; }

        [JsonProperty("Target")]
        public string Target { get; set; }

        [JsonProperty("Expiry")]
        public DateTime Expiry { get; set; }

        [JsonProperty("Wing")]
        public bool Wing { get; set; }

        [JsonProperty("Influence")]
        public string Influence { get; set; }

        [JsonProperty("Reputation")]
        public string Reputation { get; set; }

        [JsonProperty("Reward")]
        public long Reward { get; set; }

        [JsonProperty("MissionID")]
        public long MissionId { get; set; }
    }
}
