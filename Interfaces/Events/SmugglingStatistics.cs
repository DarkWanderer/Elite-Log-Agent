using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class SmugglingStatistics
    {
        [JsonProperty("Black_Markets_Traded_With")]
        public long BlackMarketsTradedWith { get; set; }

        [JsonProperty("Black_Markets_Profits")]
        public long BlackMarketsProfits { get; set; }

        [JsonProperty("Resources_Smuggled")]
        public long ResourcesSmuggled { get; set; }

        [JsonProperty("Average_Profit")]
        public double AverageProfit { get; set; }

        [JsonProperty("Highest_Single_Transaction")]
        public long HighestSingleTransaction { get; set; }
    }
}
