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
        private TextBox usernameTextBox;
        private TextBox inaraApiKeyTextBox;
        private Button testCredentialsButton;
        private Label credentialsStatusLabel;
        private Button button1;
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public InaraSettingsControl() => InitializeComponent();

        internal InaraSettings ActualSettings
        {
            get
            {
                return new InaraSettings()
                {
                    ApiKey = inaraApiKeyTextBox.Text,
                    CommanderName = usernameTextBox.Text
                };
            }
            set
            {
                inaraApiKeyTextBox.Text = value.ApiKey;
                usernameTextBox.Text = value.CommanderName;
            }
        }

        public override JObject Settings
        {
            get => JObject.FromObject(ActualSettings);
            set => ActualSettings = value.ToObject<InaraSettings>();
        }

        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.inaraApiKeyTextBox = new System.Windows.Forms.TextBox();
            this.testCredentialsButton = new System.Windows.Forms.Button();
            this.credentialsStatusLabel = new System.Windows.Forms.Label();
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
            // usernameTextBox
            // 
            this.usernameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.usernameTextBox.Location = new System.Drawing.Point(4, 4);
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(252, 20);
            this.usernameTextBox.TabIndex = 0;
            this.usernameTextBox.Text = "Inara Commander";
            this.usernameTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // inaraApiKeyTextBox
            // 
            this.inaraApiKeyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.inaraApiKeyTextBox.Location = new System.Drawing.Point(4, 30);
            this.inaraApiKeyTextBox.Name = "inaraApiKeyTextBox";
            this.inaraApiKeyTextBox.Size = new System.Drawing.Size(252, 20);
            this.inaraApiKeyTextBox.TabIndex = 1;
            this.inaraApiKeyTextBox.Text = "Inara API Key";
            this.inaraApiKeyTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // testCredentialsButton
            // 
            this.testCredentialsButton.Location = new System.Drawing.Point(4, 57);
            this.testCredentialsButton.Name = "testCredentialsButton";
            this.testCredentialsButton.Size = new System.Drawing.Size(122, 23);
            this.testCredentialsButton.TabIndex = 2;
            this.testCredentialsButton.Text = "Test (get cmdr ID)";
            this.testCredentialsButton.UseVisualStyleBackColor = true;
            this.testCredentialsButton.Click += new System.EventHandler(this.testCredentialsButton_Click);
            // 
            // credentialsStatusLabel
            // 
            this.credentialsStatusLabel.AutoSize = true;
            this.credentialsStatusLabel.Location = new System.Drawing.Point(132, 62);
            this.credentialsStatusLabel.Name = "credentialsStatusLabel";
            this.credentialsStatusLabel.Size = new System.Drawing.Size(51, 13);
            this.credentialsStatusLabel.TabIndex = 3;
            this.credentialsStatusLabel.Text = "unknown";
            // 
            // InaraSettingsControl
            // 
            this.Controls.Add(this.credentialsStatusLabel);
            this.Controls.Add(this.testCredentialsButton);
            this.Controls.Add(this.inaraApiKeyTextBox);
            this.Controls.Add(this.usernameTextBox);
            this.Name = "InaraSettingsControl";
            this.Size = new System.Drawing.Size(259, 191);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private async void testCredentialsButton_Click(object sender, EventArgs e)
        {
            try
            {
                testCredentialsButton.Enabled = false;
                var cmdrName = usernameTextBox.Text;
                var apiKey = inaraApiKeyTextBox.Text;
                var apiFacade = new ApiFacade(new ThrottlingRestClient("https://inara.cz/inapi/v1/"), apiKey, cmdrName, logger);
                var @event = new ApiEvent("getCommanderProfile") { EventData = new { searchName = cmdrName }, Timestamp = DateTime.Now };
                var result = await apiFacade.ApiCall(@event);
                credentialsStatusLabel.Text = (result.EventData as JObject)["userID"]?.ToString();
            }
            catch (Exception ex)
            {
                credentialsStatusLabel.Text = ex.Message;
            }
            finally
            {
                testCredentialsButton.Enabled = true;
            }
        }
    }
}