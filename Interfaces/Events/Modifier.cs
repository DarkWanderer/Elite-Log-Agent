namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class Modifier
    {
        [JsonProperty("Label")]
        public string Label { get; set; }

        [JsonProperty("Value")]
        public double? Value { get; set; }

        [JsonProperty("ValueStr")]
        public string ValueStr { get; set; }

        [JsonProperty("ValueStr_Localised")]
        public string ValueStrLocalised { get; set; }

        [JsonProperty("OriginalValue")]
        public double? OriginalValue { get; set; }

        [JsonProperty("LessIsGood")]
        public long? LessIsGood { get; set; }
    }
}
