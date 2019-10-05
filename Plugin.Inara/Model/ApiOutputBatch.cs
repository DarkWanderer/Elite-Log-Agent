namespace DW.ELA.Plugin.Inara.Model
{
    using System.Collections.Generic;
    using DW.ELA.Utility.Json;
    using Newtonsoft.Json;

    internal struct ApiOutputBatch
    {
        [JsonProperty("header")]
        public Header Header;

        [JsonProperty("events")]
        public IList<ApiOutputEvent> Events;

        public override readonly string ToString() => Serialize.ToJson(this);
    }
}
