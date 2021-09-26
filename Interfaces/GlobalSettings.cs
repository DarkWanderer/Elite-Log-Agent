using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DW.ELA.Interfaces.Settings
{
    public class GlobalSettings : ICloneable
    {
        [JsonIgnore]
        public static GlobalSettings Default => new();

        [JsonProperty("pluginSettings")]
        public IDictionary<string, JObject> PluginSettings { get; set; } = new Dictionary<string, JObject>();

        [JsonProperty("logLevel")]
        public string LogLevel { get; set; } = "Info";

        object ICloneable.Clone() => Clone();

        public GlobalSettings Clone() => JsonConvert.DeserializeObject<GlobalSettings>(JsonConvert.SerializeObject(this));
    }
}
