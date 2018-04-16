using Controller;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrayAgent
{
    internal class GeneralSettingsControl : Control
    {
        private CheckBox uploadOnStartupCheckbox;
        private Button uploadLatestDataButton;

        public GeneralSettingsControl()
        {
            InitializeComponent();
        }

        internal MessageBroker MessageBroker { get; set; }

        private void InitializeComponent()
        {
            this.uploadOnStartupCheckbox = new System.Windows.Forms.CheckBox();
            this.uploadLatestDataButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // uploadOnStartupCheckbox
            // 
            this.uploadOnStartupCheckbox.AutoSize = true;
            this.uploadOnStartupCheckbox.Dock = System.Windows.Forms.DockStyle.Left;
            this.uploadOnStartupCheckbox.Location = new System.Drawing.Point(0, 0);
            this.uploadOnStartupCheckbox.Name = "uploadOnStartupCheckbox";
            this.uploadOnStartupCheckbox.Size = new System.Drawing.Size(100, 25);
            this.uploadOnStartupCheckbox.TabIndex = 2;
            this.uploadOnStartupCheckbox.Text = "Upload On Startup";
            this.uploadOnStartupCheckbox.UseVisualStyleBackColor = true;
            this.uploadOnStartupCheckbox.CheckedChanged += new System.EventHandler(this.uploadOnStartupCheckbox_CheckedChanged);
            // 
            // uploadLatestDataButton
            // 
            this.uploadLatestDataButton.Location = new System.Drawing.Point(0, 30);
            this.uploadLatestDataButton.Name = "uploadLatestDataButton";
            this.uploadLatestDataButton.Size = new System.Drawing.Size(114, 25);
            this.uploadLatestDataButton.TabIndex = 1;
            this.uploadLatestDataButton.Text = "Upload latest logs";
            this.uploadLatestDataButton.UseVisualStyleBackColor = true;
            // 
            // GeneralSettingsControl
            // 
            this.BackColor = System.Drawing.Color.LightSkyBlue;
            this.ResumeLayout(false);

        }

        private async void uploadLatestDataButton_Click(object sender, EventArgs e)
        {
            uploadLatestDataButton.Enabled = false;
            await Task.Factory.StartNew(UploadLatestData);
            uploadLatestDataButton.Enabled = true;
        }

        private void UploadLatestData()
        {
            var logEventSource = new LogBurstPlayer();
            using (var subscription = logEventSource.Subscribe(MessageBroker))
            {
                logEventSource.Play();
            }
        }

        private void uploadOnStartupCheckbox_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}