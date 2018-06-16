using DW.ELA.Interfaces;
using InaraUpdater.Model;
using Interfaces;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Windows.Forms;

namespace InaraUpdater
{
    internal class InaraSettingsControl : AbstractSettingsControl
    {
        private TextBox inaraApiKeyTextBox;
        private Button testCredentialsButton;
        private Label credentialsStatusLabel;
        private Button button1;
        private Label label1;
        private CheckBox apiKeyValidatedCheckbox;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public InaraSettingsControl()
        {
            Load += InaraSettingsControl_Load;
            InitializeComponent();
        }

        private void InaraSettingsControl_Load(object sender, EventArgs e) => ReloadSettings();

        private void ReloadSettings()
        {
            inaraApiKeyTextBox.Text = Settings.ApiKey;
            apiKeyValidatedCheckbox.Checked = Settings.Verified;
        }

        internal InaraSettings Settings
        {
            get => new PluginSettingsFacade<InaraSettings>(InaraPlugin.CPluginId, GlobalSettings).Settings;
            set => new PluginSettingsFacade<InaraSettings>(InaraPlugin.CPluginId, GlobalSettings).Settings = value;
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.inaraApiKeyTextBox = new System.Windows.Forms.TextBox();
            this.testCredentialsButton = new System.Windows.Forms.Button();
            this.credentialsStatusLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.apiKeyValidatedCheckbox = new System.Windows.Forms.CheckBox();
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
            // inaraApiKeyTextBox
            // 
            this.inaraApiKeyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inaraApiKeyTextBox.Location = new System.Drawing.Point(52, 3);
            this.inaraApiKeyTextBox.Name = "inaraApiKeyTextBox";
            this.inaraApiKeyTextBox.Size = new System.Drawing.Size(263, 20);
            this.inaraApiKeyTextBox.TabIndex = 1;
            this.inaraApiKeyTextBox.Text = "Inara API Key";
            this.inaraApiKeyTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.inaraApiKeyTextBox.TextChanged += new System.EventHandler(this.inaraApiKeyTextBox_TextChanged);
            // 
            // testCredentialsButton
            // 
            this.testCredentialsButton.Location = new System.Drawing.Point(3, 27);
            this.testCredentialsButton.Name = "testCredentialsButton";
            this.testCredentialsButton.Size = new System.Drawing.Size(122, 23);
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Api Key";
            // 
            // apiKeyValidatedCheckbox
            // 
            this.apiKeyValidatedCheckbox.AutoSize = true;
            this.apiKeyValidatedCheckbox.Enabled = false;
            this.apiKeyValidatedCheckbox.Location = new System.Drawing.Point(131, 31);
            this.apiKeyValidatedCheckbox.Name = "apiKeyValidatedCheckbox";
            this.apiKeyValidatedCheckbox.Size = new System.Drawing.Size(184, 17);
            this.apiKeyValidatedCheckbox.TabIndex = 7;
            this.apiKeyValidatedCheckbox.Text = "CMDR Name / API Key validated";
            this.apiKeyValidatedCheckbox.UseVisualStyleBackColor = true;
            this.apiKeyValidatedCheckbox.CheckedChanged += new System.EventHandler(this.apiKeyValidatedCheckbox_CheckedChanged);
            // 
            // InaraSettingsControl
            // 
            this.Controls.Add(this.apiKeyValidatedCheckbox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.credentialsStatusLabel);
            this.Controls.Add(this.testCredentialsButton);
            this.Controls.Add(this.inaraApiKeyTextBox);
            this.Name = "InaraSettingsControl";
            this.Size = new System.Drawing.Size(318, 148);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private async void testCredentialsButton_Click(object sender, EventArgs e)
        {
            try
            {
                testCredentialsButton.Enabled = false;
                var apiKey = inaraApiKeyTextBox.Text;
                var cmdrName = GlobalSettings.CommanderName;
                var apiFacade = new InaraApiFacade(InaraPlugin.RestClient, apiKey, cmdrName);
                var @event = new ApiEvent("getCommanderProfile") { EventData = new { searchName = cmdrName }, Timestamp = DateTime.Now };
                var result = await apiFacade.ApiCall(@event);
                credentialsStatusLabel.Text = "Success, inara ID: " + (result.EventData as JObject)["userID"]?.ToString();
                apiKeyValidatedCheckbox.Checked = true;
            }
            catch (Exception ex)
            {
                apiKeyValidatedCheckbox.Checked = false;
                credentialsStatusLabel.Text = ex.Message;
            }
            finally
            {
                testCredentialsButton.Enabled = true;
            }
        }

        private void inaraApiKeyTextBox_TextChanged(object sender, EventArgs e)
        {
            Settings.ApiKey = inaraApiKeyTextBox.Text;
        }

        private void apiKeyValidatedCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Verified = apiKeyValidatedCheckbox.Checked;
        }
    }
}