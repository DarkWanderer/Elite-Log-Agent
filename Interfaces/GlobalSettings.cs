namespace DW.ELA.Interfaces.Settings
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class GlobalSettings : ICloneable
    {
        [JsonProperty("pluginSettings")]
        public IDictionary<string, JObject> PluginSettings { get; set; } = new Dictionary<string, JObject>();

        [JsonProperty("commanderName")]
        public string CommanderName { get; set; } = "Unknown Commander";

        [JsonProperty("logLevel")]
        public string LogLevel { get; set; } = "Debug";

        [JsonProperty("reportErrorsToCloud")]
        public bool ReportErrorsToCloud { get; set; } = true;

        public static GlobalSettings Defaults => new GlobalSettings();

        object ICloneable.Clone() => Clone();

        public GlobalSettings Clone() {
            var jsonSerializer = new JsonSerializer();
            return JsonConvert.DeserializeObject<GlobalSettings>(JsonConvert.SerializeObject(this));
        }
    }
}
