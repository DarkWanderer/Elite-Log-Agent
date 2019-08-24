namespace DW.ELA.Plugin.EDSM
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class EdsmSettings
    {
        [Obsolete("Retained for backward compatibility")]
        [JsonProperty("apiKey")]
        internal string ApiKey { get; set; } = null;

        /// <summary>
        /// Dictionary of CMDR name => API key pairs
        /// </summary>
        [JsonProperty("apiKeys")]
        public IDictionary<string, string> ApiKeys { get; internal set; } = new Dictionary<string, string>();
    }
}
