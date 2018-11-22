namespace DW.ELA.Interfaces.Events
{
    using DW.ELA.Interfaces;
    using Newtonsoft.Json;

    public class Loadout : LogEvent
    {
        [JsonProperty("Ship")]
        public string Ship { get; set; }

        [JsonProperty("ShipID")]
        public long ShipId { get; set; }

        [JsonProperty("ShipName")]
        public string ShipName { get; set; }

        [JsonProperty("ShipIdent")]
        public string ShipIdent { get; set; }

        [JsonProperty("HullValue")]
        public long? HullValue { get; set; }

        [JsonProperty("ModulesValue")]
        public long? ModulesValue { get; set; }

        [JsonProperty("Rebuy")]
        public long? Rebuy { get; set; }

        [JsonProperty]
        public bool? Hot { get; set; }

        [JsonProperty]
        public double? HullHealth { get; set; }

        [JsonProperty("Modules")]
        public Module[] Modules { get; set; }
    }

    public class Module
    {
        [JsonProperty("Slot")]
        public string Slot { get; set; }

        [JsonProperty("Item")]
        public string Item { get; set; }

        [JsonProperty("On")]
        public bool On { get; set; }

        [JsonProperty("Priority")]
        public long Priority { get; set; }

        [JsonProperty("Health")]
        public double Health { get; set; }

        [JsonProperty("Value")]
        public long? Value { get; set; }

        [JsonProperty("Engineering")]
        public Engineering Engineering { get; set; }

        [JsonProperty("AmmoInClip")]
        public long? AmmoInClip { get; set; }

        [JsonProperty("AmmoInHopper")]
        public long? AmmoInHopper { get; set; }
    }

    public class Engineering
    {
        [JsonProperty("Engineer")]
        public string Engineer { get; set; }

        [JsonProperty("EngineerID")]
        public long EngineerId { get; set; }

        [JsonProperty("BlueprintID")]
        public long BlueprintId { get; set; }

        [JsonProperty("BlueprintName")]
        public string BlueprintName { get; set; }

        [JsonProperty("Level")]
        public short Level { get; set; }

        [JsonProperty("Quality")]
        public double Quality { get; set; }

        [JsonProperty("Modifiers")]
        public Modifier[] Modifiers { get; set; }

        [JsonProperty("ExperimentalEffect")]
        public string ExperimentalEffect { get; set; }

        [JsonProperty("ExperimentalEffect_Localised")]
        public string ExperimentalEffectLocalised { get; set; }
    }

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
