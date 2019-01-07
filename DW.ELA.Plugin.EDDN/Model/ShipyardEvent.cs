namespace DW.ELA.Plugin.EDDN.Model
{
    using Newtonsoft.Json;

    public partial class ShipyardEvent : EddnEvent
    {
        [JsonProperty("message")]
        public ShipyardMessage Message { get; set; }

        public override string SchemaRef => "https://eddn.edcd.io/schemas/shipyard/2";
    }
}
