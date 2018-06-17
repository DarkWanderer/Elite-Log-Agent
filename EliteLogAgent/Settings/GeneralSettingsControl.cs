using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interfaces;
using Utility.Extensions;
using Controller;
using DW.ELA.Interfaces.Settings;
using NLog;
using System.Linq;
using Newtonsoft.Json.Linq;
using EliteLogAgent.Autorun;

namespace EliteLogAgent.Settings
{
    public partial class GeneralSettingsControl : AbstractSettingsControl
    {
        public GeneralSettingsControl()
        {
            InitializeComponent();
            logLevelComboBox.Items.AddRange(LogLevel.AllLevels.ToArray());
            Load += GeneralSettingsControl_Load;
        }

        private GlobalSettings Settings { get => GlobalSettings; set => GlobalSettings = value; }

        private void GeneralSettingsControl_Load(object sender, EventArgs e) => ReloadSettings();

        private void ReloadSettings()
        {
            checkboxAutostartApplication.Checked = AutorunManager.AutorunEnabled;
            cmdrNameTextBox.Text = GlobalSettings.CommanderName;
            logLevelComboBox.SelectedItem = logLevelComboBox.Items.OfType<LogLevel>().SingleOrDefault(t => t.Name == Settings.LogLevel) ?? LogLevel.Info;
        }

        public IMessageBroker MessageBroker { get; internal set; }

        private async void uploadLatestDataButton_Click(object sender, EventArgs e)
        {
            try
            {
                uploadLatestDataButton.Enabled = false;
                await Task.Factory.StartNew(UploadLatestData);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing update:\n" + ex.GetStackedErrorMessages(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                uploadLatestDataButton.Enabled = true;
            }
        }

        private void UploadLatestData()
        {
            var logEventSource = new LogBurstPlayer(new SavedGamesDirectoryHelper().Directory, 5);
            var logCounter = new LogEventTypeCounter();
            using (logEventSource.Subscribe(logCounter))
            using (logEventSource.Subscribe(MessageBroker))
            {
                logEventSource.Play();
            }
        }

        private void reportErrorsCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.ReportErrorsToCloud = reportErrorsCheckbox.Checked;
        }

        private void logLevelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.LogLevel = logLevelComboBox.SelectedItem.ToString();
        }

        private void autodetectCmdrNameButton_Click(object sender, EventArgs e)
        {
            try
            {
                var logEventSource = new LogBurstPlayer(new SavedGamesDirectoryHelper().Directory, 5);
                var eventFilter = new CommanderNameFilter();
                using (logEventSource.Subscribe(eventFilter))
                    logEventSource.Play();
                cmdrNameTextBox.Text = eventFilter.CmdrName ?? cmdrNameTextBox.Text;
            }
            catch { }
        }

        private class CommanderNameFilter : IObserver<JObject>
        {
            public string CmdrName { get; private set; }

            public void OnCompleted() { }
            public void OnError(Exception error) { }
            public void OnNext(JObject value)
            {
                //"event":"Commander", "Name":"John Doe"
                if (value["event"]?.ToString() != "Commander")
                    return;
                lock (this)
                    CmdrName = value["Name"]?.ToString() ?? CmdrName;
            }
        }

        private void checkboxAutostartApplication_CheckedChanged(object sender, EventArgs e)
        {
            AutorunManager.AutorunEnabled = checkboxAutostartApplication.Checked;
        }
    }
}
