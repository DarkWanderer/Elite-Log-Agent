using EliteLogAgent.Properties;
using DW.ELA.Interfaces;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Utility;

namespace EliteLogAgent
{
    internal class TrayIconController : ITrayIconController
    {
        private readonly NotifyIcon trayIcon;
        private readonly IPluginManager pluginManager;
        private readonly IMessageBroker messageBroker;
        private readonly ISettingsProvider settingsProvider;

        private NotifyIcon CreateTrayIcon()
        {
            var components = new Container();
            var notifyIcon = new NotifyIcon(components)
            {
                ContextMenuStrip = CreateMenuStrip(),
                Icon = Resources.EliteIcon,
                Text = "Elite Log Agent",
                Visible = true,
            };
            notifyIcon.BalloonTipClicked += (o, e) => OpenSettings();
            notifyIcon.DoubleClick += (o,e) => OpenSettings();
            return notifyIcon;
        }

        private ContextMenuStrip CreateMenuStrip()
        {
            var menuStrip = new ContextMenuStrip();
            menuStrip.Items.Add(new ToolStripLabel($"Version: {AppInfo.Version}") { ForeColor = SystemColors.ControlDark });
            //menuStrip.Items.Add("Upload latest log", Resources.EliteIcon.ToBitmap(), (o, e) => uploadController.UploadLatestFiles(1, false));
            //menuStrip.Items.Add("Upload 5 latest logs", Resources.EliteIcon.ToBitmap(), (o, e) => uploadController.UploadLatestFiles(5, false));
            menuStrip.Items.Add(ToolStripSeparatorLeft);
            menuStrip.Items.Add("Settings", Resources.EliteIcon.ToBitmap(), (o,e) => OpenSettings());
            menuStrip.Items.Add("Report issue", Resources.GitHub.ToBitmap(), (o, e) => OpenReportIssueLink());
            //menuStrip.Items.Add("About", SystemIcons.Information.ToBitmap(), (o, e) => { using (var form = new AboutForm()) { form.ShowDialog(); } });
            menuStrip.Items.Add(ToolStripSeparatorLeft);
            menuStrip.Items.Add("Exit", SystemIcons.Error.ToBitmap(), (o, e) => Application.Exit());
            return menuStrip;
        }

        private void OpenReportIssueLink() => Process.Start(Resources.GitHubReportIssueLink);

        private SettingsForm form;

        public void OpenSettings()
        {
            if (form != null)
            {
                form.BringToFront();
                return;
            }

            try
            {
                using (form = new SettingsForm()
                {
                    Plugins = pluginManager.LoadedPlugins.ToList(),
                    Provider = settingsProvider,
                    MessageBroker = messageBroker
                })
                {
                    form.ShowDialog();
                }
            }
            finally
            {
                form = null;
            }
        }

        private static ToolStripSeparator ToolStripSeparatorLeft => new ToolStripSeparator { Alignment = ToolStripItemAlignment.Left };

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        public TrayIconController(IPluginManager pluginManager, ISettingsProvider settingsProvider, IMessageBroker messageBroker)
        {
            trayIcon = CreateTrayIcon();
            this.pluginManager = pluginManager;
            this.messageBroker = messageBroker;
            this.settingsProvider = settingsProvider;
        }

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
