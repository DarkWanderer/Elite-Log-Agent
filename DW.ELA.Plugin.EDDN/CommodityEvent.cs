using Newtonsoft.Json;
using System;

namespace DW.ELA.Plugin.EDDN
{
    public class CommodityEvent : EddnEvent
    {
        [JsonProperty("message")]
        public CommodityMessage Message { get; set; }

        public override string SchemaRef => "https://eddn.edcd.io/schemas/commodity/3/test";
    }

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
        public double? MarketId { get; set; }

        [JsonProperty("prohibited")]
        public string[] Prohibited { get; set; }

        [JsonProperty("stationName")]
        public string StationName { get; set; }

        [JsonProperty("systemName")]
        public string SystemName { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// Commodities returned by the Companion API, with illegal commodities omitted
    /// </summary>
    public class Commodity
    {
        /// <summary>
        /// Price to buy from the market
        /// </summary>
        [JsonProperty("buyPrice")]
        public long BuyPrice { get; set; }

        [JsonProperty("demand")]
        public long Demand { get; set; }

        [JsonProperty("demandBracket")]
        public long DemandBracket { get; set; }

        [JsonProperty("meanPrice")]
        public long MeanPrice { get; set; }

        /// <summary>
        /// Symbolic name as returned by the Companion API
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Price to sell to the market
        /// </summary>
        [JsonProperty("sellPrice")]
        public long SellPrice { get; set; }

        [JsonProperty("statusFlags", NullValueHandling = NullValueHandling.Ignore)]
        public string[] StatusFlags { get; set; }

        [JsonProperty("stock")]
        public long Stock { get; set; }

        [JsonProperty("stockBracket")]
        public long StockBracket { get; set; }
    }

    public partial class Economy
    {
        /// <summary>
        /// Economy type as returned by the Companion API
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("proportion")]
        public double Proportion { get; set; }
    }
}
