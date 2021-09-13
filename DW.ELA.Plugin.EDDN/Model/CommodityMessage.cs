using System;
using Newtonsoft.Json;

namespace DW.ELA.Plugin.EDDN.Model
{
    public class CommodityMessage
    {
        /// <summary>
        /// Commodities returned by the Companion API, with illegal commodities omitted
        /// </summary>
        [JsonProperty("commodities")]
        public Commodity[] Commodities { get; set; }

        [JsonProperty("economies")]
        public Economy[] Economies { get; set; }

        [JsonProperty("marketId")]
        public long MarketId { get; set; }

        [JsonProperty("prohibited")]
        public string[] Prohibited { get; set; }

        [JsonProperty("stationName")]
        public string StationName { get; set; }

        [JsonProperty("systemName")]
        public string SystemName { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
