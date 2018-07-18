using DW.ELA.Utility;
using DW.ELA.Interfaces;
using NLog;
using System;
using System.Windows.Forms;
using Utility;
using System.Diagnostics;
using System.Drawing;

namespace ELA.Plugin.EDSM
{
    internal class EdsmSettingsControl : AbstractSettingsControl
    {
        private TextBox edsmApiKeyTextBox;
        private Button testCredentialsButton;
        private Label credentialsStatusLabel;
        private Button button1;
        private CheckBox apiKeyValidatedCheckbox;
        private LinkLabel apiKeyLabel;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public EdsmSettingsControl()
        {
            Load += EdsmSettingsControl_Load;
            InitializeComponent();
        }

        private void EdsmSettingsControl_Load(object sender, EventArgs e) => ReloadSettings();

        private void ReloadSettings()
        {
            edsmApiKeyTextBox.Text = Settings.ApiKey;
            apiKeyValidatedCheckbox.Checked = Settings.Verified;
        }

        internal EdsmSettings Settings
        {
            get => new PluginSettingsFacade<EdsmSettings>(EdsmPlugin.CPluginId, GlobalSettings).Settings;
            set => new PluginSettingsFacade<EdsmSettings>(EdsmPlugin.CPluginId, GlobalSettings).Settings = value;
        }

        internal EdsmSettings ActualSettings
        {
            get
            {
                return new EdsmSettings()
                {
                    ApiKey = edsmApiKeyTextBox.Text,
                    Verified = apiKeyValidatedCheckbox.Checked
                };
            }
            set
            {
                edsmApiKeyTextBox.Text = value.ApiKey;
                apiKeyValidatedCheckbox.Checked = value.Verified;
            }
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.edsmApiKeyTextBox = new System.Windows.Forms.TextBox();
            this.testCredentialsButton = new System.Windows.Forms.Button();
            this.credentialsStatusLabel = new System.Windows.Forms.Label();
            this.apiKeyValidatedCheckbox = new System.Windows.Forms.CheckBox();
            this.apiKeyLabel = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(0, 0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // edsmApiKeyTextBox
            // 
            this.edsmApiKeyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.edsmApiKeyTextBox.Location = new System.Drawing.Point(87, 3);
            this.edsmApiKeyTextBox.Name = "edsmApiKeyTextBox";
            this.edsmApiKeyTextBox.Size = new System.Drawing.Size(284, 20);
            this.edsmApiKeyTextBox.TabIndex = 1;
            this.edsmApiKeyTextBox.Text = "EDSM API Key";
            this.edsmApiKeyTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.edsmApiKeyTextBox.TextChanged += new System.EventHandler(this.edsmApiKeyTextBox_TextChanged);
            // 
            // testCredentialsButton
            // 
            this.testCredentialsButton.Location = new System.Drawing.Point(3, 27);
            this.testCredentialsButton.Name = "testCredentialsButton";
            this.testCredentialsButton.Size = new System.Drawing.Size(125, 23);
            this.testCredentialsButton.TabIndex = 2;
            this.testCredentialsButton.Text = "Validate";
            this.testCredentialsButton.UseVisualStyleBackColor = true;
            this.testCredentialsButton.Click += new System.EventHandler(this.testCredentialsButton_Click);
            // 
            // credentialsStatusLabel
            // 
            this.credentialsStatusLabel.AutoSize = true;
            this.credentialsStatusLabel.Location = new System.Drawing.Point(3, 55);
            this.credentialsStatusLabel.Name = "credentialsStatusLabel";
            this.credentialsStatusLabel.Size = new System.Drawing.Size(69, 13);
            this.credentialsStatusLabel.TabIndex = 3;
            this.credentialsStatusLabel.Text = "Not checked";
            // 
            // apiKeyValidatedCheckbox
            // 
            this.apiKeyValidatedCheckbox.AutoSize = true;
            this.apiKeyValidatedCheckbox.Enabled = false;
            this.apiKeyValidatedCheckbox.Location = new System.Drawing.Point(134, 31);
            this.apiKeyValidatedCheckbox.Name = "apiKeyValidatedCheckbox";
            this.apiKeyValidatedCheckbox.Size = new System.Drawing.Size(184, 17);
            this.apiKeyValidatedCheckbox.TabIndex = 4;
            this.apiKeyValidatedCheckbox.Text = "CMDR Name / API Key validated";
            this.apiKeyValidatedCheckbox.UseVisualStyleBackColor = true;
            this.apiKeyValidatedCheckbox.CheckedChanged += new System.EventHandler(this.apiKeyValidatedCheckbox_CheckedChanged);
            // 
            // apiKeyLabel
            // 
            this.apiKeyLabel.AutoSize = true;
            this.apiKeyLabel.Location = new System.Drawing.Point(3, 6);
            this.apiKeyLabel.Name = "apiKeyLabel";
            this.apiKeyLabel.Size = new System.Drawing.Size(81, 13);
            this.apiKeyLabel.TabIndex = 6;
            this.apiKeyLabel.TabStop = true;
            this.apiKeyLabel.Text = "EDSM API key:";
            this.apiKeyLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.apiKeyLabel_LinkClicked);
            // 
            // EdsmSettingsControl
            // 
            this.Controls.Add(this.apiKeyLabel);
            this.Controls.Add(this.apiKeyValidatedCheckbox);
            this.Controls.Add(this.credentialsStatusLabel);
            this.Controls.Add(this.testCredentialsButton);
            this.Controls.Add(this.edsmApiKeyTextBox);
            this.Name = "EdsmSettingsControl";
            this.Size = new System.Drawing.Size(374, 150);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private async void testCredentialsButton_Click(object sender, EventArgs e)
        {
            try
            {
                testCredentialsButton.Enabled = false;
                var apiKey = edsmApiKeyTextBox.Text;
                var apiFacade = new EdsmApiFacade(new ThrottlingRestClient("https://www.edsm.net/api-commander-v1/get-ranks"), GlobalSettings.CommanderName, apiKey);
                var result = await apiFacade.GetCommanderRanks();
                credentialsStatusLabel.Text = "Success, combat rank: " + result["ranksVerbose"]["Combat"]?.ToString();
                apiKeyValidatedCheckbox.Checked = true;
            }
            catch (Exception ex)
            {
                credentialsStatusLabel.Text = ex.Message;
                apiKeyValidatedCheckbox.Checked = false;
            }
            finally
            {
                testCredentialsButton.Enabled = true;
            }
        }

        public override void SaveSettings() => Settings = new EdsmSettings() { ApiKey = edsmApiKeyTextBox.Text, Verified = apiKeyValidatedCheckbox.Checked };

        private void apiKeyLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.edsm.net/en/settings/api");
        }

        private void edsmApiKeyTextBox_TextChanged(object sender, EventArgs e)
        {
            edsmApiKeyTextBox.BackColor = Color.LightGray;
        }

        private void apiKeyValidatedCheckbox_CheckedChanged(object sender, EventArgs e) => edsmApiKeyTextBox.BackColor = apiKeyValidatedCheckbox.Checked ? Color.LightGreen : Color.LightSalmon;
    }
}