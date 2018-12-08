namespace EliteLogAgent
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using DW.ELA.Interfaces;
    using NLog;

    internal class CastleWindsorPluginLoader : IPluginManager
    {
        private readonly IWindsorContainer container;
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

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
            try
            {
                var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if (!pluginAssemblyName.EndsWith(".dll"))
                    pluginAssemblyName += ".dll";

                //var assembly = Assembly.LoadFile(pluginAssemblyName);
                container.Register(Classes
                    .FromAssemblyNamed(Path.Combine(assemblyDirectory, pluginAssemblyName))
                    .BasedOn<IPlugin>()
                    .WithService
                    .FromInterface()
                    .LifestyleSingleton());
            }
            catch (Exception e)
            {
                Log.Error(e, "Error while loading plugin " + pluginAssemblyName);
            }
        }
    }
}
