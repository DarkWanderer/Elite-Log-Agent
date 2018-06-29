using DW.ELA.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace DW.ELA.LogModel.Events
{
    public class Commander : LogEvent
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
    }
}
