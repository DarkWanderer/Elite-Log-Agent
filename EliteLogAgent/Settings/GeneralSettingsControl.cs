using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DW.ELA.Interfaces;
using DW.ELA.Interfaces.Settings;
using NLog;

namespace EliteLogAgent.Settings
{
    public partial class GeneralSettingsControl : AbstractSettingsControl
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private CheckBox checkboxAutostartApplication;
        private CheckBox reportErrorsCheckbox;
        private Label logLevelLabel;
        private ComboBox logLevelComboBox;

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
            logLevelComboBox.SelectedItem = logLevelComboBox.Items.OfType<LogLevel>().SingleOrDefault(t => t.Name == GlobalSettings.LogLevel) ?? LogLevel.Info;
            reportErrorsCheckbox.Checked = GlobalSettings.ReportErrorsToCloud;
        }

        private void ReportErrorsCheckbox_CheckedChanged(object sender, EventArgs e) => GlobalSettings.ReportErrorsToCloud = reportErrorsCheckbox.Checked;

        private void LogLevelComboBox_SelectedIndexChanged(object sender, EventArgs e) => GlobalSettings.LogLevel = logLevelComboBox.SelectedItem.ToString();

        private void CheckboxAutostartApplication_CheckedChanged(object sender, EventArgs e)
        {
            if (AutorunManager != null)
                AutorunManager.AutorunEnabled = checkboxAutostartApplication.Checked;
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            checkboxAutostartApplication = new CheckBox();
            reportErrorsCheckbox = new CheckBox();
            logLevelLabel = new Label();
            logLevelComboBox = new ComboBox();
            SuspendLayout();
            // 
            // checkboxAutostartApplication
            // 
            checkboxAutostartApplication.AutoSize = true;
            checkboxAutostartApplication.Location = new System.Drawing.Point(3, 3);
            checkboxAutostartApplication.Name = "checkboxAutostartApplication";
            checkboxAutostartApplication.Size = new System.Drawing.Size(182, 21);
            checkboxAutostartApplication.TabIndex = 7;
            checkboxAutostartApplication.Text = "Start agent when I log in";
            checkboxAutostartApplication.UseVisualStyleBackColor = true;
            checkboxAutostartApplication.CheckedChanged += new EventHandler(CheckboxAutostartApplication_CheckedChanged);
            // 
            // reportErrorsCheckbox
            // 
            reportErrorsCheckbox.AutoSize = true;
            reportErrorsCheckbox.Location = new System.Drawing.Point(3, 30);
            reportErrorsCheckbox.Name = "reportErrorsCheckbox";
            reportErrorsCheckbox.Size = new System.Drawing.Size(220, 21);
            reportErrorsCheckbox.TabIndex = 8;
            reportErrorsCheckbox.Text = "Report errors to Cloud service";
            reportErrorsCheckbox.UseVisualStyleBackColor = true;
            reportErrorsCheckbox.CheckedChanged += new EventHandler(ReportErrorsCheckbox_CheckedChanged);
            // 
            // logLevelLabel
            // 
            logLevelLabel.AutoSize = true;
            logLevelLabel.Location = new System.Drawing.Point(0, 60);
            logLevelLabel.Name = "logLevelLabel";
            logLevelLabel.Size = new System.Drawing.Size(65, 17);
            logLevelLabel.TabIndex = 9;
            logLevelLabel.Text = "Log level";
            // 
            // logLevelComboBox
            // 
            logLevelComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            logLevelComboBox.FormattingEnabled = true;
            logLevelComboBox.Location = new System.Drawing.Point(71, 57);
            logLevelComboBox.Name = "logLevelComboBox";
            logLevelComboBox.Size = new System.Drawing.Size(149, 24);
            logLevelComboBox.TabIndex = 10;
            logLevelComboBox.SelectedIndexChanged += new EventHandler(LogLevelComboBox_SelectedIndexChanged);
            // 
            // GeneralSettingsControl
            // 
            AutoScaleMode = AutoScaleMode.Inherit;
            Controls.Add(logLevelComboBox);
            Controls.Add(logLevelLabel);
            Controls.Add(reportErrorsCheckbox);
            Controls.Add(checkboxAutostartApplication);
            Name = "GeneralSettingsControl";
            Size = new System.Drawing.Size(417, 219);
            ResumeLayout(false);
            PerformLayout();

        }
    }
}
