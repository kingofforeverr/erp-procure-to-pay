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
    public partial class DuyetHopDong : Form
    {
        private string connectionString =
            DatabaseConfig.ConnectionString;
        private DataGridView dgvDanhSach, dgvDanhSachCT, dgvDanhSachChiTiet;
        private DataGridView dgvMuaHang, dgvThanhToan;
        private TextBox txtMaHopDong, txtDienGiai, txtGiaTri, txtDiaChiGiao, txtMaNCC, txtTenNCC, txtMaNLH
                , txtDiaChi, txtMaNhanVienLap, txtMaSoThue, cboHinhThucThanhToan, cboTinhTrang;
        private DateTimePicker dtNgayKi;
        private DataTable dtHangHoa;
        public DuyetHopDong()
        {
            InitializeComponent();
            BuildUI();
        }
        private void BuildUI()
        {
            this.Text = "Duyệt hợp đồng";
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
                    Font = new Font("Segoe UI", 10, FontStyle.Regular),
                    Margin = new Padding(10, 0, 5, 0),
                    BackColor = text == "Huỷ" ? Color.LightCoral : Color.LightSkyBlue
                };
                //pnlRight.Controls.Add(btn);
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

            dgvDanhSachCT = new DataGridView
            {
                Dock = DockStyle.Fill,
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
            dgvDanhSach.RowHeadersWidth = 170;


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
            // --- Thêm 2 nút xử lý ---
            // --- Panel chứa hai nút nằm góc phải ---
            Panel pnlBottomButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                Padding = new Padding(0, 10, 20, 10),
                BackColor = Color.Transparent
            };
            pnlMainContainer.Controls.Add(pnlBottomButtons);

            // FlowLayoutPanel để canh phải
            FlowLayoutPanel pnlActions = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.LeftToRight,
                AutoSize = true,
                WrapContents = false
            };
            pnlBottomButtons.Controls.Add(pnlActions);

            // Nút phê duyệt
            Button btnPheDuyet = new Button
            {
                Text = "✅ Phê duyệt",
                Width = 180,
                Height = 45,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.LightGreen,
                Margin = new Padding(5, 0, 5, 0)
            };

            // Nút từ chối
            Button btnTuChoi = new Button
            {
                Text = "❌ Từ chối",
                Width = 180,
                Height = 45,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.LightCoral,
                Margin = new Padding(5, 0, 5, 0)
            };

            // Thêm nút vào flow layout
            pnlActions.Controls.Add(btnPheDuyet);
            pnlActions.Controls.Add(btnTuChoi);


            btnPheDuyet.Click += (s, e) =>
            {
                string maYC = txtMaHopDong.Text.Trim();

                UpdateTrangThaiHopDong(maYC, "Đã duyệt");

                SetProcessedTextBox(pnlMain, "Đã duyệt");

                MessageBox.Show("Đã phê duyệt hợp đồng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadDanhSachHopDong(dgvDanhSach);
            };

            btnTuChoi.Click += (s, e) =>
            {

                // Hiển thị hộp thoại nhập lý do từ chối
                using (Form frmLyDo = new Form())
                {
                    frmLyDo.Text = "Nhập lý do từ chối";
                    frmLyDo.StartPosition = FormStartPosition.CenterParent;
                    frmLyDo.Size = new Size(400, 250);
                    frmLyDo.FormBorderStyle = FormBorderStyle.FixedDialog;
                    frmLyDo.MaximizeBox = false;
                    frmLyDo.MinimizeBox = false;

                    Label lbl = new Label { Text = "Vui lòng nhập lý do từ chối:", Location = new Point(20, 20), AutoSize = true };
                    TextBox txtLyDo = new TextBox { Multiline = true, Location = new Point(20, 50), Size = new Size(340, 100) };
                    Button btnOK = new Button { Text = "OK", Location = new Point(170, 170), Width = 90, Height = 30, DialogResult = DialogResult.OK };
                    Button btnCancel = new Button { Text = "Hủy", Location = new Point(280, 170), Width = 90, Height = 30, DialogResult = DialogResult.Cancel };

                    frmLyDo.Controls.AddRange(new Control[] { lbl, txtLyDo, btnOK, btnCancel });
                    frmLyDo.AcceptButton = btnOK;
                    frmLyDo.CancelButton = btnCancel;

                    if (frmLyDo.ShowDialog() == DialogResult.OK)
                    {
                        string lyDo = txtLyDo.Text.Trim();
                        if (string.IsNullOrEmpty(lyDo))
                        {
                            MessageBox.Show("Bạn phải nhập lý do từ chối.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        string maYC = txtMaHopDong.Text.Trim();

                        UpdateTrangThaiHopDong(maYC, "Từ chối");


                        SetProcessedTextBox(pnlMain, "Từ chối");

                        MessageBox.Show("Hợp đồng đã bị từ chối.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    LoadDanhSachHopDong(dgvDanhSach);

                }
            };
            // Gọi hàm disable toàn bộ input khi khởi tạo
            ToggleInputs(pnlMain, false);

            // Gắn sự kiện cho nút Xem
            foreach (Control ctrl in pnlLeft.Controls)
            {
                if (ctrl is Button btn && btn.Text.Contains("Chỉnh sửa"))
                {
                    btn.Click += (s, e) =>
                    {
                        // Enable tất cả các textbox, combobox, datetimepicker
                        ToggleInputs(pnlMain, true);

                        DisableMaChungTu(pnlMain);
                    };
                }
            }

        }
        private void SetProcessedTextBox(Control container, string newValue)
        {
            foreach (Control ctrl in container.Controls)
            {
                if (ctrl is TextBox tb
                    && tb.Text.Equals("Chờ xử lý", StringComparison.OrdinalIgnoreCase))
                {
                    tb.Text = newValue;
                }

                if (ctrl.HasChildren)
                    SetProcessedTextBox(ctrl, newValue);
            }
        }

        private void UpdateTrangThaiHopDong(string maHD, string trangThai)
        {
            string sql = "UPDATE HopDong SET TinhTrang = ? WHERE MaHopDong = ?";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            using (OleDbCommand cmd = new OleDbCommand(sql, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@TinhTrang", trangThai);
                cmd.Parameters.AddWithValue("@MaHopDong", maHD);
                cmd.ExecuteNonQuery();
            }
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
        private void DgvDanhSach_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDanhSach.CurrentRow == null) return;

            var cellValue = dgvDanhSach.CurrentRow.Cells["MaYC"].Value;
            if (cellValue == null) return;

            string maHD = cellValue.ToString();

            LoadThongTinHopDong(maHD);

            LoadChiTietHopDong(maHD);

            LoadChiTietThanhToan(maHD);
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

                        txtMaNhanVienLap.Text = rd["MaNhanVienLap"].ToString();

                        txtMaNLH.Text = rd["MaNLH"].ToString();
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
                             FROM HopDong WHERE TinhTrang = 'Chờ xử lý'";

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

            string[] labels3 = {  "Địa chỉ giao", "Người lập", "Hình thức thanh toán"  };

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
        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            FormTimKiemDonMuaHang frm = new FormTimKiemDonMuaHang();
            frm.ShowDialog();


        }
    }
}
