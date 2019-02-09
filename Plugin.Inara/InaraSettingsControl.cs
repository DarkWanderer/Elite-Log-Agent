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
        private CheckBox autoManageFriendsCheckbox;
        private LinkLabel apiKeyLabel;

        public InaraSettingsControl()
        {
            Load += InaraSettingsControl_Load;
            InitializeComponent();
        }

        public override void SaveSettings() => Settings = new InaraSettings() { ApiKey = inaraApiKeyTextBox.Text, Verified = apiKeyValidatedCheckbox.Checked, ManageFriends = autoManageFriendsCheckbox.Checked };

        private void InaraSettingsControl_Load(object sender, EventArgs e) => ReloadSettings();

        private void ReloadSettings()
        {
            inaraApiKeyTextBox.Text = Settings.ApiKey;
            apiKeyValidatedCheckbox.Checked = Settings.Verified;
            autoManageFriendsCheckbox.Checked = Settings.ManageFriends;
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
            autoManageFriendsCheckbox = new CheckBox();
            SuspendLayout();
            //
            // inaraApiKeyTextBox
            //
            inaraApiKeyTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            inaraApiKeyTextBox.Location = new Point(114, 3);
            inaraApiKeyTextBox.Name = "inaraApiKeyTextBox";
            inaraApiKeyTextBox.Size = new Size(244, 22);
            inaraApiKeyTextBox.TabIndex = 1;
            inaraApiKeyTextBox.Text = "Inara API Key";
            inaraApiKeyTextBox.TextAlign = HorizontalAlignment.Center;
            //
            // testCredentialsButton
            //
            testCredentialsButton.Location = new Point(3, 27);
            testCredentialsButton.Name = "testCredentialsButton";
            testCredentialsButton.Size = new Size(122, 23);
            testCredentialsButton.TabIndex = 2;
            testCredentialsButton.Text = "Validate";
            testCredentialsButton.UseVisualStyleBackColor = true;
            testCredentialsButton.Click += new EventHandler(testCredentialsButton_Click);
            //
            // credentialsStatusLabel
            //
            credentialsStatusLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            credentialsStatusLabel.AutoSize = true;
            credentialsStatusLabel.Location = new Point(271, 55);
            credentialsStatusLabel.Name = "credentialsStatusLabel";
            credentialsStatusLabel.Size = new Size(87, 17);
            credentialsStatusLabel.TabIndex = 3;
            credentialsStatusLabel.Text = "Not checked";
            //
            // apiKeyValidatedCheckbox
            //
            apiKeyValidatedCheckbox.AutoSize = true;
            apiKeyValidatedCheckbox.Enabled = false;
            apiKeyValidatedCheckbox.Location = new Point(131, 31);
            apiKeyValidatedCheckbox.Name = "apiKeyValidatedCheckbox";
            apiKeyValidatedCheckbox.Size = new Size(233, 21);
            apiKeyValidatedCheckbox.TabIndex = 7;
            apiKeyValidatedCheckbox.Text = "CMDR Name / API Key validated";
            apiKeyValidatedCheckbox.UseVisualStyleBackColor = true;
            apiKeyValidatedCheckbox.CheckedChanged += new EventHandler(apiKeyValidatedCheckbox_CheckedChanged);
            //
            // apiKeyLabel
            //
            apiKeyLabel.AutoSize = true;
            apiKeyLabel.Location = new Point(3, 6);
            apiKeyLabel.Name = "apiKeyLabel";
            apiKeyLabel.Size = new Size(105, 17);
            apiKeyLabel.TabIndex = 8;
            apiKeyLabel.TabStop = true;
            apiKeyLabel.Text = "INARA Api Key:";
            apiKeyLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(apiKeyLabel_LinkClicked);
            //
            // autoManageFriendsCheckbox
            //
            autoManageFriendsCheckbox.AutoSize = true;
            autoManageFriendsCheckbox.Location = new Point(3, 80);
            autoManageFriendsCheckbox.Name = "autoManageFriendsCheckbox";
            autoManageFriendsCheckbox.Size = new Size(188, 21);
            autoManageFriendsCheckbox.TabIndex = 9;
            autoManageFriendsCheckbox.Text = "Add friends automatically";
            autoManageFriendsCheckbox.UseVisualStyleBackColor = true;
            //
            // InaraSettingsControl
            //
            Controls.Add(autoManageFriendsCheckbox);
            Controls.Add(apiKeyLabel);
            Controls.Add(apiKeyValidatedCheckbox);
            Controls.Add(credentialsStatusLabel);
            Controls.Add(testCredentialsButton);
            Controls.Add(inaraApiKeyTextBox);
            Name = "InaraSettingsControl";
            Size = new Size(361, 155);
            ResumeLayout(false);
            PerformLayout();
        }

        private async void testCredentialsButton_Click(object sender, EventArgs e)
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

        private void apiKeyValidatedCheckbox_CheckedChanged(object sender, EventArgs e) => inaraApiKeyTextBox.BackColor = apiKeyValidatedCheckbox.Checked ? Color.LightGreen : Color.LightSalmon;

        private void apiKeyLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) => Process.Start("https://inara.cz/settings-api/");
    }
}
