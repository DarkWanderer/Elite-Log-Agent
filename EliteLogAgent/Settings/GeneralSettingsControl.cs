using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interfaces;
using Utility.Extensions;
using Controller;
using DW.ELA.Interfaces.Settings;
using NLog;
using System.Linq;
using EliteLogAgent.Autorun;
using DW.ELA.Interfaces.Events;
using DW.ELA.Interfaces;

namespace EliteLogAgent.Settings
{
    public partial class GeneralSettingsControl : AbstractSettingsControl
    {
        private static ILogger Logger = LogManager.GetCurrentClassLogger();
        private const int uploadFileCount = 5;

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
            reportErrorsCheckbox.Checked = GlobalSettings.ReportErrorsToCloud;
        }

        public IMessageBroker MessageBroker { get; internal set; }

        private async void UploadLatestDataButton_Click(object sender, EventArgs e)
        {
            try
            {
                uploadLatestDataButton.Enabled = false;
                await Task.Factory.StartNew(UploadLatestData);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing update:\n" + ex.GetStackedErrorMessages(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Error(ex, "Error while uploading data");
            }
            finally
            {
                uploadLatestDataButton.Enabled = true;
            }
        }

        private void UploadLatestData()
        {
            Logger.Info("Starting latest data upload");
            var logEventSource = new LogBurstPlayer(new SavedGamesDirectoryHelper().Directory, uploadFileCount);
            var logCounter = new LogEventTypeCounter();

            using (logEventSource.Subscribe(logCounter))
            using (logEventSource.Subscribe(MessageBroker))
            {
                logEventSource.Play();
            }

            Logger.Info("Uploaded {0} events", logCounter.EventCounts.Values.DefaultIfEmpty(0).Sum());
        }

        private void ReportErrorsCheckbox_CheckedChanged(object sender, EventArgs e) => Settings.ReportErrorsToCloud = reportErrorsCheckbox.Checked;

        private void LogLevelComboBox_SelectedIndexChanged(object sender, EventArgs e) => Settings.LogLevel = logLevelComboBox.SelectedItem.ToString();

        private void AutodetectCmdrNameButton_Click(object sender, EventArgs e)
        {
            try
            {
                autodetectCmdrNameButton.Enabled = false;
                var logEventSource = new LogBurstPlayer(new SavedGamesDirectoryHelper().Directory, 5);
                var eventFilter = new CommanderNameFilter();
                using (logEventSource.Subscribe(eventFilter))
                    logEventSource.Play();
                cmdrNameTextBox.Text = eventFilter.CmdrName ?? cmdrNameTextBox.Text;
                Logger.Info("Detected commander name as {0}", eventFilter.CmdrName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error detecting cmdr name:\n" + ex.GetStackedErrorMessages(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Logger.Error(ex, "Error while detecting cmdr name");
            }
            finally
            {
                autodetectCmdrNameButton.Enabled = true;
            }
        }

        private class CommanderNameFilter : IObserver<LogEvent>
        {
            public string CmdrName { get; private set; }

            public void OnCompleted() { }
            public void OnError(Exception error) { }
            public void OnNext(LogEvent @event)
            {
                if (@event is Commander c)
                    CmdrName = c.Name;
            }
        }

        private void CheckboxAutostartApplication_CheckedChanged(object sender, EventArgs e) => AutorunManager.AutorunEnabled = checkboxAutostartApplication.Checked;

        private void CmdrNameTextBox_TextChanged(object sender, EventArgs e) => Settings.CommanderName = cmdrNameTextBox.Text;
    }
}
