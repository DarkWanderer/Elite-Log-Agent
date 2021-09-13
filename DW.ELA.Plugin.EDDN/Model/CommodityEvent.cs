using Newtonsoft.Json;

namespace DW.ELA.Plugin.EDDN.Model
{
    public class CommodityEvent : EddnEvent
    {
        [JsonProperty("message")]
        public CommodityMessage Message { get; set; }

        public override string SchemaRef => "https://eddn.edcd.io/schemas/commodity/3";
    }
}
