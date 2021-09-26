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
        }

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
            logLevelLabel = new Label();
            logLevelComboBox = new ComboBox();
            SuspendLayout();
            // 
            // checkboxAutostartApplication
            // 
            checkboxAutostartApplication.AutoSize = true;
            checkboxAutostartApplication.Location = new System.Drawing.Point(3, 3);
            checkboxAutostartApplication.Name = "checkboxAutostartApplication";
            checkboxAutostartApplication.Size = new System.Drawing.Size(193, 24);
            checkboxAutostartApplication.TabIndex = 7;
            checkboxAutostartApplication.Text = "Start agent when I log in";
            checkboxAutostartApplication.UseVisualStyleBackColor = true;
            checkboxAutostartApplication.CheckedChanged += new EventHandler(CheckboxAutostartApplication_CheckedChanged);
            // 
            // logLevelLabel
            // 
            logLevelLabel.AutoSize = true;
            logLevelLabel.Location = new System.Drawing.Point(4, 36);
            logLevelLabel.Name = "logLevelLabel";
            logLevelLabel.Size = new System.Drawing.Size(69, 20);
            logLevelLabel.TabIndex = 9;
            logLevelLabel.Text = "Log level";
            // 
            // logLevelComboBox
            // 
            logLevelComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            logLevelComboBox.FormattingEnabled = true;
            logLevelComboBox.Location = new System.Drawing.Point(75, 33);
            logLevelComboBox.Name = "logLevelComboBox";
            logLevelComboBox.Size = new System.Drawing.Size(149, 28);
            logLevelComboBox.TabIndex = 10;
            logLevelComboBox.SelectedIndexChanged += new EventHandler(LogLevelComboBox_SelectedIndexChanged);
            // 
            // GeneralSettingsControl
            // 
            AutoScaleMode = AutoScaleMode.Inherit;
            Controls.Add(logLevelComboBox);
            Controls.Add(logLevelLabel);
            Controls.Add(checkboxAutostartApplication);
            Name = "GeneralSettingsControl";
            Size = new System.Drawing.Size(417, 219);
            ResumeLayout(false);
            PerformLayout();

        }
    }
}
