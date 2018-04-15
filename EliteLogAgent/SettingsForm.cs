using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controller;

namespace TrayAgent
{
    public partial class SettingsForm : Form
    {
        internal IPersistentSettingsStorage SettingsProvider { get; set; }
        internal MessageBroker MessageBroker { get; set; }

        public SettingsForm()
        {
            InitializeComponent();
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            label1.Text = "Version: " + fvi.FileVersion;
        }

        private async void uploadLatestDataButton_Click(object sender, EventArgs e)
        {
            uploadLatestDataButton.Enabled = false;
            await Task.Factory.StartNew(UploadLatestData);
            uploadLatestDataButton.Enabled = true;
        }

        private void UploadLatestData()
        {
            var logEventSource = new LogBurstPlayer();
            using (var subscription = logEventSource.Subscribe(MessageBroker))
                logEventSource.Play();
        }
    }
}
