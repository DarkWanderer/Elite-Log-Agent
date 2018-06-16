using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DW.ELA.Interfaces.Settings
{
    public class GlobalSettings : ICloneable
    {
        [JsonProperty("pluginSettings")]
        public IDictionary<string, JObject> PluginSettings { get; set; } = new Dictionary<string, JObject>();

        [JsonProperty("commanderName")]
        public string CommanderName { get; set; } = "Unknown Commander";

        [JsonProperty("logLevel")]
        public string LogLevel { get; set; } = "Debug";

        [JsonProperty("setupWizardDone")]
        public bool InitialSetupDone { get; set; } = false;

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
