namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class MissionCompleted : JournalEvent
    {
        [JsonProperty("Faction")]
        public string Faction { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("MissionID")]
        public long MissionId { get; set; }

        [JsonProperty("Commodity")]
        public string Commodity { get; set; }

        [JsonProperty("Commodity_Localised")]
        public string CommodityLocalised { get; set; }

        [JsonProperty]
        public long? Count { get; set; }

        [JsonProperty]
        public long? KillCount { get; set; }

        [JsonProperty]
        public string DestinationSystem { get; set; }

        [JsonProperty]
        public string DestinationStation { get; set; }

        [JsonProperty]
        public string NewDestinationSystem { get; set; }

        [JsonProperty]
        public string NewDestinationStation { get; set; }

        [JsonProperty("Reward")]
        public long? Reward { get; set; }

        [JsonProperty("Donation")]
        public string Donation { get; set; }

        [JsonProperty("Donated")]
        public long? Donated { get; set; }

        [JsonProperty("CommodityReward")]
        public CommodityReward[] CommodityReward { get; set; }

        [JsonProperty("MaterialsReward")]
        public MaterialsReward[] MaterialsReward { get; set; }

        [JsonProperty("FactionEffects")]
        public FactionEffect[] FactionEffects { get; set; }

        [JsonProperty("TargetType")]
        public string TargetType { get; set; }

        [JsonProperty("TargetType_Localised")]
        public string TargetTypeLocalised { get; set; }

        [JsonProperty("TargetFaction")]
        public string TargetFaction { get; set; }

        [JsonProperty("Target")]
        public string Target { get; set; }
    }
}
