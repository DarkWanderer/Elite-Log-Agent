using DW.Inara.LogUploader.Inara;
using DW.Inara.LogUploader.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DW.Inara.LogUploader
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void testButton_Click(object sender, EventArgs e)
        {
            var settings = UploaderSettings;
            var credentialsCorrect = Uploader.ValidateCredentials(settings.InaraUsername, settings.InaraPassword);
            if (credentialsCorrect)
                MessageBox.Show(this, "Successfully authenticated with INARA", "Authentication successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show(this, "Incorrect credentials, failed to authenticate", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public UploaderSettings UploaderSettings
        {
            get
            {
                return new UploaderSettings
                {
                    InaraUsername = usernameTextBox.Text,
                    InaraPassword = passwordTextBox.Text
                };
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));

                usernameTextBox.Text = value.InaraUsername ?? "";
                passwordTextBox.Text = value.InaraPassword ?? "";
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void passwordLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
