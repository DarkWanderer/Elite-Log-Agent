using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DW.ELA.Interfaces.Settings
{
    public class GlobalSettings : ICloneable
    {
        [JsonIgnore]
        public static GlobalSettings Default => new GlobalSettings();

        [JsonProperty("pluginSettings")]
        public IDictionary<string, JObject> PluginSettings { get; set; } = new Dictionary<string, JObject>();

        [Obsolete("Retained for backward compatibility")]
        [JsonProperty("commanderName")]
        public string CommanderName { get; set; } = null;

        [JsonProperty("logLevel")]
        public string LogLevel { get; set; } = "Info";

        [JsonProperty("reportErrorsToCloud")]
        public bool ReportErrorsToCloud { get; set; } = true;

        object ICloneable.Clone() => Clone();

        public GlobalSettings Clone() => JsonConvert.DeserializeObject<GlobalSettings>(JsonConvert.SerializeObject(this));
    }
}
