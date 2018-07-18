using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using DW.ELA.Interfaces;
using Utility.Extensions;
using Controller;
using DW.ELA.Interfaces.Settings;
using NLog;
using System.Linq;
using EliteLogAgent.Autorun;
using DW.ELA.Interfaces.Events;

namespace EliteLogAgent.Settings
{
    public partial class GeneralSettingsControl : AbstractSettingsControl
    {
        private static ILogger Logger = LogManager.GetCurrentClassLogger();
        private const int uploadFileCount = 5;

        public GeneralSettingsControl()
        {
            Load += GeneralSettingsControl_Load;
            InitializeComponent();
        }

        private GlobalSettings Settings { get => GlobalSettings; set => GlobalSettings = value; }

        private void GeneralSettingsControl_Load(object sender, EventArgs e)
        {
            logLevelComboBox.Items.AddRange(LogLevel.AllLevels.ToArray());
            ReloadSettings();
        }

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

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdrNameLabel = new System.Windows.Forms.Label();
            this.cmdrNameTextBox = new System.Windows.Forms.TextBox();
            this.uploadLatestDataButton = new System.Windows.Forms.Button();
            this.autodetectCmdrNameButton = new System.Windows.Forms.Button();
            this.checkboxAutostartApplication = new System.Windows.Forms.CheckBox();
            this.reportErrorsCheckbox = new System.Windows.Forms.CheckBox();
            this.logLevelLabel = new System.Windows.Forms.Label();
            this.logLevelComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cmdrNameLabel
            // 
            this.cmdrNameLabel.AutoSize = true;
            this.cmdrNameLabel.Location = new System.Drawing.Point(3, 6);
            this.cmdrNameLabel.Name = "cmdrNameLabel";
            this.cmdrNameLabel.Size = new System.Drawing.Size(73, 13);
            this.cmdrNameLabel.TabIndex = 5;
            this.cmdrNameLabel.Text = "CMDR Name:";
            // 
            // cmdrNameTextBox
            // 
            this.cmdrNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmdrNameTextBox.Location = new System.Drawing.Point(82, 3);
            this.cmdrNameTextBox.Name = "cmdrNameTextBox";
            this.cmdrNameTextBox.Size = new System.Drawing.Size(332, 20);
            this.cmdrNameTextBox.TabIndex = 4;
            this.cmdrNameTextBox.Text = "Commander Name";
            this.cmdrNameTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cmdrNameTextBox.TextChanged += new System.EventHandler(this.CmdrNameTextBox_TextChanged);
            // 
            // uploadLatestDataButton
            // 
            this.uploadLatestDataButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.uploadLatestDataButton.Location = new System.Drawing.Point(3, 58);
            this.uploadLatestDataButton.Name = "uploadLatestDataButton";
            this.uploadLatestDataButton.Size = new System.Drawing.Size(411, 23);
            this.uploadLatestDataButton.TabIndex = 3;
            this.uploadLatestDataButton.Text = "Upload last 5 files via all plugins";
            this.uploadLatestDataButton.UseVisualStyleBackColor = true;
            this.uploadLatestDataButton.Click += new System.EventHandler(this.UploadLatestDataButton_Click);
            // 
            // autodetectCmdrNameButton
            // 
            this.autodetectCmdrNameButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.autodetectCmdrNameButton.Location = new System.Drawing.Point(3, 29);
            this.autodetectCmdrNameButton.Name = "autodetectCmdrNameButton";
            this.autodetectCmdrNameButton.Size = new System.Drawing.Size(411, 23);
            this.autodetectCmdrNameButton.TabIndex = 6;
            this.autodetectCmdrNameButton.Text = "Autodetect CMDR Name";
            this.autodetectCmdrNameButton.UseVisualStyleBackColor = true;
            this.autodetectCmdrNameButton.Click += new System.EventHandler(this.AutodetectCmdrNameButton_Click);
            // 
            // checkboxAutostartApplication
            // 
            this.checkboxAutostartApplication.AutoSize = true;
            this.checkboxAutostartApplication.Location = new System.Drawing.Point(6, 87);
            this.checkboxAutostartApplication.Name = "checkboxAutostartApplication";
            this.checkboxAutostartApplication.Size = new System.Drawing.Size(141, 17);
            this.checkboxAutostartApplication.TabIndex = 7;
            this.checkboxAutostartApplication.Text = "Autorun agent on sign-in";
            this.checkboxAutostartApplication.UseVisualStyleBackColor = true;
            this.checkboxAutostartApplication.CheckedChanged += new System.EventHandler(this.CheckboxAutostartApplication_CheckedChanged);
            // 
            // reportErrorsCheckbox
            // 
            this.reportErrorsCheckbox.AutoSize = true;
            this.reportErrorsCheckbox.Location = new System.Drawing.Point(6, 110);
            this.reportErrorsCheckbox.Name = "reportErrorsCheckbox";
            this.reportErrorsCheckbox.Size = new System.Drawing.Size(166, 17);
            this.reportErrorsCheckbox.TabIndex = 8;
            this.reportErrorsCheckbox.Text = "Report errors to Cloud service";
            this.reportErrorsCheckbox.UseVisualStyleBackColor = true;
            this.reportErrorsCheckbox.CheckedChanged += new System.EventHandler(this.ReportErrorsCheckbox_CheckedChanged);
            // 
            // logLevelLabel
            // 
            this.logLevelLabel.AutoSize = true;
            this.logLevelLabel.Location = new System.Drawing.Point(3, 136);
            this.logLevelLabel.Name = "logLevelLabel";
            this.logLevelLabel.Size = new System.Drawing.Size(50, 13);
            this.logLevelLabel.TabIndex = 9;
            this.logLevelLabel.Text = "Log level";
            // 
            // logLevelComboBox
            // 
            this.logLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.logLevelComboBox.FormattingEnabled = true;
            this.logLevelComboBox.Location = new System.Drawing.Point(59, 133);
            this.logLevelComboBox.Name = "logLevelComboBox";
            this.logLevelComboBox.Size = new System.Drawing.Size(113, 21);
            this.logLevelComboBox.TabIndex = 10;
            this.logLevelComboBox.SelectedIndexChanged += new System.EventHandler(this.LogLevelComboBox_SelectedIndexChanged);
            // 
            // GeneralSettingsControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.logLevelComboBox);
            this.Controls.Add(this.logLevelLabel);
            this.Controls.Add(this.reportErrorsCheckbox);
            this.Controls.Add(this.checkboxAutostartApplication);
            this.Controls.Add(this.autodetectCmdrNameButton);
            this.Controls.Add(this.cmdrNameLabel);
            this.Controls.Add(this.cmdrNameTextBox);
            this.Controls.Add(this.uploadLatestDataButton);
            this.Name = "GeneralSettingsControl";
            this.Size = new System.Drawing.Size(417, 219);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private Label cmdrNameLabel;
        private TextBox cmdrNameTextBox;
        private Button uploadLatestDataButton;
        private Button autodetectCmdrNameButton;
        private CheckBox checkboxAutostartApplication;
        private CheckBox reportErrorsCheckbox;
        private Label logLevelLabel;
        private ComboBox logLevelComboBox;
    }
}
