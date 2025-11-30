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
    public partial class DuyetPhieuDeNghiThanhToan : Form
    {
        TextBox txtMaCT;
        TextBox txtSoCT;
        TextBox txtLoaiTien;
        TextBox txtMauHoaDon;
        TextBox txtSoSeri;
        TextBox txtSoHoaDon;
        TextBox txtCTThamChieu;
        TextBox txtMaNCC;
        TextBox txtTenNCC, txtSoTK;
        TextBox txtDiaChi;
        TextBox txtMaSoThue, txtNguoiDeNghi, txtMaHoaDon, txtMaDonHang;
        TextBox txtPhuongThucTT;
        TextBox txtDienThoaiNCC, txtNguoiLienHe, txtMaLienHe, txtDienThoaiLienHe, txtSoNgayNo, txtNoiDung, txtPhongban, txtDonVi, txtNguoidangky;
        ComboBox cboLoaiTien;
        // ===== Khai báo DateTimePicker =====
        DateTimePicker dtNgayCT, dtNgayCanThanhToan;
        DateTimePicker dtNgayHoaDon, dtNgayDenHan;
        DataGridView dgvDanhSach, dgv;
        private string connectionString =
           DatabaseConfig.ConnectionString;
        public DuyetPhieuDeNghiThanhToan()
        {
            InitializeComponent();
            BuildUI();
            LoadDanhSachPhieuDeNghiThanhToan();
        }
        private void BuildUI()
        {
            this.Text = "Danh sách phiếu đề nghị thanh toán";
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
                    BackColor = text.Contains("Lưu") ? Color.LightSkyBlue : Color.Firebrick,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                btn.FlatAppearance.BorderSize = 0;
                pnlRight.Controls.Add(btn);
            }

            // === Panel chứa nội dung chính ===
            // Panel chính (chứa vùng cuộn)
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
                Text = "DANH SÁCH PHIẾU ĐỀ NGHỊ THANH TOÁN",
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
            dgvDanhSach.Columns.Add("SoTK", "Mã chứng từ");
            dgvDanhSach.Columns.Add("TenNCC", "Ngày chứng từ");
            dgvDanhSach.Columns.Add("ChiNhanh", "Hình thức thanh toán");
            dgvDanhSach.Columns.Add("Tinh", "Ngày cần thanh toán");
            dgvDanhSach.Columns.Add("TrangThai", "Trạng thái");

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
                Text = "Chi tiết phiếu đề nghị thanh toán",
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
            // --- Nhóm NGƯỜI ĐĂNG KÝ ---
            GroupBox grpNguoiDK = new GroupBox
            {
                Text = "NGƯỜI ĐĂNG KÝ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 66, 168),
                //Location = new Point(10, y),
                Margin = new Padding(10),
                Width = pnlMain.Width - 40,
                Height = 120,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlMain.Controls.Add(grpNguoiDK);

            AddTextBoxRow(grpNguoiDK, new[] { "Người đăng ký", "Đơn vị", "Phòng ban", "Bộ phận", "Chức danh" }, 10, 30);

            y += grpNguoiDK.Height + 10;

            // --- Nhóm THÔNG TIN ---
            GroupBox grpThongTin = new GroupBox
            {
                Text = "THÔNG TIN",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 66, 168),
                //Location = new Point(10, y),
                Margin = new Padding(10),
                Width = pnlMain.Width - 40,
                Height = 250,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlMain.Controls.Add(grpThongTin);


            TaoFormThongTin(grpThongTin);

            y += grpThongTin.Height + 10;
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

                        // Riêng combobox "Mã chứng từ" vẫn disable
                        DisableMaChungTu(pnlMain);
                    };
                }
            }
            // --- Nhóm CHI TIẾT MẶT HÀNG ---
            GroupBox grpChiTiet = new GroupBox
            {
                Text = "CHI TIẾT THANH TOÁN",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 66, 168),
                //Location = new Point(10, y),
                Margin = new Padding(10),
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
                Location = new Point(grpChiTiet.Width - 1000, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            grpChiTiet.Controls.Add(grpTongHop);

            // === 4 ô tổng hợp nằm thành 1 hàng ngang ===
            

            dgv = new DataGridView
            {


                Location = new Point(10, grpTongHop.Bottom + 10), // đặt phía dưới nhóm tổng hợp
                Width = grpChiTiet.Width - 20,
                Height = grpChiTiet.Height - grpTongHop.Bottom - 105,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackgroundColor = Color.White,
                AllowUserToAddRows = true,
                ColumnHeadersHeight = 35,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                EnableHeadersVisualStyles = false
            };
            grpChiTiet.Controls.Add(dgv);

            // --- Thêm 2 nút xử lý ---
            Button btnPheDuyet = new Button
            {
                Text = "✅ Phê duyệt",
                Width = 180,
                Height = 45,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.LightGreen,
                Location = new Point(1500, dgv.Bottom + 10)
            };
            Button btnTuChoi = new Button
            {
                Text = "❌ Từ chối",
                Width = 180,
                Height = 45,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.LightCoral,
                Location = new Point(btnPheDuyet.Right + 10, dgv.Bottom + 10)
            };

            grpChiTiet.Controls.Add(btnPheDuyet);
            grpChiTiet.Controls.Add(btnTuChoi);

            btnPheDuyet.Click += (s, e) =>
            {
                UpdateTrangThaiDeNghi(fullCT, "Đã duyệt");
                MessageBox.Show("Đã phê duyệt phiếu đề nghị thanh toán", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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


                        UpdateTrangThaiDeNghi(fullCT, "Từ chối");


                        MessageBox.Show("Phiếu đề nghị thanh toán đã bị từ chối.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    LoadDanhSachPhieuDeNghiThanhToan();

                }
            };

            // Tùy chỉnh header
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 235, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);

            dgv.Columns.Add("Dot", "Đợt");
            //dgv.Columns.Add("DVT", "Ngày hóa đơn");
            dgv.Columns.Add("SoTien", "Số tiền*");
            dgv.Columns.Add("HanThanhToan", "Hạn thanh toán");
            dgv.Columns.Add("GhiChu", "Ghi chú");



            // Căn giữa header và dữ liệu số
            dgv.Columns["SoTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["HanThanhToan"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;


        }
        private void UpdateTrangThaiDeNghi(string maYC, string trangThai)
        {
            string sql = "UPDATE PhieuDeNghiThanhToan SET TrangThai = ? WHERE SoPhieuDeNghi = ?";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            using (OleDbCommand cmd = new OleDbCommand(sql, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                cmd.Parameters.AddWithValue("@SoPhieuDeNghi", maYC);
                cmd.ExecuteNonQuery();
            }
        }
        private void DisableMaChungTu(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {

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
        private void ToggleInputs(Control parent, bool enable)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is TextBox || ctrl is ComboBox || ctrl is DateTimePicker)
                {
                    ctrl.Enabled = enable;
                }


                if (ctrl.HasChildren)
                {
                    ToggleInputs(ctrl, enable);
                }
            }
        }
        private void AddTextBoxRow(Control parent, string[] labels, int startX, int startY)
        {
            int x = startX;
            int labelWidth = 150;
            int textBoxWidth = 260;
            int controlHeight = 75;
            int spacingX = 55;
            int spacingY = 5;

            foreach (string label in labels)
            {
                // Label
                Label lbl = new Label
                {
                    Text = label + ":",
                    Location = new Point(x, startY),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 12, FontStyle.Bold),
                    ForeColor = Color.FromArgb(50, 66, 168)
                };
                parent.Controls.Add(lbl);


                Control inputControl;
                if (label.Contains("Ngày"))
                {
                    inputControl = new DateTimePicker
                    {
                        Location = new Point(x, startY + lbl.Height + spacingY),
                        Width = textBoxWidth,
                        Format = DateTimePickerFormat.Short
                    };
                }
                else
                {
                    inputControl = new TextBox
                    {
                        Location = new Point(x, startY + lbl.Height + spacingY),
                        Width = textBoxWidth,
                        Height = controlHeight
                    };
                }

                parent.Controls.Add(inputControl);
                if (label.Contains("Người đăng ký"))
                    txtNguoidangky = (TextBox)inputControl;
                else if (label.Contains("Đơn vị"))
                    txtDonVi = (TextBox)inputControl;
                else if (label.Contains("Phòng ban"))
                    txtPhongban = (TextBox)inputControl;
                // Di chuyển sang cột kế tiếp
                x += textBoxWidth + spacingX;
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
            int startY = 70;
            int labelWidth = 110;
            int textBoxWidth = 190;
            int controlHeight = 32;
            int spacingX = 45;
            int spacingY = 20;
            int rowSpacing = 25;

            // Nút chọn
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

            // Hàng 1: Mã CT, Ngày PO, Số PO, Loại tiền, Số hợp đồng, Ngày hợp đồng, Ngày đến hạn, Người lập
            string[] labels1 = { "Mã CT*", "Ngày CT*", "Số CT*", "Loại tiền*", "Hình thức thanh toán", "Ngày cần thanh toán", "Người đề nghị", "Chứng từ tham chiếu" };
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
                        Width = textBoxWidth,
                        Format = DateTimePickerFormat.Short
                    };
                }
                else if (label.Contains("Loại tiền") || label.Contains("Người lập"))
                {
                    cboLoaiTien = new ComboBox
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = textBoxWidth,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    LoadNgoaiTe((ComboBox)cboLoaiTien);
                    input = cboLoaiTien;
                }
                else
                {
                    input = new TextBox
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = textBoxWidth
                    };
                }

                ;
                groupBox.Controls.Add(input);
                if (label.Contains("Mã CT*"))
                    txtMaCT = (TextBox)input;
                else if (label.Contains("Ngày CT*"))
                    dtNgayCT = (DateTimePicker)input;
                else if (label.Contains("Số CT*"))
                    txtSoCT = (TextBox)input;
                else if (label.Contains("Loại tiền*"))
                    cboLoaiTien = (ComboBox)input;
                else if (label.Contains("Hình thức thanh toán"))
                    txtPhuongThucTT = (TextBox)input;
                else if (label.Contains("Ngày cần thanh toán"))
                    dtNgayCanThanhToan = (DateTimePicker)input;
                else if (label.Contains("Người đề nghị"))
                    txtNguoiDeNghi = (TextBox)input;
                else if (label.Contains("Chứng từ tham chiếu"))
                    txtCTThamChieu = (TextBox)input;
                x += textBoxWidth + spacingX;
            }

            // Hàng 2: Nhà cung cấp
            y += controlHeight + spacingY + rowSpacing;
            x = startX;

            string[] labels2 = { "Mã đơn vị", "Tên đơn vị", "Địa chỉ", "Điện thoại", "Mã liên hệ", "Người liên hệ", "ĐT liên hệ" };

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
                    Width = (label.Contains("Địa chỉ")) ? textBoxWidth * 2 + spacingX : textBoxWidth
                };
                groupBox.Controls.Add(txt);
                if (label.Contains("Mã đơn vị"))
                    txtMaNCC = (TextBox)txt;
                else if (label.Contains("Tên đơn vị"))
                    txtTenNCC = (TextBox)txt;
                else if (label.Contains("Địa chỉ"))
                    txtDiaChi = (TextBox)txt;
                else if (label.Contains("Điện thoại"))
                    txtDienThoaiNCC = (TextBox)txt;
                else if (label.Contains("Mã liên hệ"))
                    txtMaLienHe = (TextBox)txt;
                else if (label.Contains("Người liên hệ"))
                    txtNguoiLienHe = (TextBox)txt;

                else if (label.Contains("ĐT liên hệ"))
                    txtDienThoaiLienHe = (TextBox)txt;
                x += txt.Width + spacingX;
            }

            // Hàng 3: Địa điểm giao hàng
            y += controlHeight + spacingY + rowSpacing;
            x = startX;

            string[] labels3 = { "TK ngân hàng", "Nội dung" };

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
                    Width = label.Contains("ngân hàng") || label.Contains("Hợp đồng")
                                    ? textBoxWidth * 2 + spacingX
                                    : label.Contains("Nội dung")
                                        ? textBoxWidth * 4 + spacingX
                                        : textBoxWidth
                };
                groupBox.Controls.Add(txt);
                if (label.Contains("Nội dung"))
                    txtNoiDung = (TextBox)txt;
                else if (label.Contains("TK ngân hàng"))
                    txtSoTK = (TextBox)txt;
                x += txt.Width + spacingX;
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

        private void LoadDanhSachPhieuDeNghiThanhToan()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string sql = @"
            SELECT SoPhieuDeNghi,
                   NgayChungTu,
                    PhuongThucThanhToan,
                    NgayCanThanhToan,TrangThai
            FROM PhieuDeNghiThanhToan WHERE TrangThai = 'Chờ xử lý'
            ";

                using (OleDbDataAdapter da = new OleDbDataAdapter(sql, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvDanhSach.Rows.Clear();

                    foreach (DataRow r in dt.Rows)
                    {
                        dgvDanhSach.Rows.Add(
                            r["SoPhieuDeNghi"].ToString(),
                            Convert.ToDateTime(r["NgayChungTu"]).ToString("dd/MM/yyyy"),
                            r["PhuongThucThanhToan"].ToString(),
                            Convert.ToDateTime(r["NgayCanThanhToan"]).ToString("dd/MM/yyyy"),
                            r["TrangThai"].ToString()
                        );
                    }
                }
            }
        }

        private void DgvDanhSach_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDanhSach.CurrentRow == null) return;

            string soPhieu = dgvDanhSach.CurrentRow.Cells["SoTK"].Value?.ToString();
            if (string.IsNullOrEmpty(soPhieu)) return;

            LoadThongTinPhieuDeNghi(soPhieu);

            LoadChiTietThanhToan(soPhieu);

        }
        string fullCT = "";
        private void LoadThongTinPhieuDeNghi(string soPhieu)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string sql = @"SELECT * FROM PhieuDeNghiThanhToan 
                       WHERE SoPhieuDeNghi = @id";

                OleDbCommand cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", soPhieu);

                string maNCC = "";
                string maNLH = "";
                string maNVLap = "";
                string manguoidangky = "";
                using (OleDbDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        // --- Tách mã CT & số CT ---
                        fullCT = rd["SoPhieuDeNghi"].ToString();

                        txtMaCT.Text = fullCT.Substring(0, 3);
                        txtSoCT.Text = fullCT.Length > 3 ? fullCT.Substring(3) : "";

                        dtNgayCT.Value = Convert.ToDateTime(rd["NgayChungTu"]);
                        dtNgayCanThanhToan.Value = Convert.ToDateTime(rd["NgayCanThanhToan"]);

                        cboLoaiTien.Text = rd["MaNgoaiTe"].ToString();
                        txtNoiDung.Text = rd["NoiDung"].ToString();
                        txtPhuongThucTT.Text = rd["PhuongThucThanhToan"].ToString();

                        txtSoTK.Text = rd["SoTaiKhoanNganHangNCC"].ToString();
                        txtCTThamChieu.Text = rd["ChungTuThamChieu"].ToString();

                        maNCC = rd["MaDonVi"].ToString();
                        maNLH = rd["MaNLH"].ToString();
                        maNVLap = rd["MaNhanVienDN"].ToString();
                        manguoidangky = rd["MaNhanVienDK"].ToString();
                    }
                }



                if (!string.IsNullOrEmpty(maNCC))
                {
                    string sqlNCC = @"SELECT TenNCC, DiaChi, DienThoai 
                              FROM NhaCungCap WHERE MaNCC = @Ma";

                    OleDbCommand cmdNCC = new OleDbCommand(sqlNCC, conn);
                    cmdNCC.Parameters.AddWithValue("@Ma", maNCC);

                    using (OleDbDataReader rd = cmdNCC.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            txtMaNCC.Text = maNCC;
                            txtTenNCC.Text = rd["TenNCC"].ToString();
                            txtDiaChi.Text = rd["DiaChi"].ToString();
                            txtDienThoaiNCC.Text = rd["DienThoai"].ToString();
                        }
                    }
                }


                if (!string.IsNullOrEmpty(maNLH))
                {
                    string sqlNLH = @"SELECT TenNLH, DienThoai
                              FROM NguoiLienHe WHERE MaNLH = @Ma";

                    OleDbCommand cmdNLH = new OleDbCommand(sqlNLH, conn);
                    cmdNLH.Parameters.AddWithValue("@Ma", maNLH);

                    using (OleDbDataReader rd = cmdNLH.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            txtMaLienHe.Text = maNLH;
                            txtNguoiLienHe.Text = rd["TenNLH"].ToString();
                            txtDienThoaiLienHe.Text = rd["DienThoai"].ToString();
                        }
                    }
                }


                if (!string.IsNullOrEmpty(maNVLap))
                {
                    string sqlNV = @"SELECT *
                             FROM NhanVien WHERE MaNhanVien = @Ma";

                    OleDbCommand cmdNV = new OleDbCommand(sqlNV, conn);
                    cmdNV.Parameters.AddWithValue("@Ma", maNVLap);

                    using (OleDbDataReader rd = cmdNV.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            txtNguoiDeNghi.Text = rd["HoTen"].ToString();

                        }
                    }
                }
                if (!string.IsNullOrEmpty(manguoidangky))
                {
                    string sqlNV = @"SELECT *
                             FROM NhanVien WHERE MaNhanVien = @Ma";

                    OleDbCommand cmdNVDK = new OleDbCommand(sqlNV, conn);
                    cmdNVDK.Parameters.AddWithValue("@Ma", manguoidangky);

                    using (OleDbDataReader rd = cmdNVDK.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            txtNguoidangky.Text = rd["HoTen"].ToString();
                            txtDonVi.Text = rd["DonVi"].ToString();
                            txtPhongban.Text = rd["PhongBan"].ToString();

                        }
                    }
                }



            }
        }
        private void LoadChiTietThanhToan(string soPhieu)
        {
            dgv.Rows.Clear();   // Xóa dữ liệu cũ trước khi load mới

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string sql = @"
            SELECT Dot, SoTien, HanThanhToan
            FROM PhieuDeNghiThanhToan
            WHERE SoPhieuDeNghi = @id
            ORDER BY Dot";

                OleDbCommand cmd = new OleDbCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", soPhieu);

                using (OleDbDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        int dot = rd["Dot"] != DBNull.Value ? Convert.ToInt32(rd["Dot"]) : 0;

                        decimal soTien = 0;
                        if (rd["SoTien"] != DBNull.Value)
                            soTien = Convert.ToDecimal(rd["SoTien"]);

                        string soTienHienThi = soTien.ToString("N0");  // format 1,000,000

                        string hanTT = "";
                        if (rd["HanThanhToan"] != DBNull.Value)
                            hanTT = Convert.ToDateTime(rd["HanThanhToan"]).ToShortDateString();

                        dgv.Rows.Add(
                            dot,
                            soTienHienThi,
                            hanTT
                        );
                    }
                }
            }
        }
        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            FormPhieuDeNghiThanhToan ds = new FormPhieuDeNghiThanhToan();
            ds.ShowDialog();

        }
    }
}
