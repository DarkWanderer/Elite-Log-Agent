using DW.Inara.LogUploader.Inara;
using DW.Inara.LogUploader.Tray;
using System;
using System.Threading;
using System.Windows.Forms;

namespace DW.Inara.LogUploader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool createdNewMutex = false;
            var mutex = new Mutex(true, "DW.Inara.LogUploader", out createdNewMutex);
            try
            {
                if (!createdNewMutex)
                    return; // Application is already running

                using (var uploadController = new UploadController())
                using (var trayController = new TrayController(uploadController))
                    Application.Run();
            }
            finally
            {
                mutex.Dispose();
            }
        }
    }
}
