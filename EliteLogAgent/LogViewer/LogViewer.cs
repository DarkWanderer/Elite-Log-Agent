using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DW.ELA.Utility.Json;

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

                        var str = new StringBuilder();
                        str.Append($"[{entry["level"]}]");
                        str.Append($"[{entry["time"]}]");
                        
                        if (entry.ContainsKey("commander"))
                            str.Append($" - commander: {entry["commander"]}");
                        
                        if (entry.ContainsKey("eventsCount"))
                            str.Append($" - events count: {entry["eventsCount"]}");
                        
                        str.Append($" - {entry["message"]}");
                        
                        listBox1.Items.Add(str.ToString());
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