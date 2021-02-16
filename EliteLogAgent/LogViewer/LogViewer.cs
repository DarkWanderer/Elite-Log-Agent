using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DW.ELA.Utility.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Targets;

namespace EliteLogAgent.LogViewer
{
    public partial class LogViewer : Form
    {
        private readonly string logDirectory;

        public LogViewer(string pathManagerLogDirectory)
        {
            logDirectory = pathManagerLogDirectory;
            InitializeComponent();
        }

        private void LogViewer_Load(object sender, EventArgs e)
        {
            foreach (string file in Directory.GetFiles(logDirectory))
            {
                foreach (string line in File.ReadAllLines(file))
                {
                    try
                    {
                        var entry = Serialize.FromJson<Dictionary<string,string>>(line);
                        listBox1.Items.Add($"[{entry["level"]}][{entry["time"]}] - {entry["message"]}");
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }
        }
    }
}