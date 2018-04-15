using Controller;
using InaraUpdater;
using InaraUpdater.Model;
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
            var mainBroker = new MessageBroker();
            var inaraUploader = new EventBroker(
                new ApiFacade(new ThrottlingRestClient("https://inara.cz/inapi/v1/"),
                "7nkcf9cb8vkskwwkk8osck0s0g8k8wckoc8cokg",
                "EliteLogAgentTestUser"));
            using (var subscription = mainBroker.Subscribe(inaraUploader))
            using (var settingsForm = new SettingsForm()
            {
                SettingsProvider = new RegistryInformationStorage(),
                MessageBroker = mainBroker
            })
            {
                Application.Run(settingsForm);
            }
        }
    }
}
