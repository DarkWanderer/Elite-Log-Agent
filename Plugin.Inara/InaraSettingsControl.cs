using InaraUpdater.Model;
using Interfaces;
using Newtonsoft.Json.Linq;
using NLog;
using System;
using System.Windows.Forms;
using Utility;

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

        public InaraSettingsControl() => InitializeComponent();

        internal InaraSettings ActualSettings
        {
            get
            {
                return new InaraSettings()
                {
                    ApiKey = inaraApiKeyTextBox.Text,
                    Verified = apiKeyValidatedCheckbox.Checked
                };
            }
            set
            {
                inaraApiKeyTextBox.Text = value.ApiKey;
                apiKeyValidatedCheckbox.Checked = value.Verified;
            }
        }

        public override JObject Settings
        {
            get => JObject.FromObject(ActualSettings);
            set => ActualSettings = value.ToObject<InaraSettings>();
        }

        internal InaraUpdaterPlugin Plugin;

        private void InitializeComponent()
        {
            this.button1 = new Button();
            this.inaraApiKeyTextBox = new TextBox();
            this.testCredentialsButton = new Button();
            this.credentialsStatusLabel = new Label();
            this.label1 = new Label();
            this.apiKeyValidatedCheckbox = new CheckBox();
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
            this.inaraApiKeyTextBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Left)
            | AnchorStyles.Right);
            this.inaraApiKeyTextBox.Location = new System.Drawing.Point(52, 3);
            this.inaraApiKeyTextBox.Name = "inaraApiKeyTextBox";
            this.inaraApiKeyTextBox.Size = new System.Drawing.Size(349, 20);
            this.inaraApiKeyTextBox.TabIndex = 1;
            this.inaraApiKeyTextBox.Text = "Inara API Key";
            this.inaraApiKeyTextBox.TextAlign = HorizontalAlignment.Center;
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
            this.credentialsStatusLabel.Size = new System.Drawing.Size(51, 13);
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
            // 
            // InaraSettingsControl
            // 
            this.Controls.Add(this.apiKeyValidatedCheckbox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.credentialsStatusLabel);
            this.Controls.Add(this.testCredentialsButton);
            this.Controls.Add(this.inaraApiKeyTextBox);
            this.Name = "InaraSettingsControl";
            this.Size = new System.Drawing.Size(404, 276);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private async void testCredentialsButton_Click(object sender, EventArgs e)
        {
            try
            {
                testCredentialsButton.Enabled = false;
                var apiKey = inaraApiKeyTextBox.Text;
                var cmdrName = Plugin.GlobalSettings.CommanderName;
                var apiFacade = new InaraApiFacade(InaraUpdaterPlugin.RestClient, apiKey, cmdrName);
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
    }
}