namespace DW.ELA.Plugin.EDDN.Model
{
    using Newtonsoft.Json;

    internal class OutfittingEvent : EddnEvent
    {
        [JsonProperty("message")]
        public OutfittingMessage Message { get; set; }

        public override string SchemaRef => "https://eddn.edcd.io/schemas/outfitting/2";
    }
}
