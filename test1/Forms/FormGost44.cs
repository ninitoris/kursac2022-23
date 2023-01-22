using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test1
{
    public partial class FormGost44 : Form
    {
        public FormGost44()
        {
            InitializeComponent();
        }

        private SqlConnection sqlConnection = null;
        private SqlCommandBuilder sqlCommandBuilder = null;
        private SqlDataAdapter sqlDataAdapter = null;
        private DataSet dataSet = null;


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
            float D3 = float.Parse(tbDD3.Text, CultureInfo.InvariantCulture.NumberFormat);
            float l = float.Parse(tbl.Text, CultureInfo.InvariantCulture.NumberFormat);
            float l1 = float.Parse(tbl1.Text, CultureInfo.InvariantCulture.NumberFormat);
            float L = float.Parse(tbLL.Text, CultureInfo.InvariantCulture.NumberFormat);
            float h = float.Parse(tbh.Text, CultureInfo.InvariantCulture.NumberFormat);
            float S = float.Parse(tbS.Text, CultureInfo.InvariantCulture.NumberFormat);

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
            if (D3 >= S) { MessageBox.Show("D3 должен быть меньше S", "Ошибка!"); return; }
            if (D >= S) { MessageBox.Show("D должен быть меньше S", "Ошибка!"); return; }
            if (d1 >= D) { MessageBox.Show("d1 должен быть меньше D", "Ошибка!"); return; }
            if (d >= d1) { MessageBox.Show("d должен быть меньше d1", "Ошибка!"); return; }
            if (d2 <= d) { MessageBox.Show("d2 должен быть больше d", "Ошибка!"); return; }
            if (D1 <= d1) { MessageBox.Show("D1 дольжен быть больше d1", "Ошибка!"); return; }
            if (D1 <= D3) { MessageBox.Show("D1 должен быть больше D3", "Ошибка!"); return; }
            if (l >= L) { MessageBox.Show("l должен быть меньше L", "Ошибка!"); return; }
            if (l1 >= L) { MessageBox.Show("l1 должен быть меньше L", "Ошибка!"); return; }
            if (l + h >= L) { MessageBox.Show("l+h >= L. Необходимо увеличить L", "Ошибка!"); return; }
            
            if (false) { MessageBox.Show("бидибобабуба", "Ошибка!"); return; }
            // если проверки пройдены - строим деталь
            // MessageBox.Show("Проверки пройдены");
            ManualBuild();
        }

        private void ManualBuild()
        {
            var gost = "44";
            MyPlugin.ExecuteBuilder(
                0,
                float.Parse(tbd.Text, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(tbd1.Text, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(tbd2.Text, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(tbd3.Text, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(tbDD.Text, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(tbDD1.Text, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(tbDD3.Text, CultureInfo.InvariantCulture.NumberFormat),
                0,
                float.Parse(tbl.Text, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(tbl1.Text, CultureInfo.InvariantCulture.NumberFormat),
                0,
                float.Parse(tbl2.Text, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(tbl3.Text, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(tbLL.Text, CultureInfo.InvariantCulture.NumberFormat),
                float.Parse(tbh.Text, CultureInfo.InvariantCulture.NumberFormat),
                0,
                gost
                );
        }

        private void PluginForm_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Stewie\source\repos\kursac2022-23\test1\Database1.mdf;Integrated Security=True");
            sqlConnection.Open();

            setAllTBEnabledParam(cbManualParams.Checked);

            PreloadData();
        }

        private void PreloadData()
        {
            var gost = "44";
            try
            {
                dataSet = new DataSet();
                string query = String.Join("", new string[] { "SELECT D_n FROM [GOST160", gost, "-70]" });

                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);
                sqlDataAdapter.Fill(dataSet, "GOST16044-70");
                comboBox1.DisplayMember = "D_n";
                comboBox1.ValueMember = "D_n";
                comboBox1.DataSource = dataSet.Tables[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error PreloadData!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadData()
        {
            var gost = "44";

            try
            {
                dataSet = new DataSet();
                //sqlDataAdapter.Fill(dataSet, "GOST16044-70");
                string query = String.Join("", new string[] { "SELECT * FROM [GOST160", gost, "-70] WHERE D_n = ", comboBox1.Text });
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
                MessageBox.Show(ex.Message, "Error LoadData!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            tbDD1.Enabled = false;
            tbDD3.Enabled = enabled;
            tbl.Enabled = enabled;
            tbl1.Enabled = enabled;
            tbLL.Enabled = enabled;
            tbh.Enabled = enabled;
            tbS.Enabled = enabled;
            tbDD2.Enabled = enabled;
            tbd3.Enabled = enabled;
            tbl3.Enabled = enabled;
            tbl2.Enabled = enabled;


            comboBox1.Enabled = !enabled;  
            if(!enabled)
            {
                comboBox1_SelectedIndexChanged(this, null);
            }

            //setGOSTDependantTBParams();
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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var gost = "44";

            try
            {
                dataSet = new DataSet();
                //sqlDataAdapter.Fill(dataSet, "GOST16044-70");
                string query;
                if (comboBox1.Text == null || comboBox1.Text == "")
                {
                    query = String.Join("", new string[] { "SELECT TOP 1 * FROM [GOST160", gost, "-70]"});

                } else
                {
                    query = String.Join("", new string[] { "SELECT * FROM [GOST160", gost, "-70] WHERE D_n = ", comboBox1.Text });
                }

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);

                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        MessageBox.Show("Ошибка бд");
                        return;
                    }
                    reader.Read();

                    tbd.Text = reader["d"].ToString().Replace(",", ".");
                    tbd1.Text = reader["d1"].ToString().Replace(",", ".");
                    tbd2.Text = reader["d2"].ToString().Replace(",", ".");
                    tbS.Text = reader["S"].ToString().Replace(",", ".");
                    tbDD.Text = reader["D_thread"].ToString().Replace(",", ".");
                    tbDD1.Text = reader["DD1"].ToString().Replace(",", ".");
                    tbDD3.Text = reader["DD3"].ToString().Replace(",", ".");
                    tbd3.Text = reader["d3"].ToString().Replace(",", ".");
                    tbl.Text = reader["L"].ToString().Replace(",", ".");
                    tbl1.Text = reader["l1_nom"].ToString().Replace(",", ".");
                    tbLL.Text = reader["LL"].ToString().Replace(",", ".");
                    tbh.Text = reader["h"].ToString().Replace(",", ".");
                    tbl3.Text = reader["l3"].ToString().Replace(",", ".");
                    tbDD2.Text = reader["DD3"].ToString().Replace(",", ".");
                    tbl2.Text = reader["l2"].ToString().Replace(",", ".");
                       
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error comboBox1!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tbS_TextChanged(object sender, EventArgs e)
        {
            float value = tbS.Text != "" ? float.Parse(tbS.Text, CultureInfo.InvariantCulture.NumberFormat) : 0;
            tbDD1.Text = String.Format("{0:N3}", value * 2 / Math.Sqrt(3)).Replace(",", ".");
        }

    }
}
