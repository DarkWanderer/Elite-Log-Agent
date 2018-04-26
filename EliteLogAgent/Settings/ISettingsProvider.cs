using EliteLogAgent.Settings;

namespace EliteLogAgent
{
    internal interface ISettingsProvider
    {
        GlobalSettings Settings { get; set; }
    }
}