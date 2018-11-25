namespace DW.ELA.Interfaces.Events
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum FactionState
    {
        None,
        CivilUnrest,
        CivilWar,
        Boom,
        Bust,
        Election,
        Expansion,
        Famine,
        Investment,
        Lockdown,
        Outbreak,
        Retreat,
        War,
        CivilLiberty
    }
}
