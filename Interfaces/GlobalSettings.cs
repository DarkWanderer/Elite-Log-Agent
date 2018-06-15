using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Interfaces.Settings
{
    public class GlobalSettings : ICloneable
    {
        public GlobalSettings()
        {
            PluginSettings = new Dictionary<string, JObject>();
        }

        [JsonProperty("pluginSettings")]
        public IDictionary<string, JObject> PluginSettings { get; set; }

        [JsonProperty("commanderName")]
        public string CommanderName { get; set; } = "Unknown Commander";

        [JsonProperty("logLevel")]
        public string LogLevel { get; set; } = "Info";

        [JsonProperty("setupWizardDone")]
        public bool InitialSetupDone { get; set; } = false;

        object ICloneable.Clone() => Clone();

        public GlobalSettings Clone() {
            var jsonSerializer = new JsonSerializer();
            return JsonConvert.DeserializeObject<GlobalSettings>(JsonConvert.SerializeObject(this));
        }
    }
}
