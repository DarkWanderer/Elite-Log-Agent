using DW.ELA.Interfaces;
using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class FsdJump : LocationEventBase
    {
        [JsonProperty("JumpDist")]
        public double JumpDist { get; set; }

        [JsonProperty("FuelUsed")]
        public double FuelUsed { get; set; }

        [JsonProperty("FuelLevel")]
        public double FuelLevel { get; set; }

        [JsonProperty("ActiveFine")]
        public long? ActiveFine { get; set; }

        [JsonProperty]
        public int? BoostUsed { get; set; }
    }
}
