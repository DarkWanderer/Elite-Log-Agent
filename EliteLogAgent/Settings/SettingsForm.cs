using DW.ELA.Interfaces.Settings;
using EliteLogAgent.Properties;
using EliteLogAgent.Settings;
using DW.ELA.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Utility;
using System.Linq;
using NLog;

namespace EliteLogAgent
{
    public partial class SettingsForm : Form
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        // These fields have to be properties because Form designer does not allow arguments in constructor
        internal ISettingsProvider Provider { get; set; }
        internal IMessageBroker MessageBroker { get; set; }
        internal List<IPlugin> Plugins { get; set; }

        private GlobalSettings currentSettings;

        private IDictionary<string, AbstractSettingsControl> SettingsControls = new Dictionary<string, AbstractSettingsControl>();

        public SettingsForm()
        {
            InitializeComponent();
            Icon = Resources.EliteIcon;
            var versionLabel = "Version: " + AppInfo.Version;
            Text += ". " + versionLabel;
            Load += SettingsForm_Load;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            currentSettings = Provider.Settings.Clone();
            SettingsControls.Add("General", new GeneralSettingsControl() { MessageBroker = MessageBroker, GlobalSettings = currentSettings, Dock = DockStyle.Fill });
            settingsCategoriesListView.Items.Add("General");

            foreach (var plugin in Plugins)
            {
                try
                {
                    var control = plugin.GetPluginSettingsControl(currentSettings);
                    if (control == null)
                        continue;
                    control.Dock = DockStyle.Fill;
                    control.PerformLayout();
                    SettingsControls.Add(plugin.PluginName, control);
                }
                catch (Exception ex)
                {
                    logger.Error(ex, "Error loading plugin {0}", plugin.PluginId);
                }
            }

            foreach (var category in SettingsControls.Keys.OrderBy(x => x))
                if (category != "General")
                    settingsCategoriesListView.Items.Add(category);
            settingsCategoriesListView.SelectedIndices.Add(0);
        }

        private void settingsCategoriesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (settingsCategoriesListView.SelectedIndices.Count > 0)
            {
                var selectedIndex = settingsCategoriesListView.SelectedIndices.Cast<int>().Single();
                settingsControlContainer.Controls.OfType<AbstractSettingsControl>().SingleOrDefault()?.SaveSettings();
                settingsControlContainer.Controls.Clear();
                settingsControlContainer.Controls.Add(SettingsControls[settingsCategoriesListView.Items[selectedIndex].Text]);
                settingsControlContainer.PerformLayout();
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            ApplySettings();
            Close();
        }

        private void ApplyButton_Click(object sender, EventArgs e)
        {
            ApplySettings();
        }

        private void ApplySettings()
        {
            settingsControlContainer.Controls.OfType<AbstractSettingsControl>().SingleOrDefault()?.SaveSettings();
            Provider.Settings = currentSettings;
        }

        private void CancelButton_Click(object sender, EventArgs e) => Close();
    }
}
