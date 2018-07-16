using DW.ELA.Plugin.Inara.Model;
using DW.ELA.Utility;
using DW.ELA.Interfaces;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Linq;
using System.Windows.Forms;

namespace DW.ELA.Plugin.Inara
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
            button1 = new Button();
            inaraApiKeyTextBox = new TextBox();
            testCredentialsButton = new Button();
            credentialsStatusLabel = new Label();
            label1 = new Label();
            apiKeyValidatedCheckbox = new CheckBox();
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
            inaraApiKeyTextBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right);
            inaraApiKeyTextBox.Location = new System.Drawing.Point(52, 3);
            inaraApiKeyTextBox.Name = "inaraApiKeyTextBox";
            inaraApiKeyTextBox.Size = new System.Drawing.Size(263, 20);
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
            testCredentialsButton.Click += new EventHandler(testCredentialsButton_Click);
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
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(3, 6);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(43, 13);
            label1.TabIndex = 6;
            label1.Text = "Api Key";
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
            // 
            // InaraSettingsControl
            // 
            Controls.Add(apiKeyValidatedCheckbox);
            Controls.Add(label1);
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
    }
}