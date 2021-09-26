using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace DW.ELA.Plugin.EDSM
{
    public class EdsmSettings
    {
        /// <summary>
        /// Dictionary of CMDR name => API key pairs
        /// </summary>
        [JsonProperty("apiKeys")]
        public IDictionary<string, string> ApiKeys { get; internal set; } = new Dictionary<string, string>();
    }
}
