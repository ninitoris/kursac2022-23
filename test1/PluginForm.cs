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
            LoadData();
        }

        private void PluginForm_Load(object sender, EventArgs e)
        {
            sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Stewie\source\repos\kursac2022-23\test1\Database1.mdf;Integrated Security=True");
            sqlConnection.Open();


        }

        private void LoadData()
        {
            try
            {
                dataSet = new DataSet();
                //sqlDataAdapter.Fill(dataSet, "GOST16044-70");
                SqlCommand sqlCommand = new SqlCommand("SELECT * FROM [GOST16044-70] WHERE D_n = 6", sqlConnection);

                using (SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    if (!reader.HasRows)
                    {
                        MessageBox.Show("Ошибка бд");
                        return;
                    }
                    reader.Read();
                    //MessageBox.Show(reader["id"].ToString());
                    MyPlugin.Sample3dLoft(
                        (Int32)reader["D_n"],
                        (Int32)reader["d"],
                        (Int32)reader["d1"],
                        (Int32)reader["d2"],
                        (float)reader["d3"],
                        (Int32)reader["D_thread"],
                        (float)reader["DD1"],
                        (float)reader["DD2"],
                        (Int32)reader["S"],
                        (Int32)reader["L"],
                        (Int32)reader["l1_nom"],
                        (float)reader["l1_otkl"],
                        (float)reader["l2"],
                        (float)reader["l3"],
                        (float)reader["LL"],
                        (Int32)reader["h"],
                        (float)reader["MASS"]
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
    }
}
