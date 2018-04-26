using Interfaces;

namespace InaraUpdater
{
    public class InaraUpdaterPlugin : IPlugin
    {
        public string SettingsLabel => "INARA settings";

        public AbstractSettingsControl GetPluginSettingsControl()
        {
            return new InaraSettingsControl();
        }
    }
}
