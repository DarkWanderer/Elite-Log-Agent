namespace EliteLogAgent.Settings
{
    partial class GeneralSettingsControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

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
            this.cmdrNameTextBox.Size = new System.Drawing.Size(183, 20);
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
            this.uploadLatestDataButton.Size = new System.Drawing.Size(262, 23);
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
            this.autodetectCmdrNameButton.Size = new System.Drawing.Size(262, 23);
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
            this.checkboxAutostartApplication.Size = new System.Drawing.Size(104, 17);
            this.checkboxAutostartApplication.TabIndex = 7;
            this.checkboxAutostartApplication.Text = "Autorun program";
            this.checkboxAutostartApplication.UseVisualStyleBackColor = true;
            this.checkboxAutostartApplication.CheckedChanged += new System.EventHandler(this.CheckboxAutostartApplication_CheckedChanged);
            // 
            // reportErrorsCheckbox
            // 
            this.reportErrorsCheckbox.AutoSize = true;
            this.reportErrorsCheckbox.Location = new System.Drawing.Point(116, 87);
            this.reportErrorsCheckbox.Name = "reportErrorsCheckbox";
            this.reportErrorsCheckbox.Size = new System.Drawing.Size(129, 17);
            this.reportErrorsCheckbox.TabIndex = 8;
            this.reportErrorsCheckbox.Text = "Report errors to Cloud";
            this.reportErrorsCheckbox.UseVisualStyleBackColor = true;
            this.reportErrorsCheckbox.CheckedChanged += new System.EventHandler(this.ReportErrorsCheckbox_CheckedChanged);
            // 
            // logLevelLabel
            // 
            this.logLevelLabel.AutoSize = true;
            this.logLevelLabel.Location = new System.Drawing.Point(3, 107);
            this.logLevelLabel.Name = "logLevelLabel";
            this.logLevelLabel.Size = new System.Drawing.Size(50, 13);
            this.logLevelLabel.TabIndex = 9;
            this.logLevelLabel.Text = "Log level";
            // 
            // logLevelComboBox
            // 
            this.logLevelComboBox.FormattingEnabled = true;
            this.logLevelComboBox.Location = new System.Drawing.Point(59, 104);
            this.logLevelComboBox.Name = "logLevelComboBox";
            this.logLevelComboBox.Size = new System.Drawing.Size(206, 21);
            this.logLevelComboBox.TabIndex = 10;
            this.logLevelComboBox.SelectedIndexChanged += new System.EventHandler(this.LogLevelComboBox_SelectedIndexChanged);
            // 
            // GeneralSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.logLevelComboBox);
            this.Controls.Add(this.logLevelLabel);
            this.Controls.Add(this.reportErrorsCheckbox);
            this.Controls.Add(this.checkboxAutostartApplication);
            this.Controls.Add(this.autodetectCmdrNameButton);
            this.Controls.Add(this.cmdrNameLabel);
            this.Controls.Add(this.cmdrNameTextBox);
            this.Controls.Add(this.uploadLatestDataButton);
            this.Name = "GeneralSettingsControl";
            this.Size = new System.Drawing.Size(268, 184);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label cmdrNameLabel;
        private System.Windows.Forms.TextBox cmdrNameTextBox;
        private System.Windows.Forms.Button uploadLatestDataButton;
        private System.Windows.Forms.Button autodetectCmdrNameButton;
        private System.Windows.Forms.CheckBox checkboxAutostartApplication;
        private System.Windows.Forms.CheckBox reportErrorsCheckbox;
        private System.Windows.Forms.Label logLevelLabel;
        private System.Windows.Forms.ComboBox logLevelComboBox;
    }
}
