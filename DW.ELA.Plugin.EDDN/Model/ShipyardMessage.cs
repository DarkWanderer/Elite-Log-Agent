using System;
using Newtonsoft.Json;

namespace DW.ELA.Plugin.EDDN.Model
{
    public partial class ShipyardMessage
    {
        [JsonProperty("marketId", NullValueHandling = NullValueHandling.Ignore)]
        public long MarketId { get; set; }

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
