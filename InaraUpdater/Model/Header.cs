using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InaraUpdater.Model
{
    [JsonObject(Title = "header")]
    public class Header
    {
        [JsonProperty("appName")]
        public const string AppName = "EliteLogAgent";
        [JsonProperty("appVersion")]
        public const string AppVersion = "0.1";
        [JsonProperty("isDeveloped")]
        public const bool IsDeveloped = true;
        [JsonProperty("APIKey")]
        public string ApiKey { get; }
        [JsonProperty("commanderName")]
        public string CommanderName { get; }

        public Header(string commander, string apiKey)
        {
            CommanderName = commander;
            ApiKey = apiKey;
        }
    }
}
