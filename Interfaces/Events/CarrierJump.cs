using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class CarrierJump : LocationEventBase
    {
        [JsonProperty("Docked")]
        public bool Docked { get; set; }
    }
}
