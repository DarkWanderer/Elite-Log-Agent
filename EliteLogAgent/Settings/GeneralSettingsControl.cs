using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controller;
using Utility.Extensions;
using Newtonsoft.Json.Linq;

namespace Interfaces.Settings
{
    public partial class GeneralSettingsControl : AbstractSettingsControl
    {
        internal AsyncMessageBroker MessageBroker { get; set; }

        public override JObject Settings { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
