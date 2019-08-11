namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public partial class Docked : JournalEvent
    {
        [JsonProperty("StationName")]
        public string StationName { get; set; }

        [JsonProperty("StationType")]
        public string StationType { get; set; }

        [JsonProperty("StarSystem")]
        public string StarSystem { get; set; }

        [JsonProperty("SystemAddress")]
        public ulong? SystemAddress { get; set; }

        [JsonProperty("MarketID")]
        public long MarketId { get; set; }

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
}
