namespace DW.ELA.Plugin.Inara
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using DW.ELA.Interfaces;
    using DW.ELA.Plugin.Inara.Model;
    using DW.ELA.Utility;
    using Newtonsoft.Json.Linq;
    using NLog;

    internal class InaraSettingsControl : AbstractSettingsControl
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();
        private TextBox inaraApiKeyTextBox;
        private Button testCredentialsButton;
        private Label credentialsStatusLabel;
        private CheckBox apiKeyValidatedCheckbox;
        private LinkLabel apiKeyLabel;

        public InaraSettingsControl()
        {
            Load += InaraSettingsControl_Load;
            InitializeComponent();
        }

        public override void SaveSettings() => Settings = new InaraSettings() { ApiKey = inaraApiKeyTextBox.Text, Verified = apiKeyValidatedCheckbox.Checked };

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

        internal IRestClient RestClient { get; set; }

        private void InitializeComponent()
        {
            this.inaraApiKeyTextBox = new System.Windows.Forms.TextBox();
            this.testCredentialsButton = new System.Windows.Forms.Button();
            this.credentialsStatusLabel = new System.Windows.Forms.Label();
            this.apiKeyValidatedCheckbox = new System.Windows.Forms.CheckBox();
            this.apiKeyLabel = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // inaraApiKeyTextBox
            // 
            this.inaraApiKeyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inaraApiKeyTextBox.Location = new System.Drawing.Point(114, 3);
            this.inaraApiKeyTextBox.Name = "inaraApiKeyTextBox";
            this.inaraApiKeyTextBox.Size = new System.Drawing.Size(303, 22);
            this.inaraApiKeyTextBox.TabIndex = 1;
            this.inaraApiKeyTextBox.Text = "Inara API Key";
            this.inaraApiKeyTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // testCredentialsButton
            // 
            this.testCredentialsButton.Location = new System.Drawing.Point(3, 27);
            this.testCredentialsButton.Name = "testCredentialsButton";
            this.testCredentialsButton.Size = new System.Drawing.Size(122, 23);
            this.testCredentialsButton.TabIndex = 2;
            this.testCredentialsButton.Text = "Validate";
            this.testCredentialsButton.UseVisualStyleBackColor = true;
            this.testCredentialsButton.Click += new System.EventHandler(this.TestCredentialsButton_Click);
            // 
            // credentialsStatusLabel
            // 
            this.credentialsStatusLabel.AutoSize = true;
            this.credentialsStatusLabel.Location = new System.Drawing.Point(3, 53);
            this.credentialsStatusLabel.Name = "credentialsStatusLabel";
            this.credentialsStatusLabel.Size = new System.Drawing.Size(87, 17);
            this.credentialsStatusLabel.TabIndex = 3;
            this.credentialsStatusLabel.Text = "Not checked";
            // 
            // apiKeyValidatedCheckbox
            // 
            this.apiKeyValidatedCheckbox.AutoSize = true;
            this.apiKeyValidatedCheckbox.Enabled = false;
            this.apiKeyValidatedCheckbox.Location = new System.Drawing.Point(131, 31);
            this.apiKeyValidatedCheckbox.Name = "apiKeyValidatedCheckbox";
            this.apiKeyValidatedCheckbox.Size = new System.Drawing.Size(233, 21);
            this.apiKeyValidatedCheckbox.TabIndex = 7;
            this.apiKeyValidatedCheckbox.Text = "CMDR Name / API Key validated";
            this.apiKeyValidatedCheckbox.UseVisualStyleBackColor = true;
            this.apiKeyValidatedCheckbox.CheckedChanged += new System.EventHandler(this.ApiKeyValidatedCheckbox_CheckedChanged);
            // 
            // apiKeyLabel
            // 
            this.apiKeyLabel.AutoSize = true;
            this.apiKeyLabel.Location = new System.Drawing.Point(3, 6);
            this.apiKeyLabel.Name = "apiKeyLabel";
            this.apiKeyLabel.Size = new System.Drawing.Size(105, 17);
            this.apiKeyLabel.TabIndex = 8;
            this.apiKeyLabel.TabStop = true;
            this.apiKeyLabel.Text = "INARA Api Key:";
            this.apiKeyLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.ApiKeyLabel_LinkClicked);
            // 
            // InaraSettingsControl
            // 
            this.Controls.Add(this.apiKeyLabel);
            this.Controls.Add(this.apiKeyValidatedCheckbox);
            this.Controls.Add(this.credentialsStatusLabel);
            this.Controls.Add(this.testCredentialsButton);
            this.Controls.Add(this.inaraApiKeyTextBox);
            this.Name = "InaraSettingsControl";
            this.Size = new System.Drawing.Size(420, 177);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private async void TestCredentialsButton_Click(object sender, EventArgs e)
        {
            try
            {
                testCredentialsButton.Enabled = false;
                string apiKey = inaraApiKeyTextBox.Text;
                string cmdrName = GlobalSettings.CommanderName;
                var apiFacade = new InaraApiFacade(RestClient, apiKey, cmdrName);
                var @event = new ApiEvent("getCommanderProfile") { EventData = new { searchName = cmdrName }, Timestamp = DateTime.Now };
                var result = await apiFacade.ApiCall(@event);
                credentialsStatusLabel.Text = "Success, inara ID: " + (result.Single().EventData as JObject)["userID"]?.ToString();
                apiKeyValidatedCheckbox.Checked = true;
                Settings.Verified = true;
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

        private void ApiKeyValidatedCheckbox_CheckedChanged(object sender, EventArgs e) => inaraApiKeyTextBox.BackColor = apiKeyValidatedCheckbox.Checked ? Color.LightGreen : Color.LightSalmon;

        private void ApiKeyLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start("https://inara.cz/settings-api/");
    }
}
