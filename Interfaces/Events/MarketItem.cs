using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class MarketItem
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Name_Localised")]
        public string NameLocalised { get; set; }

        [JsonProperty("Category")]
        public string Category { get; set; }

        [JsonProperty("Category_Localised")]
        public string CategoryLocalised { get; set; }

        [JsonProperty("BuyPrice")]
        public long BuyPrice { get; set; }

        [JsonProperty("SellPrice")]
        public long SellPrice { get; set; }

        [JsonProperty("MeanPrice")]
        public long MeanPrice { get; set; }

        [JsonProperty("StockBracket")]
        public long StockBracket { get; set; }

        [JsonProperty("DemandBracket")]
        public long DemandBracket { get; set; }

        [JsonProperty("Stock")]
        public long Stock { get; set; }

        [JsonProperty("Demand")]
        public long Demand { get; set; }

        [JsonProperty("Consumer")]
        public bool Consumer { get; set; }

        [JsonProperty("Producer")]
        public bool Producer { get; set; }

        [JsonProperty("Rare")]
        public bool Rare { get; set; }

        [JsonProperty("Legality")]
        public string Legality { get; set; }
    }
}
