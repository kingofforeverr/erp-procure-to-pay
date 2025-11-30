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
    public partial class DanhSachHoaDonMuaHang : Form
    {
        private Label lblTongSL, lblTongTien, lblTonHienTai, lblDuKienNhap;

        TextBox txtMaCT;
        TextBox txtSoCT;
        TextBox txtLoaiTien;
        TextBox txtMauHoaDon;
        TextBox txtSoSeri;
        TextBox txtSoHoaDon;
        TextBox txtCTThamChieu;
        TextBox txtMaNCC;
        TextBox txtTenNCC;
        TextBox txtDiaChi;
        TextBox txtMaSoThue;
        TextBox txtDienThoaiNCC, txtNguoiLienHe, txtMaLienHe, txtDienThoaiLienHe, txtSoNgayNo, txtNoiDung;
        ComboBox cboLoaiTien, cboMaCT;
        private bool isDirty = false;
        // ===== Khai báo DateTimePicker =====
        DateTimePicker dtNgayCT;
        DateTimePicker dtNgayHoaDon, dtNgayDenHan;
        DataGridView dgv, dgvthue, dgvDanhSach;
        private string connectionString = DatabaseConfig.ConnectionString;
        public DanhSachHoaDonMuaHang()
        {
            InitializeComponent();
            BuildUI();
            AttachControlChangeEvents();
        }
        private void BuildUI()
        {
            this.Text = "Danh sách hóa đơn mua hàng";
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
                if (ctrl is Button btn)
                {
                    if (btn.Text.Contains("Xem"))
                    {
                        btn.Click += BtnXem_Click;
                    }
                    
                    else if (btn.Text.Contains("Tìm"))
                    {
                        btn.Click += BtnTimKiem_Click;
                    }
                    else if (btn.Text.Contains("Thêm"))
                        btn.Click += BtnThem_Click;
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
                {
                    btn.Click += BtnLuu_Click;
                }
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
                Text = "DANH SÁCH HÓA ĐƠN MUA HÀNG",
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
            dgvDanhSach.Columns.Add("MaYC", "Số chứng từ hóa đơn");
            dgvDanhSach.Columns.Add("NgayCT", "Ngày chứng từ");
            dgvDanhSach.Columns.Add("TenNCC", "Tên nhà cung cấp");
            dgvDanhSach.Columns.Add("SoNgayNo", "Số ngày nợ");
            dgvDanhSach.Columns.Add("NgayDenHan", "Ngày đến hạn");

            LoadDanhSach(dgvDanhSach);
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
                Text = "Chi tiết hóa đơn mua hàng",
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

            // --- Nhóm THÔNG TIN ---




            // --- Nhóm CHI TIẾT MẶT HÀNG ---
            GroupBox grpChiTiet = new GroupBox
            {
                Text = "CHI TIẾT MẶT HÀNG / DỊCH VỤ",
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
                Location = new Point(grpChiTiet.Width - 600, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            grpChiTiet.Controls.Add(grpTongHop);

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

            dgv = new DataGridView
            {


                Location = new Point(10, grpTongHop.Bottom + 10), // đặt phía dưới nhóm tổng hợp
                Width = grpChiTiet.Width - 20,
                Height = grpChiTiet.Height - grpTongHop.Bottom - 100,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackgroundColor = Color.White,
                AllowUserToAddRows = true,
                ColumnHeadersHeight = 35,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
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
            dgv.Columns.Add("MaHang", "Mã hàng");

            dgv.Columns.Add("TenHang", "Tên hàng");

            dgv.Columns.Add("DVT", "Đvt");

            dgv.Columns.Add("SL", "Số lượng");

            dgv.Columns.Add("DonGia", "Đơn giá");

            dgv.Columns.Add("TongTien", "Thành tiền");

            dgv.Columns.Add("TKCO", "TK Có");
            dgv.Columns["TKCO"].Width = 90;

            dgv.Columns.Add("TKNO", "TK Nợ");
            dgv.Columns["TKNO"].Width = 90;

            // Căn giữa header và dữ liệu số
            dgv.Columns["SL"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["DonGia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["TongTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["DVT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;


            dgvthue = new DataGridView
            {


                Location = new Point(10, dgv.Bottom + 10),
                Width = grpChiTiet.Width - 20,
                Height = grpChiTiet.Height - grpTongHop.Bottom - 205,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackgroundColor = Color.White,
                AllowUserToAddRows = true,
                ColumnHeadersHeight = 35,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                EnableHeadersVisualStyles = false
            };
            grpChiTiet.Controls.Add(dgvthue);
            dgvthue.CellEndEdit += DgvThue_CellEndEdit;

            dgvthue.Columns.Add("MaHang", "Mã hàng");
            dgvthue.Columns.Add("TenHang", "Tên hàng");
            dgvthue.Columns.Add("Vat", "%Vat");
            dgvthue.Columns.Add("TongVat", "Tổng tiền thuế");
            dgvthue.Columns.Add("TKNO", "TK Nợ");
            dgvthue.Columns.Add("TKCO", "TK Có");

            dgv.DataError += dgv_DataError;
            dgvthue.DataError += dgv_DataError;
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
        }
        private void CapNhatTongHop()
        {
            decimal tongSL = 0;
            decimal tongTienHang = 0;
            decimal tongTienthue = 0;
            decimal tongtien = 0;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                decimal sl = 0, dongia = 0;

                decimal.TryParse(Convert.ToString(row.Cells["SL"].Value), out sl);
                decimal.TryParse(Convert.ToString(row.Cells["DonGia"].Value), out dongia);
                //decimal.TryParse(Convert.ToString(row.Cells["TienVat"].Value), out tienvat);

                tongSL += sl;
                tongTienHang += (sl * dongia);
            }
            foreach (DataGridViewRow row in dgvthue.Rows)
            {
                if (row.IsNewRow) continue;

                decimal tongvat = 0;

                decimal.TryParse(Convert.ToString(row.Cells["TongVat"].Value), out tongvat);

                tongTienthue += tongvat;
            }
            tongtien = tongTienHang + tongTienthue;
            // Cập nhật lên giao diện
            lblTongSL.Text = tongSL.ToString("N0");
            lblTongTien.Text = tongtien.ToString("N0");
        }
        private void DgvThue_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow rowThue = dgvthue.Rows[e.RowIndex];
            string colName = dgvthue.Columns[e.ColumnIndex].Name;

            if (colName != "Vat") return;

            // Lấy Mã hàng ở grid thuế
            string maHH = Convert.ToString(rowThue.Cells["MaHang"].Value);

            if (string.IsNullOrEmpty(maHH)) return;

            // Tìm hàng tương ứng trong grid hàng hóa
            decimal thanhTien = 0;

            foreach (DataGridViewRow rowHang in dgv.Rows)
            {
                if (Convert.ToString(rowHang.Cells["MaHang"].Value) == maHH)
                {
                    decimal.TryParse(Convert.ToString(rowHang.Cells["TongTien"].Value),
                                     out thanhTien);
                    break;
                }
            }

            // Lấy VAT
            decimal vat = 0;
            decimal.TryParse(Convert.ToString(rowThue.Cells["Vat"].Value), out vat);

            // Tính tiền VAT
            decimal tienVat = (vat / 100) * thanhTien;

            // Cập nhật lại grid thuế
            rowThue.Cells["TongVat"].Value = tienVat.ToString("N0");
            if (vat > 0)
            {
                rowThue.Cells["TKNo"].Value = "1331";
                rowThue.Cells["TKCo"].Value = "331";
            }
            else
            {
                // Nếu VAT = 0 thì xóa TK
                rowThue.Cells["TKNo"].Value = "";
                rowThue.Cells["TKCo"].Value = "";
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
            int spacingX = 20;
            int spacingY = 20;
            int rowSpacing = 25;

            // Nút chọn
            Button btnChon = new Button
            {
                Text = "Chọn phiếu nhập kho",
                Location = new Point(startX, 30),
                Size = new Size(200, controlHeight),
                BackColor = Color.MediumSeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            groupBox.Controls.Add(btnChon);

            // Hàng 1: Mã CT, Ngày PO, Số PO, Loại tiền, Số hợp đồng, Ngày hợp đồng, Ngày đến hạn, Người lập
            string[] labels1 = { "Số CT*", "Ngày CT*", "Loại tiền*", "Mẫu hóa đơn", "Số seri", "Số hóa đơn", "Ngày hóa đơn", "CT tham chiếu" };
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
                if (label.Contains("Ngày CT*"))
                    dtNgayCT = (DateTimePicker)input;
                else if(label.Contains("Mã CT"))
                    txtMaCT = (TextBox)input;
                else if (label.Contains("Số CT*"))
                    txtMaCT = (TextBox)input;

                else if (label.Contains("Loại tiền*"))
                    cboLoaiTien = (ComboBox)input;

                else if (label.Contains("Mẫu hóa đơn"))
                    txtMauHoaDon = (TextBox)input;

                else if (label.Contains("Số seri"))
                    txtSoSeri = (TextBox)input;

                else if (label.Contains("Số hóa đơn"))
                    txtSoHoaDon = (TextBox)input;

                else if (label.Contains("Ngày hóa đơn"))
                    dtNgayHoaDon = (DateTimePicker)input;

                else if (label.Contains("CT tham chiếu"))
                    txtCTThamChieu = (TextBox)input;
                x += input.Width + spacingX;
            }

            // Hàng 2: Nhà cung cấp
            y += controlHeight + spacingY + rowSpacing;
            x = startX;

            string[] labels2 = { "Mã NCC", "Tên nhà cung cấp", "Địa chỉ", "Mã số thuế", "Điện thoại" };

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
                    Width = (label.Contains("nhà cung cấp") || label.Contains("Địa chỉ") ? textBoxWidth * 3 + spacingX * 2
                                    : textBoxWidth)
                };
                groupBox.Controls.Add(txt);
                if (label.Contains("Mã NCC"))
                    txtMaNCC = txt;
                else if (label.Contains("Tên nhà cung cấp"))
                    txtTenNCC = txt;
                else if (label.Contains("Địa chỉ"))
                    txtDiaChi = txt;
                else if (label.Contains("Mã số thuế"))
                    txtMaSoThue = txt;
                else if (label.Contains("Điện thoại"))
                    txtDienThoaiNCC = txt;
                x += txt.Width + spacingX;
            }

            // Hàng 3: Địa điểm giao hàng
            y += controlHeight + spacingY + rowSpacing;
            x = startX;

            string[] labels3 = { "Mã liên hệ", "Người liên hệ", "Điện thoại liên hệ", "Số ngày nợ", "Ngày đến hạn" };

            foreach (string label in labels3)
            {
                Label lbl = new Label
                {
                    Text = label,
                    Location = new Point(x, y),
                    AutoSize = true
                };
                groupBox.Controls.Add(lbl);

                Control txt;
                if (label.Contains("Ngày"))
                {
                    txt = new DateTimePicker
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = textBoxWidth * 2 + spacingX,
                        Format = DateTimePickerFormat.Short
                    };
                }
                else
                    txt = new TextBox
                {
                    Location = new Point(x, y + lbl.Height + 2),
                    Width = label.Contains("Mã liên hệ") ? textBoxWidth : textBoxWidth * 2 + spacingX
                };
                groupBox.Controls.Add(txt);
                if (label.Contains("Mã liên hệ"))
                    txtMaLienHe = (TextBox)txt;
                else if (label.Contains("Người liên hệ"))
                    txtNguoiLienHe = (TextBox)txt;
                else if (label.Contains("Điện thoại liên hệ"))
                    txtDienThoaiLienHe = (TextBox)txt;
                else if (label.Contains("Số ngày nợ"))
                    txtSoNgayNo = (TextBox)txt;
                else if (label.Contains("Ngày đến hạn"))
                    dtNgayDenHan = (DateTimePicker)txt;
                x += txt.Width + spacingX;
            }


            // Hàng cuối – Nội dung
            y += controlHeight + spacingY + rowSpacing;
            Label lblNoiDung = new Label
            {
                Text = "Nội dung",
                Location = new Point(startX, y),
                AutoSize = true
            };
            groupBox.Controls.Add(lblNoiDung);

            txtNoiDung = new TextBox
            {
                Location = new Point(startX, y + lblNoiDung.Height + 2),
                Width = groupBox.Width - 60,
                Height = 60,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            groupBox.Controls.Add(txtNoiDung);
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
        private void LoadDanhSach(DataGridView dgv)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"SELECT p.SoChungTuHoaDon, p.NgayChungTu, p.MaNCC, n.TenNCC, p.NgayDenHan, p.SoNgayNo
                                    FROM HoaDonMuaHang AS p
                                    LEFT JOIN NhaCungCap AS n
                                        ON p.MaNCC = n.MaNCC
                                    ORDER BY p.SoChungTuHoaDon DESC";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgv.Rows.Clear();

                        foreach (DataRow row in dt.Rows)
                        {
                            dgv.Rows.Add(
                                row["SoChungTuHoaDon"].ToString(),
                                Convert.ToDateTime(row["NgayChungTu"]).ToString("dd/MM/yyyy"),
                                row["TenNCC"].ToString(),
                                row["SoNgayNo"].ToString(),
                                Convert.ToDateTime(row["NgayDenHan"]).ToString("dd/MM/yyyy")
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải danh sách phiếu xuất trả: " + ex.Message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void DgvDanhSach_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDanhSach.CurrentRow == null) return;

            var cellValue = dgvDanhSach.CurrentRow.Cells["MaYC"].Value;
            if (cellValue == null) return;

            string maHD = cellValue.ToString();

            LoadThongTinHoaDon(maHD);

            LoadChiTietHoaDonMua(maHD);
            LoadChiTietThueHoaDonMua(maHD);
            CapNhatTongHop();

        }
        private void LoadThongTinHoaDon(string maHD)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string sqlHD = @"SELECT * FROM HoaDonMuaHang WHERE SoChungTuHoaDon = @SoChungTuHoaDon";
                OleDbCommand cmdHD = new OleDbCommand(sqlHD, conn);
                cmdHD.Parameters.AddWithValue("@SoChungTuHoaDon", maHD);

                string maNCC = "";
                string maNLH = "";
                using (OleDbDataReader rd = cmdHD.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        txtMaCT.Text = rd["SoChungTuHoaDon"].ToString();

                        if (rd["NgayChungTu"] != DBNull.Value)
                            dtNgayCT.Value = Convert.ToDateTime(rd["NgayChungTu"]);

                        txtNoiDung.Text = rd["NoiDung"].ToString();
                        cboLoaiTien.Text = rd["MaNgoaiTe"].ToString();
                        txtSoHoaDon.Text = rd["SoHoaDon"].ToString();
                        txtSoSeri.Text = rd["SoSeri"].ToString();
                        txtMauHoaDon.Text = rd["MauHoaDon"].ToString();
                        txtSoNgayNo.Text = rd["SoNgayNo"].ToString();
                        txtCTThamChieu.Text = rd["ChungTuThamChieu"].ToString();
                        dtNgayDenHan.Value = Convert.ToDateTime(rd["NgayDenHan"]);
                        maNCC = rd["MaNCC"].ToString();


                        maNLH = rd["MaNLH"].ToString();
                    }
                }

                // 2) Lấy thông tin nhà cung cấp
                if (!string.IsNullOrEmpty(maNCC))
                {
                    string sqlNCC = @"SELECT TenNCC, DiaChi, MaSoThue ,DienThoai
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
                            txtDienThoaiNCC.Text = rdNCC["DienThoai"].ToString();
                        }
                    }
                }

                //người liên hệ
                if (!string.IsNullOrEmpty(maNLH))
                {
                    string sqlNLH = @"SELECT TenNLH, DiaChi,DienThoai
                              FROM NguoiLienHe 
                              WHERE MaNLH = @MaNLH";

                    OleDbCommand cmdNLH = new OleDbCommand(sqlNLH, conn);
                    cmdNLH.Parameters.AddWithValue("@MaNLH", maNLH);

                    using (OleDbDataReader rdNLH = cmdNLH.ExecuteReader())
                    {
                        if (rdNLH.Read())
                        {
                            txtMaLienHe.Text = maNLH;
                            txtNguoiLienHe.Text = rdNLH["TenNLH"].ToString();
                            txtDienThoaiLienHe.Text = rdNLH["DienThoai"].ToString();
                        }
                    }
                }


            }
        }
        private void LoadChiTietHoaDonMua(string soChungTu)
        {
            string sql = @"
                    SELECT 
                        ct.MaHH,
                        hh.TenHH,
                        hh.DonViTinh,
                        ct.SoLuong,
                        ct.DonGia,
                        (ct.SoLuong * ct.DonGia) AS ThanhTien,
                        ct.TKNo,
                        ct.TKCo
                    FROM ChiTietHoaDonMua ct
                    LEFT JOIN HangHoa hh ON ct.MaHH = hh.MaHH
                    WHERE ct.SoChungTuHoaDon = ?
                ";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            using (OleDbDataAdapter da = new OleDbDataAdapter(sql, conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@SoChungTuHoaDon", soChungTu);

                DataTable dt = new DataTable();
                da.Fill(dt);

             
                dgv.Columns["MaHang"].DataPropertyName = "MaHH";
                dgv.Columns["TenHang"].DataPropertyName = "TenHH";
                dgv.Columns["DVT"].DataPropertyName = "DonViTinh";
                dgv.Columns["SL"].DataPropertyName = "SoLuong";
                dgv.Columns["DonGia"].DataPropertyName = "DonGia";
                dgv.Columns["TongTien"].DataPropertyName = "ThanhTien";

                dgv.Columns["TKNO"].DataPropertyName = "TKNo";
                dgv.Columns["TKCO"].DataPropertyName = "TKCo";

                dgv.DataSource = dt;
            }
        }
        private void LoadChiTietThueHoaDonMua(string soChungTu)
        {
            string sql = @"
        SELECT 
            ChiTietThueHoaDonMua.PhanTramVAT,
            ChiTietThueHoaDonMua.GiaTriThue,
            ChiTietThueHoaDonMua.TKNo,
            ChiTietThueHoaDonMua.TKCo,
            ChiTietHoaDonMua.MaHH,
            HangHoa.TenHH
        FROM 
            (ChiTietThueHoaDonMua 
            INNER JOIN ChiTietHoaDonMua 
                ON ChiTietThueHoaDonMua.MaCTHD = ChiTietHoaDonMua.MaCTHD)
            LEFT JOIN HangHoa 
                ON ChiTietHoaDonMua.MaHH = HangHoa.MaHH
        WHERE 
            ChiTietHoaDonMua.SoChungTuHoaDon = ?
    ";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            using (OleDbDataAdapter da = new OleDbDataAdapter(sql, conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@SoChungTuHoaDon", soChungTu);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvthue.Columns["MaHang"].DataPropertyName = "MaHH";
                dgvthue.Columns["TenHang"].DataPropertyName = "TenHH";
                dgvthue.Columns["Vat"].DataPropertyName = "PhanTramVAT";
                dgvthue.Columns["TongVat"].DataPropertyName = "GiaTriThue";
                dgvthue.Columns["TKNO"].DataPropertyName = "TKNo";
                dgvthue.Columns["TKCO"].DataPropertyName = "TKCo";

                dgvthue.DataSource = dt;
            }
        }


        private void AttachControlChangeEvents()
        {
            AttachEventsRecursive(this);
            AttachDataGridEvents(dgv);
            AttachDataGridEvents(dgvthue);
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

            string soChungTu = txtMaCT.Text.Trim();
            DateTime ngayCT = dtNgayCT.Value;
            DateTime ngayHD = dtNgayHoaDon.Value;
            DateTime ngayDenHan = dtNgayDenHan.Value;

            string soSeri = txtSoSeri.Text.Trim();
            string soHoaDon = txtSoHoaDon.Text.Trim();
            string mauHoaDon = txtMauHoaDon.Text.Trim();
            string loaiTien = cboLoaiTien.Text.Trim();

            string noiDung = txtNoiDung.Text.Trim();
            string ctThamChieu = txtCTThamChieu.Text.Trim();
            string soNgayNo = txtSoNgayNo.Text.Trim();

            string maNCC = txtMaNCC.Text.Trim();
            string maNLH = txtMaLienHe.Text.Trim();

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string sqlUpdate = @"
            UPDATE HoaDonMuaHang
            SET NgayChungTu = ?,
                NgayHoaDon = ?,
                NgayDenHan = ?,
                SoHoaDon = ?,
                SoSeri = ?,
                MauHoaDon = ?,
                SoNgayNo = ?,
                MaNgoaiTe = ?,
                NoiDung = ?,
                ChungTuThamChieu = ?,
                MaNCC = ?,
                MaNLH = ?
            WHERE SoChungTuHoaDon = ?
        ";

                using (OleDbCommand cmd = new OleDbCommand(sqlUpdate, conn))
                {
                    cmd.Parameters.Add("@NgayChungTu", OleDbType.Date).Value = ngayCT;
                    cmd.Parameters.Add("@NgayHoaDon", OleDbType.Date).Value = ngayHD;
                    cmd.Parameters.Add("@NgayDenHan", OleDbType.Date).Value = ngayDenHan;

                    cmd.Parameters.Add("@SoHoaDon", OleDbType.VarChar).Value = soHoaDon;
                    cmd.Parameters.Add("@SoSeri", OleDbType.VarChar).Value = soSeri;
                    cmd.Parameters.Add("@MauHoaDon", OleDbType.VarChar).Value = mauHoaDon;
                    cmd.Parameters.Add("@SoNgayNo", OleDbType.Integer).Value = string.IsNullOrEmpty(soNgayNo) ? 0 : Convert.ToInt32(soNgayNo);

                    cmd.Parameters.Add("@MaNgoaiTe", OleDbType.VarChar).Value = loaiTien;
                    cmd.Parameters.Add("@NoiDung", OleDbType.VarChar).Value = noiDung;
                    cmd.Parameters.Add("@ChungTuThamChieu", OleDbType.VarChar).Value = ctThamChieu;

                    cmd.Parameters.Add("@MaNCC", OleDbType.VarChar).Value = maNCC;
                    cmd.Parameters.Add("@MaNLH", OleDbType.VarChar).Value = maNLH;

                    cmd.Parameters.Add("@ID", OleDbType.VarChar).Value = soChungTu;

                    cmd.ExecuteNonQuery();
                }

                // ❗ Nếu có bảng chi tiết, gọi hàm UpdateChiTiet(conn, soChungTu);
                // UpdateChiTiet(conn, soChungTu);
                UpdateChiTietHoaDonMua(conn, soChungTu);
                UpdateChiTietThueHoaDonMua(conn, soChungTu);
            }

            isDirty = false;
            MessageBox.Show("Đã lưu thay đổi vào hóa đơn mua hàng.");
        }
        private void dgv_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;   // ngăn dialog lỗi
            e.Cancel = true;            // không cho grid tự parse
        }
        private void UpdateChiTietHoaDonMua(OleDbConnection conn, string soChungTu)
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                string maHH = Convert.ToString(row.Cells["MaHang"].Value);
                if (string.IsNullOrEmpty(maHH)) continue;

                decimal soLuong = Convert.ToDecimal(row.Cells["SL"].Value ?? 0);
                decimal donGia = Convert.ToDecimal(row.Cells["DonGia"].Value ?? 0);

                string tkNo = Convert.ToString(row.Cells["TKNO"].Value ?? "");
                string tkCo = Convert.ToString(row.Cells["TKCO"].Value ?? "");

                // 1. Kiểm tra tồn tại
                string checkSQL =
                    "SELECT COUNT(*) FROM ChiTietHoaDonMua WHERE SoChungTuHoaDon = ? AND MaHH = ?";

                bool exists;
                using (OleDbCommand cmdCheck = new OleDbCommand(checkSQL, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@p1", soChungTu);
                    cmdCheck.Parameters.AddWithValue("@p2", maHH);

                    exists = (int)cmdCheck.ExecuteScalar() > 0;
                }

                if (exists)
                {
                    // 2. UPDATE
                    string updateSQL = @"
                UPDATE ChiTietHoaDonMua
                SET SoLuong = ?, DonGia = ?, TKNo = ?, TKCo = ?
                WHERE SoChungTuHoaDon = ? AND MaHH = ?
            ";

                    using (OleDbCommand cmd = new OleDbCommand(updateSQL, conn))
                    {
                        cmd.Parameters.AddWithValue("@SoLuong", soLuong);
                        cmd.Parameters.AddWithValue("@DonGia", donGia);
                        cmd.Parameters.AddWithValue("@TKNo", tkNo);
                        cmd.Parameters.AddWithValue("@TKCo", tkCo);
                        cmd.Parameters.AddWithValue("@SoCT", soChungTu);
                        cmd.Parameters.AddWithValue("@MaHH", maHH);

                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // 3. INSERT
                    string insertSQL = @"
                INSERT INTO ChiTietHoaDonMua
                (SoLuong, DonGia, PhanTramVAT, TKNo, TKCo, MaHH, SoChungTuHoaDon)
                VALUES (?, ?, ?, ?, ?, ?, ?)
            ";

                    using (OleDbCommand cmd = new OleDbCommand(insertSQL, conn))
                    {
                        cmd.Parameters.AddWithValue("@SoLuong", soLuong);
                        cmd.Parameters.AddWithValue("@DonGia", donGia);
                        cmd.Parameters.AddWithValue("@TKNo", tkNo);
                        cmd.Parameters.AddWithValue("@TKCo", tkCo);
                        cmd.Parameters.AddWithValue("@MaHH", maHH);
                        cmd.Parameters.AddWithValue("@SoCT", soChungTu);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        private void UpdateChiTietThueHoaDonMua(OleDbConnection conn, string soChungTu)
        {
            foreach (DataGridViewRow row in dgvthue.Rows)
            {
                if (row.IsNewRow) continue;

                string maHH = Convert.ToString(row.Cells["MaHang"].Value);
                if (string.IsNullOrEmpty(maHH)) continue;

                double thueSuat = Convert.ToDouble(row.Cells["Vat"].Value);
                double tienThue = Convert.ToDouble(row.Cells["TongVat"].Value);

                string tkNo = Convert.ToString(row.Cells["TKNO"].Value ?? "");
                string tkCo = Convert.ToString(row.Cells["TKCO"].Value ?? "");

                // 1. Lấy MaCTHD từ ChiTietHoaDonMua
                int maCTHD = -1;
                string sqlGet = "SELECT MaCTHD FROM ChiTietHoaDonMua WHERE SoChungTuHoaDon = ? AND MaHH = ?";

                using (OleDbCommand cmdGet = new OleDbCommand(sqlGet, conn))
                {
                    cmdGet.Parameters.AddWithValue("@sct", soChungTu);
                    cmdGet.Parameters.AddWithValue("@hh", maHH);

                    var result = cmdGet.ExecuteScalar();
                    if (result == null) continue;

                    maCTHD = Convert.ToInt32(result);
                }

                // 2. Kiểm tra tồn tại
                string checkSQL =
                    "SELECT COUNT(*) FROM ChiTietThueHoaDonMua WHERE MaCTHD = ?";

                bool exists;
                using (OleDbCommand cmdCheck = new OleDbCommand(checkSQL, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@p1", maCTHD);
                    exists = (int)cmdCheck.ExecuteScalar() > 0;
                }

                if (exists)
                {
                    // 3. UPDATE
                    string updateSQL = @"
                UPDATE ChiTietThueHoaDonMua
                SET PhanTramVAT = ?, GiaTriThue = ?, TKNo = ?, TKCo = ?
                WHERE MaCTHD = ?
            ";

                    using (OleDbCommand cmd = new OleDbCommand(updateSQL, conn))
                    {
                        cmd.Parameters.Add("@ThueSuat", OleDbType.Double).Value = thueSuat;
                        cmd.Parameters.Add("@TienThue", OleDbType.Currency).Value = tienThue;
                        cmd.Parameters.Add("@TKNo", OleDbType.VarChar).Value = tkNo;
                        cmd.Parameters.Add("@TKCo", OleDbType.VarChar).Value = tkCo;
                        cmd.Parameters.Add("@MaCTHD", OleDbType.Integer).Value = maCTHD;

                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    // 4. INSERT
                    string insertSQL = @"
                INSERT INTO ChiTietThueHoaDonMua
                (MaCTHD, PhanTramVAT, GiaTriThue, TKNo, TKCo)
                VALUES (?, ?, ?, ?, ?)
            ";

                    using (OleDbCommand cmd = new OleDbCommand(insertSQL, conn))
                    {
                        cmd.Parameters.Add("@MaCTHD", OleDbType.Integer).Value = maCTHD;
                        cmd.Parameters.Add("@ThueSuat", OleDbType.Double).Value = thueSuat;
                        cmd.Parameters.Add("@TienThue", OleDbType.Currency).Value = tienThue;
                        cmd.Parameters.Add("@TKNo", OleDbType.VarChar).Value = tkNo;
                        cmd.Parameters.Add("@TKCo", OleDbType.VarChar).Value = tkCo;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (dgvDanhSach.SelectedRows.Count == 0)
            {
                MessageBox.Show("Không có hóa đơn nào được chọn để xóa!",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvDanhSach.SelectedRows[0];
            string soChungTu = Convert.ToString(selectedRow.Cells["MaYC"].Value);

            DialogResult confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa hóa đơn mua hàng '{soChungTu}' không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                using (OleDbConnection conn = new OleDbConnection(connectionString))
                {
                    conn.Open();

                    string sqlThue = @"
                DELETE FROM ChiTietThueHoaDonMua 
                WHERE MaCTHD IN (
                    SELECT MaCTHD 
                    FROM ChiTietHoaDonMua 
                    WHERE SoChungTuHoaDon = ?
                )";

                    using (OleDbCommand cmdThue = new OleDbCommand(sqlThue, conn))
                    {
                        cmdThue.Parameters.AddWithValue("@p1", soChungTu);
                        cmdThue.ExecuteNonQuery();
                    }

                    string sqlCT = "DELETE FROM ChiTietHoaDonMua WHERE SoChungTuHoaDon = ?";
                    using (OleDbCommand cmdCT = new OleDbCommand(sqlCT, conn))
                    {
                        cmdCT.Parameters.AddWithValue("@p1", soChungTu);
                        cmdCT.ExecuteNonQuery();
                    }

                    string sqlMain = "DELETE FROM HoaDonMuaHang WHERE SoChungTuHoaDon = ?";
                    using (OleDbCommand cmdMain = new OleDbCommand(sqlMain, conn))
                    {
                        cmdMain.Parameters.AddWithValue("@p1", soChungTu);
                        cmdMain.ExecuteNonQuery();
                    }
                }

                dgvDanhSach.Rows.Remove(selectedRow);

                MessageBox.Show($"Đã xóa hóa đơn '{soChungTu}' thành công.",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa hóa đơn: " + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void BtnXem_Click(object sender, EventArgs e)
        {
            DanhSachTaiKhoanNH ds = new DanhSachTaiKhoanNH();
            ds.ShowDialog();

        }

        private void BtnThem_Click(object sender, EventArgs e)
        {
            HoaDonMuaHang ds = new HoaDonMuaHang();
            ds.ShowDialog();

        }

        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            FormTimKiemHoaDonMuaHang form = new FormTimKiemHoaDonMuaHang();

            if (form.ShowDialog() == DialogResult.OK)
            {
                DateTime? ngayTu = null;
                DateTime? ngayDen = null;

                if (form.LocTheoNgay)
                {
                    ngayTu = form.NgayTu;
                    ngayDen = form.NgayDen;
                }

                string maCT = form.MaChungTu;  
                string soCT = form.SoCT;        
                string ncc = form.NhaCC;

                LocDanhSachHoaDon(ngayTu, ngayDen, maCT, soCT, ncc);
            }
        }


        private void LocDanhSachHoaDon(DateTime? ngayTu, DateTime? ngayDen,
                               string maCT, string soCT, string nhaCC)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"
                SELECT hd.SoChungTuHoaDon,
                       hd.NgayChungTu,
                       ncc.TenNCC,
                       hd.SoNgayNo,
                       hd.NgayDenHan
                FROM HoaDonMuaHang hd
                LEFT JOIN NhaCungCap ncc ON hd.MaNCC = ncc.MaNCC
                WHERE 1 = 1
            ";

                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = conn;

                    // ======= Lọc theo ngày =======
                    if (ngayTu.HasValue)
                    {
                        query += " AND DateValue(hd.NgayChungTu) >= DateValue(@Tu)";
                        cmd.Parameters.AddWithValue("@Tu", ngayTu.Value);
                    }

                    if (ngayDen.HasValue)
                    {
                        query += " AND DateValue(hd.NgayChungTu) <= DateValue(@Den)";
                        cmd.Parameters.AddWithValue("@Den", ngayDen.Value);
                    }

                    // ======= Mã chứng từ (4 ký tự đầu) =======
                    if (!string.IsNullOrWhiteSpace(maCT))
                    {
                        query += " AND LEFT(hd.SoChungTuHoaDon, 3) = @MaCT";
                        cmd.Parameters.AddWithValue("@MaCT", maCT);
                    }

                    // ======= Số chứng từ (từ ký tự thứ 5 trở đi) =======
                    if (!string.IsNullOrWhiteSpace(soCT))
                    {
                        query += " AND MID(hd.SoChungTuHoaDon, 5) LIKE @SoCT";
                        cmd.Parameters.AddWithValue("@SoCT", "%" + soCT + "%");
                    }

                    // ======= Nhà cung cấp =======
                    if (!string.IsNullOrWhiteSpace(nhaCC))
                    {
                        query += " AND ncc.TenNCC LIKE @NCC";
                        cmd.Parameters.AddWithValue("@NCC", "%" + nhaCC + "%");
                    }

                    cmd.CommandText = query;

                    // Fill DataTable
                    DataTable dt = new DataTable();
                    using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    // Đổ vào grid chính
                    dgvDanhSach.Rows.Clear();

                    foreach (DataRow row in dt.Rows)
                    {
                        dgvDanhSach.Rows.Add(
                            row["SoChungTuHoaDon"].ToString(),
                            Convert.ToDateTime(row["NgayChungTu"]).ToString("dd/MM/yyyy"),
                            row["TenNCC"].ToString(),
                            row["SoNgayNo"].ToString(),
                            Convert.ToDateTime(row["NgayDenHan"]).ToString("dd/MM/yyyy")
                        );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lọc hóa đơn mua hàng: " + ex.Message);
                }
            }
        }

    }
}
