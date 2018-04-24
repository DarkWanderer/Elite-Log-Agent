using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Controller;
using EliteLogAgent.Settings;
using Interfaces;

namespace TrayAgent
{
    public partial class SettingsForm : Form
    {
        internal IPersistentSettingsStorage SettingsProvider { get; set; }
        internal AsyncMessageBroker MessageBroker { get; set; }
        internal List<IPlugin> Plugins { get; set; }

        private IDictionary<string, Control> SettingsCategories = new Dictionary<string, Control>();

        public SettingsForm()
        {
            InitializeComponent();
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            var versionLabel = "Version: " + fvi.FileVersion;
            Text += ". " + versionLabel;
            Load += SettingsForm_Load;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            SettingsCategories.Add("General", new GeneralSettingsControl() { MessageBroker = MessageBroker });
            foreach (var plugin in Plugins)
            {
                var control = plugin.GetPluginSettingsControl();
                control.Dock = DockStyle.Fill;
                control.PerformLayout();
                SettingsCategories.Add(plugin.SettingsLabel, control);
            }

            foreach (var category in SettingsCategories.Keys)
                settingsCategorySelector.Items.Add(category);
        }

        private void settingsCategorySelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedSettingsItem = settingsCategorySelector.SelectedItem as string;
            splitContainer1.Panel2.Controls.Clear();
            if (selectedSettingsItem != null)
                splitContainer1.Panel2.Controls.Add(SettingsCategories[selectedSettingsItem]);
        }
    }
}
