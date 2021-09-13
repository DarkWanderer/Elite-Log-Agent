using Newtonsoft.Json;

namespace DW.ELA.Interfaces.Events
{
    public class FactionInfluenceEffect
    {
        [JsonProperty("Effect")]
        public string EffectEffect { get; set; }

        [JsonProperty("Effect_Localised")]
        public string EffectLocalised { get; set; }

        [JsonProperty("Trend")]
        public string Trend { get; set; }
    }
}
