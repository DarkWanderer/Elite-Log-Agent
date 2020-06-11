namespace DW.ELA.Interfaces.Events
{
    using Newtonsoft.Json;

    public class Faction
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("FactionState")]
        public string FactionState { get; set; }

        [JsonProperty("Government")]
        public string Government { get; set; }

        [JsonProperty("Influence")]
        public double? Influence { get; set; }

        [JsonProperty("Allegiance")]
        public string Allegiance { get; set; }

        [JsonProperty("ActiveStates")]
        public FactionStateTrend[] ActiveStates { get; set; }

        [JsonProperty("PendingStates")]
        public FactionStateTrend[] PendingStates { get; set; }

        [JsonProperty("RecoveringStates")]
        public FactionStateTrend[] RecoveringStates { get; set; }

        [JsonProperty("Happiness")]
        public string Happiness { get; set; }

        [JsonProperty("Happiness_Localised")]
        public string HappinessLocalised { get; set; }

        [JsonProperty("MyReputation")]
        public double? MyReputation { get; set; }
    }
}
