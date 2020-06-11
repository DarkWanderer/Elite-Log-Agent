namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class CarrierJump : LocationEventBase
    {
        [JsonProperty("Docked")]
        public bool Docked { get; set; }
    }
}
