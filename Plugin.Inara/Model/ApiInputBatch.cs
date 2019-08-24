namespace DW.ELA.Plugin.Inara.Model
{
    using System.Collections.Generic;
    using DW.ELA.Utility.Json;
    using Newtonsoft.Json;

    internal struct ApiInputBatch
    {
        [JsonProperty("header")]
        public Header Header;

        [JsonProperty("events")]
        public IList<ApiInputEvent> Events;

        public override string ToString() => Serialize.ToJson(this);
    }
}
