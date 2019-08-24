namespace DW.ELA.Interfaces.Events
{
    using Newtonsoft.Json;

    public class MaterialTrade : JournalEvent
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
}
