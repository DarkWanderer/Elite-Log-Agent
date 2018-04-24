using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controller;
using Utility.Extensions;

namespace EliteLogAgent.Settings
{
    public partial class GeneralSettingsControl : UserControl
    {
        internal AsyncMessageBroker MessageBroker { get; set; }

        public GeneralSettingsControl()
        {
            InitializeComponent();
        }
        
        private async void uploadLatestDataButton_Click(object sender, EventArgs e)
        {
            try
            {
                uploadLatestDataButton.Enabled = false;
                await Task.Factory.StartNew(UploadLatestData);
                uploadLatestDataButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing update:\n" + ex.GetStackedErrorMessages(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UploadLatestData()
        {
            var logEventSource = new LogBurstPlayer(SavedGamesDirectoryHelper.Directory);
            var logCounter = new LogEventTypeCounter();
            using (logEventSource.Subscribe(logCounter))
            using (logEventSource.Subscribe(MessageBroker))
            {
                logEventSource.Play();
            }
        }
    }
}
