using System;
using Newtonsoft.Json;

namespace DW.ELA.Plugin.EDDN.Model
{
    internal partial class OutfittingMessage
    {
        [JsonProperty("marketId")]
        public long MarketId { get; set; }

        [JsonProperty("modules")]
        public string[] Modules { get; set; }

        [JsonProperty("stationName")]
        public string StationName { get; set; }

        [JsonProperty("systemName")]
        public string SystemName { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
