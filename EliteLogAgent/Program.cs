using Controller;
using InaraUpdater;
using InaraUpdater.Model;
using Interfaces;
using PowerplayGoogleSheetReporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrayAgent
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

            // TODO: add dynamic plugin loader
            loadedPlugins.Add(new InaraUpdaterPlugin());
            loadedPlugins.Add(new PowerplayReporterPlugin());

            var mainBroker = new AsyncMessageBroker();
            var inaraUploader = new EventBroker(
                new ApiFacade(new ThrottlingRestClient("https://inara.cz/inapi/v1/"),
                "7nkcf9cb8vkskwwkk8osck0s0g8k8wckoc8cokg",
                "EliteLogAgentTestUser"));

            var logMonitor = new JsonLogMonitor(SavedGamesDirectoryHelper.Directory);

            using (mainBroker.Subscribe(inaraUploader))
            using (logMonitor.Subscribe(mainBroker))
            using (var settingsForm = new SettingsForm()
            {
                SettingsProvider = new RegistryInformationStorage(),
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
