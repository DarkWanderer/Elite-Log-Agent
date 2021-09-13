using System.Collections.Generic;

namespace DW.ELA.Interfaces
{
    public interface IPluginManager
    {
        IReadOnlyCollection<string> LoadedPluginIds { get; }

        IReadOnlyCollection<IPlugin> LoadedPlugins { get; }

        IPlugin GetPluginById(string pluginId);

        void LoadPlugin(string pluginAssemblyName);

        void LoadEmbeddedPlugins();
    }
}
