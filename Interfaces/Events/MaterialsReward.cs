namespace DW.ELA.Interfaces.Events
{
    using Newtonsoft.Json;

    public class MaterialsReward
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Name_Localised")]
        public string NameLocalised { get; set; }

        [JsonProperty("Category")]
        public string Category { get; set; }

        [JsonProperty("Category_Localised")]
        public string CategoryLocalised { get; set; }

        [JsonProperty("Count")]
        public long Count { get; set; }
    }
}
