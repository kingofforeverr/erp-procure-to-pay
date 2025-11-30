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
    public partial class ChonHopDongPopup : Form
    {
        public string SelectedMaHopDong { get; private set; }
        public int SelectedDot { get; private set; } = 0;

        private DataGridView dgv;
        private DataGridView dgvDot;
        private string connectionString = DatabaseConfig.ConnectionString;


        public ChonHopDongPopup()
        {
            this.Text = "Chọn hợp đồng";
            this.Size = new Size(800, 800);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            // PANEL CHỨA 2 GRID
            Panel panelTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 250
            };
            this.Controls.Add(panelTop);

            Panel panelBottom = new Panel
            {
                Dock = DockStyle.Top,
                Height = 250
            };
            this.Controls.Add(panelBottom);

            // GRID HỢP ĐỒNG
            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            dgv.SelectionChanged += DgvHD_SelectionChanged;
            panelTop.Controls.Add(dgv);

            // GRID ĐỢT
            dgvDot = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            panelBottom.Controls.Add(dgvDot);

            // Button chọn
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

            // Load danh sách HĐ
            LoadHopDong();
        }

        private void LoadHopDong()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                string sql = "SELECT MaHopDong, NgayKy, GiaTriHopDong, MaNCC, MaNLH FROM HopDong";
                OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.DataSource = dt;
            }
        }

        // Khi chọn hợp đồng → load ĐỢT
        private void DgvHD_SelectionChanged(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0) return;

            string maHD = dgv.SelectedRows[0].Cells["MaHopDong"].Value.ToString();

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                SELECT DISTINCT Dot 
                FROM ChiTietDieuKhoanMuaHang
                WHERE MaHopDong=@MaHopDong
                ORDER BY Dot";

                OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
                da.SelectCommand.Parameters.AddWithValue("@MaHopDong", maHD);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvDot.DataSource = dt;
            }
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (dgv.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn hợp đồng!");
                return;
            }

            if (dgvDot.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ĐỢT của hợp đồng!");
                return;
            }

            SelectedMaHopDong = dgv.SelectedRows[0].Cells["MaHopDong"].Value.ToString();
            SelectedDot = Convert.ToInt32(dgvDot.SelectedRows[0].Cells["Dot"].Value);

            this.DialogResult = DialogResult.OK;
        }
    }
}
