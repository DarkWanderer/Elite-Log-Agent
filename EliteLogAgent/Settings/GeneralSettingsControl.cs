namespace EliteLogAgent.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using DW.ELA.Controller;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Events;
    using DW.ELA.Interfaces.Settings;
    using DW.ELA.Utility.Extensions;
    using EliteLogAgent.Autorun;
    using NLog;
    using NLog.Fluent;

    public partial class GeneralSettingsControl : AbstractSettingsControl
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private readonly int uploadFileCount = 5;
        private ProgressBar progressBarUploadLatest;

        public GeneralSettingsControl()
        {
            Load += GeneralSettingsControl_Load;
            InitializeComponent();
        }

        public IPlayerStateHistoryRecorder PlayerStateRecorder { get; internal set; }

        public IReadOnlyCollection<IPlugin> Plugins { get; internal set; }

        public IAutorunManager AutorunManager { get; set; }

        private void GeneralSettingsControl_Load(object sender, EventArgs e)
        {
            logLevelComboBox.Items.AddRange(LogLevel.AllLevels.ToArray());
            ReloadSettings();
        }

        private void ReloadSettings()
        {
            checkboxAutostartApplication.Checked = AutorunManager?.AutorunEnabled ?? false;
            cmdrNameTextBox.Text = GlobalSettings.CommanderName;
            logLevelComboBox.SelectedItem = logLevelComboBox.Items.OfType<LogLevel>().SingleOrDefault(t => t.Name == GlobalSettings.LogLevel) ?? LogLevel.Info;
            reportErrorsCheckbox.Checked = GlobalSettings.ReportErrorsToCloud;
        }

        private async void UploadLatestDataButton_Click(object sender, EventArgs e)
        {
            try
            {
                uploadLatestDataButton.Enabled = false;
                progressBarUploadLatest.Maximum = Plugins.Count;
                progressBarUploadLatest.Value = 0;
                foreach (var plugin in Plugins)
                {
                    await Task.Factory.StartNew(() => UploadLatestData(plugin));
                    plugin.FlushQueue();
                    progressBarUploadLatest.Value += 1;
                    await Task.Delay(1); // yield to redraw UI
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing update:\n" + ex.GetStackedErrorMessages(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Error(ex, "Error while uploading data");
            }
            finally
            {
                uploadLatestDataButton.Enabled = true;
            }
        }

        private void UploadLatestData(IPlugin plugin)
        {
            Log.Info("Starting latest data upload");
            var logEventSource = new LogBurstPlayer(new SavedGamesDirectoryHelper().Directory, uploadFileCount);
            var logCounter = new LogEventTypeCounter();

            using (logEventSource.Subscribe(PlayerStateRecorder))
            using (logEventSource.Subscribe(logCounter))
            using (logEventSource.Subscribe(plugin.GetLogObserver()))
            {
                logEventSource.Play();
            }

            Log.Info()
                .Message("Uploaded events")
                .Property("eventsCount", logCounter.EventCounts.Sum(kv => kv.Value))
                .Property("plugin", plugin.PluginName)
                .Write();
        }

        private void ReportErrorsCheckbox_CheckedChanged(object sender, EventArgs e) => GlobalSettings.ReportErrorsToCloud = reportErrorsCheckbox.Checked;

        private void LogLevelComboBox_SelectedIndexChanged(object sender, EventArgs e) => GlobalSettings.LogLevel = logLevelComboBox.SelectedItem.ToString();

        private void AutodetectCmdrNameButton_Click(object sender, EventArgs e)
        {
            try
            {
                autodetectCmdrNameButton.Enabled = false;
                var logEventSource = new LogBurstPlayer(new SavedGamesDirectoryHelper().Directory, 5);
                var cmdrEvent = logEventSource.Events.OfType<Commander>().FirstOrDefault();
                cmdrNameTextBox.Text = cmdrEvent?.Name ?? cmdrNameTextBox.Text;
                Log.Info("Detected commander name as {0}", cmdrEvent?.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error detecting cmdr name:\n" + ex.GetStackedErrorMessages(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Log.Error(ex, "Error while detecting cmdr name");
            }
            finally
            {
                autodetectCmdrNameButton.Enabled = true;
            }
        }

        private void CheckboxAutostartApplication_CheckedChanged(object sender, EventArgs e)
        {
            if (AutorunManager != null)
                AutorunManager.AutorunEnabled = checkboxAutostartApplication.Checked;
        }

        private void CmdrNameTextBox_TextChanged(object sender, EventArgs e) => GlobalSettings.CommanderName = cmdrNameTextBox.Text;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            cmdrNameLabel = new Label();
            cmdrNameTextBox = new TextBox();
            uploadLatestDataButton = new Button();
            autodetectCmdrNameButton = new Button();
            checkboxAutostartApplication = new CheckBox();
            reportErrorsCheckbox = new CheckBox();
            logLevelLabel = new Label();
            logLevelComboBox = new ComboBox();
            progressBarUploadLatest = new ProgressBar();
            SuspendLayout();

            // cmdrNameLabel
            cmdrNameLabel.AutoSize = true;
            cmdrNameLabel.Location = new System.Drawing.Point(3, 6);
            cmdrNameLabel.Name = "cmdrNameLabel";
            cmdrNameLabel.Size = new System.Drawing.Size(93, 17);
            cmdrNameLabel.TabIndex = 5;
            cmdrNameLabel.Text = "CMDR Name:";

            // cmdrNameTextBox
            cmdrNameTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left
            | AnchorStyles.Right;
            cmdrNameTextBox.Location = new System.Drawing.Point(102, 3);
            cmdrNameTextBox.Name = "cmdrNameTextBox";
            cmdrNameTextBox.Size = new System.Drawing.Size(312, 22);
            cmdrNameTextBox.TabIndex = 4;
            cmdrNameTextBox.Text = "Commander Name";
            cmdrNameTextBox.TextAlign = HorizontalAlignment.Center;
            cmdrNameTextBox.TextChanged += new EventHandler(CmdrNameTextBox_TextChanged);

            // uploadLatestDataButton
            uploadLatestDataButton.Anchor = AnchorStyles.Top | AnchorStyles.Left
            | AnchorStyles.Right;
            uploadLatestDataButton.Location = new System.Drawing.Point(3, 58);
            uploadLatestDataButton.Name = "uploadLatestDataButton";
            uploadLatestDataButton.Size = new System.Drawing.Size(411, 23);
            uploadLatestDataButton.TabIndex = 3;
            uploadLatestDataButton.Text = "Upload last 5 files via all plugins";
            uploadLatestDataButton.UseVisualStyleBackColor = true;
            uploadLatestDataButton.Click += new EventHandler(UploadLatestDataButton_Click);

            // autodetectCmdrNameButton
            autodetectCmdrNameButton.Anchor = AnchorStyles.Top | AnchorStyles.Left
            | AnchorStyles.Right;
            autodetectCmdrNameButton.Location = new System.Drawing.Point(3, 29);
            autodetectCmdrNameButton.Name = "autodetectCmdrNameButton";
            autodetectCmdrNameButton.Size = new System.Drawing.Size(411, 23);
            autodetectCmdrNameButton.TabIndex = 6;
            autodetectCmdrNameButton.Text = "Autodetect CMDR Name";
            autodetectCmdrNameButton.UseVisualStyleBackColor = true;
            autodetectCmdrNameButton.Click += new EventHandler(AutodetectCmdrNameButton_Click);

            // checkboxAutostartApplication
            checkboxAutostartApplication.AutoSize = true;
            checkboxAutostartApplication.Location = new System.Drawing.Point(3, 104);
            checkboxAutostartApplication.Name = "checkboxAutostartApplication";
            checkboxAutostartApplication.Size = new System.Drawing.Size(186, 21);
            checkboxAutostartApplication.TabIndex = 7;
            checkboxAutostartApplication.Text = "Autorun agent on sign-in";
            checkboxAutostartApplication.UseVisualStyleBackColor = true;
            checkboxAutostartApplication.CheckedChanged += new EventHandler(CheckboxAutostartApplication_CheckedChanged);

            // reportErrorsCheckbox
            reportErrorsCheckbox.AutoSize = true;
            reportErrorsCheckbox.Location = new System.Drawing.Point(3, 125);
            reportErrorsCheckbox.Name = "reportErrorsCheckbox";
            reportErrorsCheckbox.Size = new System.Drawing.Size(220, 21);
            reportErrorsCheckbox.TabIndex = 8;
            reportErrorsCheckbox.Text = "Report errors to Cloud service";
            reportErrorsCheckbox.UseVisualStyleBackColor = true;
            reportErrorsCheckbox.CheckedChanged += new EventHandler(ReportErrorsCheckbox_CheckedChanged);

            // logLevelLabel
            logLevelLabel.AutoSize = true;
            logLevelLabel.Location = new System.Drawing.Point(3, 155);
            logLevelLabel.Name = "logLevelLabel";
            logLevelLabel.Size = new System.Drawing.Size(65, 17);
            logLevelLabel.TabIndex = 9;
            logLevelLabel.Text = "Log level";

            // logLevelComboBox
            logLevelComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            logLevelComboBox.FormattingEnabled = true;
            logLevelComboBox.Location = new System.Drawing.Point(74, 152);
            logLevelComboBox.Name = "logLevelComboBox";
            logLevelComboBox.Size = new System.Drawing.Size(149, 24);
            logLevelComboBox.TabIndex = 10;
            logLevelComboBox.SelectedIndexChanged += new EventHandler(LogLevelComboBox_SelectedIndexChanged);

            // progressBarUploadLatest
            progressBarUploadLatest.Anchor = AnchorStyles.Top | AnchorStyles.Left
            | AnchorStyles.Right;
            progressBarUploadLatest.Location = new System.Drawing.Point(3, 85);
            progressBarUploadLatest.Name = "progressBarUploadLatest";
            progressBarUploadLatest.Size = new System.Drawing.Size(411, 13);
            progressBarUploadLatest.Style = ProgressBarStyle.Continuous;
            progressBarUploadLatest.TabIndex = 11;

            // GeneralSettingsControl
            AutoScaleMode = AutoScaleMode.Inherit;
            Controls.Add(progressBarUploadLatest);
            Controls.Add(logLevelComboBox);
            Controls.Add(logLevelLabel);
            Controls.Add(reportErrorsCheckbox);
            Controls.Add(checkboxAutostartApplication);
            Controls.Add(autodetectCmdrNameButton);
            Controls.Add(cmdrNameLabel);
            Controls.Add(cmdrNameTextBox);
            Controls.Add(uploadLatestDataButton);
            Name = "GeneralSettingsControl";
            Size = new System.Drawing.Size(417, 219);
            ResumeLayout(false);
            PerformLayout();
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
