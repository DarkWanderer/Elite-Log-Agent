using DW.ELA.Interfaces;
using DW.ELA.Utility;
using Interfaces;
using NLog;
using System;
using System.Windows.Forms;
using Utility;

namespace ELA.Plugin.EDSM
{
    internal class EdsmSettingsControl : AbstractSettingsControl
    {
        private TextBox edsmApiKeyTextBox;
        private Button testCredentialsButton;
        private Label credentialsStatusLabel;
        private Button button1;
        private CheckBox apiKeyValidatedCheckbox;
        private Label apiKeyLabel;
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
            button1 = new Button();
            edsmApiKeyTextBox = new TextBox();
            testCredentialsButton = new Button();
            credentialsStatusLabel = new Label();
            apiKeyValidatedCheckbox = new CheckBox();
            apiKeyLabel = new Label();
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
            // edsmApiKeyTextBox
            // 
            edsmApiKeyTextBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right);
            edsmApiKeyTextBox.Location = new System.Drawing.Point(52, 3);
            edsmApiKeyTextBox.Name = "edsmApiKeyTextBox";
            edsmApiKeyTextBox.Size = new System.Drawing.Size(264, 20);
            edsmApiKeyTextBox.TabIndex = 1;
            edsmApiKeyTextBox.Text = "EDSM API Key";
            edsmApiKeyTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // testCredentialsButton
            // 
            testCredentialsButton.Location = new System.Drawing.Point(3, 27);
            testCredentialsButton.Name = "testCredentialsButton";
            testCredentialsButton.Size = new System.Drawing.Size(125, 23);
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
            apiKeyValidatedCheckbox.Location = new System.Drawing.Point(134, 31);
            apiKeyValidatedCheckbox.Name = "apiKeyValidatedCheckbox";
            apiKeyValidatedCheckbox.Size = new System.Drawing.Size(184, 17);
            apiKeyValidatedCheckbox.TabIndex = 4;
            apiKeyValidatedCheckbox.Text = "CMDR Name / API Key validated";
            apiKeyValidatedCheckbox.UseVisualStyleBackColor = true;
            // 
            // apiKeyLabel
            // 
            apiKeyLabel.AutoSize = true;
            apiKeyLabel.Location = new System.Drawing.Point(3, 6);
            apiKeyLabel.Name = "apiKeyLabel";
            apiKeyLabel.Size = new System.Drawing.Size(43, 13);
            apiKeyLabel.TabIndex = 5;
            apiKeyLabel.Text = "Api Key";
            // 
            // EdsmSettingsControl
            // 
            Controls.Add(apiKeyLabel);
            Controls.Add(apiKeyValidatedCheckbox);
            Controls.Add(credentialsStatusLabel);
            Controls.Add(testCredentialsButton);
            Controls.Add(edsmApiKeyTextBox);
            Name = "EdsmSettingsControl";
            Size = new System.Drawing.Size(319, 148);
            ResumeLayout(false);
            PerformLayout();

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
    }
}