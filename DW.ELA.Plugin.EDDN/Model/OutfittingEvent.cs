namespace DW.ELA.Plugin.EDDN.Model
{
    using System;
    using Newtonsoft.Json;

    class OutfittingEvent : EddnEvent
    {
        [JsonProperty("message")]
        public OutfittingMessage Message { get; set; }

        public override string SchemaRef => "https://eddn.edcd.io/schemas/outfitting/2";
    }

    public partial class OutfittingMessage
    {
        [JsonProperty("marketId")]
        public long MarketId { get; set; }

        [JsonProperty("modules")]
        public string[] Modules { get; set; }

        [JsonProperty("stationName")]
        public string StationName { get; set; }

        [JsonProperty("systemName")]
        public string SystemName { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
