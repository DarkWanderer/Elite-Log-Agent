using EliteLogAgent.Settings;

namespace EliteLogAgent
{
    internal class FileSettingsStorage : ISettingsProvider
    {
        public GlobalSettings Settings
        {
            get
            {
                return new GlobalSettings();
            }
            set { }
        }
    }
}