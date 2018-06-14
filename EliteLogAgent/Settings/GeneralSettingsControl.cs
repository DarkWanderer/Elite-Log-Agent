using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interfaces;
using Utility.Extensions;
using Controller;

namespace EliteLogAgent.Settings
{
    public partial class GeneralSettingsControl : AbstractSettingsControl
    {
        public GeneralSettingsControl()
        {
            InitializeComponent();
        }

        public IMessageBroker MessageBroker { get; internal set; }

        private async void uploadLatestDataButton_Click(object sender, EventArgs e)
        {
            try
            {
                uploadLatestDataButton.Enabled = false;
                await Task.Factory.StartNew(UploadLatestData);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing update:\n" + ex.GetStackedErrorMessages(), "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                uploadLatestDataButton.Enabled = true;
            }
        }

        private void UploadLatestData()
        {
            var logEventSource = new LogBurstPlayer(new SavedGamesDirectoryHelper().Directory, 5);
            var logCounter = new LogEventTypeCounter();
            using (logEventSource.Subscribe(logCounter))
            using (logEventSource.Subscribe(MessageBroker))
            {
                logEventSource.Play();
            }
        }
    }
}
