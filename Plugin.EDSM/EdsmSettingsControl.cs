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
        private CheckBox apiKeyValidatedCheckbox;
        private Label apiKeyLabel;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public EdsmSettingsControl() => InitializeComponent();

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

        internal EdsmPlugin Plugin;

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.edsmApiKeyTextBox = new System.Windows.Forms.TextBox();
            this.testCredentialsButton = new System.Windows.Forms.Button();
            this.credentialsStatusLabel = new System.Windows.Forms.Label();
            this.apiKeyValidatedCheckbox = new System.Windows.Forms.CheckBox();
            this.apiKeyLabel = new System.Windows.Forms.Label();
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
            this.edsmApiKeyTextBox.Location = new System.Drawing.Point(52, 3);
            this.edsmApiKeyTextBox.Name = "edsmApiKeyTextBox";
            this.edsmApiKeyTextBox.Size = new System.Drawing.Size(264, 20);
            this.edsmApiKeyTextBox.TabIndex = 1;
            this.edsmApiKeyTextBox.Text = "EDSM API Key";
            this.edsmApiKeyTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
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
            // 
            // apiKeyLabel
            // 
            this.apiKeyLabel.AutoSize = true;
            this.apiKeyLabel.Location = new System.Drawing.Point(3, 6);
            this.apiKeyLabel.Name = "apiKeyLabel";
            this.apiKeyLabel.Size = new System.Drawing.Size(43, 13);
            this.apiKeyLabel.TabIndex = 5;
            this.apiKeyLabel.Text = "Api Key";
            // 
            // EdsmSettingsControl
            // 
            this.Controls.Add(this.apiKeyLabel);
            this.Controls.Add(this.apiKeyValidatedCheckbox);
            this.Controls.Add(this.credentialsStatusLabel);
            this.Controls.Add(this.testCredentialsButton);
            this.Controls.Add(this.edsmApiKeyTextBox);
            this.Name = "EdsmSettingsControl";
            this.Size = new System.Drawing.Size(319, 148);
            this.ResumeLayout(false);
            this.PerformLayout();

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