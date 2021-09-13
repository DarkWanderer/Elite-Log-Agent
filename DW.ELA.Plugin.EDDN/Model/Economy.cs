using Newtonsoft.Json;

namespace DW.ELA.Plugin.EDDN.Model
{
    public partial class Economy
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("proportion")]
        public double Proportion { get; set; }
    }
}
