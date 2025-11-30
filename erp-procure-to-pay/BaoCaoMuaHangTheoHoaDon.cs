using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TestAccess
{
    public partial class BaoCaoMuaHangTheoHoaDon : Form
    {
        private string connectionString = DatabaseConfig.ConnectionString;

        private DataGridView dgvHoaDon, dgvChiTiet;
        private Label lblTongTien;


        public BaoCaoMuaHangTheoHoaDon()
        {
            InitializeComponent();
            TaoGiaoDien();
            LoadDanhSachHoaDon();
        }

        private void TaoGiaoDien()
        {
            this.Text = "Báo cáo mua hàng theo hóa đơn";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.WhiteSmoke;

            // ===== TIÊU ĐỀ =====
            Label lbl = new Label
            {
                Text = "BÁO CÁO MUA HÀNG THEO HÓA ĐƠN",
                Dock = DockStyle.Top,
                Height = 45,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lbl);

            // ===== LƯỚI HÓA ĐƠN =====
            dgvHoaDon = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            this.Controls.Add(dgvHoaDon);
            dgvHoaDon.BringToFront();

            dgvHoaDon.EnableHeadersVisualStyles = false;
            dgvHoaDon.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dgvHoaDon.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvHoaDon.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvHoaDon.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvHoaDon.DefaultCellStyle.SelectionBackColor = Color.LightSkyBlue;

            // ===== LABEL TỔNG TIỀN =====
            lblTongTien = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Blue,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 40, 0)
            };
            this.Controls.Add(lblTongTien);
        }


        private void LoadDanhSachHoaDon()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                SELECT 
                    hd.SoChungTuHoaDon      AS MaChungTu,
                    hd.SoHoaDon,
                    hd.SoSeri,
                    hd.NgayChungTu,
                    hd.NgayHoaDon,
                    ncc.TenNCC AS NhaCungCap,

                    IIF(ISNULL(SUM(ct.SoLuong * ct.DonGia)), 0, SUM(ct.SoLuong * ct.DonGia)) AS TienHang,

                    IIF(ISNULL(SUM(th.GiaTriThue)), 0, SUM(th.GiaTriThue)) AS TienThue,

                    IIF(ISNULL(SUM(ct.SoLuong * ct.DonGia)), 0, SUM(ct.SoLuong * ct.DonGia)) +
                    IIF(ISNULL(SUM(th.GiaTriThue)), 0, SUM(th.GiaTriThue)) AS TongTien

                FROM 
                     ((HoaDonMuaHang AS hd
                LEFT JOIN ChiTietHoaDonMua AS ct
                     ON hd.SoChungTuHoaDon = ct.SoChungTuHoaDon)
                LEFT JOIN ChiTietThueHoaDonMua AS th
                     ON ct.MaCTHD = th.MaCTHD)
                LEFT JOIN NhaCungCap AS ncc
                     ON hd.MaNCC = ncc.MaNCC

                GROUP BY 
                    hd.SoChungTuHoaDon, hd.SoHoaDon, hd.SoSeri,
                    hd.NgayChungTu, hd.NgayHoaDon, ncc.TenNCC

                ORDER BY hd.NgayChungTu DESC;
                ";

                OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvHoaDon.DataSource = dt;

                lblTongTien.Text =
                    "Tổng giá trị tất cả hóa đơn: " +
                    dt.AsEnumerable().Sum(r => Convert.ToDecimal(r["TongTien"])).ToString("N0");

                DatLaiHeader();
            }
        }

        private void DatLaiHeader()
        {
            dgvHoaDon.Columns["MaChungTu"].HeaderText = "Mã chứng từ";
            dgvHoaDon.Columns["SoHoaDon"].HeaderText = "Số hóa đơn";
            dgvHoaDon.Columns["SoSeri"].HeaderText = "Số seri";
            dgvHoaDon.Columns["NgayChungTu"].HeaderText = "Ngày chứng từ";
            dgvHoaDon.Columns["NgayHoaDon"].HeaderText = "Ngày hóa đơn";
            dgvHoaDon.Columns["NhaCungCap"].HeaderText = "Nhà cung cấp";
            dgvHoaDon.Columns["TienHang"].HeaderText = "Tiền hàng";
            dgvHoaDon.Columns["TienThue"].HeaderText = "Tiền thuế";
            dgvHoaDon.Columns["TongTien"].HeaderText = "Tổng tiền";
        }

        // ==================== LOAD CHI TIẾT THEO HÓA ĐƠN ====================
        private void DgvHoaDon_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvHoaDon.CurrentRow == null) return;

            string soCT = dgvHoaDon.CurrentRow.Cells["MaChungTu"].Value.ToString();
            LoadChiTietHoaDon(soCT);
        }

        private void LoadChiTietHoaDon(string soCT)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                SELECT
                    ct.MaHH,
                    hh.TenHH,
                    ct.SoLuong,
                    ct.DonGia,
                    (ct.SoLuong * ct.DonGia) AS ThanhTien,
                    th.PhanTramVAT,
                    th.GiaTriThue,
                    ct.TKNo,
                    ct.TKCo
                FROM 
                    ((ChiTietHoaDonMua AS ct
                LEFT JOIN HangHoa AS hh 
                    ON ct.MaHH = hh.MaHH)
                LEFT JOIN ChiTietThueHoaDonMua AS th
                    ON ct.MaCTHD = th.MaCTHD)
                WHERE ct.SoChungTuHoaDon = ?
                ORDER BY ct.MaCTHD;
                ";

                OleDbCommand cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("@p1", soCT);

                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvChiTiet.DataSource = dt;

                dgvChiTiet.Columns["MaHH"].HeaderText = "Mã hàng";
                dgvChiTiet.Columns["TenHH"].HeaderText = "Tên hàng";
                dgvChiTiet.Columns["SoLuong"].HeaderText = "Số lượng";
                dgvChiTiet.Columns["DonGia"].HeaderText = "Đơn giá";
                dgvChiTiet.Columns["ThanhTien"].HeaderText = "Thành tiền";
                dgvChiTiet.Columns["PhanTramVAT"].HeaderText = "% VAT";
                dgvChiTiet.Columns["GiaTriThue"].HeaderText = "Tiền thuế";
                dgvChiTiet.Columns["TKNo"].HeaderText = "TK Nợ";
                dgvChiTiet.Columns["TKCo"].HeaderText = "TK Có";
            }
        }
    }
}
