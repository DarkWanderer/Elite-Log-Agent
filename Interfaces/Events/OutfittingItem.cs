namespace DW.ELA.Interfaces.Events
{
    using Newtonsoft.Json;

    public class OutfittingItem
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("BuyPrice")]
        public long BuyPrice { get; set; }
    }
}
