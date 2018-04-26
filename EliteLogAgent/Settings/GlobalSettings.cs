using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace EliteLogAgent.Settings
{
    public class GlobalSettings
    {
        public GlobalSettings()
        {
            PluginSettings = new Dictionary<string, JObject>();
        }

        [JsonProperty("pluginSettings")]
        public IDictionary<string, JObject> PluginSettings { get; set; }
    }
}
