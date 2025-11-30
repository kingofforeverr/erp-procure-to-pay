using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestAccess
{
    public partial class BaoCaoCongNoTheoNCCcs : Form
    {
        public BaoCaoCongNoTheoNCCcs()
        {
            InitializeComponent();
            TaoGiaoDienBaoCao();
        }

        private DataGridView dgvTongHop, dgvChiTiet;
        private DataTable dtTongHop, dtChiTiet;
        private ComboBox cboKhachHang;
        private DateTimePicker dtTuNgay, dtDenNgay;
        private Button btnTim, btnIn, btnExcel, btnThoat;



        private void TaoGiaoDienBaoCao()
        {
            this.Text = "Báo cáo công nợ phải trả nhà cung cấp";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.WhiteSmoke;

            // === TIÊU ĐỀ CHÍNH ===
            Label lblTieuDe = new Label
            {
                Text = "TỔNG HỢP & CHI TIẾT CÔNG NỢ PHẢI TRẢ NHÀ CUNG CẤP",
                Dock = DockStyle.Top,
                Height = 50,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.SteelBlue,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblTieuDe);

            // === PANEL TIÊU CHÍ BÁO CÁO ===
            Panel pnlFilter = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                Padding = new Padding(10),
                BackColor = Color.AliceBlue
            };
            this.Controls.Add(pnlFilter);

            Label lblTuNgay = new Label { Text = "Từ ngày:", AutoSize = true, Left = 10, Top = 15 };
            dtTuNgay = new DateTimePicker { Left = 70, Top = 10, Width = 120 };
            Label lblDenNgay = new Label { Text = "Đến ngày:", AutoSize = true, Left = 210, Top = 15 };
            dtDenNgay = new DateTimePicker { Left = 280, Top = 10, Width = 120 };

            Label lblKH = new Label { Text = "Khách hàng:", AutoSize = true, Left = 430, Top = 15 };
            cboKhachHang = new ComboBox { Left = 510, Top = 10, Width = 200, DropDownStyle = ComboBoxStyle.DropDownList };

            btnTim = new Button { Text = "Tìm (Ctrl+R)", Left = 730, Top = 10, Width = 100, Height = 30 };
            btnIn = new Button { Text = "In (Ctrl+P)", Left = 840, Top = 10, Width = 90, Height = 30 };
            btnExcel = new Button { Text = "Xuất Excel (Ctrl+E)", Left = 940, Top = 10, Width = 130, Height = 30 };
            btnThoat = new Button { Text = "Thoát (F12)", Left = 1080, Top = 10, Width = 100, Height = 30 };

            pnlFilter.Controls.AddRange(new Control[]
            {
                lblTuNgay, dtTuNgay, lblDenNgay, dtDenNgay,
                lblKH, cboKhachHang, btnTim, btnIn, btnExcel, btnThoat
            });

            // === SPLIT CONTAINER ===
            SplitContainer split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Horizontal,
                SplitterDistance = 300
            };
            this.Controls.Add(split);
            split.BringToFront();
            // ==== BẢNG TỔNG HỢP ====
            dgvTongHop = new DataGridView
            {
                Dock = DockStyle.Top,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            split.Panel1.Controls.Add(dgvTongHop);
            //StyleHeader(dgvTongHop);

            dtTongHop = TaoBangTongHop();
            dgvTongHop.DataSource = dtTongHop;
            dgvTongHop.BringToFront();  
            dgvTongHop.SelectionChanged += DgvTongHop_SelectionChanged;

            // ==== BẢNG CHI TIẾT ====
            dgvChiTiet = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White
            };
            split.Panel2.Controls.Add(dgvChiTiet);
            StyleHeader(dgvChiTiet);

            dtChiTiet = TaoBangChiTiet();
            dgvChiTiet.DataSource = dtChiTiet;
            TaoHeaderGopChiTiet();

            // Sự kiện nút
            btnTim.Click += (s, e) => LocDuLieu();
            btnThoat.Click += (s, e) => this.Close();
            btnExcel.Click += (s, e) => MessageBox.Show("Xuất Excel (demo)");
            btnIn.Click += (s, e) => MessageBox.Show("In báo cáo (demo)");
        }

        private void StyleHeader(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
        }

        private void DgvTongHop_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvTongHop.CurrentRow == null) return;
            string ncc = dgvTongHop.CurrentRow.Cells["Nhà cung cấp"].Value.ToString();

            var filtered = dtChiTiet.AsEnumerable()
                .Where(r => r["Nhà cung cấp"].ToString() == ncc);

            dgvChiTiet.DataSource = filtered.Any() ? filtered.CopyToDataTable() : null;
        }

        private void LocDuLieu()
        {
            MessageBox.Show("Lọc dữ liệu theo ngày và khách hàng (demo)");
        }

        // === HEADER GỘP PHẦN CHI TIẾT ===
        private void TaoHeaderGopChiTiet()
        {
            dgvChiTiet.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            dgvChiTiet.ColumnHeadersHeight = 105;
            dgvChiTiet.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvChiTiet.Paint += (s, e) =>
            {
                using (SolidBrush b = new SolidBrush(Color.SteelBlue))
                {
                    // CHỨNG TỪ
                    Rectangle r1 = dgvChiTiet.GetCellDisplayRectangle(1, -1, true);
                    int w1 = dgvChiTiet.Columns["Số HĐ"].Width + dgvChiTiet.Columns["Ngày HĐ"].Width - 2;
                    r1.Width = w1;
                    r1.Height = dgvChiTiet.ColumnHeadersHeight / 3;
                    e.Graphics.FillRectangle(b, r1);
                    e.Graphics.DrawString("CHỨNG TỪ", new Font("Segoe UI", 9, FontStyle.Bold), Brushes.White,
                        r1, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                    // SỐ TIỀN CÔNG NỢ
                    Rectangle r2 = dgvChiTiet.GetCellDisplayRectangle(3, -1, true);
                    int w2 = dgvChiTiet.Columns["Phải trả"].Width + dgvChiTiet.Columns["Đã trả"].Width - 2;
                    r2.Width = w2;
                    r2.Height = dgvChiTiet.ColumnHeadersHeight / 3;
                    e.Graphics.FillRectangle(b, r2);
                    e.Graphics.DrawString("SỐ TIỀN CÔNG NỢ", new Font("Segoe UI", 9, FontStyle.Bold), Brushes.White,
                        r2, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });

                    // THỜI GIAN CÔNG NỢ
                    Rectangle r3 = dgvChiTiet.GetCellDisplayRectangle(5, -1, true);
                    int w3 = dgvChiTiet.Columns.Cast<DataGridViewColumn>()
                        .Skip(5).Take(4).Sum(c => c.Width) - 2;
                    r3.Width = w3;
                    r3.Height = dgvChiTiet.ColumnHeadersHeight / 3;
                    e.Graphics.FillRectangle(b, r3);
                    e.Graphics.DrawString("THỜI GIAN CÔNG NỢ", new Font("Segoe UI", 9, FontStyle.Bold), Brushes.White,
                        r3, new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                }
            };

            dgvChiTiet.CellPainting += (s, e) =>
            {
                if (e.RowIndex == -1)
                {
                    e.PaintBackground(e.ClipBounds, false);
                    Rectangle r2 = e.CellBounds;
                    r2.Y += dgvChiTiet.ColumnHeadersHeight / 2;
                    r2.Height = dgvChiTiet.ColumnHeadersHeight / 2;
                    e.PaintContent(r2);
                    e.Handled = true;
                }
            };
        }

        // === DỮ LIỆU GIẢ ===
        private DataTable TaoBangTongHop()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("STT");
            dt.Columns.Add("Nhà cung cấp");
            dt.Columns.Add("Nợ đầu kỳ", typeof(decimal));
            dt.Columns.Add("Phát sinh nợ", typeof(decimal));
            dt.Columns.Add("Phát sinh có", typeof(decimal));
            dt.Columns.Add("Nợ cuối kỳ", typeof(decimal));
            dt.Columns.Add("Có cuối kỳ", typeof(decimal));

            dt.Rows.Add("1", "Công ty Hà Tôn Cung", 20000000, 10000000, 5000000, 15000000, 0);
            dt.Rows.Add("2", "Công ty TNHH GearXS", 10000000, 8000000, 2000000, 16000000, 0);
            dt.Rows.Add("3", "Công ty Cổ phần WORKIT", 5000000, 4000000, 1000000, 8000000, 0);

            cboKhachHang.DataSource = dt.Copy();
            cboKhachHang.DisplayMember = "Nhà cung cấp";

            return dt;
        }

        private DataTable TaoBangChiTiet()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("STT");
            dt.Columns.Add("Số HĐ");
            dt.Columns.Add("Ngày HĐ");
            dt.Columns.Add("Phải trả", typeof(decimal));
            dt.Columns.Add("Đã trả", typeof(decimal));
            dt.Columns.Add("Từ 01-30 ngày", typeof(decimal));
            dt.Columns.Add("Từ 31-60 ngày", typeof(decimal));
            dt.Columns.Add("Từ 61-90 ngày", typeof(decimal));
            dt.Columns.Add("Quá 90 ngày", typeof(decimal));
            dt.Columns.Add("Ngày tuổi nợ", typeof(int));
            dt.Columns.Add("Tổng số tiền còn nợ", typeof(decimal));
            dt.Columns.Add("Nhà cung cấp");

            dt.Rows.Add("1", "HD001", "01/09/2023", 20000000, 10000000, 5000000, 3000000, 2000000, 0, 45, 10000000, "Công ty Hà Tôn Cung");
            dt.Rows.Add("2", "HD002", "05/09/2023", 15000000, 5000000, 4000000, 3000000, 3000000, 0, 60, 10000000, "Công ty Hà Tôn Cung");
            dt.Rows.Add("3", "HD003", "10/09/2023", 12000000, 4000000, 2000000, 3000000, 500000, 0, 40, 8000000, "Công ty TNHH GearXS");
            return dt;
        }
    }
}
