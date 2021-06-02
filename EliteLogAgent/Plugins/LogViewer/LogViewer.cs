using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DW.ELA.Interfaces;
using DW.ELA.Utility.Json;

namespace EliteLogAgent.Plugins.LogViewer
{
    public partial class LogViewer : Form
    {
        private readonly IPathManager pathManager;

        public LogViewer(IPathManager pathManager)
        {
            this.pathManager = pathManager;
            InitializeComponent();
        }

        private void LogViewer_Load(object sender, EventArgs e)
        {
            string file = Path.Combine(pathManager.LogDirectory, "EliteLogAgent.json");
            
            foreach (string line in File.ReadAllLines(file))
            {
                try
                {
                    string stringBuilder = string.Empty;
                    var entry = Serialize.FromJson<Dictionary<string, string>>(line);

                    stringBuilder = entry.Aggregate(stringBuilder, (current, keyValuePair) => current + $"- {keyValuePair.Key}: {keyValuePair.Value}");

                    listBox1.Items.Add(stringBuilder);
                }
                catch (Exception exception)
                {
                    listBox1.Items.Add($"Unable to read log: {file}");
                    listBox1.Items.Add($"{exception.Message}");
                    listBox1.Items.Add($"{exception.Source}");
                    listBox1.Items.Add($"{exception.StackTrace}");
                }
            }
        }
    }
}