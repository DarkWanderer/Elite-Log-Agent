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
            inaraApiKeyTextBox = new TextBox();
            testCredentialsButton = new Button();
            credentialsStatusLabel = new Label();
            apiKeyValidatedCheckbox = new CheckBox();
            apiKeyLabel = new LinkLabel();
            SuspendLayout();

            // inaraApiKeyTextBox
            inaraApiKeyTextBox.Anchor = ((AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            inaraApiKeyTextBox.Location = new Point(114, 3);
            inaraApiKeyTextBox.Name = "inaraApiKeyTextBox";
            inaraApiKeyTextBox.Size = new Size(303, 22);
            inaraApiKeyTextBox.TabIndex = 1;
            inaraApiKeyTextBox.Text = "Inara API Key";
            inaraApiKeyTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            // testCredentialsButton
            testCredentialsButton.Location = new Point(3, 27);
            testCredentialsButton.Name = "testCredentialsButton";
            testCredentialsButton.Size = new Size(122, 23);
            testCredentialsButton.TabIndex = 2;
            testCredentialsButton.Text = "Validate";
            testCredentialsButton.UseVisualStyleBackColor = true;
            testCredentialsButton.Click += new EventHandler(TestCredentialsButton_Click);

            // credentialsStatusLabel
            credentialsStatusLabel.AutoSize = true;
            credentialsStatusLabel.Location = new Point(3, 53);
            credentialsStatusLabel.Name = "credentialsStatusLabel";
            credentialsStatusLabel.Size = new Size(87, 17);
            credentialsStatusLabel.TabIndex = 3;
            credentialsStatusLabel.Text = "Not checked";

            // apiKeyValidatedCheckbox
            apiKeyValidatedCheckbox.AutoSize = true;
            apiKeyValidatedCheckbox.Enabled = false;
            apiKeyValidatedCheckbox.Location = new Point(131, 31);
            apiKeyValidatedCheckbox.Name = "apiKeyValidatedCheckbox";
            apiKeyValidatedCheckbox.Size = new Size(233, 21);
            apiKeyValidatedCheckbox.TabIndex = 7;
            apiKeyValidatedCheckbox.Text = "CMDR Name / API Key validated";
            apiKeyValidatedCheckbox.UseVisualStyleBackColor = true;
            apiKeyValidatedCheckbox.CheckedChanged += new EventHandler(ApiKeyValidatedCheckbox_CheckedChanged);

            // apiKeyLabel
            apiKeyLabel.AutoSize = true;
            apiKeyLabel.Location = new Point(3, 6);
            apiKeyLabel.Name = "apiKeyLabel";
            apiKeyLabel.Size = new Size(105, 17);
            apiKeyLabel.TabIndex = 8;
            apiKeyLabel.TabStop = true;
            apiKeyLabel.Text = "INARA Api Key:";
            apiKeyLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(ApiKeyLabel_LinkClicked);

            // InaraSettingsControl
            Controls.Add(apiKeyLabel);
            Controls.Add(apiKeyValidatedCheckbox);
            Controls.Add(credentialsStatusLabel);
            Controls.Add(testCredentialsButton);
            Controls.Add(inaraApiKeyTextBox);
            Name = "InaraSettingsControl";
            Size = new Size(420, 177);
            ResumeLayout(false);
            PerformLayout();

        }

        private async void TestCredentialsButton_Click(object sender, EventArgs e)
        {
            try
            {
                testCredentialsButton.Enabled = false;
                string apiKey = inaraApiKeyTextBox.Text;
                string cmdrName = GlobalSettings.CommanderName;
                var apiFacade = new InaraApiFacade(RestClient, cmdrName, apiKey);
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
