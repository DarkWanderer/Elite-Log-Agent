using DW.Inara.LogUploader.Inara;
using DW.Inara.LogUploader.Persistence;
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

                var registryInfoStorage = new RegistryInformationStorage();

                using (var uploadController = new UploadController(registryInfoStorage))
                using (var processWatcher = new ProcessWatcher("EliteDangerous64.exe", "EliteDangerous32.exe"))
                using (var trayController = new TrayController(uploadController, registryInfoStorage))
                {
                    uploadController.Settings = registryInfoStorage.Load();
                    processWatcher.ProcessFinished += (o, e) => uploadController.UploadLatestFile(true);
                    Application.Run();
                }
            }
            finally
            {
                mutex.Dispose();
            }
        }
    }
}
