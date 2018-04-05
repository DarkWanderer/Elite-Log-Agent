using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InaraUpdater.Model
{
    public class Header
    {
        public Header(string commander, string apiKey)
        {
            CommanderName = commander;
            ApiKey = apiKey;
            EventStatus = 200;
        }

        // Input fields
        [JsonProperty("appName")]
        public const string AppName = "EliteLogAgent";
        [JsonProperty("appVersion")]
        public const string AppVersion = "0.1";
        [JsonProperty("isDeveloped")]
        public const bool IsDeveloped = true;
        [JsonProperty("APIkey")]
        public string ApiKey { get; }
        [JsonProperty("commanderName")]
        public string CommanderName { get; }

        // Output fields
        [JsonProperty("eventStatus")]
        public int EventStatus { get; internal set; }
        [JsonProperty("eventStatusText")]
        public string EventStatusText { get; internal set; }

        public override string ToString() => JsonConvert.SerializeObject(this);

    }
}
