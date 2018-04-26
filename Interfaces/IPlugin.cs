namespace Interfaces
{
    public interface IPlugin
    {
        string SettingsLabel { get; }

        //IObserver<JObject> GetLogObserver();
        AbstractSettingsControl GetPluginSettingsControl();
    }
}
