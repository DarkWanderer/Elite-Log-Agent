using Newtonsoft.Json;
using System;

namespace DW.ELA.Interfaces.Events
{
    public class MaterialTrade : LogEvent
    {
        [JsonProperty("MarketID")]
        public long MarketId { get; set; }

        [JsonProperty("TraderType")]
        public string TraderType { get; set; }

        [JsonProperty("Paid")]
        public MaterialDealLeg Paid { get; set; }

        [JsonProperty("Received")]
        public MaterialDealLeg Received { get; set; }
    }

    public class MaterialDealLeg
    {
        [JsonProperty("Material")]
        public string Material { get; set; }

        [JsonProperty("Material_Localised")]
        public string MaterialLocalised { get; set; }

        [JsonProperty("Category")]
        public string Category { get; set; }

        [JsonProperty("Category_Localised")]
        public string CategoryLocalised { get; set; }

        [JsonProperty("Quantity")]
        public long Quantity { get; set; }
    }
}
