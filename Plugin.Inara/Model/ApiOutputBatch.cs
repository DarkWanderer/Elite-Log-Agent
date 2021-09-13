using System.Collections.Generic;
using DW.ELA.Utility.Json;
using Newtonsoft.Json;

namespace DW.ELA.Plugin.Inara.Model
{
    internal struct ApiOutputBatch
    {
        [JsonProperty("header")]
        public Header Header;

        [JsonProperty("events")]
        public IList<ApiOutputEvent> Events;

        public override string ToString() => Serialize.ToJson(this);
    }
}
