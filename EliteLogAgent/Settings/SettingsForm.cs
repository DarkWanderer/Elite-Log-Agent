using Interfaces;
using Interfaces.Settings;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Utility;

namespace EliteLogAgent
{
    public partial class SettingsForm : Form
    {
        // These fields have to be properties because Form designer does not allow arguments in constructor
        internal ISettingsProvider Provider { get; set; }
        internal IMessageBroker MessageBroker { get; set; }
        internal List<IPlugin> Plugins { get; set; }

        private IDictionary<string, AbstractSettingsControl> SettingsCategories = new Dictionary<string, AbstractSettingsControl>();

        public SettingsForm()
        {
            InitializeComponent();

            var versionLabel = "Version: " + AppInfo.Version;
            Text += ". " + versionLabel;
            Load += SettingsForm_Load;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            SettingsCategories.Add("General", new GeneralSettingsControl() { MessageBroker = MessageBroker });
            settingsCategorySelector.Items.Add("General");

            foreach (var plugin in Plugins)
            {
                var control = plugin.GetPluginSettingsControl();
                if (control == null)
                    continue;
                control.Dock = DockStyle.Fill;
                control.PerformLayout();
                SettingsCategories.Add(plugin.SettingsLabel, control);
            }

            foreach (var category in SettingsCategories.Keys.OrderBy(x => x))
                if (category != "General")
                    settingsCategorySelector.Items.Add(category);

            Settings = Provider.Settings;
        }

        private void SettingsCategorySelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            splitContainer1.Panel2.Controls.Clear();
            if (settingsCategorySelector.SelectedItem is string selectedSettingsItem)
                splitContainer1.Panel2.Controls.Add(SettingsCategories[selectedSettingsItem]);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            ApplySettings();
            Close();
        }

        private void ApplyButton_Click(object sender, EventArgs e) => ApplySettings();

        private void ApplySettings()
        {
            Provider.Settings = Settings;
        }

        private void CancelButton_Click(object sender, EventArgs e) => Close();

        GlobalSettings Settings
        {
            get
            {
                var newSettings = new GlobalSettings
                {
                    PluginSettings = SettingsCategories.ToDictionary(c => c.Key, c => c.Value.Settings)
                };
                return newSettings;
            }
            set
            {
                var newSettings = value;
                foreach (var category in SettingsCategories)
                {
                    if (newSettings.PluginSettings.TryGetValue(category.Key, out JObject settings))
                        category.Value.Settings = settings;
                }
            }
        }
    }
}
