namespace DW.ELA.Plugin.EDDN.Model
{
    using System;
    using Newtonsoft.Json;

    public partial class ShipyardMessage
    {
        [JsonProperty("marketId", NullValueHandling = NullValueHandling.Ignore)]
        public double? MarketId { get; set; }

        [JsonProperty("ships")]
        public string[] Ships { get; set; }

        [JsonProperty("stationName")]
        public string StationName { get; set; }

        [JsonProperty("systemName")]
        public string SystemName { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
