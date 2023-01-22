using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace test1
{
    public partial class PluginForm : Form
    {

        private void PluginForm_Load(object sender, EventArgs e)
        {
            OpenChildForm(form: new FormGost44(), sender);
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.BackColor = Color.White;
            button1.BackColor = Color.LightGray;
            OpenChildForm(form: new FormGost45(), sender);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            button1.BackColor = Color.White;
            button2.BackColor = Color.LightGray;
            OpenChildForm(form: new FormGost44(), sender);
        }

        private Form activeForm;

        public PluginForm()
        {
            InitializeComponent();
            button2.BackColor = Color.LightGray;
        }

        private void OpenChildForm(Form form, object sender)
        {
            activeForm?.Close();
            activeForm = form;
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Dock = DockStyle.Fill;
            panelTabManager.Controls.Add(form);
            panelTabManager.Tag = form;
            form.BringToFront();
            form.Show();
            // lblTitle.Text = form.Text; ??

        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackColor = Color.Red;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = Color.Transparent;
        }
    }
}
