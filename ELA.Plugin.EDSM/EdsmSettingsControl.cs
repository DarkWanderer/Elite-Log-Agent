using Interfaces;
using Newtonsoft.Json.Linq;
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
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public EdsmSettingsControl() => InitializeComponent();

        internal EdsmSettings ActualSettings
        {
            get
            {
                return new EdsmSettings()
                {
                    ApiKey = edsmApiKeyTextBox.Text,
                };
            }
            set
            {
                edsmApiKeyTextBox.Text = value.ApiKey;
            }
        }

        internal EdsmPlugin Plugin;

        public override JObject Settings
        {
            get => JObject.FromObject(ActualSettings);
            set => ActualSettings = value.ToObject<EdsmSettings>();
        }

        private void InitializeComponent()
        {
            button1 = new Button();
            edsmApiKeyTextBox = new TextBox();
            testCredentialsButton = new Button();
            credentialsStatusLabel = new Label();
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
            edsmApiKeyTextBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right);
            edsmApiKeyTextBox.Location = new System.Drawing.Point(4, 3);
            edsmApiKeyTextBox.Name = "edsmApiKeyTextBox";
            edsmApiKeyTextBox.Size = new System.Drawing.Size(252, 20);
            edsmApiKeyTextBox.TabIndex = 1;
            edsmApiKeyTextBox.Text = "EDSM API Key";
            edsmApiKeyTextBox.TextAlign = HorizontalAlignment.Center;
            // 
            // testCredentialsButton
            // 
            testCredentialsButton.Location = new System.Drawing.Point(4, 29);
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
            credentialsStatusLabel.Location = new System.Drawing.Point(132, 34);
            credentialsStatusLabel.Name = "credentialsStatusLabel";
            credentialsStatusLabel.Size = new System.Drawing.Size(51, 13);
            credentialsStatusLabel.TabIndex = 3;
            credentialsStatusLabel.Text = "unknown";
            // 
            // InaraSettingsControl
            // 
            Controls.Add(credentialsStatusLabel);
            Controls.Add(testCredentialsButton);
            Controls.Add(edsmApiKeyTextBox);
            Name = "EdsmSettingsControl";
            Size = new System.Drawing.Size(259, 191);
            ResumeLayout(false);
            PerformLayout();

        }

        private async void testCredentialsButton_Click(object sender, EventArgs e)
        {
            try
            {
                testCredentialsButton.Enabled = false;
                var apiKey = edsmApiKeyTextBox.Text;
                var apiFacade = new EdsmApiFacade(new ThrottlingRestClient("https://www.edsm.net/api-commander-v1/get-ranks"), Plugin?.GlobalSettings.CommanderName, apiKey);
                var result = await apiFacade.GetCommanderRanks();
                credentialsStatusLabel.Text = "Success, combat rank: " + result["ranksVerbose"]["Combat"]?.ToString();
                ActualSettings.Verified = true;
            }
            catch (Exception ex)
            {
                credentialsStatusLabel.Text = ex.Message;
                ActualSettings.Verified = false;
            }
            finally
            {
                testCredentialsButton.Enabled = true;
            }
        }
    }
}