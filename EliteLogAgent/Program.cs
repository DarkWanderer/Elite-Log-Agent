namespace EliteLogAgent
{
    using System;
    using System.Linq;
    using System.Windows.Forms;
    using Castle.Windsor;
    using DW.ELA.Interfaces;
    using DW.ELA.Utility;
    using NLog;
    using NLog.Fluent;

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
                RootLog.Info()
                    .Message("Application started")
                    .Property("version", AppInfo.Version)
                    .Write();

                // Load plugins
                var pluginManager = container.Resolve<IPluginManager>();
                pluginManager.LoadPlugin("DW.ELA.Plugin.Inara");
                pluginManager.LoadPlugin("DW.ELA.Plugin.EDDN");
                pluginManager.LoadPlugin("DW.ELA.Plugin.EDSM");
                pluginManager.LoadEmbeddedPlugins();

                var logMonitor = container.Resolve<ILogRealTimeDataSource>();
                var trayController = container.Resolve<ITrayIconController>();
                var playerStateRecorder = container.Resolve<IPlayerStateHistoryRecorder>();

                // subscription 'token' is IDisposable
                // subscribing the PlayerStateRecorder first to avoid potential issues with out-of-order execution because of threading
                using (logMonitor.Subscribe(playerStateRecorder))
                using (new CompositeDisposable(pluginManager.LoadedPlugins.Select(p => logMonitor.Subscribe(p.GetLogObserver()))))
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
