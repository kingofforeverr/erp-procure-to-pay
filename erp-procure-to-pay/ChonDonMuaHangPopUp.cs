using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestAccess
{
    public partial class ChonDonMuaHangPopUp : Form
    {
        public string SelectedMaHopDong { get; private set; }

        private DataGridView dgv;

        private string connectionString = DatabaseConfig.ConnectionString;


        public ChonDonMuaHangPopUp()
        {
            this.Text = "Chọn đơn mua hàng";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            dgv = new DataGridView
            {
                Dock = DockStyle.Top,
                Height = 400,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            this.Controls.Add(dgv);

            Button btnOK = new Button
            {
                Text = "Chọn",
                Dock = DockStyle.Bottom,
                Height = 40,
                BackColor = Color.MediumSeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnOK.Click += BtnOK_Click;
            this.Controls.Add(btnOK);

            LoadDonMuaHang();
        }

        private void LoadDonMuaHang()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT SoDonDatHang, MaNCC, MaNLH FROM DonMuaHang";
                OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count > 0)
            {
                SelectedMaHopDong = dgv.SelectedRows[0].Cells["SoDonDatHang"].Value.ToString();
                this.DialogResult = DialogResult.OK;
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một đơn mua hàng!");
            }
        }
    }
}
