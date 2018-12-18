namespace EliteLogAgent
{
    using System;
    using System.Deployment.Application;
    using System.Linq;
    using System.Windows.Forms;
    using Castle.Facilities.Logging;
    using Castle.MicroKernel.Registration;
    using Castle.Services.Logging.NLogIntegration;
    using Castle.Windsor;
    using DW.ELA.Controller;
    using DW.ELA.Interfaces;
    using DW.ELA.Utility;
    using EliteLogAgent.Autorun;
    using NLog;

    internal static partial class Program
    {
        private static readonly ILogger RootLog = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        internal static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            if (SingleLaunch.IsRunning)
                return; // only one instance should be running

            using (var container = new WindsorContainer())
            {
                ContainerBootstrapper.Initalize(container);

                // Load plugins
                var pluginManager = container.Resolve<IPluginManager>();
                pluginManager.LoadPlugin("DW.ELA.Plugin.Inara");
                pluginManager.LoadPlugin("DW.ELA.Plugin.EDDN");
                pluginManager.LoadPlugin("DW.ELA.Plugin.EDSM");
                pluginManager.LoadEmbeddedPlugins();

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

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var message = e.IsTerminating ? "Unhandled fatal exception" : "Unhandled exception";
            var senderString = sender?.GetType()?.ToString() ?? "null";
            var exceptionTypeString = e.ExceptionObject?.GetType()?.ToString() ?? "null";
            var exceptionObjectString = e.ExceptionObject?.ToString() ?? "null";

            if (e.ExceptionObject is Exception)
                RootLog.Fatal(e.ExceptionObject as Exception, message + " from {0}", senderString);
            else
                RootLog.Fatal(message + " of unknown type: {0} {1}", exceptionTypeString, exceptionObjectString);
        }
    }
}
