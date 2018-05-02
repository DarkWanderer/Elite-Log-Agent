using Controller;
using InaraUpdater;
using Interfaces;
using PowerplayGoogleSheetReporter;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Utility;

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

            var settingsProvider = new FileSettingsStorage();
            LogManager.Setup(settingsProvider);

            // TODO: add dynamic plugin loader

            var mainBroker = new AsyncMessageBroker();

            loadedPlugins.Add(new InaraUpdaterPlugin(settingsProvider));
            loadedPlugins.Add(new PowerplayReporterPlugin(settingsProvider));

            var logMonitor = new JsonLogMonitor(SavedGamesDirectoryHelper.Directory);

            using (new CompositeDisposable(logMonitor.Subscribe(mainBroker)))
            using (var settingsForm = new SettingsForm()
            {
                Provider = new FileSettingsStorage(),
                MessageBroker = mainBroker,
                Plugins = loadedPlugins
            })
            {
                Application.Run(settingsForm);
            }
        }

        private static List<IPlugin> loadedPlugins = new List<IPlugin>();
    }
}
