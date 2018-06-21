using Newtonsoft.Json;

namespace DW.ELA.LogModel.Events
{
    public partial class Docked : LogEvent
    {
        [JsonProperty("StationName")]
        public string StationName { get; set; }

        [JsonProperty("StationType")]
        public string StationType { get; set; }

        [JsonProperty("StarSystem")]
        public string StarSystem { get; set; }

        [JsonProperty("SystemAddress")]
        public long SystemAddress { get; set; }

        [JsonProperty("MarketID")]
        public long MarketId { get; set; }

        [JsonProperty("StationFaction")]
        public string StationFaction { get; set; }

        [JsonProperty("StationGovernment")]
        public string StationGovernment { get; set; }

        [JsonProperty("StationGovernment_Localised")]
        public string StationGovernmentLocalised { get; set; }

        [JsonProperty("StationAllegiance")]
        public string StationAllegiance { get; set; }

        [JsonProperty("StationServices")]
        public string[] StationServices { get; set; }

        [JsonProperty("StationEconomy")]
        public string StationEconomy { get; set; }

        [JsonProperty("StationEconomy_Localised")]
        public string StationEconomyLocalised { get; set; }

        [JsonProperty("StationEconomies")]
        public StationEconomy[] StationEconomies { get; set; }

        [JsonProperty("DistFromStarLS")]
        public double DistFromStarLs { get; set; }
    }

    public partial class StationEconomy
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Name_Localised")]
        public string NameLocalised { get; set; }

        [JsonProperty("Proportion")]
        public long Proportion { get; set; }
    }
}
