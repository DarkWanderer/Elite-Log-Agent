using DW.ELA.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace DW.ELA.Interfaces.Events
{
    public class Commander : LogEvent
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("FID")]
        public string FrontierId { get; set; }
    }
}
