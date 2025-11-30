using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TestAccess
{
    public partial class BaoCaoMuaHangTheoNCC : Form
    {
        private string connectionString = DatabaseConfig.ConnectionString;

        private DataTable dtHoaDon;
        private DataGridView dgvTongHop;
        private DataGridView dgvChiTiet;
        private Label lblTongTien;

        public BaoCaoMuaHangTheoNCC()
        {
            InitializeComponent();

            TaoGiaoDienBaoCao();

            try
            {
                LoadDataFromDatabase();
                HienThiTongHop();

                if (dgvTongHop.Rows.Count > 0)
                {
                    dgvTongHop.Rows[0].Selected = true;
                    string maNCC = dgvTongHop.Rows[0].Cells["MaNCC"].Value.ToString();
                    HienThiChiTietTheoNCC(maNCC);
                }

                dgvTongHop.SelectionChanged += DgvTongHop_SelectionChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi nạp dữ liệu báo cáo: " + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================== TẠO GIAO DIỆN ==================
        private void TaoGiaoDienBaoCao()
        {
            this.Text = "Báo cáo mua hàng theo nhà cung cấp";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.WhiteSmoke;

            Label lblTieuDe = new Label
            {
                Text = "BÁO CÁO MUA HÀNG THEO NHÀ CUNG CẤP",
                Dock = DockStyle.Top,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.SteelBlue
            };
            this.Controls.Add(lblTieuDe);

            SplitContainer split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 300
            };
            this.Controls.Add(split);
            split.BringToFront();

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
            dgvChiTiet.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dgvChiTiet.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvChiTiet.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvChiTiet.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvChiTiet.DefaultCellStyle.SelectionBackColor = Color.LightSkyBlue;

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
        }

        // ================== NẠP DỮ LIỆU TỪ ACCESS ==================
        private void LoadDataFromDatabase()
        {
            dtHoaDon = new DataTable();

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT  
                        ncc.MaNCC,
                        ncc.TenNCC,
                        hd.SoChungTuHoaDon,
                        hd.NgayChungTu,
                        hd.SoSeri,
                        hd.SoHoaDon,
                        hd.NgayHoaDon,
                        ct.MaHH,
                        hh.TenHH,
                        ct.SoLuong,
                        ct.DonGia,
                        (ct.SoLuong * ct.DonGia) AS ThanhTien
                    FROM 
                        ((NhaCungCap ncc
                        INNER JOIN HoaDonMuaHang hd ON hd.MaNCC = ncc.MaNCC)
                        INNER JOIN ChiTietHoaDonMua ct ON hd.SoChungTuHoaDon = ct.SoChungTuHoaDon)
                        LEFT JOIN HangHoa hh ON hh.MaHH = ct.MaHH
                    ORDER BY 
                        ncc.MaNCC, hd.NgayChungTu;
                ";

                OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
                da.Fill(dtHoaDon);
            }
        }

        // ================== TỔNG HỢP THEO NCC ==================
        private void HienThiTongHop()
        {
            var tongHop = from r in dtHoaDon.AsEnumerable()
                          group r by new
                          {
                              MaNCC = r["MaNCC"].ToString(),
                              TenNCC = r["TenNCC"].ToString()
                          }
                into g
                          select new
                          {
                              MaNCC = g.Key.MaNCC,
                              TenNCC = g.Key.TenNCC,
                              TongTien = g.Sum(x => ChuyenDecimal(x["ThanhTien"]))
                          };

            DataTable dtTongHop = new DataTable();
            dtTongHop.Columns.Add("MaNCC");
            dtTongHop.Columns.Add("TenNCC");
            dtTongHop.Columns.Add("TongTien", typeof(decimal));

            foreach (var item in tongHop)
            {
                dtTongHop.Rows.Add(item.MaNCC, item.TenNCC, item.TongTien);
            }

            dgvTongHop.DataSource = dtTongHop;

            decimal tongTatCa = dtTongHop.AsEnumerable()
                .Sum(r => r.Field<decimal>("TongTien"));

            lblTongTien.Text = $"Tổng cộng tất cả NCC: {tongTatCa:N0}";
        }

        private void DgvTongHop_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTongHop.CurrentRow == null) return;

            string maNCC = dgvTongHop.CurrentRow.Cells["MaNCC"].Value.ToString();
            HienThiChiTietTheoNCC(maNCC);
        }

        // ================== CHI TIẾT THEO TỪNG NCC ==================
        private void HienThiChiTietTheoNCC(string maNCC)
        {
            var chiTiet = dtHoaDon.AsEnumerable()
                .Where(r => r["MaNCC"].ToString() == maNCC)
                .Select(r => new
                {
                    SoChungTu = r["SoChungTuHoaDon"].ToString(),
                    NgayChungTu = ConvertDate(r["NgayChungTu"]),
                    SoSeri = r["SoSeri"].ToString(),
                    SoHoaDon = r["SoHoaDon"].ToString(),
                    NgayHoaDon = ConvertDate(r["NgayHoaDon"]),
                    MaHH = r["MaHH"].ToString(),
                    TenHH = r["TenHH"].ToString(),
                    SoLuong = r["SoLuong"].ToString(),
                    DonGia = ChuyenDecimal(r["DonGia"]),
                    ThanhTien = ChuyenDecimal(r["ThanhTien"])
                });

            DataTable dtChiTiet = new DataTable();
            dtChiTiet.Columns.Add("Số chứng từ");
            dtChiTiet.Columns.Add("Ngày CT");
            dtChiTiet.Columns.Add("Số seri");
            dtChiTiet.Columns.Add("Số HĐ");
            dtChiTiet.Columns.Add("Ngày HĐ");
            dtChiTiet.Columns.Add("Mã hàng");
            dtChiTiet.Columns.Add("Tên hàng");
            dtChiTiet.Columns.Add("Số lượng");
            dtChiTiet.Columns.Add("Đơn giá", typeof(decimal));
            dtChiTiet.Columns.Add("Thành tiền", typeof(decimal));

            foreach (var c in chiTiet)
            {
                dtChiTiet.Rows.Add(
                    c.SoChungTu,
                    c.NgayChungTu,
                    c.SoSeri,
                    c.SoHoaDon,
                    c.NgayHoaDon,
                    c.MaHH,
                    c.TenHH,
                    c.SoLuong,
                    c.DonGia,
                    c.ThanhTien
                );
            }

            dgvChiTiet.DataSource = dtChiTiet;
        }

        // ================== HÀM PHỤ ==================
        private decimal ChuyenDecimal(object value)
        {
            if (value == null || value == DBNull.Value) return 0m;
            decimal.TryParse(value.ToString().Replace(",", ""), out decimal result);
            return result;
        }

        private string ConvertDate(object obj)
        {
            if (obj == DBNull.Value || obj == null) return "";
            DateTime d;
            if (DateTime.TryParse(obj.ToString(), out d))
                return d.ToString("dd/MM/yyyy");
            return "";
        }
    }
}
