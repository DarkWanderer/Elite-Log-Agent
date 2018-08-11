using EliteLogAgent.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utility;

namespace EliteLogAgent
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
            Load += AboutForm_Load;
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            logoBox.Image = Resources.EliteIcon.ToBitmap();
            titleLabel.Text = AppInfo.Name;
        }
    }
}
