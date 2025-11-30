using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TestAccess
{
    public partial class BaoCaoCongNoHopDong : Form
    {
        private string connectionString = DatabaseConfig.ConnectionString;

        private DataGridView dgvBaoCao;
        private Label lblTongCongNo;

        public BaoCaoCongNoHopDong()
        {
            InitializeComponent();
            TaoGiaoDien();
            LoadBaoCao();   // ⬅️ Vừa mở form là tự load luôn
        }

        private void TaoGiaoDien()
        {
            this.Text = "BÁO CÁO CÔNG NỢ THEO HỢP ĐỒNG";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.WhiteSmoke;

            // Tiêu đề
            Label lblTieuDe = new Label
            {
                Text = "BÁO CÁO CÔNG NỢ THEO HỢP ĐỒNG",
                Dock = DockStyle.Top,
                Height = 45,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                ForeColor = Color.SteelBlue
            };
            this.Controls.Add(lblTieuDe);

            // Lưới hiển thị báo cáo
            dgvBaoCao = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            dgvBaoCao.EnableHeadersVisualStyles = false;
            dgvBaoCao.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dgvBaoCao.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvBaoCao.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvBaoCao.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvBaoCao.DefaultCellStyle.SelectionBackColor = Color.LightSkyBlue;

            this.Controls.Add(dgvBaoCao);
            dgvBaoCao.BringToFront();

            // Label tổng công nợ
            lblTongCongNo = new Label
            {
                Dock = DockStyle.Bottom,
                Height = 35,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 40, 0),
                ForeColor = Color.DarkBlue,
                BackColor = Color.WhiteSmoke
            };
            this.Controls.Add(lblTongCongNo);
        }

        private void LoadBaoCao()
        {
            string sql = @"
SELECT 
    hd.MaHopDong,
    ncc.TenNCC,
    hd.NgayKy,
    hd.GiaTriHopDong,

    IIF(t.GiaTriHoaDon IS NULL,0,t.GiaTriHoaDon) AS GiaTriHoaDon,
    IIF(t.DaThanhToan IS NULL,0,t.DaThanhToan) AS DaThanhToan,

    IIF(t.GiaTriHoaDon IS NULL,0,t.GiaTriHoaDon) -
    IIF(t.DaThanhToan IS NULL,0,t.DaThanhToan) AS ConPhaiTra,

    IIF(
        IIF(t.GiaTriHoaDon IS NULL,0,t.GiaTriHoaDon)=0,
        0,
        ROUND(
            IIF(t.DaThanhToan IS NULL,0,t.DaThanhToan)*100.0 / 
            IIF(t.GiaTriHoaDon IS NULL,0,t.GiaTriHoaDon),2
        )
    ) AS PhanTramThanhToan,

    t.HanThanhToanTiep

FROM HopDong hd
LEFT JOIN NhaCungCap ncc ON hd.MaNCC = ncc.MaNCC

LEFT JOIN (
    SELECT 
        X.MaHopDong,
        SUM(X.GiaTriHoaDon) AS GiaTriHoaDon,
        SUM(X.DaThanhToan) AS DaThanhToan,
        MIN(X.HanThanhToanTiep) AS HanThanhToanTiep
    FROM (
        SELECT
            dmh.MaHopDong,
            SUM(IIF(ct.SoLuong*ct.DonGia IS NULL,0,ct.SoLuong*ct.DonGia) +
                IIF(th.GiaTriThue IS NULL,0,th.GiaTriThue)) AS GiaTriHoaDon,
            0 AS DaThanhToan,
            NULL AS HanThanhToanTiep
        FROM DonMuaHang dmh
        INNER JOIN PhieuNhapKho pnk ON dmh.SoDonDatHang = pnk.ChungTuThamChieu
        INNER JOIN HoaDonMuaHang hdMH ON pnk.SoPNK = hdMH.ChungTuThamChieu
        LEFT JOIN ChiTietHoaDonMua ct ON hdMH.SoChungTuHoaDon = ct.SoChungTuHoaDon
        LEFT JOIN ChiTietThueHoaDonMua th ON ct.MaCTHD = th.MaCTHD
        WHERE dmh.MaHopDong IS NOT NULL
        GROUP BY dmh.MaHopDong

        UNION ALL

        SELECT
            pdt.ChungTuThamChieu AS MaHopDong,
            0 AS GiaTriHoaDon,
            SUM(IIF(ptt.SoTien IS NULL,0,ptt.SoTien)) AS DaThanhToan,
            NULL AS HanThanhToanTiep
        FROM PhieuDeNghiThanhToan pdt
        INNER JOIN PhieuThanhToan ptt ON pdt.SoPhieuDeNghi = ptt.ChungTuThamChieu
        GROUP BY pdt.ChungTuThamChieu

        UNION ALL

        SELECT 
            pdt.ChungTuThamChieu AS MaHopDong,
            0 AS GiaTriHoaDon,
            0 AS DaThanhToan,
            MIN(pdt.HanThanhToan) AS HanThanhToanTiep
        FROM PhieuDeNghiThanhToan pdt
        LEFT JOIN PhieuThanhToan ptt ON pdt.SoPhieuDeNghi = ptt.ChungTuThamChieu
        WHERE ptt.SoPhieuThanhToan IS NULL
        GROUP BY pdt.ChungTuThamChieu
    ) AS X
    GROUP BY X.MaHopDong
) AS t ON hd.MaHopDong = t.MaHopDong

ORDER BY hd.MaHopDong;
";


            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvBaoCao.DataSource = dt;

                // Đặt header tiếng Việt
                dgvBaoCao.Columns["MaHopDong"].HeaderText = "Số hợp đồng";
                dgvBaoCao.Columns["NhaCungCap"].HeaderText = "Nhà cung cấp";
                dgvBaoCao.Columns["NgayKy"].HeaderText = "Ngày ký";
                dgvBaoCao.Columns["GiaTriHD"].HeaderText = "Giá trị HĐ";
                dgvBaoCao.Columns["GiaTriHoaDon"].HeaderText = "Giá trị hóa đơn";
                dgvBaoCao.Columns["DaThanhToan"].HeaderText = "Đã thanh toán";
                dgvBaoCao.Columns["ConPhaiTra"].HeaderText = "Còn phải trả";
                dgvBaoCao.Columns["PhanTramThanhToan"].HeaderText = "% Thanh toán";
                dgvBaoCao.Columns["HanThanhToanTiep"].HeaderText = "Hạn thanh toán tiếp";

                // Căn phải các cột tiền
                foreach (string col in new[] { "GiaTriHD", "GiaTriHoaDon", "DaThanhToan", "ConPhaiTra", "PhanTramThanhToan" })
                {
                    if (dgvBaoCao.Columns.Contains(col))
                        dgvBaoCao.Columns[col].DefaultCellStyle.Alignment =
                            DataGridViewContentAlignment.MiddleRight;
                }

                // ===== TỔNG CÔNG NỢ CÒN PHẢI TRẢ =====
                decimal tongCongNo = 0;
                foreach (DataRow r in dt.Rows)
                {
                    if (r["ConPhaiTra"] != DBNull.Value)
                        tongCongNo += Convert.ToDecimal(r["ConPhaiTra"]);
                }

                lblTongCongNo.Text = "TỔNG CÔNG NỢ CÒN PHẢI TRẢ: " + tongCongNo.ToString("N0");
            }
        }

    }
}
