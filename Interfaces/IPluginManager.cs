namespace DW.ELA.Interfaces
{
    using System.Collections.Generic;

    public interface IPluginManager
    {
        IReadOnlyCollection<string> LoadedPluginIds { get; }

        IPlugin GetPluginById(string pluginId);

        IReadOnlyCollection<IPlugin> LoadedPlugins { get; }

        void LoadPlugin(string pluginAssemblyName);
    }
}
