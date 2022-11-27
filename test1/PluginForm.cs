using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test1
{
    public partial class PluginForm : Form
    {
        public PluginForm()
        {
            InitializeComponent();
        }

        private void cmd1_Click(object sender, EventArgs e)
        {
            // DataEvent.paramRadius = Convert.ToInt32(param1.Text);
            MyPlugin.cmd_Loft();
        }

        
    }
}
