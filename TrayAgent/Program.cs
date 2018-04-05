using InaraUpdater;
using InaraUpdater.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrayAgent
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var restClient = new RestClient("https://inara.cz/inapi/v1/");
            var cmdrName = "John Kozak";
            var facade = new ApiFacade(restClient, "4yibcstuoiskg0kkcgcocksgok84cwk0ossoc4g", cmdrName);

            var @event = new GetCommanderProfileEvent(cmdrName);
            var result = facade.ApiCall(@event).Result;
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());
        }
    }
}
