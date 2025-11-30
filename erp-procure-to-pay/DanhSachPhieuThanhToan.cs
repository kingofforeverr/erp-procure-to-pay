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
    public partial class DanhSachPhieuThanhToan : Form
    {
        
        private DataGridView dgvDanhSach;

        private ComboBox cboMaCT;
        private DateTimePicker dtNgayCT;
        private TextBox txtSoCT;
        private ComboBox cboLoaiTien;
        private TextBox txtCTThamChieu;

        private TextBox txtMaNCC;
        private TextBox txtTenNCC;
        private TextBox txtDiaChi;
        private TextBox txtMaSoThue;
        private TextBox txtDienThoaiNCC;

        private TextBox txtMaNhanVienNop;
        private TextBox txtTenNguoiNop;
        private TextBox txtDienThoaiNguoiNop;
        private TextBox txtNoiDungNN;

        private TextBox txtTenNguoiNop_TM;
        private TextBox txtDienThoaiNguoiNop_TM;
        private TextBox txtLyDoChi_TM;
        private TextBox txtQuyChi_TM;

        private TextBox txtTaiKhoanChi;
        private TextBox txtNganHangChi;
        private TextBox txtChiNhanhChi;
        private TextBox txtNoiDungChuyenKhoan;
        private TextBox txtTaiKhoanThuHuong;
        private TextBox txtTenChuTK;
        private TextBox txtNganHangThuHuong;
        private TextBox txtChiNhanhThuHuong;
        private TextBox txtSwiftCode;

        private GroupBox grpTienMat;
        private GroupBox grpChuyenKhoan;
        private Label lblTongSL, lblTongTien, lblTonHienTai, lblDuKienNhap;

        private DataGridView dgvChiTietThanhToan, dgv;

        private bool isDirty = false;     // theo dõi thay đổi
        private string currentPhieu = ""; 

        // ==== CHUỖI KẾT NỐI DATABASE ====
        private string connectionString =
            DatabaseConfig.ConnectionString;
        public DanhSachPhieuThanhToan()
        {
            InitializeComponent();
            BuildUI();
            LoadMaChungTu();
            LoadDanhSachPhieuThanhToan();
            AttachControlChangeEvents();
        }
        private void BuildUI()

        {
            this.Text = "Danh sách phiếu thanh toán";
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
                if (btn.Text.Contains("Tìm"))
                {
                    btn.Click += BtnTimKiem_Click;
                }
                else if (btn.Text.Contains("Thêm mới"))
                {
                    btn.Click += BtnThem_Click;
                }
                else if (btn.Text.Contains("Xoá"))
                {
                    btn.Click += BtnXoa_Click;
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
                    BackColor = text.Contains("Lưu") ? Color.LightSkyBlue : Color.Firebrick,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btn.FlatAppearance.BorderSize = 0;
                pnlRight.Controls.Add(btn);
                if (text.Contains("Lưu"))
                {
                    btn.Click += BtnLuu_Click;
                }
            }


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
            int y = 10;
            GroupBox grpDanhSach = new GroupBox
            {
                Text = "DANH SÁCH PHIẾU THANH TOÁN",
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
            dgvDanhSach.Columns.Add("MaYC", "Mã chứng từ");
            dgvDanhSach.Columns.Add("NguoiDK", "Tên nhà cung cấp");
            dgvDanhSach.Columns.Add("NgayCT", "Ngày chứng từ");

            dgvDanhSach.SelectionChanged += DgvDanhSach_SelectionChanged;
            // --- Nhóm Thông tin ---
            GroupBox grpNguoiDK = new GroupBox
            {
                Text = "Thông tin",
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

            // --- Nhóm THÔNG TIN phương thức “Tiền mặt ---
            // --- Nhóm THÔNG TIN phương thức “Tiền mặt ---
            grpTienMat = new GroupBox
            {
                Text = "PHƯƠNG THỨC: TIỀN MẶT",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 66, 168),
                Location = new Point(10, y),
                Width = pnlMain.Width - 40,
                Height = 150,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlMain.Controls.Add(grpTienMat);
            TaoFormTienMat(grpTienMat);
            y += grpTienMat.Height + 10;

            // --- Nhóm THÔNG TIN phương thức “Chuyển khoản” ---
            grpChuyenKhoan = new GroupBox
            {
                Text = "PHƯƠNG THỨC: CHUYỂN KHOẢN",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 66, 168),
                Location = new Point(10, y),
                Width = pnlMain.Width - 40,
                Height = 300,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlMain.Controls.Add(grpChuyenKhoan);
            TaoFormChuyenKhoan(grpChuyenKhoan);
            y += grpChuyenKhoan.Height + 10;

            // --- Khi khởi tạo: disable 2 group này ---
            SetGroupEnabled(grpTienMat, false);
            SetGroupEnabled(grpChuyenKhoan, false);


            // --- Nhóm THÔNG TIN phương thức “Chuyển khoản" ---
            // --- Nhóm CHI TIẾT MẶT HÀNG ---
            GroupBox grpChiTiet = new GroupBox
            {
                Text = "CHI TIẾT THANH TOÁN",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 66, 168),
                Location = new Point(10, y),
                Width = pnlMain.Width - 40,
                Height = 600,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            pnlMain.Controls.Add(grpChiTiet);

            // === Nhóm nhỏ: Tổng hợp (ở góc trên bên phải) ===
            GroupBox grpTongHop = new GroupBox
            {
                Text = "TỔNG HỢP",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                Width = 990,
                Height = 80,
                Location = new Point(grpChiTiet.Width - 400, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            grpChiTiet.Controls.Add(grpTongHop);

            // === 4 ô tổng hợp nằm thành 1 hàng ngang ===
            int startX = 10;
            int labelWidth = 90;
            int textBoxWidth = 120;
            int spacingX = 30;
            string[] thongTin = { "Tổng tiền" };
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
            lblTongTien = arrLabels[0];

            dgv = new DataGridView
            {


                Location = new Point(10, grpTongHop.Bottom + 10), // đặt phía dưới nhóm tổng hợp
                Width = grpChiTiet.Width - 20,
                Height = grpChiTiet.Height - grpTongHop.Bottom - 25,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackgroundColor = Color.White,
                AllowUserToAddRows = true,
                ColumnHeadersHeight = 35,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                BorderStyle = BorderStyle.FixedSingle,
                EnableHeadersVisualStyles = false
            };
            grpChiTiet.Controls.Add(dgv);

            // Tùy chỉnh header
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 235, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);

            // Thêm các cột với độ rộng hợp lý

            dgv.Columns.Add("SoTien", "Số tiền");
            dgv.Columns["SoTien"].Width = 400;

            dgv.Columns.Add("DienGiai", "Diễn giải");
            dgv.Columns["DienGiai"].Width = 410;

            dgv.Columns.Add("Vat", "TK Nợ");
            dgv.Columns["Vat"].Width = 500;

            dgv.Columns.Add("TienVat", "TK Có");
            dgv.Columns["TienVat"].Width = 500;



            // Căn giữa header và dữ liệu số
            dgv.Columns["SoTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["DienGiai"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["Vat"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["TienVat"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }


        private void TaoFormThongTin(GroupBox groupBox)
        {
            groupBox.Text = "THÔNG TIN";
            groupBox.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            groupBox.ForeColor = Color.FromArgb(50, 66, 168);
            groupBox.Padding = new Padding(10, 20, 10, 10);
            groupBox.AutoSize = true;

            int startX = 15, startY = 70, textBoxWidth = 350;
            int controlHeight = 32, spacingX = 30, spacingY = 20, rowSpacing = 25;

            // --- Button Chọn ---
            Button btnChon = new Button
            {
                Text = "Chọn",
                Location = new Point(startX, 30),
                Size = new Size(80, controlHeight),
                BackColor = Color.MediumSeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            groupBox.Controls.Add(btnChon);
            //btnChon.Click += BtnLayChungTuThamChieu_Click;

            string[] labels1 = { "Mã CT*", "Ngày CT*", "Số CT*", "Loại tiền*", "CT gốc" };
            int x = startX, y = startY;

            foreach (string label in labels1)
            {
                Label lbl = new Label { Text = label, Location = new Point(x, y), AutoSize = true };
                groupBox.Controls.Add(lbl);

                Control input;

                if (label == "Mã CT*")
                {
                    cboMaCT = new ComboBox
                    {
                        Location = new Point(x, y + 20),
                        Width = textBoxWidth,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    cboMaCT.SelectedIndexChanged += CboMaCT_SelectedIndexChanged;
                    groupBox.Controls.Add(cboMaCT);
                    x += textBoxWidth + spacingX;
                    continue;
                }
                else if (label == "Ngày CT*")
                {
                    dtNgayCT = new DateTimePicker
                    {
                        Location = new Point(x, y + 20),
                        Width = textBoxWidth,
                        Format = DateTimePickerFormat.Short
                    };
                    input = dtNgayCT;
                }
                else if (label == "Số CT*")
                {
                    txtSoCT = new TextBox { Location = new Point(x, y + 20), Width = textBoxWidth };
                    input = txtSoCT;
                }
                else if (label == "Loại tiền*")
                {
                    cboLoaiTien = new ComboBox
                    {
                        Location = new Point(x, y + 20),
                        Width = textBoxWidth,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    LoadNgoaiTe(cboLoaiTien);
                    input = cboLoaiTien;
                }
                else // CT gốc
                {
                    txtCTThamChieu = new TextBox { Location = new Point(x, y + 20), Width = textBoxWidth };
                    input = txtCTThamChieu;
                }

                groupBox.Controls.Add(input);
                x += textBoxWidth + spacingX;
            }

            // ===============================
            // === HÀNG 2: ĐƠN VỊ / NCC ======
            // ===============================
            y += controlHeight + spacingY + rowSpacing;
            x = startX;

            string[] labels2 = { "Mã đơn vị", "Tên đơn vị", "Địa chỉ", "Mã số thuế", "Điện thoại" };

            foreach (string label in labels2)
            {
                Label lbl = new Label { Text = label, Location = new Point(x, y), AutoSize = true };
                groupBox.Controls.Add(lbl);

                TextBox txt = new TextBox { Location = new Point(x, y + lbl.Height + 2), Width = textBoxWidth };
                groupBox.Controls.Add(txt);

                if (label == "Mã đơn vị") txtMaNCC = txt;
                else if (label == "Tên đơn vị") txtTenNCC = txt;
                else if (label == "Địa chỉ") txtDiaChi = txt;
                else if (label == "Mã số thuế") txtMaSoThue = txt;
                else if (label == "Điện thoại") txtDienThoaiNCC = txt;

                x += textBoxWidth + spacingX;
            }

            
        }

        private void TaoFormTienMat(GroupBox groupBox)
        {
            int startX = 15, startY = 40, textBoxWidth = 320, spacingX = 30;

            string[] labels = { "Mã nhân viên nộp","Tên người nộp", "Điện thoại người nộp", "Lý do chi", "Quỹ chi" };
            int x = startX, y = startY;

            foreach (string label in labels)
            {
                Label lbl = new Label { Text = label, Location = new Point(x, y), AutoSize = true };
                groupBox.Controls.Add(lbl);

                TextBox txt = new TextBox
                {
                    Location = new Point(x, y + lbl.Height + 3),
                    Width = textBoxWidth
                };
                groupBox.Controls.Add(txt);

                if (label == "Tên người nộp") txtTenNguoiNop_TM = txt;
                else if (label == "Điện thoại người nộp") txtDienThoaiNguoiNop_TM = txt;
                else if (label == "Lý do chi") txtLyDoChi_TM = txt;
                else if (label == "Quỹ chi") txtQuyChi_TM = txt;
                else if (label == "Mã nhân viên nộp") txtMaNhanVienNop = txt;

                x += txt.Width + spacingX;
            }
        }

        private void TaoFormChuyenKhoan(GroupBox groupBox)
        {
            int startX = 15, startY = 40, textBoxWidth = 300, spacingX = 25;
            int controlHeight = 32, spacingY = 25, rowSpacing = 35;

            string[] labels1 =
            {
        "Tài khoản chi*", "Ngân hàng chi*", "Chi nhánh",
        "Nội dung chuyển khoản*", "Tài khoản thụ hưởng*", "Tên chủ TK*"
    };

            int x = startX, y = startY;

            foreach (string label in labels1)
            {
                Label lbl = new Label { Text = label, Location = new Point(x, y), AutoSize = true };
                groupBox.Controls.Add(lbl);

                TextBox txt = new TextBox { Location = new Point(x, y + 20), Width = textBoxWidth };
                groupBox.Controls.Add(txt);

                if (label == "Tài khoản chi*") txtTaiKhoanChi = txt;
                else if (label == "Ngân hàng chi*") txtNganHangChi = txt;
                else if (label == "Chi nhánh") txtChiNhanhChi = txt;
                else if (label == "Nội dung chuyển khoản*") txtNoiDungChuyenKhoan = txt;
                else if (label == "Tài khoản thụ hưởng*") txtTaiKhoanThuHuong = txt;
                else if (label == "Tên chủ TK*") txtTenChuTK = txt;

                x += textBoxWidth + spacingX;
            }

            // Hàng thứ 2
            y += controlHeight + spacingY + rowSpacing;
            x = startX;

            string[] labels2 =
            {
        "Ngân hàng thụ hưởng*", "Chi nhánh", "SWIFT/BIC (nếu cần)"
    };

            foreach (string label in labels2)
            {
                Label lbl = new Label { Text = label, Location = new Point(x, y), AutoSize = true };
                groupBox.Controls.Add(lbl);

                TextBox txt = new TextBox { Location = new Point(x, y + 20), Width = textBoxWidth };
                groupBox.Controls.Add(txt);

                if (label == "Ngân hàng thụ hưởng*") txtNganHangThuHuong = txt;
                else if (label == "Chi nhánh") txtChiNhanhThuHuong = txt;
                else if (label.Contains("SWIFT")) txtSwiftCode = txt;

                x += textBoxWidth + spacingX;
            }
        }


        private void LoadNgoaiTe(ComboBox input)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT MaNgoaiTe, TenNgoaiTe FROM NgoaiTe";
                    using (OleDbDataAdapter da = new OleDbDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        input.DataSource = dt;
                        input.DisplayMember = "MaNgoaiTe";
                        input.ValueMember = "MaNgoaiTe";
                        input.SelectedIndex = -1;

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
                }
            }
        }
        private void LoadMaChungTu()
        {
            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                SELECT MaCT ,TenNghiepVu
                FROM NghiepVu 
                WHERE LoaiNghiepVu = 'Phiếu thanh toán'
            ";

                    OleDbDataAdapter da = new OleDbDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cboMaCT.DataSource = dt;
                    cboMaCT.DisplayMember = "TenNghiepVu";
                    cboMaCT.ValueMember = "MaCT";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi load mã chứng từ: " + ex.Message);
            }
        }

        private void SetGroupEnabled(Control parent, bool enabled)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is TextBox || ctrl is ComboBox || ctrl is DateTimePicker)
                    ctrl.Enabled = enabled;
            }
        }
        // Event khi chọn loại Mã CT
        private void CboMaCT_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboMaCT.SelectedItem == null) return;
            string selected = cboMaCT.SelectedItem.ToString();

            if (selected == "Phiếu chi")
            {
                SetGroupEnabled(grpTienMat, true);
                SetGroupEnabled(grpChuyenKhoan, false);
            }
            else if (selected == "Ủy nhiệm chi")
            {
                SetGroupEnabled(grpTienMat, false);
                SetGroupEnabled(grpChuyenKhoan, true);
            }
        }

        private void LoadDanhSachPhieuThanhToan()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string sql = @"
            SELECT p.SoPhieuThanhToan,
                   p.NgayChungTu,
                   dv.TenNCC
            FROM PhieuThanhToan p
            LEFT JOIN NhaCungCap dv ON p.MaDonVi = dv.MaNCC
            ORDER BY p.SoPhieuThanhToan DESC";

                using (OleDbDataAdapter da = new OleDbDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvDanhSach.Rows.Clear();

                    foreach (DataRow r in dt.Rows)
                    {
                        dgvDanhSach.Rows.Add(
                            r["SoPhieuThanhToan"].ToString(),
                            r["TenNCC"].ToString(),
                            Convert.ToDateTime(r["NgayChungTu"]).ToString("dd/MM/yyyy")
                        );
                    }
                }
            }
        }
        private void ClearGroup(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is TextBox txt)
                    txt.Text = "";

                if (ctrl is ComboBox cb)
                    cb.SelectedIndex = -1;

                if (ctrl.HasChildren)
                    ClearGroup(ctrl);
            }
        }

        private void DgvDanhSach_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDanhSach.CurrentRow == null) return;

            var cellValue = dgvDanhSach.CurrentRow.Cells["MaYC"].Value;
            if (cellValue == null) return;

            string maHD = cellValue.ToString();

            string maCT = "";
            if (maHD.StartsWith("PC"))
                maCT = "Phiếu chi";
            else if (maHD.StartsWith("UNC"))
                maCT = "Ủy nhiệm chi";

            // --------- Xoá thông tin không phù hợp ---------
            if (maCT == "Phiếu chi")
            {
                ClearGroup(grpChuyenKhoan);

                SetGroupEnabled(grpTienMat, true);
                SetGroupEnabled(grpChuyenKhoan, false);
            }
            else if (maCT == "Ủy nhiệm chi")
            {
                ClearGroup(grpTienMat);

                SetGroupEnabled(grpTienMat, false);
                SetGroupEnabled(grpChuyenKhoan, true);
            }

            LoadThongTinPhieuThanhToan(maHD);
            LoadChiTietPhieuThanhToan(maHD);
            CapNhatTongHop();
        }

        private void LoadThongTinPhieuThanhToan(string soPhieu)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string sql = @"SELECT * FROM PhieuThanhToan WHERE SoPhieuThanhToan = ?";
                OleDbCommand cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("@p1", soPhieu);

                string maNCC = "";
                string maNhanVienNop = "";
                string tkthuhuong = "";
                string tkchi = "";
                using (OleDbDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        // ===== THÔNG TIN CHUNG =====
                        string cbomact = rd["SoPhieuThanhToan"].ToString().Trim().ToUpper();

                        // Xác định loại chứng từ dựa vào 2–3 ký tự đầu
                        if (cbomact.StartsWith("PC"))
                            cboMaCT.Text = "Phiếu chi";
                        else if (cbomact.StartsWith("UNC"))
                            cboMaCT.Text = "Ủy nhiệm chi";
                        else
                            cboMaCT.Text = ""; // Phiếu chi / Ủy nhiệm chi
                        dtNgayCT.Value = Convert.ToDateTime(rd["NgayChungTu"]);
                        txtSoCT.Text = rd["SoPhieuThanhToan"].ToString();
                        cboLoaiTien.Text = rd["MaNgoaiTe"].ToString();
                        txtCTThamChieu.Text = rd["ChungTuThamChieu"].ToString();
                        txtLyDoChi_TM.Text = rd["LyDoChi"].ToString();
                        txtQuyChi_TM.Text = rd["QuyChi"].ToString();
                        // ===== LẤY MÃ NCC VÀ MÃ NGƯỜI NỘP =====
                        maNCC = rd["MaDonVi"].ToString();
                        maNhanVienNop = rd["MaNhanVienNop"].ToString();

                        tkchi = rd["SoTaiKhoanNganHang"].ToString();
                        tkthuhuong = rd["SoTaiKhoanNganHangNCC"].ToString();
                        
                    }
                }
                if (!string.IsNullOrWhiteSpace(tkchi))
                {
                    string sqlNCC = @"SELECT * FROM TaiKhoanNganHang WHERE SoTaiKhoanNganHang = ?";
                    OleDbCommand cmdNCC = new OleDbCommand(sqlNCC, conn);
                    cmdNCC.Parameters.AddWithValue("@p1", tkchi);

                    using (OleDbDataReader rdN = cmdNCC.ExecuteReader())
                    {
                        if (rdN.Read())
                        {
                            txtTaiKhoanChi.Text = tkchi;
                            txtNganHangChi.Text = rdN["MaNganHang"].ToString();
                            txtChiNhanhChi.Text = rdN["TenChiNhanh"].ToString();
                        }
                    }
                }
                if (!string.IsNullOrWhiteSpace(tkthuhuong))
                {
                    string sqlNCC1 = @"SELECT * FROM TaiKhoanNganHang WHERE SoTaiKhoanNganHang= ?";
                    OleDbCommand cmdNCC = new OleDbCommand(sqlNCC1, conn);
                    cmdNCC.Parameters.AddWithValue("@p1", tkthuhuong);

                    using (OleDbDataReader rdN = cmdNCC.ExecuteReader())
                    {
                        if (rdN.Read())
                        {
                            txtTaiKhoanThuHuong.Text = tkthuhuong;
                            txtNganHangThuHuong.Text = rdN["MaNganHang"].ToString();
                            txtChiNhanhThuHuong.Text = rdN["TenChiNhanh"].ToString();
                            txtTenChuTK.Text = rdN["TenTaiKhoanNganHang"].ToString();
                            txtSwiftCode.Text = rdN["SwiftCode"].ToString();
                        }
                    }
                }
              
                if (!string.IsNullOrEmpty(maNCC))
                {
                    string sqlNCC = @"SELECT TenNCC, DiaChi, MaSoThue, DienThoai FROM NhaCungCap WHERE MaNCC = ?";
                    OleDbCommand cmdNCC = new OleDbCommand(sqlNCC, conn);
                    cmdNCC.Parameters.AddWithValue("@p1", maNCC);

                    using (OleDbDataReader rdN = cmdNCC.ExecuteReader())
                    {
                        if (rdN.Read())
                        {
                            txtMaNCC.Text = maNCC;
                            txtTenNCC.Text = rdN["TenNCC"].ToString();
                            txtDiaChi.Text = rdN["DiaChi"].ToString();
                            txtMaSoThue.Text = rdN["MaSoThue"].ToString();
                            txtDienThoaiNCC.Text = rdN["DienThoai"].ToString();
                        }
                    }
                }

           
                if (!string.IsNullOrEmpty(maNhanVienNop))
                {
                    string sqlNN = @"SELECT *
                             FROM NhanVien 
                             WHERE MaNhanVien = ?";

                    OleDbCommand cmdNN = new OleDbCommand(sqlNN, conn);
                    cmdNN.Parameters.AddWithValue("@p1", maNhanVienNop);

                    using (OleDbDataReader rdNN = cmdNN.ExecuteReader())
                    {
                        if (rdNN.Read())
                        {
                            txtMaNhanVienNop.Text = maNhanVienNop;
                            //txtTenNguoiNop.Text = rdNN["HoTen"].ToString();
                            //txtDienThoaiNguoiNop.Text = rdNN["DienThoai"].ToString();
                            txtTenNguoiNop_TM.Text = rdNN["HoTen"].ToString();
                            txtDienThoaiNguoiNop_TM.Text = rdNN["DienThoai"].ToString();

                        }
                    }
                }
            }
            LoadChiTietTheoLoai(soPhieu);
           
            CboMaCT_SelectedIndexChanged(null, null);

            isDirty = false;
        }
        private void CapNhatTongHop()
        {
            decimal tongTien = 0;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                decimal sotien = 0;

                decimal.TryParse(Convert.ToString(row.Cells["SoTien"].Value), out sotien);


                tongTien += sotien;
            }

            // Cập nhật lên giao diện
            lblTongTien.Text = tongTien.ToString("N0");
        }
        private void LoadChiTietTheoLoai(string soPhieu)
        {
            if (cboMaCT.Text == "Phiếu chi")
            {
                SetGroupEnabled(grpTienMat, true);
                SetGroupEnabled(grpChuyenKhoan, false);
            }
            else
            {
                SetGroupEnabled(grpTienMat, false);
                SetGroupEnabled(grpChuyenKhoan, true);
            }
        }
        private void LoadChiTietPhieuThanhToan(string soPhieu)
        {
            string sql = @"
        SELECT 
            SoTien,
            DienGiai,
            TKNo,
            TKCo
        FROM ChiTietPhieuThanhToan
        WHERE SoPhieuThanhToan = ?
    ";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            using (OleDbCommand cmd = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@p1", soPhieu);

                conn.Open();
                OleDbDataReader rd = cmd.ExecuteReader();

                // Xóa dòng cũ trước khi load
                dgv.Rows.Clear();

                while (rd.Read())
                {
                    string soTien = rd["SoTien"]?.ToString() ?? "";
                    string dienGiai = rd["DienGiai"]?.ToString() ?? "";
                    string tkNo = rd["TKNo"]?.ToString() ?? "";
                    string tkCo = rd["TKCo"]?.ToString() ?? "";

                    dgv.Rows.Add(
                        soTien,
                        dienGiai,
                        tkNo,      
                        tkCo      
                    );
                }

                rd.Close();
            }

            CapNhatTongHop();
        }

        private void AttachControlChangeEvents()
        {
            AttachEventsRecursive(this);
            AttachDataGridEvents(dgv);
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
        private void AttachDataGridEvents(DataGridView grid)
        {
            if (grid == null) return;

            // Khi sửa giá trị ô
            grid.CellValueChanged -= GridChanged;
            grid.CellValueChanged += GridChanged;

            // Khi thêm / xoá dòng
            grid.RowsAdded -= GridChanged;
            grid.RowsAdded += GridChanged;

            grid.RowsRemoved -= GridChanged;
            grid.RowsRemoved += GridChanged;

            // Khi người dùng kết thúc edit ô
            grid.CellEndEdit -= GridChanged;
            grid.CellEndEdit += GridChanged;

            // Khi paste dữ liệu
            grid.CurrentCellDirtyStateChanged -= GridChanged;
            grid.CurrentCellDirtyStateChanged += GridChanged;
        }

        // Sự kiện dùng chung
        private void GridChanged(object sender, EventArgs e)
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
            DataGridViewRow selectedRow = dgvDanhSach.SelectedRows[0];
            string soPTT = selectedRow.Cells["MaYC"].Value.ToString();


            DateTime ngayCT = dtNgayCT.Value;

            string maNgoaiTe = cboLoaiTien.Text.Trim();
            string noiDung = txtLyDoChi_TM.Text.Trim();
            string chungTuThamChieu = txtCTThamChieu.Text.Trim();

            string maNCC = txtMaNCC.Text.Trim();
            string maNVNop = txtMaNhanVienNop.Text.Trim();

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                // === UPDATE bảng PHIEUTHANHTOAN ===
                UpdatePhieuThanhToan(conn, soPTT);

                // === UPDATE bảng CHITIETPHIEUTHANHTOAN ===
                UpdateChiTietPhieuThanhToan(conn, soPTT);
            }

            isDirty = false;
            MessageBox.Show(
                "Đã lưu thay đổi thành công!",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        private void UpdatePhieuThanhToan(OleDbConnection conn, string soPTT)
        {
            string sql = @"
        UPDATE PhieuThanhToan
        SET NgayChungTu = ?, 
            MaNgoaiTe = ?, 
            ChungTuThamChieu = ?, 
            PhuongThucThanhToan = ?, 
            MaNhanVienNop = ?, 
            LyDoChi = ?, 
            QuyChi = ?
        WHERE SoPhieuThanhToan = ?
    ";

            using (OleDbCommand cmd = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.Add("@NgayChungTu", OleDbType.Date).Value = dtNgayCT.Value;
                cmd.Parameters.Add("@MaNgoaiTe", OleDbType.VarChar).Value = cboLoaiTien.Text.Trim();
                cmd.Parameters.Add("@ChungTuThamChieu", OleDbType.VarChar).Value = txtCTThamChieu.Text.Trim();
                cmd.Parameters.Add("@PhuongThucThanhToan", OleDbType.VarChar).Value = cboMaCT.Text.Trim();
                cmd.Parameters.Add("@MaNhanVienNop", OleDbType.VarChar).Value = txtMaNhanVienNop.Text.Trim();
                cmd.Parameters.Add("@LyDoChi", OleDbType.VarChar).Value = txtLyDoChi_TM.Text.Trim();
                cmd.Parameters.Add("@QuyChi", OleDbType.VarChar).Value = txtQuyChi_TM.Text.Trim();

                cmd.Parameters.Add("@SoPTT", OleDbType.VarChar).Value = soPTT;

                cmd.ExecuteNonQuery();
            }
        }

        private T SafeValue<T>(object value, T defaultValue)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            return (T)Convert.ChangeType(value, typeof(T));
        }

        private void UpdateChiTietPhieuThanhToan(OleDbConnection conn, string soPTT)
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                decimal soTien = SafeValue(row.Cells["SoTien"].Value, 0m);
                string dienGiai = SafeValue(row.Cells["DienGiai"].Value, "");
                string tkNo = SafeValue(row.Cells["Vat"].Value, "");
                string tkCo = SafeValue(row.Cells["TienVat"].Value, "");

                // Bỏ qua dòng trống hẳn
                if (soTien == 0 && dienGiai == "" && tkNo == "" && tkCo == "")
                    continue;

                // Kiểm tra tồn tại
                string checkSQL = @"SELECT COUNT(*) FROM ChiTietPhieuThanhToan 
                            WHERE SoPhieuThanhToan = ? AND DienGiai = ?";

                bool exists;

                using (OleDbCommand cmdCheck = new OleDbCommand(checkSQL, conn))
                {
                    cmdCheck.Parameters.Add("@SoPTT", OleDbType.VarChar).Value = soPTT;
                    cmdCheck.Parameters.Add("@DG", OleDbType.VarChar).Value = dienGiai;

                    exists = (int)cmdCheck.ExecuteScalar() > 0;
                }

                if (exists)
                {
                    // UPDATE
                    string updateSQL = @"
                UPDATE ChiTietPhieuThanhToan
                SET SoTien = ?, TKNo = ?, TKCo = ?
                WHERE SoPhieuThanhToan = ? AND DienGiai = ?
            ";

                    using (OleDbCommand cmd = new OleDbCommand(updateSQL, conn))
                    {
                        cmd.Parameters.Add("@SoTien", OleDbType.Double).Value = soTien;
                        cmd.Parameters.Add("@TKNo", OleDbType.VarChar).Value = tkNo;
                        cmd.Parameters.Add("@TKCo", OleDbType.VarChar).Value = tkCo;
                        cmd.Parameters.Add("@SoPTT", OleDbType.VarChar).Value = soPTT;
                        cmd.Parameters.Add("@DG", OleDbType.VarChar).Value = dienGiai;

                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // INSERT
                    string insertSQL = @"
                INSERT INTO ChiTietPhieuThanhToan
                (SoTien, DienGiai, TKNo, TKCo, SoPhieuThanhToan)
                VALUES (?, ?, ?, ?, ?)
            ";

                    using (OleDbCommand cmd = new OleDbCommand(insertSQL, conn))
                    {
                        cmd.Parameters.Add("@SoTien", OleDbType.Double).Value = soTien;
                        cmd.Parameters.Add("@DG", OleDbType.VarChar).Value = dienGiai;
                        cmd.Parameters.Add("@TKNo", OleDbType.VarChar).Value = tkNo;
                        cmd.Parameters.Add("@TKCo", OleDbType.VarChar).Value = tkCo;
                        cmd.Parameters.Add("@SoPTT", OleDbType.VarChar).Value = soPTT;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (dgvDanhSach.SelectedRows.Count == 0)
            {
                MessageBox.Show("Không thể xóa phiếu thanh toán này!", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvDanhSach.SelectedRows[0];
            string soPTT = selectedRow.Cells["MaYC"].Value.ToString();

            DialogResult result = MessageBox.Show(
                $"Bạn có chắc muốn xóa phiếu thanh toán '{soPTT}' không?",
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


                    string sqlMain = "DELETE FROM PhieuThanhToan WHERE SoPhieuThanhToan = ?";
                    using (OleDbCommand cmdMain = new OleDbCommand(sqlMain, conn))
                    {
                        cmdMain.Parameters.AddWithValue("@p1", soPTT);
                        cmdMain.ExecuteNonQuery();
                    }
                }

                dgvDanhSach.Rows.Remove(selectedRow);

                MessageBox.Show($"Đã xóa phiếu thanh toán '{soPTT}' thành công.", "Thông báo",
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
            FormTimKiemPhieuThanhToan form = new FormTimKiemPhieuThanhToan();

            if (form.ShowDialog() == DialogResult.OK)
            {
                DateTime? ngayTu = null;
                DateTime? ngayDen = null;

                if (form.LocTheoNgay)
                {
                    ngayTu = form.NgayTu;
                    ngayDen = form.NgayDen;
                }

                string maCT = form.MaChungTu;   // 4 ký tự đầu
                string soCT = form.SoCT;        // phần số CT (từ ký tự 5 trở đi)
                string donvi = form.NhaCC;      // Tên đơn vị

                LocDanhSachPhieuThanhToan(ngayTu, ngayDen, maCT, soCT, donvi);
            }
        }
        private void LocDanhSachPhieuThanhToan(DateTime? ngayTu, DateTime? ngayDen,
                                       string maCT, string soCT, string donvi)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"
                SELECT 
                    ptt.SoPhieuThanhToan,
                    ptt.NgayChungTu,
                    ncc.TenNCC
                FROM PhieuThanhToan ptt
                LEFT JOIN NhaCungCap ncc ON ptt.MaDonVi = ncc.MaNCC
                WHERE 1 = 1
            ";

                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = conn;

                    // ===== Lọc theo ngày =====
                    if (ngayTu.HasValue)
                    {
                        query += " AND DateValue(ptt.NgayChungTu) >= DateValue(@Tu)";
                        cmd.Parameters.AddWithValue("@Tu", ngayTu.Value);
                    }

                    if (ngayDen.HasValue)
                    {
                        query += " AND DateValue(ptt.NgayChungTu) <= DateValue(@Den)";
                        cmd.Parameters.AddWithValue("@Den", ngayDen.Value);
                    }

                    if (!string.IsNullOrWhiteSpace(maCT))
                    {
                        query += " AND ptt.SoPhieuThanhToan LIKE @MaCTLike";
                        cmd.Parameters.AddWithValue("@MaCTLike", maCT + "%");
                    }

                    if (!string.IsNullOrWhiteSpace(soCT))
                    {
                        query += " AND MID(ptt.SoPhieuThanhToan, 4) LIKE @SoCT";
                        cmd.Parameters.AddWithValue("@SoCT", "%" + soCT + "%");
                    }

                    // ===== Tên đơn vị =====
                    if (!string.IsNullOrWhiteSpace(donvi))
                    {
                        query += " AND ncc.TenNCC LIKE @Donvi";
                        cmd.Parameters.AddWithValue("@Donvi", "%" + donvi + "%");
                    }

                    cmd.CommandText = query;

                    // Lấy dữ liệu
                    DataTable dt = new DataTable();
                    using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    dgvDanhSach.Rows.Clear();

                    foreach (DataRow row in dt.Rows)
                    {
                        dgvDanhSach.Rows.Add(
                            row["SoPhieuThanhToan"].ToString(),
                            Convert.ToDateTime(row["NgayChungTu"]).ToString("dd/MM/yyyy"),
                            row["TenNCC"].ToString()
                        );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lọc phiếu thanh toán: " + ex.Message);
                }
            }
        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            PhieuThanhToan ds = new PhieuThanhToan();
            ds.ShowDialog();

        }
    }
}
