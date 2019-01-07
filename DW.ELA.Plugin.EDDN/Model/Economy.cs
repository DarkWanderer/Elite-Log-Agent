namespace DW.ELA.Plugin.EDDN.Model
{
    using Newtonsoft.Json;

    public partial class Economy
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("proportion")]
        public double Proportion { get; set; }
    }
}
