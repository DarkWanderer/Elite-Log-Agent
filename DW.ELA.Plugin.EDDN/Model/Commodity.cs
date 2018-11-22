namespace DW.ELA.Plugin.EDDN.Model
{
    using System;
    using Newtonsoft.Json;

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
}
