namespace EliteLogAgent.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using DW.ELA.Controller;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Events;
    using DW.ELA.Interfaces.Settings;
    using DW.ELA.Utility.Extensions;
    using NLog;

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
            this.checkboxAutostartApplication = new System.Windows.Forms.CheckBox();
            this.reportErrorsCheckbox = new System.Windows.Forms.CheckBox();
            this.logLevelLabel = new System.Windows.Forms.Label();
            this.logLevelComboBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // checkboxAutostartApplication
            // 
            this.checkboxAutostartApplication.AutoSize = true;
            this.checkboxAutostartApplication.Location = new System.Drawing.Point(3, 3);
            this.checkboxAutostartApplication.Name = "checkboxAutostartApplication";
            this.checkboxAutostartApplication.Size = new System.Drawing.Size(186, 21);
            this.checkboxAutostartApplication.TabIndex = 7;
            this.checkboxAutostartApplication.Text = "Autorun agent on sign-in";
            this.checkboxAutostartApplication.UseVisualStyleBackColor = true;
            this.checkboxAutostartApplication.CheckedChanged += new System.EventHandler(this.CheckboxAutostartApplication_CheckedChanged);
            // 
            // reportErrorsCheckbox
            // 
            this.reportErrorsCheckbox.AutoSize = true;
            this.reportErrorsCheckbox.Location = new System.Drawing.Point(3, 30);
            this.reportErrorsCheckbox.Name = "reportErrorsCheckbox";
            this.reportErrorsCheckbox.Size = new System.Drawing.Size(220, 21);
            this.reportErrorsCheckbox.TabIndex = 8;
            this.reportErrorsCheckbox.Text = "Report errors to Cloud service";
            this.reportErrorsCheckbox.UseVisualStyleBackColor = true;
            this.reportErrorsCheckbox.CheckedChanged += new System.EventHandler(this.ReportErrorsCheckbox_CheckedChanged);
            // 
            // logLevelLabel
            // 
            this.logLevelLabel.AutoSize = true;
            this.logLevelLabel.Location = new System.Drawing.Point(0, 60);
            this.logLevelLabel.Name = "logLevelLabel";
            this.logLevelLabel.Size = new System.Drawing.Size(65, 17);
            this.logLevelLabel.TabIndex = 9;
            this.logLevelLabel.Text = "Log level";
            // 
            // logLevelComboBox
            // 
            this.logLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.logLevelComboBox.FormattingEnabled = true;
            this.logLevelComboBox.Location = new System.Drawing.Point(71, 57);
            this.logLevelComboBox.Name = "logLevelComboBox";
            this.logLevelComboBox.Size = new System.Drawing.Size(149, 24);
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
            this.Name = "GeneralSettingsControl";
            this.Size = new System.Drawing.Size(417, 219);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
