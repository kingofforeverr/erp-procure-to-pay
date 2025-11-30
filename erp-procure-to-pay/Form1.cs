using System.Data;
using System.Data;
using System.Data.OleDb;
using System;
using System.Windows.Forms;
namespace TestAccess
{
    public partial class Form1 : Form
    {
        OleDbConnection conn;
        OleDbDataAdapter adapter;
        DataTable dt;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "" || txtAge.Text == "")
            {
                MessageBox.Show("Nhập đủ Name và Age!");
                return;
            }

            string sql = "INSERT INTO Students (Name, Age) VALUES (@name, @age)";
            OleDbCommand cmd = new OleDbCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", txtName.Text);
            cmd.Parameters.AddWithValue("@age", int.Parse(txtAge.Text));

            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();

            //LoadData(); // refresh
            txtName.Clear();
            txtAge.Clear();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            string connStr =
                @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=F:\reportkt\Database1.accdb;";
            conn = new OleDbConnection(connStr);

            //LoadData();
        }
        //private void LoadData()
        //{
        //    adapter = new OleDbDataAdapter("SELECT * FROM Students", conn);
        //    dt = new DataTable();
        //    adapter.Fill(dt);
        //    dataGridView1.DataSource = dt;
        //}
    }
}
