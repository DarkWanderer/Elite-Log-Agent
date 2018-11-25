namespace DW.ELA.Interfaces.Events
{
    using Newtonsoft.Json;

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
