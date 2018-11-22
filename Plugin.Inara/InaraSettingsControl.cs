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
        private TextBox inaraApiKeyTextBox;
        private Button testCredentialsButton;
        private Label credentialsStatusLabel;
        private Button button1;
        private CheckBox apiKeyValidatedCheckbox;
        private LinkLabel apiKeyLabel;
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
            button1 = new System.Windows.Forms.Button();
            inaraApiKeyTextBox = new System.Windows.Forms.TextBox();
            testCredentialsButton = new System.Windows.Forms.Button();
            credentialsStatusLabel = new System.Windows.Forms.Label();
            apiKeyValidatedCheckbox = new System.Windows.Forms.CheckBox();
            apiKeyLabel = new System.Windows.Forms.LinkLabel();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(0, 0);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            // 
            // inaraApiKeyTextBox
            // 
            inaraApiKeyTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left
            | AnchorStyles.Right;
            inaraApiKeyTextBox.Location = new System.Drawing.Point(91, 3);
            inaraApiKeyTextBox.Name = "inaraApiKeyTextBox";
            inaraApiKeyTextBox.Size = new System.Drawing.Size(224, 20);
            inaraApiKeyTextBox.TabIndex = 1;
            inaraApiKeyTextBox.Text = "Inara API Key";
            inaraApiKeyTextBox.TextAlign = HorizontalAlignment.Center;
            // 
            // testCredentialsButton
            // 
            testCredentialsButton.Location = new System.Drawing.Point(3, 27);
            testCredentialsButton.Name = "testCredentialsButton";
            testCredentialsButton.Size = new System.Drawing.Size(122, 23);
            testCredentialsButton.TabIndex = 2;
            testCredentialsButton.Text = "Validate";
            testCredentialsButton.UseVisualStyleBackColor = true;
            testCredentialsButton.Click += new System.EventHandler(testCredentialsButton_Click);
            // 
            // credentialsStatusLabel
            // 
            credentialsStatusLabel.AutoSize = true;
            credentialsStatusLabel.Location = new System.Drawing.Point(3, 55);
            credentialsStatusLabel.Name = "credentialsStatusLabel";
            credentialsStatusLabel.Size = new System.Drawing.Size(69, 13);
            credentialsStatusLabel.TabIndex = 3;
            credentialsStatusLabel.Text = "Not checked";
            // 
            // apiKeyValidatedCheckbox
            // 
            apiKeyValidatedCheckbox.AutoSize = true;
            apiKeyValidatedCheckbox.Enabled = false;
            apiKeyValidatedCheckbox.Location = new System.Drawing.Point(131, 31);
            apiKeyValidatedCheckbox.Name = "apiKeyValidatedCheckbox";
            apiKeyValidatedCheckbox.Size = new System.Drawing.Size(184, 17);
            apiKeyValidatedCheckbox.TabIndex = 7;
            apiKeyValidatedCheckbox.Text = "CMDR Name / API Key validated";
            apiKeyValidatedCheckbox.UseVisualStyleBackColor = true;
            apiKeyValidatedCheckbox.CheckedChanged += new System.EventHandler(apiKeyValidatedCheckbox_CheckedChanged);
            // 
            // apiKeyLabel
            // 
            apiKeyLabel.AutoSize = true;
            apiKeyLabel.Location = new System.Drawing.Point(3, 6);
            apiKeyLabel.Name = "apiKeyLabel";
            apiKeyLabel.Size = new System.Drawing.Size(82, 13);
            apiKeyLabel.TabIndex = 8;
            apiKeyLabel.TabStop = true;
            apiKeyLabel.Text = "INARA Api Key:";
            apiKeyLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(apiKeyLabel_LinkClicked);
            // 
            // InaraSettingsControl
            // 
            Controls.Add(apiKeyLabel);
            Controls.Add(apiKeyValidatedCheckbox);
            Controls.Add(credentialsStatusLabel);
            Controls.Add(testCredentialsButton);
            Controls.Add(inaraApiKeyTextBox);
            Name = "InaraSettingsControl";
            Size = new System.Drawing.Size(318, 148);
            ResumeLayout(false);
            PerformLayout();
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

        public override void SaveSettings() => Settings = new InaraSettings() { ApiKey = inaraApiKeyTextBox.Text, Verified = apiKeyValidatedCheckbox.Checked };

        private void apiKeyValidatedCheckbox_CheckedChanged(object sender, EventArgs e) => inaraApiKeyTextBox.BackColor = apiKeyValidatedCheckbox.Checked ? Color.LightGreen : Color.LightSalmon;

        private void apiKeyLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start("https://inara.cz/settings-api/");
    }
}