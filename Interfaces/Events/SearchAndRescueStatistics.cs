namespace DW.ELA.Interfaces.Events
{
    using System.Collections.Generic;
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class SearchAndRescueStatistics
    {
        [JsonProperty("SearchRescue_Traded")]
        public long SearchRescueTraded { get; set; }

        [JsonProperty("SearchRescue_Profit")]
        public long SearchRescueProfit { get; set; }

        [JsonProperty("SearchRescue_Count")]
        public long SearchRescueCount { get; set; }
    }
}
