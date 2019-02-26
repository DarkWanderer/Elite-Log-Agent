namespace DW.ELA.Interfaces
{
    using System.Collections.Generic;

    public interface IPluginManager
    {
        IReadOnlyCollection<string> LoadedPluginIds { get; }

        IReadOnlyCollection<IPlugin> LoadedPlugins { get; }

        IPlugin GetPluginById(string pluginId);

        void LoadPlugin(string pluginAssemblyName);

        void LoadEmbeddedPlugins();
    }
}
