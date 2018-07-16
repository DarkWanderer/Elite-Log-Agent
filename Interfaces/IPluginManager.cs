using System.Collections.Generic;

namespace DW.ELA.Interfaces
{
    public interface IPluginManager
    {
        IReadOnlyCollection<string> LoadedPluginIds { get; }
        IPlugin GetPluginById(string pluginId);
        IReadOnlyCollection<IPlugin> LoadedPlugins { get; }
        void LoadPlugin(string pluginAssemblyName);
    }
}
