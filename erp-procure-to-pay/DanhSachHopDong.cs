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
    public partial class DanhSachHopDong : Form
    {
        private Label lblTongSL, lblTongTien, lblTonHienTai, lblDuKienNhap;
        private bool isDirty = false;
        private string connectionString =
            DatabaseConfig.ConnectionString;
        private DataGridView dgvDanhSach, dgvDanhSachCT, dgvDanhSachChiTiet;
        private DataGridView dgvMuaHang, dgvThanhToan;
        private TextBox txtMaHopDong, txtDienGiai, txtGiaTri, txtDiaChiGiao, txtMaNCC,txtTenNCC, txtMaNLH
                ,txtDiaChi, txtMaNhanVienLap, txtMaSoThue, cboHinhThucThanhToan, cboTinhTrang;
        private DateTimePicker dtNgayKi;
        private DataTable dtHangHoa;
        public DanhSachHopDong()
        {
            InitializeComponent();
            BuildUI();
            AttachControlChangeEvents();
        }
        private void BuildUI()
        {
            this.Text = "Quản lý hợp đồng";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.WhiteSmoke;
            this.AutoScroll = true;
            // === Nút Trang chủ (nằm trên thanh công cụ) ===
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(210, 230, 250)
            };
            this.Controls.Add(pnlHeader);

            Button btnTrangChu = new Button
            {
                Text = "🏠 Trang chủ",
                Height = 45,
                Width = 180,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.LightSteelBlue,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(10, 8)
            };
            btnTrangChu.FlatAppearance.BorderSize = 0;
            pnlHeader.Controls.Add(btnTrangChu);

            btnTrangChu.Click += (s, e) =>
            {
                TrangChu frmTrangChu = new TrangChu();
                frmTrangChu.Show();
                this.Hide();
            };
            // === Thanh công cụ ===
            // === Thanh công cụ bên dưới nút Trang chủ ===
            Panel pnlToolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.FromArgb(220, 235, 250),
                Padding = new Padding(10, 10, 10, 10)
            };
            this.Controls.Add(pnlToolbar);
            pnlToolbar.BringToFront();

            // Nhóm nút bên trái (Thêm, Sửa, Xoá, Xem, In, Tìm kiếm)
            FlowLayoutPanel pnlLeft = new FlowLayoutPanel
            {
                Dock = DockStyle.Left,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };
            pnlToolbar.Controls.Add(pnlLeft);

            // Nhóm nút bên phải (Lưu, Huỷ)
            FlowLayoutPanel pnlRight = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false
            };
            pnlToolbar.Controls.Add(pnlRight);

            string[] btnLeftTexts = {
                "📝 Thêm mới",
                "✏️ Chỉnh sửa",
                "🗑️ Xoá",
                "👁️ Xem",
                "🖨️ In",
                "🔍 Tìm kiếm"
            };
            foreach (var text in btnLeftTexts)
            {
                Button btn = new Button
                {
                    Text = text,
                    Height = 60,
                    Width = 150,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Margin = new Padding(5, 0, 5, 0),
                    BackColor = Color.FromArgb(220, 225, 250)
                };
                pnlLeft.Controls.Add(btn);
                if (text.Contains("Xoá"))
                    btn.Click += BtnXoa_Click;
                else if (text.Contains("Thêm"))
                    btn.Click += BtnThem_Click;
            }
            // === Gắn sự kiện cho nút "Tìm kiếm" ===
            foreach (Control ctrl in pnlLeft.Controls)
            {
                if (ctrl is Button btn && btn.Text.Contains("Tìm"))
                {
                    btn.Click += BtnTimKiem_Click;
                }
            }
            // Hai nút “Lưu” và “Huỷ” nằm bên phải, cách xa phần còn lại
            string[] btnRightTexts = {
                "💾 Lưu",
                "❌ Huỷ"
            };
            foreach (var text in btnRightTexts)
            {
                Button btn = new Button
                {
                    Text = text,
                    Height = 60,
                    Width = 150,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Margin = new Padding(10, 0, 5, 0),
                    BackColor = text.Contains("Lưu") ? Color.LightSkyBlue : Color.FromArgb(242, 52, 52),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btn.FlatAppearance.BorderSize = 0;
                pnlRight.Controls.Add(btn);
                if (text.Contains("Lưu"))
                    btn.Click += BtnLuu_Click;
            }

            // === Panel chứa nội dung chính ===
            Panel pnlMainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.WhiteSmoke
            };
            this.Controls.Add(pnlMainContainer);
            pnlMainContainer.BringToFront();

            FlowLayoutPanel pnlMain = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoScroll = false,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(10)
            };
            pnlMainContainer.Controls.Add(pnlMain);
            GroupBox grpDanhSach = new GroupBox
            {
                Text = "DANH SÁCH HỢP ĐỒNG",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 66, 168),
                Dock = DockStyle.Top,
                Height = 350,
                Padding = new Padding(10)
            };
            pnlMain.Controls.Add(grpDanhSach);

            // DataGridView hiển thị danh sách yêu cầu
            dgvDanhSach = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight = 35,
                EnableHeadersVisualStyles = false
            };
            grpDanhSach.Controls.Add(dgvDanhSach);

            // Cấu hình header
            dgvDanhSach.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 235, 250);
            dgvDanhSach.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvDanhSach.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvDanhSach.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);


            // Thêm các cột ví dụ
            dgvDanhSach.Columns.Add("MaYC", "Mã hợp đồng");
            dgvDanhSach.Columns.Add("NguoiDK", "Người đăng ký");
            dgvDanhSach.Columns.Add("NgayCT", "Ngày chứng từ");
            dgvDanhSach.Columns.Add("TrangThai", "Trạng thái");

            LoadDanhSachHopDong(dgvDanhSach);

            dgvDanhSach.SelectionChanged += DgvDanhSach_SelectionChanged;


            Panel pnlLabel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(0, 10, 0, 0),
                BackColor = Color.Transparent
            };
            pnlMain.Controls.Add(pnlLabel);
            // Label chính giữa
            Label lblThongBao = new Label
            {
                Text = "Chi tiết hợp đồng",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 66, 168),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 30
            };
            pnlLabel.Controls.Add(lblThongBao);
            // Đường line dưới label
            Panel line = new Panel
            {
                Height = 2,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(50, 66, 168),
                Margin = new Padding(0, 5, 0, 0)
            };
            pnlLabel.Controls.Add(line);
            int y = 10;

            // --- Nhóm Thông tin ---
            GroupBox grpNguoiDK = new GroupBox
            {
                Text = "Thông tin hợp đồng",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 66, 168),
                Location = new Point(10, y),
                Width = pnlMain.Width - 40,
                Height = 120,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlMain.Controls.Add(grpNguoiDK);
            // === Gọi hàm dựng layout chi tiết ===
            TaoFormThongTin(grpNguoiDK);

            y += grpNguoiDK.Height + 10;

            GroupBox grpDanhSachCT = new GroupBox
            {
                Text = "Chi tiết điều khoản mua hàng",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 66, 168),
                Dock = DockStyle.Top,
                Height = 350,
                Padding = new Padding(10)
            };
            pnlMain.Controls.Add(grpDanhSachCT);
            GroupBox grpTongHop = new GroupBox
            {
                Text = "TỔNG HỢP",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                Width = 990,
                Height = 80,
                Location = new Point(grpDanhSach.Width - 600, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            grpDanhSachCT.Controls.Add(grpTongHop);

            // === 4 ô tổng hợp nằm thành 1 hàng ngang ===
            int startX = 10;
            int labelWidth = 90;
            int textBoxWidth = 120;
            int spacingX = 30;
            string[] thongTin = { "Tổng SL", "Tổng tiền" };
            Label[] arrLabels = new Label[4];
            Color[] mauNen = { Color.MistyRose, Color.Honeydew, Color.Lavender, Color.LightCyan };

            for (int i = 0; i < thongTin.Length; i++)
            {
                Label lbl = new Label
                {
                    Text = thongTin[i] + ":",
                    Location = new Point(startX, 35),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    ForeColor = Color.DarkSlateBlue
                };

                Label lblValue = new Label
                {
                    Text = "0",
                    Location = new Point(startX + labelWidth + 10, 30),
                    Width = textBoxWidth,
                    Height = 25,
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = mauNen[i],
                    BorderStyle = BorderStyle.FixedSingle,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };

                // LƯU THAM CHIẾU RA BIẾN
                arrLabels[i] = lblValue;

                grpTongHop.Controls.Add(lbl);
                grpTongHop.Controls.Add(lblValue);

                startX += labelWidth + textBoxWidth + spacingX;
            }

            // Gán vào biến dùng toàn form
            lblTongSL = arrLabels[0];
            lblTongTien = arrLabels[1];
            lblTonHienTai = arrLabels[2];
            lblDuKienNhap = arrLabels[3];
            dgvDanhSachCT = new DataGridView
            {
                Location = new Point(10, grpTongHop.Bottom + 20),
                Width = grpDanhSachCT.Width - 20,
                Height = grpDanhSachCT.Height - grpTongHop.Bottom - 25,
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight = 35,
                EnableHeadersVisualStyles = false
            };
            grpDanhSachCT.Controls.Add(dgvDanhSachCT);

            // Cấu hình header
            dgvDanhSach.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 235, 250);
            dgvDanhSach.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvDanhSach.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvDanhSach.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);


            // Thêm các cột ví dụ
            dgvDanhSachCT.Columns.Add("MaYC", "Đợt");
            dgvDanhSachCT.Columns.Add("MaHH", "Mã hàng");
            dgvDanhSachCT.Columns.Add("TenHH", "Tên hàng");
            dgvDanhSachCT.Columns.Add("DVT", "Đơn vị tính");
            dgvDanhSachCT.Columns.Add("SL", "Số lượng");
            dgvDanhSachCT.Columns.Add("DonGia", "Đơn giá");
            dgvDanhSachCT.Columns.Add("ThanhTien", "Thành tiền");
            dgvDanhSachCT.Columns.Add("DienGiai", "Diễn giải");
            dgvDanhSachCT.Columns.Add("Vat", "%VAT");
            dgvDanhSachCT.Columns.Add("TienVat", "Tiền VAT");
            dgvDanhSachCT.Columns.Add("NgayGH", "Ngày giao hàng");
            dgvDanhSachCT.Columns.Add("ThangBH", "Số tháng bảo hành");


            dgvDanhSachCT.CellEndEdit += Dgv_CellEndEdit;
            dgvDanhSachCT.DataError += dgvDanhSachCT_DataError;
            GroupBox grpDanhSachChiTiet = new GroupBox
            {
                Text = "Chi tiết điều khoản thanh toán",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 66, 168),
                Dock = DockStyle.Top,
                Height = 350,
                Padding = new Padding(10)
            };
            pnlMain.Controls.Add(grpDanhSachChiTiet);

            // DataGridView hiển thị danh sách yêu cầu
            dgvDanhSachChiTiet = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeight = 35,
                EnableHeadersVisualStyles = false
            };
            grpDanhSachChiTiet.Controls.Add(dgvDanhSachChiTiet);

            // Cấu hình header
            dgvDanhSachChiTiet.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 235, 250);
            dgvDanhSachChiTiet.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvDanhSachChiTiet.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvDanhSachChiTiet.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);

            dgvDanhSachChiTiet.Columns.Add("STT", "Đợt");
            dgvDanhSachChiTiet.Columns.Add("TyLe", "Tỷ lệ");
            dgvDanhSachChiTiet.Columns.Add("SoTien", "Số tiền");
            dgvDanhSachChiTiet.Columns.Add("HanThanhToan", "Hạn thanh toán");
            dgvDanhSachChiTiet.Columns.Add("GhiChu", "Ghi chú");

            // Gọi hàm disable toàn bộ input khi khởi tạo
            ToggleInputs(pnlMain, false);

            // Gắn sự kiện cho nút Xem
            foreach (Control ctrl in pnlLeft.Controls)
            {
                if (ctrl is Button btn && btn.Text.Contains("Chỉnh sửa"))
                {
                    btn.Click += (s, e) =>
                    {

                        if (dgvDanhSach.SelectedRows.Count == 0)
                        {
                            MessageBox.Show("Vui lòng chọn một dòng để chỉnh sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }

                        // Lấy dòng được chọn
                        var row = dgvDanhSach.SelectedRows[0];
                        var trangThai = row.Cells[3].Value?.ToString();

                        if (!string.IsNullOrEmpty(trangThai) && trangThai.Equals("Đã xử lý", StringComparison.OrdinalIgnoreCase))
                        {
                            MessageBox.Show("Không thể chỉnh sửa hợp đồng này", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Enable tất cả các textbox, combobox, datetimepicker
                        ToggleInputs(pnlMain, true);

                        // Riêng combobox "Mã chứng từ" vẫn disable
                        DisableMaChungTu(pnlMain);
                    };
                }
            }

        }

        private void DgvDanhSach_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDanhSach.CurrentRow == null) return;

            var cellValue = dgvDanhSach.CurrentRow.Cells["MaYC"].Value;
            if (cellValue == null) return;

            string maHD = cellValue.ToString();

            LoadThongTinHopDong(maHD);

            LoadChiTietHopDong(maHD);


            LoadChiTietThanhToan(maHD);
            CapNhatTongHop();
        }

        private void LoadThongTinHopDong(string maHD)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string sqlHD = @"SELECT * FROM HopDong WHERE MaHopDong = @MaHopDong";
                OleDbCommand cmdHD = new OleDbCommand(sqlHD, conn);
                cmdHD.Parameters.AddWithValue("@MaHopDong", maHD);

                string maNCC = "";
                string manlh = "";
                string manv = "";
                using (OleDbDataReader rd = cmdHD.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        txtMaHopDong.Text = rd["MaHopDong"].ToString();

                        if (rd["NgayKy"] != DBNull.Value)
                            dtNgayKi.Value = Convert.ToDateTime(rd["NgayKy"]);

                        txtDienGiai.Text = rd["DienGiai"].ToString();
                        txtGiaTri.Text = rd["GiaTriHopDong"].ToString();
                        cboTinhTrang.Text = rd["TinhTrang"].ToString();

                        txtDiaChiGiao.Text = rd["DiaChiGiao"].ToString();

                        maNCC = rd["MaNCC"].ToString();
                        manlh = rd["MaNLH"].ToString();
                        manv = rd["MaNhanVienLap"].ToString();
                        //txtMaNhanVienLap.Text = rd["MaNhanVienLap"].ToString();

                        txtMaNLH.Text = rd["MaNLH"].ToString();
                        cboHinhThucThanhToan.Text = rd["HinhThucThanhToan"].ToString();
                    }
                }
                if (!string.IsNullOrEmpty(manv))
                {
                    string sqlNLH1 = @"SELECT * 
                              FROM NhanVien
                              WHERE MaNhanVien = @MaNLH";

                    OleDbCommand cmdNLH1 = new OleDbCommand(sqlNLH1, conn);
                    cmdNLH1.Parameters.AddWithValue("@MaNLH", manv);

                    using (OleDbDataReader rdNLH1 = cmdNLH1.ExecuteReader())
                    {
                        if (rdNLH1.Read())
                        {

                            txtMaNhanVienLap.Text = rdNLH1["HoTen"].ToString();
                        }
                    }
                }

                if (!string.IsNullOrEmpty(manlh))
                {
                    string sqlNLH = @"SELECT * 
                              FROM NguoiLienHe 
                              WHERE MaNLH = @MaNLH";

                    OleDbCommand cmdNLH = new OleDbCommand(sqlNLH, conn);
                    cmdNLH.Parameters.AddWithValue("@MaNLH", manlh);

                    using (OleDbDataReader rdNLH = cmdNLH.ExecuteReader())
                    {
                        if (rdNLH.Read())
                        {
                           
                           txtMaNLH.Text = rdNLH["TenNLH"].ToString();
                        }
                    }
                }
                if (!string.IsNullOrEmpty(manlh))
                {
                    string sqlNLH = @"SELECT * 
                              FROM NguoiLienHe 
                              WHERE MaNLH = @MaNLH";

                    OleDbCommand cmdNLH = new OleDbCommand(sqlNLH, conn);
                    cmdNLH.Parameters.AddWithValue("@MaNLH", manlh);

                    using (OleDbDataReader rdNLH = cmdNLH.ExecuteReader())
                    {
                        if (rdNLH.Read())
                        {

                            txtMaNLH.Text = rdNLH["TenNLH"].ToString();
                        }
                    }
                }
                // 2) Lấy thông tin nhà cung cấp
                if (!string.IsNullOrEmpty(maNCC))
                {
                    string sqlNCC = @"SELECT TenNCC, DiaChi, MaSoThue 
                              FROM NhaCungCap 
                              WHERE MaNCC = @MaNCC";

                    OleDbCommand cmdNCC = new OleDbCommand(sqlNCC, conn);
                    cmdNCC.Parameters.AddWithValue("@MaNCC", maNCC);

                    using (OleDbDataReader rdNCC = cmdNCC.ExecuteReader())
                    {
                        if (rdNCC.Read())
                        {
                            txtMaNCC.Text = maNCC;
                            txtTenNCC.Text = rdNCC["TenNCC"].ToString();
                            txtDiaChi.Text = rdNCC["DiaChi"].ToString();
                            txtMaSoThue.Text = rdNCC["MaSoThue"].ToString();
                        }
                    }
                }

                // 3) Lấy hình thức thanh toán
                //string sqlHTTT = @"SELECT TOP 1 HinhThucThanhToan 
                //           FROM ChiTietDieuKhoanThanhToan 
                //           WHERE MaHopDong = @MaHopDong";

                //OleDbCommand cmdTT = new OleDbCommand(sqlHTTT, conn);
                //cmdTT.Parameters.AddWithValue("@MaHopDong", maHD);

                //object httt = cmdTT.ExecuteScalar();
                //if (httt != null)
                //    cboHinhThucThanhToan.Text = httt.ToString();
            }
        }
        private void LoadChiTietHopDong(string maHD)
        {
            string sql = @"
                    SELECT 
                        ct.Dot,
                        ct.MaHH,
                        hh.TenHH,
                        hh.DonViTinh,
                        ct.SoLuongDat,
                        ct.DonGia,
                        (ct.SoLuongDat * ct.DonGia) AS ThanhTien,
                        ct.DienGiai,
                        ct.PhanTramVAT,
                        (ct.SoLuongDat * ct.DonGia * ct.PhanTramVAT / 100) AS TienVAT,
                        ct.NgayGiaoHang,
                        ct.SoThangBaoHanh
                    FROM  ChiTietDieuKhoanMuaHang ct
                    LEFT JOIN HangHoa hh ON ct.MaHH = hh.MaHH
                    WHERE ct.MaHopDong = @MaHopDong
                ";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            using (OleDbDataAdapter da = new OleDbDataAdapter(sql, conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@MaHopDong", maHD);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvDanhSachCT.Columns["MaYC"].DataPropertyName = "Dot";
                dgvDanhSachCT.Columns["MaHH"].DataPropertyName = "MaHH";
                dgvDanhSachCT.Columns["TenHH"].DataPropertyName = "TenHH";
                dgvDanhSachCT.Columns["DVT"].DataPropertyName = "DonViTinh";
                dgvDanhSachCT.Columns["SL"].DataPropertyName = "SoLuongDat";
                dgvDanhSachCT.Columns["DonGia"].DataPropertyName = "DonGia";
                dgvDanhSachCT.Columns["ThanhTien"].DataPropertyName = "ThanhTien";
                dgvDanhSachCT.Columns["DienGiai"].DataPropertyName = "DienGiai";
                dgvDanhSachCT.Columns["Vat"].DataPropertyName = "PhanTramVAT";
                dgvDanhSachCT.Columns["TienVAT"].DataPropertyName = "TienVAT";
                dgvDanhSachCT.Columns["NgayGH"].DataPropertyName = "NgayGiaoHang";
                dgvDanhSachCT.Columns["ThangBH"].DataPropertyName = "SoThangBaoHanh";

                dgvDanhSachCT.DataSource = dt;
                dgvDanhSachCT.Columns["DonGia"].DefaultCellStyle.Format = "N0";
                dgvDanhSachCT.Columns["ThanhTien"].DefaultCellStyle.Format = "N0";
                dgvDanhSachCT.Columns["TienVAT"].DefaultCellStyle.Format = "N0";
            }
        }
        private void LoadChiTietThanhToan(string maHD)
        {
            string sql = @"
                    SELECT 
                        ct.Dot,
                        ct.TyLe,
                        ct.SoTien,
                        ct.HanThanhToan,
                        ct.GhiChu,
                        ct.MaHopDong
                    FROM  ChiTietDieuKhoanThanhToan ct
                    WHERE ct.MaHopDong = @MaHopDong
                ";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            using (OleDbDataAdapter da = new OleDbDataAdapter(sql, conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@MaHopDong", maHD);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvDanhSachChiTiet.Columns["STT"].DataPropertyName = "Dot";
                dgvDanhSachChiTiet.Columns["TyLe"].DataPropertyName = "TyLe";
                dgvDanhSachChiTiet.Columns["SoTien"].DataPropertyName = "SoTien";
                dgvDanhSachChiTiet.Columns["HanThanhToan"].DataPropertyName = "HanThanhToan";
                dgvDanhSachChiTiet.Columns["GhiChu"].DataPropertyName = "GhiChu";

                dgvDanhSachChiTiet.DataSource = dt;
            }
        }
        

        private void LoadDanhSachHopDong(DataGridView dgv)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"SELECT MaHopDong, NgayKy,MaNhanVienLap, TinhTrang
                             FROM HopDong";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgv.Rows.Clear();

                        foreach (DataRow row in dt.Rows)
                        {
                            dgv.Rows.Add(
                                row["MaHopDong"].ToString(),
                                row["MaNhanVienLap"].ToString(),
                                Convert.ToDateTime(row["NgayKy"]).ToString("dd/MM/yyyy"),
                                row["TinhTrang"].ToString()
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải danh sách yêu cầu: " + ex.Message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool IsAnyTextBoxProcessed(Control container)
        {
            foreach (Control ctrl in container.Controls)
            {
                if (ctrl is TextBox tb && tb.Text.Equals("Đã xử lý", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (ctrl.HasChildren)
                {
                    if (IsAnyTextBoxProcessed(ctrl))
                        return true;
                }
            }
            return false;
        }
        private void ToggleInputs(Control parent, bool enable)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is TextBox || ctrl is ComboBox || ctrl is DateTimePicker)
                {
                    ctrl.Enabled = enable;
                }

                // Đệ quy để áp dụng cho tất cả groupbox/panel con
                if (ctrl.HasChildren)
                {
                    ToggleInputs(ctrl, enable);
                }
            }
        }
        private void TaoFormThongTin(GroupBox groupBox)
        {
            groupBox.Text = "THÔNG TIN";
            groupBox.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            groupBox.ForeColor = Color.FromArgb(50, 66, 168);
            groupBox.Padding = new Padding(10, 20, 10, 10);
            groupBox.AutoSize = true;

            int startX = 15;
            int startY = 40;
            int labelWidth = 110;
            int textBoxWidth = 200;
            int controlHeight = 32;
            int spacingX = 45;
            int spacingY = 20;
            int rowSpacing = 25;


            // Hàng 1: 
            string[] labels1 = { "Mã hợp đồng", "Ngày ký", "Diễn giải", "Giá trị hợp đồng", "Tình trạng" };
            int x = startX;
            int y = startY;

            foreach (string label in labels1)
            {
                Label lbl = new Label
                {
                    Text = label,
                    Location = new Point(x, y),
                    AutoSize = true
                };
                groupBox.Controls.Add(lbl);

                Control input;
                if (label.Contains("Ngày"))
                {
                    input = new DateTimePicker
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = textBoxWidth * 2 + spacingX,
                        Format = DateTimePickerFormat.Short
                    };
                }
                
                else
                {
                    input = new TextBox
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = label.Contains("Diễn giải") ? textBoxWidth * 2 + spacingX
                        : label.Contains("Giá trị") ? textBoxWidth * 2
                        : textBoxWidth,
                    };
                }

                ;
                groupBox.Controls.Add(input);
                if (label.Contains("Mã hợp đồng")) txtMaHopDong = (TextBox)input;
                else if (label.Contains("Ngày ký")) dtNgayKi = (DateTimePicker)input;
                else if (label.Contains("Diễn giải")) txtDienGiai = (TextBox)input;
                else if (label.Contains("Giá trị")) txtGiaTri = (TextBox)input;
                else if (label.Contains("Nhà cung cấp")) txtMaNCC = (TextBox)input;
                else if (label.Contains("Người liên hệ")) txtMaNLH = (TextBox)input;
                else if (label.Contains("Tình trạng")) cboTinhTrang = (TextBox)input;

                x += input.Width + spacingX;
            }

            // Hàng 2: Nhà cung cấp
            y += controlHeight + spacingY + rowSpacing;
            x = startX;

            string[] labels2 = { "Nhà cung cấp", "Tên nhà cung cấp", "Địa chỉ", "Mã số thuế", "Người liên hệ" };

            foreach (string label in labels2)
            {
                Label lbl = new Label
                {
                    Text = label,
                    Location = new Point(x, y),
                    AutoSize = true
                };
                groupBox.Controls.Add(lbl);

                TextBox txt = new TextBox
                {
                    Location = new Point(x, y + lbl.Height + 2),
                    //Width = textBoxWidth
                    Width = (label.Contains("Địa chỉ") || label.Contains("Tên nhà cung cấp")
                            ? textBoxWidth * 2 + spacingX
                            : label.Contains("Người liên hệ") ? textBoxWidth * 2
                            : textBoxWidth)
                };
                groupBox.Controls.Add(txt);
                if (label.Contains("Nhà cung cấp")) txtMaNCC = (TextBox)txt;
                else if (label.Contains("Tên nhà cung cấp")) txtTenNCC = (TextBox)txt;
                else if (label.Contains("Địa chỉ")) txtDiaChi = (TextBox)txt;
                else if (label.Contains("Người liên hệ")) txtMaNLH = (TextBox)txt;
                else if (label.Contains("Mã số thuế")) txtMaSoThue = (TextBox)txt;
                x += txt.Width + spacingX;
            }

            // Hàng 3: Địa điểm giao hàng
            y += controlHeight + spacingY + rowSpacing;
            x = startX;

            string[] labels3 = { "Địa chỉ giao", "Người lập", "Hình thức thanh toán" };

            foreach (string label in labels3)
            {
                Label lbl = new Label
                {
                    Text = label,
                    Location = new Point(x, y),
                    AutoSize = true
                };
                groupBox.Controls.Add(lbl);

                TextBox txt = new TextBox
                {
                    Location = new Point(x, y + lbl.Height + 2),
                    Width = (label.Contains("Địa chỉ")) ? textBoxWidth * 2 + spacingX
                    : label.Contains("điều khoản") ? textBoxWidth * 3 + spacingX
                    : textBoxWidth
                };
                groupBox.Controls.Add(txt);
                if (label.Contains("Người lập")) txtMaNhanVienLap = (TextBox)txt;
                else if (label.Contains("Địa chỉ giao")) txtDiaChiGiao = (TextBox)txt;
                else if (label.Contains("Hình thức thanh toán")) cboHinhThucThanhToan = (TextBox)txt;

                x += txt.Width + spacingX;
            }


        }
        private void DisableMaChungTu(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                // Kiểm tra chính xác tên control
                if (ctrl is ComboBox cbo && cbo.Name.Equals("cboMaChungTu", StringComparison.OrdinalIgnoreCase))
                {
                    cbo.Enabled = false;
                }

                // Đệ quy để kiểm tra các control con
                if (ctrl.HasChildren)
                {
                    DisableMaChungTu(ctrl);
                }
            }
        }
        private decimal SafeDecimal(object v)
        {
            if (v == null || v == DBNull.Value) return 0;

            string s = v.ToString()
                         .Replace(",", "")
                         .Replace(".", "")
                         .Trim();

            if (decimal.TryParse(s, out decimal result))
                return result;

            return 0;
        }
        private void Dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var row = dgvDanhSachCT.Rows[e.RowIndex];

            if (row.IsNewRow) return;

            decimal sl = SafeDecimal(row.Cells["SL"].Value);
            decimal donGia = SafeDecimal(row.Cells["DonGia"].Value);
            decimal vat = SafeDecimal(row.Cells["Vat"].Value);

            // Tính toán
            decimal thanhTien = sl * donGia;
            decimal tienVat = (thanhTien * vat) / 100;

            // Gán lại giá trị
            row.Cells["ThanhTien"].Value = thanhTien;
            row.Cells["TienVAT"].Value = tienVat;

            // Định dạng đẹp lại
            row.Cells["DonGia"].Value = donGia.ToString("N0");
            row.Cells["ThanhTien"].Value = thanhTien.ToString("N0");
            row.Cells["TienVAT"].Value = tienVat.ToString("N0");

            CapNhatTongHop();   
        }
        private void CapNhatTongHop()
        {
            decimal tongSL = 0;
            decimal tongTien = 0;

            foreach (DataGridViewRow row in dgvDanhSachCT.Rows)
            {
                if (row.IsNewRow) continue;

                decimal sl = 0, dongia = 0, tienvat = 0;

                decimal.TryParse(Convert.ToString(row.Cells["SL"].Value), out sl);
                decimal.TryParse(Convert.ToString(row.Cells["DonGia"].Value), out dongia);
                decimal.TryParse(Convert.ToString(row.Cells["TienVat"].Value), out tienvat);

                tongSL += sl;
                tongTien += (sl * dongia) + tienvat;
            }

            // Cập nhật lên giao diện
            lblTongSL.Text = tongSL.ToString("N0");
            lblTongTien.Text = tongTien.ToString("N0");
        }
        private void dgvDanhSachCT_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;  // không cho popup lỗi
        }

        private void AttachControlChangeEvents()
        {
            AttachEventsRecursive(this);
        }
        private void AttachEventsRecursive(Control parent)
        {
            foreach (Control ctl in parent.Controls)
            {
                if (ctl is TextBox txt && txt.Name != "txtMaChungTu")
                {
                    txt.TextChanged -= ControlChanged;
                    txt.TextChanged += ControlChanged;
                }

                if (ctl is ComboBox cb)
                {
                    cb.SelectedIndexChanged -= ControlChanged;
                    cb.SelectedIndexChanged += ControlChanged;
                }

                if (ctl is DateTimePicker dtp)
                {
                    dtp.ValueChanged -= ControlChanged;
                    dtp.ValueChanged += ControlChanged;
                }

                // Nếu control này chứa control con → gọi lại
                if (ctl.HasChildren)
                {
                    AttachEventsRecursive(ctl);
                }
            }
        }

        private void ControlChanged(object sender, EventArgs e)
        {
            isDirty = true;
        }


        private void BtnLuu_Click(object sender, EventArgs e)
        {
            if (!isDirty)
            {
                MessageBox.Show("Không có thay đổi để lưu.");
                return;
            }

            string maHopDong = txtMaHopDong.Text.Trim();
            double giaTriHopDong = 0;
            double.TryParse(txtGiaTri.Text.Trim(), out giaTriHopDong);
            string diaChiGiao = txtDiaChiGiao.Text.Trim();
            DateTime ngayKi = dtNgayKi.Value;
            string dienGiai = txtDienGiai.Text.Trim();
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                // UPDATE bảng chính
                string sqlUpdate = @"
            UPDATE HopDong
            SET NgayKy = ?, 
                GiaTriHopDong = ?, 
                DiaChiGiao = ?,
                DienGiai = ?
            WHERE MaHopDong = ?
        ";

                using (OleDbCommand cmd = new OleDbCommand(sqlUpdate, conn))
                {
                    cmd.Parameters.Add("@NgayChungTu", OleDbType.Date).Value = ngayKi;
                    cmd.Parameters.AddWithValue("@GiaTri", OleDbType.Currency).Value = giaTriHopDong;
                    cmd.Parameters.AddWithValue("@DiaChiGiao", OleDbType.VarChar).Value = diaChiGiao;
                    cmd.Parameters.AddWithValue("@DienGiai", OleDbType.VarChar).Value = dienGiai;
                    cmd.Parameters.AddWithValue("@MaHopDong", OleDbType.VarChar).Value = maHopDong;

                    cmd.ExecuteNonQuery();
                }

                // UPDATE chi tiết (dgv)
                UpdateChiTietHopDong(conn, maHopDong);
            }

            isDirty = false; // đã lưu xong
            MessageBox.Show(
                "Đã lưu thay đổi thành công!",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        private void UpdateChiTietHopDong(OleDbConnection conn, string maHopDong)
        {
            // 1) Lấy danh sách Dot + MaHH đang có trong DB (để tìm xem dòng nào bị xóa)
            Dictionary<string, bool> existingKeys = new Dictionary<string, bool>();

            string sqlGet = "SELECT Dot, MaHH FROM ChiTietDieuKhoanMuaHang WHERE MaHopDong = ?";
            using (OleDbCommand cmdGet = new OleDbCommand(sqlGet, conn))
            {
                cmdGet.Parameters.AddWithValue("@MaHopDong", maHopDong);
                using (OleDbDataReader rd = cmdGet.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string key = rd["Dot"].ToString() + "|" + rd["MaHH"].ToString();
                        existingKeys[key] = false; // false = chưa thấy trong grid
                    }
                }
            }

            // 2) Duyệt từng dòng trong grid để UPDATE hoặc INSERT
            foreach (DataGridViewRow row in dgvDanhSachCT.Rows)
            {
                if (row.IsNewRow) continue;

                string dot = Convert.ToString(row.Cells["MaYC"].Value);
                string maHH = Convert.ToString(row.Cells["MaHH"].Value);
                if (string.IsNullOrEmpty(dot) || string.IsNullOrEmpty(maHH)) continue;

                decimal soLuong = Convert.ToDecimal(row.Cells["SL"].Value ?? 0);
                decimal donGia = Convert.ToDecimal(row.Cells["DonGia"].Value ?? 0);
                decimal vat = Convert.ToDecimal(row.Cells["Vat"].Value ?? 0);
                decimal soThangBH = Convert.ToDecimal(row.Cells["ThangBH"].Value ?? 0);

                string dienGiai = Convert.ToString(row.Cells["DienGiai"].Value) ?? "";

                DateTime ngayGH = DateTime.Now;
                DateTime.TryParse(Convert.ToString(row.Cells["NgayGH"].Value), out ngayGH);

                string keyCheck = dot + "|" + maHH;
                bool isExist = existingKeys.ContainsKey(keyCheck);

                if (isExist)
                {
                    existingKeys[keyCheck] = true;

                    // 2.1 UPDATE nếu đã tồn tại
                    string sqlUpdate = @"
                UPDATE ChiTietDieuKhoanMuaHang
                SET 
                    SoLuongDat = ?, 
                    DonGia = ?, 
                    PhanTramVAT = ?, 
                    DienGiai = ?, 
                    NgayGiaoHang = ?, 
                    SoThangBaoHanh = ?
                WHERE MaHopDong = ? AND Dot = ? AND MaHH = ?
            ";

                    using (OleDbCommand cmd = new OleDbCommand(sqlUpdate, conn))
                    {
                        cmd.Parameters.AddWithValue("@SoLuong", soLuong);
                        cmd.Parameters.AddWithValue("@DonGia", donGia);
                        cmd.Parameters.AddWithValue("@Vat", vat);
                        cmd.Parameters.AddWithValue("@DienGiai", dienGiai);
                        cmd.Parameters.AddWithValue("@NgayGH", ngayGH);
                        cmd.Parameters.AddWithValue("@SoThangBH", soThangBH);

                        cmd.Parameters.AddWithValue("@MaHopDong", maHopDong);
                        cmd.Parameters.AddWithValue("@Dot", dot);
                        cmd.Parameters.AddWithValue("@MaHH", maHH);

                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // 2.2 INSERT nếu chưa có trong DB
                    string sqlInsert = @"
                INSERT INTO ChiTietDieuKhoanMuaHang
                (MaHopDong, Dot, MaHH, SoLuongDat, DonGia, PhanTramVAT, DienGiai, NgayGiaoHang, SoThangBaoHanh)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)
            ";

                    using (OleDbCommand cmd = new OleDbCommand(sqlInsert, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaHopDong", maHopDong);
                        cmd.Parameters.AddWithValue("@Dot", dot);
                        cmd.Parameters.AddWithValue("@MaHH", maHH);
                        cmd.Parameters.AddWithValue("@SoLuong", soLuong);
                        cmd.Parameters.AddWithValue("@DonGia", donGia);
                        cmd.Parameters.AddWithValue("@Vat", vat);
                        cmd.Parameters.AddWithValue("@DienGiai", dienGiai);
                        cmd.Parameters.AddWithValue("@NgayGH", ngayGH);
                        cmd.Parameters.AddWithValue("@SoThangBH", soThangBH);

                        cmd.ExecuteNonQuery();
                    }
                }
            }

            // 3) DELETE: những dòng trong DB nhưng không còn trong grid
            foreach (var pair in existingKeys)
            {
                if (pair.Value == false) // nghĩa là không xuất hiện trong grid
                {
                    var parts = pair.Key.Split('|');
                    string dot = parts[0];
                    string maHH = parts[1];

                    string sqlDelete = @"
                DELETE FROM ChiTietDieuKhoanMuaHang 
                WHERE MaHopDong = ? AND Dot = ? AND MaHH = ?
            ";

                    using (OleDbCommand cmd = new OleDbCommand(sqlDelete, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaHopDong", maHopDong);
                        cmd.Parameters.AddWithValue("@Dot", dot);
                        cmd.Parameters.AddWithValue("@MaHH", maHH);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (dgvDanhSach.SelectedRows.Count == 0)
            {
                MessageBox.Show("Không thể xóa hợp đồng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvDanhSach.SelectedRows[0];
            string trangThai = selectedRow.Cells["TrangThai"].Value.ToString();
            string maYeuCau = selectedRow.Cells["MaYC"].Value.ToString();

            if (trangThai == "Đã duyệt")
            {
                MessageBox.Show("Không thể xóa hợp đồng đã duyệt!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Bạn có chắc muốn xóa hợp đồng '{maYeuCau}' không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
                return;

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    // 1) Xóa chi tiết trước
                    string sqlCT = "DELETE FROM ChiTietDieuKhoanMuaHang WHERE MaHopDong = ?";
                    using (OleDbCommand cmdCT = new OleDbCommand(sqlCT, conn))
                    {
                        cmdCT.Parameters.AddWithValue("@p1", maYeuCau);
                        cmdCT.ExecuteNonQuery();
                    }

                    string sqlTT = "DELETE FROM ChiTietDieuKhoanThanhToan WHERE MaHopDong = ?";
                    using (OleDbCommand cmdCTTT = new OleDbCommand(sqlTT, conn))
                    {
                        cmdCTTT.Parameters.AddWithValue("@p1", maYeuCau);
                        cmdCTTT.ExecuteNonQuery();
                    }

                    // 2) Xóa bảng chính
                    string sqlMain = "DELETE FROM HopDong WHERE MaHopDong = ?";
                    using (OleDbCommand cmdMain = new OleDbCommand(sqlMain, conn))
                    {
                        cmdMain.Parameters.AddWithValue("@p1", maYeuCau);
                        cmdMain.ExecuteNonQuery();
                    }
                }

                // 3) Xóa khỏi lưới
                dgvDanhSach.Rows.Remove(selectedRow);

                MessageBox.Show($"Đã xóa '{maYeuCau}' thành công.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void BtnTimKiem_Click(object sender, EventArgs e)
        {

            FormTimKiemHopDongcs formTimKiem = new FormTimKiemHopDongcs();

            if (formTimKiem.ShowDialog() == DialogResult.OK)
            {

                DateTime? ngayCTTu = null;
                DateTime? ngayCTDen = null;

                // Chỉ lấy ngày khi checkbox được tick
                if (formTimKiem.LocTheoNgay)
                {
                    ngayCTTu = formTimKiem.NgayTu;
                    ngayCTDen = formTimKiem.NgayDen;
                }

                string trangThai = formTimKiem.TrangThai;
                string maChungTu = formTimKiem.MaChungTu;
                string nguoidangki = formTimKiem.NguoiDangKi;
                LocDanhSach(ngayCTTu, ngayCTDen, trangThai, maChungTu,nguoidangki);
            }
        }
        private void LocDanhSach(DateTime? ngayCTTu, DateTime? ngayCTDen,
                 string trangThai, string maChungTu, string nguoidangki)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"
                SELECT MaHopDong, NgayKy, MaNhanVienLap, TinhTrang
                FROM HopDong
                WHERE 1 = 1
            ";

                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = conn;

                    // ===== LỌC THEO NGÀY (NẾU CÓ) =====
                    if (ngayCTTu.HasValue)
                    {
                        query += " AND DateValue(NgayKy) >= DateValue(@Tu)";
                        cmd.Parameters.AddWithValue("@Tu", ngayCTTu.Value);
                    }

                    if (ngayCTDen.HasValue)
                    {
                        query += " AND DateValue(NgayKy) <= DateValue(@Den)";
                        cmd.Parameters.AddWithValue("@Den", ngayCTDen.Value);
                    }

                    // ===== TRẠNG THÁI =====
                    if (!string.IsNullOrWhiteSpace(trangThai))
                    {
                        query += " AND TinhTrang = @TrangThai";
                        cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                    }

                    // ===== MÃ CHỨNG TỪ =====
                    if (!string.IsNullOrWhiteSpace(maChungTu))
                    {
                        query += " AND  MaHopDong LIKE @MaCT";
                        cmd.Parameters.AddWithValue("@MaCT", "%" + maChungTu + "%");
                    }

                    if (!string.IsNullOrWhiteSpace(nguoidangki))
                    {
                        query += " AND MaNhanVienLap LIKE @MaNV";
                        cmd.Parameters.AddWithValue("@MaNV", "%" + nguoidangki + "%");
                    }

                    cmd.CommandText = query;

                    DataTable dt = new DataTable();
                    using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }


                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show(
                            "Không tìm thấy hợp đồng phù hợp!",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                        dgvDanhSach.Rows.Clear();
                        return;
                    }

                    dgvDanhSach.Rows.Clear();

                    foreach (DataRow row in dt.Rows)
                    {
                        dgvDanhSach.Rows.Add(
                            row["MaHopDong"].ToString(),
                            row["MaNhanVienLap"].ToString(),
                            Convert.ToDateTime(row["NgayKy"]).ToString("dd/MM/yyyy"),
                            row["TinhTrang"].ToString()
                        );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lọc: " + ex.Message);
                }
            }
        }
        private void BtnThem_Click(object sender, EventArgs e)
        {
            QuanLyHopDong ds = new QuanLyHopDong();
            ds.ShowDialog();
        }
    }
}
