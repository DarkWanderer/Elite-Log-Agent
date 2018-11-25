namespace DW.ELA.Interfaces.Events
{
    using System.Collections.Generic;
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class MiningStatistics
    {
        [JsonProperty("Mining_Profits")]
        public long MiningProfits { get; set; }

        [JsonProperty("Quantity_Mined")]
        public long QuantityMined { get; set; }

        [JsonProperty("Materials_Collected")]
        public long MaterialsCollected { get; set; }
    }
}
