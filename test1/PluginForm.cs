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
        private SqlConnection sqlConnection = null;
        private SqlCommandBuilder sqlCommandBuilder = null;
        private SqlDataAdapter sqlDataAdapter = null;
        private DataSet dataSet = null;

        public PluginForm()
        {
            InitializeComponent();
        }

        private void cmd1_Click(object sender, EventArgs e)
        {
            // DataEvent.paramRadius = Convert.ToInt32(param1.Text);
            if (cbManualParams.Checked)
            {
                // валидация и построение вручную
                ValidateParamsAndBuild();
            }
            else
            {
                LoadData();
            }
        }

        private void ValidateParamsAndBuild()
        {
            float d = float.Parse(tbd.Text, CultureInfo.InvariantCulture.NumberFormat);
            float d1 = float.Parse(tbd1.Text, CultureInfo.InvariantCulture.NumberFormat);
            float d2 = float.Parse(tbd2.Text, CultureInfo.InvariantCulture.NumberFormat);
            float D = float.Parse(tbDD.Text, CultureInfo.InvariantCulture.NumberFormat);
            float D1 = float.Parse(tbDD1.Text, CultureInfo.InvariantCulture.NumberFormat);
            float D3 = float.Parse(tbD3.Text, CultureInfo.InvariantCulture.NumberFormat);
            float l = float.Parse(tbl.Text, CultureInfo.InvariantCulture.NumberFormat);
            float l1 = float.Parse(tbl1.Text, CultureInfo.InvariantCulture.NumberFormat);
            float L = float.Parse(tbLL.Text, CultureInfo.InvariantCulture.NumberFormat);
            float h = float.Parse(tbh.Text, CultureInfo.InvariantCulture.NumberFormat);

            float[] floats = { d, d1, d2, D, D1, D3, l, l1, L, h };
            foreach (float f in floats)
            {
                if (f < 0)
                {
                    MessageBox.Show("Все значения должны быть больше 0", "Ошибка!");
                    return;
                }
            }
            foreach (float f in floats)
            {
                if (f > 10000)
                {
                    MessageBox.Show("Все значения должны быть менее 10 000 (программное ограничение)", "Ошибка!");
                    return;
                }
            }
            if (d1 >= D) { MessageBox.Show("d1 должен быть меньше D", "Ошибка!"); return; }
            if (d >= d1) { MessageBox.Show("d должен быть меньше d1", "Ошибка!"); return; }
            if (d2 <= d) { MessageBox.Show("d2 должен быть больше d", "Ошибка!");  return; }
            if (D1 <= d1) { MessageBox.Show("D1 дольжен быть больше d1", "Ошибка!"); return; }
            if (D1 <= D3) { MessageBox.Show("D1 должен быть больше D3", "Ошибка!"); return; }
            if (l >= L)  { MessageBox.Show("l должен быть меньше L", "Ошибка!"); return; }
            if (l1 >= L) { MessageBox.Show("l1 должен быть меньше L", "Ошибка!"); return; }
            if (l+h >= L) { MessageBox.Show("l+h >= L. Необходимо увеличить L", "Ошибка!"); return; }
            if (false) { MessageBox.Show("бидибобабуба", "Ошибка!"); return; }
            // если проверки пройдены - строим деталь
            // MessageBox.Show("Проверки пройдены");
            ManualBuild();
        }

        private void ManualBuild ()
        {
            var gost = getGOST();
            MyPlugin.ExecuteBuilder(
                0,
                float.Parse(tbd.Text, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(tbd1.Text, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(tbd2.Text, CultureInfo.InvariantCulture.NumberFormat),
                0,
                float.Parse(tbDD.Text, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(tbDD1.Text, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(tbD3.Text, CultureInfo.InvariantCulture.NumberFormat),
                0,
                float.Parse(tbl.Text, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(tbl1.Text, CultureInfo.InvariantCulture.NumberFormat),
                0,
                0,
                0,
                float.Parse(tbLL.Text, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(tbh.Text, CultureInfo.InvariantCulture.NumberFormat),
                0,
                gost
                );
        }

        private void PluginForm_Load(object sender, EventArgs e)
        {
            setAllTBEnabledParam(cbManualParams.Checked);
            sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Stewie\source\repos\kursac2022-23\test1\Database1.mdf;Integrated Security=True");
            sqlConnection.Open();
        }

        private void LoadData()
        {
            var gost = getGOST();

            try
            {
                dataSet = new DataSet();
                //sqlDataAdapter.Fill(dataSet, "GOST16044-70");
                string query = String.Join("", new string[] { "SELECT * FROM [GOST160", gost, "-70] WHERE D_n = 6" });
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        MessageBox.Show("Ошибка бд");
                        return;
                    }
                    reader.Read();


                    MyPlugin.ExecuteBuilder(
                        (Int32)reader["D_n"],
                        (Int32)reader["d"],
                        (Int32)reader["d1"],
                        (Int32)reader["d2"],
                        (float)reader["d3"],
                        (Int32)reader["D_thread"],
                        (float)reader["DD1"],
                        (float)reader["DD3"],
                        (Int32)reader["S"],
                        (Int32)reader["L"],
                        (Int32)reader["l1_nom"],
                        (float)reader["l1_otkl"],
                        (float)reader["l2"],
                        (float)reader["l3"],
                        (float)reader["LL"],
                        (Int32)reader["h"],
                        (float)reader["MASS"],
                        (string)gost
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.LoadData();
        }

        private void rbGOST44_Click(object sender, EventArgs e)
        {
            this.rbGOST44.Checked = true;
            this.rbGOST45.Checked = false;
            gb44.Visible = true;
            gb45.Visible = false;
            setGOSTDependantTBParams();
        }

        private void rbGOST45_Click(object sender, EventArgs e)
        {
            this.rbGOST44.Checked = false;
            this.rbGOST45.Checked = true;
            gb44.Visible = false;
            gb45.Visible = true;
            setGOSTDependantTBParams();
        }

        private void cbManualParams_CheckedChanged(object sender, EventArgs e)
        {
            setAllTBEnabledParam(cbManualParams.Checked);
        }

        private void setAllTBEnabledParam(bool enabled)
        {
            tbd.Enabled = enabled;
            tbd1.Enabled = enabled;
            tbd2.Enabled = enabled;
            tbDD.Enabled = enabled;
            tbDD1.Enabled = enabled;
            tbD3.Enabled = enabled;
            tbl.Enabled = enabled;
            tbl1.Enabled = enabled;
            tbLL.Enabled = enabled;
            tbh.Enabled = enabled;

            setGOSTDependantTBParams();
        }

        private void setGOSTDependantTBParams()
        {
            if (cbManualParams.Checked)
            {
                if(getGOST() == "45")
                {
                    tbd2.Enabled = false;
                    tbl1.Enabled = false;
                } else
                {
                    tbd2.Enabled = true;
                    tbl1.Enabled = true;
                }
            }
;
        }

        private string getGOST()
        {
            var gost = "44";
            if (this.rbGOST44.Checked == true)
            {
                gost = "44"; // на всякий случай)
            }
            if (this.rbGOST45.Checked == true)
            {
                gost = "45";
            }
            return gost;
        }

        private void tb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }
    }
}
