namespace DW.ELA.Interfaces.Events
{
    using System.Collections.Generic;
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class MaterialTraderStatistics
    {
        [JsonProperty("Trades_Completed")]
        public long? TradesCompleted { get; set; }

        [JsonProperty("Materials_Traded")]
        public long? MaterialsTraded { get; set; }

        [JsonProperty("Encoded_Materials_Traded")]
        public long? EncodedMaterialsTraded { get; set; }

        [JsonProperty("Raw_Materials_Traded")]
        public long? RawMaterialsTraded { get; set; }

        [JsonProperty("Grade_1_Materials_Traded")]
        public long? Grade1_MaterialsTraded { get; set; }

        [JsonProperty("Grade_2_Materials_Traded")]
        public long? Grade2_MaterialsTraded { get; set; }

        [JsonProperty("Grade_3_Materials_Traded")]
        public long? Grade3_MaterialsTraded { get; set; }

        [JsonProperty("Grade_4_Materials_Traded")]
        public long? Grade4_MaterialsTraded { get; set; }

        [JsonProperty("Grade_5_Materials_Traded")]
        public long? Grade5_MaterialsTraded { get; set; }
    }
}
