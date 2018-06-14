using Controller;
using Interfaces;
using System;
using System.Windows.Forms;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.Facilities.Logging;
using Castle.Services.Logging.NLogIntegration;
using NLog;
using Utility;
using System.Linq;

namespace EliteLogAgent
{
    static partial class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppDomain.CurrentDomain.UnhandledException += (o, e) => rootLogger.Fatal(e.ExceptionObject as Exception, "Unhandled exception");

            using (var container = new WindsorContainer())
            {
                // Initalize infrastructure classes - NLog, Windsor
                container.AddFacility<LoggingFacility>(f => f.LogUsing<NLogFactory>().ConfiguredExternally());
                container.Register(
                    Component.For<ISettingsProvider>().ImplementedBy<FileSettingsStorage>().LifestyleSingleton(),
                    Component.For<ILogSettingsBootstrapper>().ImplementedBy<NLogSettingsManager>().LifestyleTransient(),
                    Component.For<IPluginManager>().ImplementedBy<CastleWindsorPluginLoader>().LifestyleSingleton(),
                    Component.For<IWindsorContainer>().Instance(container)

                );
                container.Resolve<ILogSettingsBootstrapper>().Setup();

                // Register core classes
                container.Register(
                    Component.For<ILogDirectoryNameProvider>().ImplementedBy<SavedGamesDirectoryHelper>().LifestyleSingleton(),
                    Component.For<ILogRealTimeDataSource>().ImplementedBy<JsonLogMonitor>().LifestyleSingleton(),
                    Component.For<IMessageBroker>().ImplementedBy<AsyncMessageBroker>().LifestyleSingleton(),
                    Component.For<IPlayerStateHistoryRecorder>().ImplementedBy<PlayerStateRecorder>().LifestyleSingleton()
                );

                // Register UI classes
                container.Register(Component.For<ITrayIconController>().ImplementedBy<TrayIconController>().LifestyleSingleton());

                // Load plugins
                // TODO: add dynamic plugin loader
                var pluginManager = container.Resolve<IPluginManager>();
                pluginManager.LoadPlugin("Plugin.Inara");
                pluginManager.LoadPlugin("Plugin.EDSM");

                var broker = container.Resolve<IMessageBroker>();
                var logMonitor = container.Resolve<ILogRealTimeDataSource>();
                var trayController = container.Resolve<ITrayIconController>();
                var playerStateRecorder = container.Resolve<IPlayerStateHistoryRecorder>();

                using (logMonitor.Subscribe(broker)) // subscription 'token' is IDisposable
                using (broker.Subscribe(playerStateRecorder))
                using (new CompositeDisposable(pluginManager.LoadedPlugins.Select(p => broker.Subscribe(p.GetLogObserver()))))
                {
                    Application.Run();
                }
            }
        }

        private static readonly ILogger rootLogger = LogManager.GetCurrentClassLogger();

    }
}
