namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class MissionCompleted : LogEvent
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

        [JsonProperty("Count")]
        public long? Count { get; set; }

        [JsonProperty("DestinationSystem")]
        public string DestinationSystem { get; set; }

        [JsonProperty("DestinationStation")]
        public string DestinationStation { get; set; }

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

    public class FactionEffect
    {
        [JsonProperty("Faction")]
        public string Faction { get; set; }

        [JsonProperty("Effects")]
        public Effect[] Effects { get; set; }

        [JsonProperty("Influence")]
        public Influence[] Influence { get; set; }

        [JsonProperty("Reputation")]
        public string Reputation { get; set; }

        [JsonProperty("ReputationTrend")]
        public string ReputationTrend { get; set; }
    }

    public class Effect
    {
        [JsonProperty("Effect")]
        public string EffectEffect { get; set; }

        [JsonProperty("Effect_Localised")]
        public string EffectLocalised { get; set; }

        [JsonProperty("Trend")]
        public string Trend { get; set; }
    }

    public class CommodityReward
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Name_Localised")]
        public string NameLocalised { get; set; }

        [JsonProperty("Count")]
        public long Count { get; set; }
    }

    public class MaterialsReward
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Name_Localised")]
        public string NameLocalised { get; set; }

        [JsonProperty("Category")]
        public string Category { get; set; }

        [JsonProperty("Category_Localised")]
        public string CategoryLocalised { get; set; }

        [JsonProperty("Count")]
        public long Count { get; set; }
    }

    public class Influence
    {
        [JsonProperty("SystemAddress")]
        public long SystemAddress { get; set; }

        [JsonProperty("Trend")]
        public string Trend { get; set; }

        [JsonProperty("Influence")]
        public string InfluenceValue { get; set; }
    }
}
