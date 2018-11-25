namespace DW.ELA.Plugin.Inara.Model
{
    using DW.ELA.Utility.Json;
    using Newtonsoft.Json;

    public class Header
    {
        public Header(string commander, string apiKey)
        {
            CommanderName = commander;
            ApiKey = apiKey;
        }

        // Input fields
        [JsonProperty("appName")]
        public string AppName => Utility.AppInfo.Name;

        [JsonProperty("appVersion")]
        public string AppVersion => Utility.AppInfo.Version;

        [JsonProperty("isDeveloped")]
        public bool IsDeveloped => false;

        [JsonProperty("APIkey")]
        public string ApiKey { get; }

        [JsonProperty("commanderName")]
        public string CommanderName { get; }

        // Output fields
        [JsonProperty("eventStatus")]
        public int? EventStatus { get; internal set; }

        [JsonProperty("eventStatusText")]
        public string EventStatusText { get; internal set; }

        public override string ToString() => Serialize.ToJson(this);
    }
}
