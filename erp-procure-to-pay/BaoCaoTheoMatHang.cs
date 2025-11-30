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
    public partial class BaoCaoTheoMatHang : Form
    {
        private string connectionString = DatabaseConfig.ConnectionString;

        public BaoCaoTheoMatHang()
        {
            InitializeComponent();
            TaoGiaoDienBaoCao();
        }
        private DataTable dtHoaDon;
        private DataGridView dgvTongHop, dgvChiTiet;
        private Label lblTongTien;

        private void TaoGiaoDienBaoCao()
        {
            this.Text = "Báo cáo mua hàng theo mặt hàng";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.WhiteSmoke;

            // Tiêu đề
            Label lblTieuDe = new Label
            {
                Text = "BÁO CÁO MUA HÀNG THEO MẶT HÀNG",
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.SteelBlue
            };
            this.Controls.Add(lblTieuDe);

            // SplitContainer chia hai phần: Tổng hợp (trên) và Chi tiết (dưới)
            SplitContainer split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 300
            };
            this.Controls.Add(split);
            split.BringToFront();

            // DataGridView Tổng hợp
            dgvTongHop = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            split.Panel1.Controls.Add(dgvTongHop);

            dgvTongHop.EnableHeadersVisualStyles = false;
            dgvTongHop.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dgvTongHop.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvTongHop.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvTongHop.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvTongHop.DefaultCellStyle.SelectionBackColor = Color.LightSkyBlue;

            // DataGridView Chi tiết
            dgvChiTiet = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            split.Panel2.Controls.Add(dgvChiTiet);

            dgvChiTiet.EnableHeadersVisualStyles = false;
            dgvChiTiet.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue; ;
            dgvChiTiet.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvChiTiet.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvChiTiet.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvChiTiet.DefaultCellStyle.SelectionBackColor = Color.LightSkyBlue;

            // Label tổng tiền
            lblTongTien = new Label
            {
                Text = "",
                Dock = DockStyle.Bottom,
                Height = 40,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Padding = new Padding(0, 0, 40, 0),
                ForeColor = Color.Blue
            };
            this.Controls.Add(lblTongTien);
            LoadTongHop();
            //dtHoaDon = TaoDataTableHoaDon();

            

            if (dgvTongHop.Rows.Count > 0)
            {
                dgvTongHop.Rows[0].Selected = true;
                string maHang = dgvTongHop.Rows[0].Cells["MaHH"].Value.ToString();
                LoadChiTietTheoMatHang(maHang);
            }

            // Sự kiện chọn dòng tổng hợp -> hiển thị chi tiết
            dgvTongHop.SelectionChanged += DgvTongHop_SelectionChanged;
        }

        private void LoadTongHop()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string sql = @"
            SELECT 
                hh.MaHH,
                hh.TenHH,
                SUM(ct.SoLuong) AS TongSoLuong,
                SUM(ct.SoLuong * ct.DonGia) AS TongTien
            FROM HangHoa hh
            LEFT JOIN ChiTietHoaDonMua ct ON hh.MaHH = ct.MaHH
            GROUP BY hh.MaHH, hh.TenHH
            ORDER BY hh.MaHH";

                OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                // Tính tổng tất cả mặt hàng để tính %
                decimal tongTatCa = (decimal)dt.AsEnumerable()
                 .Sum(r => r.Field<double?>("TongTien") ?? 0);

                // Thêm cột tỉ lệ %
                dt.Columns.Add("TiLe", typeof(decimal));

                foreach (DataRow r in dt.Rows)
                {
                    decimal tien = (decimal)(r.Field<double?>("TongTien") ?? 0);
                    r["TiLe"] = tongTatCa == 0 ? 0 : Math.Round((tien / tongTatCa) * 100, 2);
                }

                dgvTongHop.DataSource = dt;
                dgvTongHop.Columns["MaHH"].HeaderText = "Mã hàng";
                dgvTongHop.Columns["TenHH"].HeaderText = "Tên hàng";
                dgvTongHop.Columns["TongSoLuong"].HeaderText = "Số lượng";
                dgvTongHop.Columns["TongTien"].HeaderText = "Thành tiền";
                dgvTongHop.Columns["TiLe"].HeaderText = "Tỉ lệ (%)";
                lblTongTien.Text = $"Tổng giá trị tất cả mặt hàng: {tongTatCa:N0}";
            }
        }


        private void DgvTongHop_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTongHop.CurrentRow == null) return;
            string maHang = dgvTongHop.CurrentRow.Cells["MaHH"].Value.ToString();
            LoadChiTietTheoMatHang(maHang);
        }

        private void LoadChiTietTheoMatHang(string maHH)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string sql = @"
            SELECT  
                hd.SoChungTuHoaDon      AS SoChungTu,
                hd.NgayChungTu          AS NgayChungTu,
                hd.SoSeri               AS SoSeri,
                hd.SoHoaDon             AS SoHoaDon,
                hd.NgayHoaDon           AS NgayHoaDon,
                ncc.TenNCC              AS TenNCC,
                ct.SoLuong              AS SoLuong,
                ct.DonGia               AS DonGia,
                (ct.SoLuong * ct.DonGia) AS ThanhTien
            FROM 
                (ChiTietHoaDonMua AS ct
                 INNER JOIN HoaDonMuaHang AS hd 
                        ON hd.SoChungTuHoaDon = ct.SoChungTuHoaDon)
                LEFT JOIN NhaCungCap AS ncc 
                        ON hd.MaNCC = ncc.MaNCC
            WHERE ct.MaHH = ?
            ORDER BY hd.NgayChungTu;
        ";

                using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                {
                    // OleDb chỉ quan tâm thứ tự dấu ? nên đặt tên gì cũng được
                    cmd.Parameters.AddWithValue("@p1", maHH);

                    OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvChiTiet.DataSource = dt;

                    // Đặt lại header tiếng Việt
                    dgvChiTiet.Columns["SoChungTu"].HeaderText = "Số chứng từ";
                    dgvChiTiet.Columns["NgayChungTu"].HeaderText = "Ngày chứng từ";
                    dgvChiTiet.Columns["SoSeri"].HeaderText = "Số seri";
                    dgvChiTiet.Columns["SoHoaDon"].HeaderText = "Số hóa đơn";
                    dgvChiTiet.Columns["NgayHoaDon"].HeaderText = "Ngày hóa đơn";
                    dgvChiTiet.Columns["TenNCC"].HeaderText = "Nhà cung cấp";
                    dgvChiTiet.Columns["SoLuong"].HeaderText = "Số lượng";
                    dgvChiTiet.Columns["DonGia"].HeaderText = "Đơn giá";
                    dgvChiTiet.Columns["ThanhTien"].HeaderText = "Thành tiền";

                    // Canh phải các cột số cho đẹp
                    dgvChiTiet.Columns["SoLuong"].DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;
                    dgvChiTiet.Columns["DonGia"].DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;
                    dgvChiTiet.Columns["ThanhTien"].DefaultCellStyle.Alignment =
                        DataGridViewContentAlignment.MiddleRight;
                }
            }
        }



        private decimal ChuyenDecimal(object value)
        {
            decimal.TryParse(value.ToString().Replace(",", ""), out decimal result);
            return result;
        }

        
    }
}
