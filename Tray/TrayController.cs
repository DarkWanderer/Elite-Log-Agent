using DW.Inara.LogUploader.Inara;
using DW.Inara.LogUploader.Properties;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace DW.Inara.LogUploader.Tray
{
    internal class TrayController : IDisposable
    {
        private readonly NotifyIcon trayIcon;
        private readonly UploadController uploadController;
        private readonly Persistence.IPersistentSettingsStorage settingsStorage;

        public TrayController(UploadController uploadController, Persistence.IPersistentSettingsStorage settingsStorage)
        {
            this.uploadController = uploadController;
            this.settingsStorage = settingsStorage;
            trayIcon = CreateTrayIcon();
            uploadController.LogUploadSuccessful += UploadController_LogUploadSuccessful;
            uploadController.LogUploadFailed += UploadController_LogUploadFailed;
        }

        private void UploadController_LogUploadFailed(object sender, LogUploadEventArgs e)
        {
            trayIcon.ShowBalloonTip(5, $"Failed: {e.FileName}", $"Could not upload file to Inara.cz\nError: {e.Error}", ToolTipIcon.Error);
        }

        private void UploadController_LogUploadSuccessful(object sender, LogUploadEventArgs e)
        {
            trayIcon.ShowBalloonTip(5, $"Uploaded: {e.FileName}", $"Log file uploaded to Inara.cz successfully", ToolTipIcon.Info);
        }

        private NotifyIcon CreateTrayIcon()
        {
            var components = new Container();
            var notifyIcon = new NotifyIcon(components)
            {
                ContextMenuStrip = CreateMenuStrip(),
                Icon = Resources.InaraIcon,
                Text = "INARA log uploader",
                Visible = true
            };
            notifyIcon.BalloonTipClicked += NotifyIcon_BalloonTipClicked;
            return notifyIcon;
        }

        private static void NotifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            Process.Start("https://inara.cz/cmdr/");
        }

        private ContextMenuStrip CreateMenuStrip()
        {
            var menuStrip = new ContextMenuStrip();
            menuStrip.Items.Add("Upload latest log", Resources.InaraIcon.ToBitmap(), (o, e) => uploadController.UploadLatestFiles(1, false));
            menuStrip.Items.Add("Upload 5 latest logs", Resources.InaraIcon.ToBitmap(), (o, e) => uploadController.UploadLatestFiles(5, false));
            menuStrip.Items.Add(ToolStripSeparatorLeft);
            menuStrip.Items.Add("Settings", SystemIcons.Application.ToBitmap(), (o, e) =>
            {
                using (var form = new SettingsForm())
                {
                    form.UploaderSettings = uploadController.Settings ?? new Settings.UploaderSettings();
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        uploadController.Settings = form.UploaderSettings;
                        settingsStorage.Save(form.UploaderSettings);
                    }
                }
            });
            menuStrip.Items.Add("About", SystemIcons.Information.ToBitmap(), (o, e) => { using (var form = new AboutForm()) { form.ShowDialog(); } });
            menuStrip.Items.Add(ToolStripSeparatorLeft);
            menuStrip.Items.Add("Exit", SystemIcons.Error.ToBitmap(), (o, e) => Application.Exit());
            return menuStrip;
        }

        private static ToolStripSeparator ToolStripSeparatorLeft => new ToolStripSeparator { Alignment = ToolStripItemAlignment.Left };

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    trayIcon?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~TrayController() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
