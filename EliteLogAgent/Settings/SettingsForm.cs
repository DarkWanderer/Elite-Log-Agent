namespace EliteLogAgent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using DW.ELA.Interfaces;
    using DW.ELA.Interfaces.Settings;
    using DW.ELA.Utility;
    using EliteLogAgent.Properties;
    using EliteLogAgent.Settings;
    using NLog;

    public partial class SettingsForm : Form
    {
        private static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        private GlobalSettings currentSettings;

        // These have to be properties because Form designer does not allow arguments in constructor
        internal ISettingsProvider Provider { get; set; }

        internal IMessageBroker MessageBroker { get; set; }

        internal List<IPlugin> Plugins { get; set; }

        internal IAutorunManager AutorunManager { get; set; }

        private IDictionary<string, AbstractSettingsControl> settingsControls = new Dictionary<string, AbstractSettingsControl>();

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
            // Will check passed properties for null here as they are not yet set in constructor
            var generalSettingsControl = new GeneralSettingsControl()
            {
                MessageBroker = MessageBroker ?? throw new ArgumentNullException("MessageBroker"),
                GlobalSettings = currentSettings ?? throw new ArgumentNullException("Settings"),
                Plugins = Plugins ?? throw new ArgumentNullException("Plugins"),
                AutorunManager = AutorunManager ?? throw new ArgumentNullException("AutorunManager"),
                Dock = DockStyle.Fill
            };
            settingsControls.Add("General", generalSettingsControl);
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
                    settingsControls.Add(plugin.PluginName, control);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error loading plugin {0}", plugin.PluginId);
                }
            }

            foreach (var category in settingsControls.Keys.OrderBy(x => x))
            {
                if (category != "General")
                    settingsCategoriesListView.Items.Add(category);
            }

            settingsCategoriesListView.SelectedIndices.Add(0);
        }

        private void settingsCategoriesListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (settingsCategoriesListView.SelectedIndices.Count > 0)
            {
                var selectedIndex = settingsCategoriesListView.SelectedIndices.Cast<int>().Single();
                settingsControlContainer.Controls.OfType<AbstractSettingsControl>().SingleOrDefault()?.SaveSettings();
                settingsControlContainer.Controls.Clear();
                settingsControlContainer.Controls.Add(settingsControls[settingsCategoriesListView.Items[selectedIndex].Text]);
                settingsControlContainer.PerformLayout();
            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            ApplySettings();
            Close();
        }

        private void ApplyButton_Click(object sender, EventArgs e) => ApplySettings();

        private void ApplySettings()
        {
            settingsControlContainer.Controls.OfType<AbstractSettingsControl>().SingleOrDefault()?.SaveSettings();
            Provider.Settings = currentSettings;
        }

        private void CancelButton_Click(object sender, EventArgs e) => Close();
    }
}
