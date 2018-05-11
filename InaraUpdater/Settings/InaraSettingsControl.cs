using InaraUpdater.Model;
using Interfaces;
using Newtonsoft.Json.Linq;
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

        public InaraSettingsControl() => InitializeComponent();

        public override JObject Settings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private void InitializeComponent()
        {
            button1 = new Button();
            usernameTextBox = new TextBox();
            inaraApiKeyTextBox = new TextBox();
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
            // usernameTextBox
            // 
            usernameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            usernameTextBox.Location = new System.Drawing.Point(4, 4);
            usernameTextBox.Name = "usernameTextBox";
            usernameTextBox.Size = new System.Drawing.Size(252, 20);
            usernameTextBox.TabIndex = 0;
            usernameTextBox.Text = "Inara Commander";
            usernameTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // inaraApiKeyTextBox
            // 
            inaraApiKeyTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            inaraApiKeyTextBox.Location = new System.Drawing.Point(4, 30);
            inaraApiKeyTextBox.Name = "inaraApiKeyTextBox";
            inaraApiKeyTextBox.PasswordChar = '*';
            inaraApiKeyTextBox.Size = new System.Drawing.Size(252, 20);
            inaraApiKeyTextBox.TabIndex = 1;
            inaraApiKeyTextBox.Text = "Inara API Key";
            inaraApiKeyTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // testCredentialsButton
            // 
            testCredentialsButton.Location = new System.Drawing.Point(4, 57);
            testCredentialsButton.Name = "testCredentialsButton";
            testCredentialsButton.Size = new System.Drawing.Size(122, 23);
            testCredentialsButton.TabIndex = 2;
            testCredentialsButton.Text = "Save && Test";
            testCredentialsButton.UseVisualStyleBackColor = true;
            testCredentialsButton.Click += new System.EventHandler(testCredentialsButton_Click);
            // 
            // credentialsStatusLabel
            // 
            credentialsStatusLabel.AutoSize = true;
            credentialsStatusLabel.Location = new System.Drawing.Point(132, 62);
            credentialsStatusLabel.Name = "credentialsStatusLabel";
            credentialsStatusLabel.Size = new System.Drawing.Size(51, 13);
            credentialsStatusLabel.TabIndex = 3;
            credentialsStatusLabel.Text = "unknown";
            // 
            // InaraSettingsControl
            // 
            Controls.Add(credentialsStatusLabel);
            Controls.Add(testCredentialsButton);
            Controls.Add(inaraApiKeyTextBox);
            Controls.Add(usernameTextBox);
            Name = "InaraSettingsControl";
            Size = new System.Drawing.Size(259, 191);
            ResumeLayout(false);
            PerformLayout();
        }

        private async void testCredentialsButton_Click(object sender, System.EventArgs e)
        {
            var cmdrName = usernameTextBox.Text;
            var apiKey = inaraApiKeyTextBox.Text;
            var apiFacade = new ApiFacade(new ThrottlingRestClient("https://inara.cz/inapi/v1/"), apiKey, cmdrName);
            var @event = new ApiEvent("getCommanderProfile") { EventData = new { searchName = cmdrName }, Timestamp = DateTime.Now };
            var result = await apiFacade.ApiCall(@event);
        }
    }
}