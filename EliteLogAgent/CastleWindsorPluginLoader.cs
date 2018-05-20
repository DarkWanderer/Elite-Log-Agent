using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace EliteLogAgent
{
    internal class CastleWindsorPluginLoader : IPluginManager
    {
        private readonly IWindsorContainer container;

        public CastleWindsorPluginLoader(IWindsorContainer container)
        {
            this.container = container;
        }

        public IReadOnlyCollection<string> LoadedPluginIds => Plugins.Select(p => p.PluginId).ToArray();
        public IReadOnlyCollection<IPlugin> LoadedPlugins => Plugins.ToArray();

        public IList<IPlugin> Plugins => container.ResolveAll<IPlugin>();

        public IPlugin GetPluginById(string pluginId) => Plugins.SingleOrDefault(p => p.PluginId == pluginId);

        public void LoadPlugin(string pluginAssemblyName)
        {
            var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(Path.GetExtension(pluginAssemblyName)))
                pluginAssemblyName = Path.ChangeExtension(pluginAssemblyName, ".dll");

            //var assembly = Assembly.LoadFile(pluginAssemblyName);
            container.Register(Classes
                .FromAssemblyNamed(Path.Combine(assemblyDirectory, pluginAssemblyName))
                .BasedOn<IPlugin>()
                .WithService
                .FromInterface()
                .LifestyleSingleton());
        }
    }
}
