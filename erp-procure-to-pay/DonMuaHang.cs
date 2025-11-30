using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using static System.Net.Mime.MediaTypeNames;

namespace TestAccess
{
    public partial class DonMuaHang : Form
    {

        private string connectionString = DatabaseConfig.ConnectionString;
        private Label lblTongSL, lblTongTien, lblTonHienTai, lblDuKienNhap;
        TextBox txtSoHopDong, txtMaNCC, txtTenNCC, txtDiaChi, txtSoChungTu, txtPhuongThucGiaoHang, txtSoNgayNo, txtTrangThai, txtDot, txtHinhThucThanhToan;
        TextBox txtMaLienHe, txtNguoiLienHe, txtDienThoaiLienHe, txtDienThoai, txtNoiDung;
        TextBox txtNguoiLap;
        TextBox txtDTGiao, txtDiaChiGiao, txtTenDiaDiem;
        ComboBox cboMaChungTu, cboLoaiTien, cboPhuongThucThanhToan;
        DateTimePicker dtNgayHopDong, dtNgayDenHan, dtNgayChungTu, dtThoiGianGiaoHang;
        DataGridView dgv;
        private DataGridView dgvDanhSach;

        public DonMuaHang()
        {
            InitializeComponent();
            BuildUI();
        }

        private void DonMuaHang_Load(object sender, EventArgs e)
        {

        }
        private void BuildUI()
        {
            this.Text = "Đơn mua hàng";
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
                if (text.Contains("In"))
                {
                    btn.Click += BtnIn_Click;
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
            dgv.Columns.Add("MaHang", "Mã hàng");
            dgv.Columns["MaHang"].Width = 120;

            dgv.Columns.Add("TenHang", "Tên hàng");
            dgv.Columns["TenHang"].Width = 300;

            dgv.Columns.Add("DVT", "Đvt");
            dgv.Columns["DVT"].Width = 100;

            dgv.Columns.Add("SL", "Số lượng");
            dgv.Columns["SL"].Width = 100;

            dgv.Columns.Add("DonGia", "Đơn giá");
            dgv.Columns["DonGia"].Width = 180;

            dgv.Columns.Add("TongTien", "Thành tiền");
            dgv.Columns["TongTien"].Width = 180;

            dgv.Columns.Add("DienGiai", "Diễn giải");
            dgv.Columns["DienGiai"].Width = 110;

            dgv.Columns.Add("Vat", "%VAT");
            dgv.Columns["Vat"].Width = 100;

            dgv.Columns.Add("TienVat", "Tiền VAT");
            dgv.Columns["TienVat"].Width = 180;

            dgv.Columns.Add("NgayGH", "Ngày giao hàng");
            dgv.Columns["NgayGH"].Width = 180;

            dgv.Columns.Add("ThangBH", "Số tháng bảo hành");
            dgv.Columns["ThangBH"].Width = 200;

            // Căn giữa header và dữ liệu số
            dgv.Columns["SL"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["DonGia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["TongTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["DVT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgv.Columns["TongTien"].DefaultCellStyle.Format = "N0";
            dgv.Columns["TienVat"].DefaultCellStyle.Format = "N0";
        }
        private void LoadMaChungTu(ComboBox input)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT MaCT, TenNghiepVu FROM NghiepVu WHERE LoaiNghiepVu='Đơn mua hàng'";
                    using (OleDbDataAdapter da = new OleDbDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        input.DataSource = dt;
                        input.DisplayMember = "TenNghiepVu";
                        input.ValueMember = "MaCT";
                        input.SelectedIndex = -1;

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
                }
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
                Text = "Lấy chứng từ tham chiếu",
                Location = new Point(startX, 30),
                Size = new Size(240, controlHeight),
                BackColor = Color.MediumSeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            groupBox.Controls.Add(btnChon);
            btnChon.Click += BtnLayChungTuThamChieu_Click;

            // Hàng 1: Mã CT, Ngày PO, Số PO, Loại tiền, Số hợp đồng, Ngày hợp đồng, Ngày đến hạn, Người lập
            string[] labels1 = { "Mã chứng từ*", "Ngày chứng từ*", "Số chứng từ*", "Loại tiền*", "Số hợp đồng", "Ngày hợp đồng", "Ngày đến hạn", "Người lập" };
            int x = startX ;
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
                else if (label.Contains("Loại tiền") )
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
                else if (label.Contains("Mã chứng từ"))
                {
                    cboMaChungTu = new ComboBox
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = textBoxWidth,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    LoadMaChungTu(cboMaChungTu);
                    input = cboMaChungTu;

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
                if (label.Contains("Số hợp đồng"))
                    txtSoHopDong = (TextBox)input;
                else if (label.Contains("Ngày hợp đồng"))
                    dtNgayHopDong = (DateTimePicker)input;
                else if (label.Contains("Ngày đến hạn"))
                    dtNgayDenHan = (DateTimePicker)input;
                else if (label.Contains("Ngày chứng từ"))
                    dtNgayChungTu = (DateTimePicker)input;
                else if (label.Contains("Số chứng từ"))
                    txtSoChungTu = (TextBox)input;
                else if (label.Contains("Mã NCC"))
                    txtMaNCC = (TextBox)input;
                else if (label.Contains("Tên nhà cung cấp"))
                    txtTenNCC = (TextBox)input;
                else if (label.Contains("Địa chỉ"))
                    txtDiaChi = (TextBox)input;
                else if (label.Contains("Mã liên hệ"))
                    txtMaLienHe = (TextBox)input;
                else if (label.Contains("Người liên hệ"))
                    txtNguoiLienHe = (TextBox)input;
                else if (label.Contains("ĐT liên hệ"))
                    txtDienThoaiLienHe = (TextBox)input;
                else if (label.Contains("Người lập"))
                    txtNguoiLap = (TextBox)input;
                x += textBoxWidth + spacingX;
            }

            // Hàng 2: Nhà cung cấp
            y += controlHeight + spacingY + rowSpacing;
            x = startX;

            string[] labels2 = { "Mã NCC", "Tên nhà cung cấp", "Địa chỉ", "Điện thoại", "Mã liên hệ", "Người liên hệ", "ĐT liên hệ" };

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
                    Width = (label.Contains("Địa chỉ")) ? textBoxWidth * 2 + spacingX : textBoxWidth
                };
                groupBox.Controls.Add(txt);
                if (label.Contains("Mã NCC"))
                    txtMaNCC = (TextBox)txt;
                else if (label.Contains("Tên nhà cung cấp"))
                    txtTenNCC = (TextBox)txt;
                else if (label.Contains("Địa chỉ"))
                    txtDiaChi = (TextBox)txt;
                else if (label.Contains("Mã liên hệ"))
                    txtMaLienHe = (TextBox)txt;
                else if (label.Contains("Người liên hệ"))
                    txtNguoiLienHe = (TextBox)txt;
                else if (label.Contains("Điện thoại") && !label.Contains("liên hệ"))
                    txtDienThoai = txt;
                else if (label.Contains("ĐT liên hệ"))
                    txtDienThoaiLienHe = (TextBox)txt;
                x +=  txt.Width + spacingX;
            }

            // Hàng 3: Địa điểm giao hàng
            y += controlHeight + spacingY + rowSpacing;
            x = startX;

            string[] labels3 = {"Tên địa điểm giao/nhận", "Địa chỉ giao/nhận", "Điện thoại giao" };

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
                    Width = (label.Contains("Địa chỉ") || label.Contains("địa điểm") ? textBoxWidth * 3 + spacingX * 2 : textBoxWidth * 2 + spacingX)
                };
                groupBox.Controls.Add(txt);
                if (label.Contains("Tên địa điểm"))
                    txtTenDiaDiem = (TextBox)txt;
                else if (label.Contains("Địa chỉ giao"))
                    txtDiaChiGiao = (TextBox)txt;
                else if (label.Contains("Điện thoại giao"))
                    txtDTGiao = (TextBox)txt;
                x +=  txt.Width + spacingX ;
            }

            // Hàng 4: Thanh toán - giao hàng
            y += controlHeight + spacingY + rowSpacing;
            x = startX;

            string[] labels4 = { "Hình thức thanh toán", "Phương thức thanh toán", "Phương thức giao hàng", "Thời gian giao hàng", "Đợt","Trạng thái" };

            foreach (string label in labels4)
            {
                Label lbl = new Label
                {
                    Text = label,
                    Location = new Point(x, y),
                    AutoSize = true
                };
                groupBox.Controls.Add(lbl);

                if (label == "Hình thức thanh toán")
                {
                    txtHinhThucThanhToan = new TextBox
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = textBoxWidth,
                    };
                    
                    groupBox.Controls.Add(txtHinhThucThanhToan);

                }
                else if (label == "Phương thức thanh toán")
                {
                    cboPhuongThucThanhToan = new ComboBox
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = textBoxWidth,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    cboPhuongThucThanhToan.Items.Add("Tiền mặt");
                    cboPhuongThucThanhToan.Items.Add("Chuyển khoản");
                    cboPhuongThucThanhToan.Items.Add("Bù trừ công nợ");
                    cboPhuongThucThanhToan.Items.Add("Ghi nợ");
                    cboPhuongThucThanhToan.SelectedIndex = -1; 
                    groupBox.Controls.Add(cboPhuongThucThanhToan);

                  
                }
                else if (label == "Thời gian giao hàng")
                {
                    dtThoiGianGiaoHang = new DateTimePicker
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = textBoxWidth*2 + spacingX,
                        Format = DateTimePickerFormat.Short
                    };
                    groupBox.Controls.Add(dtThoiGianGiaoHang);
                }
                else
                {
                    TextBox txt = new TextBox
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = (label.Contains("giao hàng")) ? textBoxWidth * 2 + spacingX : textBoxWidth
                    };
                    groupBox.Controls.Add(txt);
                    if (label.Contains("Phương thức giao hàng"))
                        txtPhuongThucGiaoHang = txt;
                    else if (label.Contains("Số ngày nợ"))
                        txtSoNgayNo = txt;
                    else if (label.Contains("Trạng thái"))
                        txtTrangThai = txt;
                    else if (label.Contains("Đợt"))
                        txtDot = txt;
                }

                x += (label.Contains("giao hàng") ? textBoxWidth * 2 + spacingX : textBoxWidth) + spacingX;

                //x +=  cbo.Width + spacingX;
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

        private void CapNhatTongHop()
        {
            decimal tongSL = 0;
            decimal tongTien = 0;

            foreach (DataGridViewRow row in dgv.Rows)
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

        private DateTime? GetDateFromGridCell(object cellValue)
        {
            if (cellValue == null) return null;

            string raw = cellValue.ToString().Trim();
            if (raw == "") return null;

            string[] formats = { "dd/MM/yyyy", "d/M/yyyy", "dd/M/yyyy", "d/MM/yyyy" };

            DateTime parsed;

            if (DateTime.TryParseExact(
                raw,
                formats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out parsed))
            {
                return parsed;
            }

            throw new Exception($"Ngày không hợp lệ: '{raw}'. Định dạng phải là dd/MM/yyyy.");
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            string soDonDatHang = $"{cboMaChungTu.SelectedValue}{txtSoChungTu.Text.Trim()}";
            string dot = txtDot.Text.Trim();
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string sql = @"
                INSERT INTO DonMuaHang
                 (SoDonDatHang, NgayChungTu, MaHopDong, NgayDenHan,MaNLH, MaNCC,MaNgoaiTe,ThoiGianGiaoHang,
                 TrangThai, HinhThucThanhToan,TenDiaDiemGiao,DiaChiGiao,DienThoaiGiao, PhuongThucThanhToan, PhuongThucGiao, NoiDung,MaNhanVienLap, Dot)
                 
                VALUES
                (@SoDonDatHang, @NgayChungTu, @MaHopDong, @NgayDenHan, @MaNLH, @MaNCC, @MaNgoaiTe, @ThoiGianGiaoHang, 
                 @TrangThai, @HinhThucTT,@TenDiaDiemGiao,@DiaChiGiao,@DienThoaiGiao, @PhuongThucTT, @PhuongThucGiao, @NoiDung, @MaNhanVienLap, @Dot)";

                    using (OleDbCommand cmd = new OleDbCommand(sql, conn))
                    {
                        cmd.Parameters.Add(new OleDbParameter("@SoDonDatHang", OleDbType.VarChar)).Value = soDonDatHang;
                        cmd.Parameters.Add(new OleDbParameter("@NgayChungTu", OleDbType.Date)).Value = dtNgayChungTu?.Value ?? DateTime.Now;
                        cmd.Parameters.Add(new OleDbParameter("@MaHopDong", OleDbType.VarChar)).Value = txtSoHopDong?.Text ?? "";
                        cmd.Parameters.Add(new OleDbParameter("@NgayDenHan", OleDbType.Date)).Value = dtNgayDenHan?.Value ?? DateTime.Now;
                        cmd.Parameters.Add(new OleDbParameter("@MaNLH", OleDbType.VarChar)).Value = txtMaLienHe?.Text ?? "";
                        cmd.Parameters.Add(new OleDbParameter("@MaNCC", OleDbType.VarChar)).Value = txtMaNCC?.Text ?? "";
                        cmd.Parameters.Add(new OleDbParameter("@MaNgoaiTe", OleDbType.VarChar)).Value = cboLoaiTien?.Text ?? "";
                        cmd.Parameters.Add(new OleDbParameter("@ThoiGianGiaoHang", OleDbType.Date)).Value = dtThoiGianGiaoHang?.Value ?? DateTime.Now;
                        cmd.Parameters.Add(new OleDbParameter("@TrangThai", OleDbType.VarChar)).Value = string.IsNullOrWhiteSpace(txtTrangThai?.Text) ? "Đang thực hiện" : txtTrangThai.Text;
                        cmd.Parameters.Add(new OleDbParameter("@HinhThucTT", OleDbType.VarChar)).Value = txtHinhThucThanhToan?.Text ?? "";
                        cmd.Parameters.Add(new OleDbParameter("@TenDiaDiemGiao", OleDbType.VarChar)).Value = txtTenDiaDiem?.Text ?? "";
                        cmd.Parameters.Add(new OleDbParameter("@DiaChiGiao", OleDbType.VarChar)).Value = txtDiaChiGiao?.Text ?? "";
                        cmd.Parameters.Add(new OleDbParameter("@DienThoaiGiao", OleDbType.VarChar)).Value = txtDTGiao?.Text ?? "";
                        cmd.Parameters.Add(new OleDbParameter("@PhuongThucTT", OleDbType.VarChar)).Value = cboPhuongThucThanhToan?.Text ?? "";
                        cmd.Parameters.Add(new OleDbParameter("@PhuongThucGiao", OleDbType.VarChar)).Value = txtPhuongThucGiaoHang?.Text ?? "";
                        cmd.Parameters.Add(new OleDbParameter("@NoiDung", OleDbType.LongVarChar)).Value = txtNoiDung?.Text ?? "";
                        cmd.Parameters.Add(new OleDbParameter("@MaNhanVienLap", OleDbType.VarChar)).Value = "NV010";
                        cmd.Parameters.Add(new OleDbParameter("@Dot", OleDbType.VarChar)).Value = dot ;


                        cmd.ExecuteNonQuery();
                    }
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.IsNewRow) continue;

                        string maHH = Convert.ToString(row.Cells["MaHang"].Value);
                        if (string.IsNullOrEmpty(maHH)) continue;

                        double soLuong = 0, donGia = 0, vat = 0, tienVat = 0, sothangbaohanh = 0;
                        double.TryParse(Convert.ToString(row.Cells["SL"].Value), out soLuong);
                        double.TryParse(Convert.ToString(row.Cells["DonGia"].Value), out donGia);
                        double.TryParse(Convert.ToString(row.Cells["Vat"].Value), out vat);
                        double.TryParse(Convert.ToString(row.Cells["TienVat"].Value), out tienVat);
                        string dienGiaiCT = Convert.ToString(row.Cells["DienGiai"].Value) ?? "";

                        DateTime? ngaygiao = GetDateFromGridCell(row.Cells["NgayGH"].Value);

                        double.TryParse(Convert.ToString(row.Cells["ThangBH"].Value), out sothangbaohanh);

                        string queryCT = @"INSERT INTO ChiTietDonMua 
                    (SoDonDatHang, MaHH, SoLuong, DonGia, PhanTramVAT, DienGiai, NgayGiaoHang,SoThangBaoHanh)
                    VALUES (@SoDonDatHang, @MaHH, @SoLuong, @DonGia, @Vat, @DienGiai, @NgayGiaoHang, @SoThangBaoHanh)";

                        using (OleDbCommand cmdCT = new OleDbCommand(queryCT, conn))
                        {
                            cmdCT.Parameters.Add("@SoDonDatHang", OleDbType.VarChar).Value = soDonDatHang;
                            cmdCT.Parameters.Add("@MaHH", OleDbType.VarChar).Value = maHH;
                            cmdCT.Parameters.Add("@SoLuong", OleDbType.Double).Value = soLuong;
                            cmdCT.Parameters.Add("@DonGia", OleDbType.Currency).Value = donGia;
                            cmdCT.Parameters.Add("@Vat", OleDbType.Double).Value = vat;
                            cmdCT.Parameters.Add("@DienGiai", OleDbType.LongVarChar).Value = dienGiaiCT;
                            cmdCT.Parameters.Add("@NgayGiaoHang", OleDbType.Date).Value =
                                ngaygiao.HasValue ? (object)ngaygiao.Value : DBNull.Value;
                            cmdCT.Parameters.Add("@SoThangBaoHanh", OleDbType.Double).Value = sothangbaohanh;
                            cmdCT.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("✅ Lưu đơn mua hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lưu đơn mua hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void BtnIn_Click(object sender, EventArgs e)
        {
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += printDocument1_PrintPage;

            PrintPreviewDialog preview = new PrintPreviewDialog();
            preview.Document = printDoc;
            preview.Width = 1000;
            preview.Height = 800;
            preview.ShowDialog();
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen pen = new Pen(Color.Black, 1);
            Font fontTitle = new Font("Segoe UI", 14, FontStyle.Bold);
            Font fontHeader = new Font("Segoe UI", 10, FontStyle.Bold);
            Font fontText = new Font("Segoe UI", 10, FontStyle.Regular);
            Font fontSmall = new Font("Segoe UI", 9, FontStyle.Italic);

            int y = 60;

            // ====== TIÊU ĐỀ ======
            g.DrawString("ĐƠN MUA HÀNG", fontTitle, Brushes.Black, 300, y);
            y += 40;

            // ====== THÔNG TIN CHUNG ======
            g.DrawString($"Mã chứng từ: {cboMaChungTu?.Text ?? ""}", fontText, Brushes.Black, 80, y);
            g.DrawString($"Ngày: {dtNgayChungTu?.Value.ToShortDateString() ?? ""}", fontText, Brushes.Black, 500, y);
            y += 25;

            g.DrawString($"Số hợp đồng: {txtSoHopDong?.Text ?? ""}", fontText, Brushes.Black, 80, y);
            g.DrawString($"Ngày HĐ: {dtNgayHopDong?.Value.ToShortDateString() ?? ""}", fontText, Brushes.Black, 500, y);
            y += 25;

            g.DrawString($"Nhà cung cấp: {txtTenNCC?.Text ?? ""}", fontText, Brushes.Black, 80, y);
            y += 25;

            g.DrawString($"Địa chỉ: {txtDiaChi?.Text ?? ""}", fontText, Brushes.Black, 80, y);
            y += 25;

            g.DrawString($"Điện thoại: {txtDienThoai?.Text ?? ""}", fontText, Brushes.Black, 80, y);
            g.DrawString($"Người liên hệ: {txtNguoiLienHe?.Text ?? ""}", fontText, Brushes.Black, 500, y);
            y += 35;

            // ====== BẢNG CHI TIẾT ======
            int startX = 80;
            int[] colWidths = { 80, 230, 80, 100, 120, 120 }; // Ma, Ten, DVT, SL, Đơn giá, Thành tiền
            string[] headers = { "Mã hàng", "Tên hàng", "ĐVT", "Số lượng", "Đơn giá", "Thành tiền" };

            // Vẽ header
            int x = startX;
            for (int i = 0; i < headers.Length; i++)
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(230, 240, 255)), new Rectangle(x, y, colWidths[i], 25));
                g.DrawRectangle(pen, x, y, colWidths[i], 25);
                g.DrawString(headers[i], fontHeader, Brushes.Black, x + 5, y + 5);
                x += colWidths[i];
            }
            y += 25;

            // Dữ liệu
            decimal tongTien = 0;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;
                x = startX;
                g.DrawRectangle(pen, x, y, colWidths[0], 25);
                g.DrawString(row.Cells["MaHang"].Value?.ToString(), fontText, Brushes.Black, x + 3, y + 5);
                x += colWidths[0];

                g.DrawRectangle(pen, x, y, colWidths[1], 25);
                g.DrawString(row.Cells["TenHang"].Value?.ToString(), fontText, Brushes.Black, x + 3, y + 5);
                x += colWidths[1];

                g.DrawRectangle(pen, x, y, colWidths[2], 25);
                g.DrawString(row.Cells["DVT"].Value?.ToString(), fontText, Brushes.Black, x + 3, y + 5);
                x += colWidths[2];

                g.DrawRectangle(pen, x, y, colWidths[3], 25);
                g.DrawString(row.Cells["SL"].Value?.ToString(), fontText, Brushes.Black, x + 3, y + 5);
                x += colWidths[3];

                g.DrawRectangle(pen, x, y, colWidths[4], 25);
                g.DrawString(row.Cells["DonGia"].Value?.ToString(), fontText, Brushes.Black, x + 3, y + 5);
                x += colWidths[4];

                g.DrawRectangle(pen, x, y, colWidths[5], 25);
                g.DrawString(row.Cells["TongTien"].Value?.ToString(), fontText, Brushes.Black, x + 3, y + 5);

                if (decimal.TryParse(row.Cells["TongTien"].Value?.ToString(), out decimal thanhTien))
                    tongTien += thanhTien;

                y += 25;
            }

            // ====== PHẦN TỔNG HỢP, VAT ======
            y += 15;
            g.DrawLine(pen, 80, y, 700, y);
            y += 10;

            decimal vatPhanTram = 10; // có thể thay bằng txtVAT.Text nếu có
            decimal tienVAT = tongTien * vatPhanTram / 100;
            decimal tongThanhToan = tongTien + tienVAT;

            g.DrawString("Cộng tiền hàng:", fontText, Brushes.Black, 430, y);
            g.DrawString($"{tongTien:N0} VND", fontText, Brushes.Black, 580, y);
            y += 25;

            g.DrawString($"Thuế VAT ({vatPhanTram}%):", fontText, Brushes.Black, 430, y);
            g.DrawString($"{tienVAT:N0} VND", fontText, Brushes.Black, 580, y);
            y += 25;

            g.DrawString("TỔNG CỘNG THANH TOÁN:", fontHeader, Brushes.Black, 380, y);
            g.DrawString($"{tongThanhToan:N0} VND", fontHeader, Brushes.Black, 580, y);
            y += 30;
            g.DrawLine(pen, 80, y, 700, y);

            // ====== PHẦN CHỮ KÝ ======
            y += 60;
            Font fontSignature = new Font("Segoe UI", 10, FontStyle.Bold);
            Font fontNote = new Font("Segoe UI", 9, FontStyle.Italic);

            int colWidthSig = 220;
            int startXSig = 100;

            g.DrawString("Người lập", fontSignature, Brushes.Black, startXSig, y);
            g.DrawString("Kế toán", fontSignature, Brushes.Black, startXSig + colWidthSig + 60, y);
            g.DrawString("Giám đốc", fontSignature, Brushes.Black, startXSig + (colWidthSig + 60) * 2, y);
            y += 20;

            g.DrawString("(Ký, ghi rõ họ tên)", fontNote, Brushes.Black, startXSig, y);
            g.DrawString("(Ký, ghi rõ họ tên)", fontNote, Brushes.Black, startXSig + colWidthSig + 60, y);
            g.DrawString("(Ký, ghi rõ họ tên)", fontNote, Brushes.Black, startXSig + (colWidthSig + 60) * 2, y);
        }


        private decimal TinhTongTien()
        {
            decimal tong = 0;
            foreach (DataGridViewRow r in dgv.Rows)
            {
                if (r.IsNewRow) continue;
                if (decimal.TryParse(r.Cells["TongTien"].Value?.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal val))
                    tong += val;
            }
            return tong;
        }
       
        private void BtnLayChungTuThamChieu_Click(object sender, EventArgs e)
        {
            using (var popup = new ChonHopDongPopup())
            {
                if (popup.ShowDialog() == DialogResult.OK)
                {
                    string maHD = popup.SelectedMaHopDong;
                    int dot = popup.SelectedDot;
                    using (OleDbConnection conn = new OleDbConnection(connectionString))
                    {
                        conn.Open();
                        string manv = "";
                        // 1. Lấy thông tin chính hợp đồng
                        string queryHD = "SELECT * FROM HopDong WHERE MaHopDong = @MaHopDong";
                        OleDbCommand cmdHD = new OleDbCommand(queryHD, conn);
                        cmdHD.Parameters.AddWithValue("@MaHopDong", maHD);
                        OleDbDataReader rd = cmdHD.ExecuteReader();
                        if (rd.Read())
                        {
                            txtSoHopDong.Text = rd["MaHopDong"].ToString();
                            dtNgayHopDong.Value = Convert.ToDateTime(rd["NgayKy"]);
                            txtMaNCC.Text = rd["MaNCC"].ToString();
                            txtMaLienHe.Text = rd["MaNLH"].ToString();
                            manv = rd["MaNhanVienLap"].ToString();
                            txtDot.Text = dot.ToString();
                            txtHinhThucThanhToan.Text = rd["HinhThucThanhToan"].ToString();

                        }
                        rd.Close();

                        OleDbCommand cmdNV = new OleDbCommand(
                            "SELECT * FROM NhanVien WHERE MaNhanVien=@MaNhanVien", conn);
                        cmdNV.Parameters.AddWithValue("@MaNhanVien", manv);
                        OleDbDataReader rdNV = cmdNV.ExecuteReader();
                        if (rdNV.Read())
                        {
                            txtNguoiLap.Text = rdNV["HoTen"].ToString();
                        }
                        rdNV.Close();

                        // 2. Lấy tên và địa chỉ NCC
                        OleDbCommand cmdNCC = new OleDbCommand(
                            "SELECT * FROM NhaCungCap WHERE MaNCC=@MaNCC", conn);
                        cmdNCC.Parameters.AddWithValue("@MaNCC", txtMaNCC.Text);
                        OleDbDataReader rdNCC = cmdNCC.ExecuteReader();
                        if (rdNCC.Read())
                        {
                            txtTenNCC.Text = rdNCC["TenNCC"].ToString();
                            txtDiaChi.Text = rdNCC["DiaChi"].ToString();
                            txtDienThoai.Text = rdNCC["DienThoai"].ToString();
                        }
                        rdNCC.Close();

                        // 3. Lấy thông tin người liên hệ
                        OleDbCommand cmdLH = new OleDbCommand(
                            "SELECT TenNLH, DienThoai FROM NguoiLienHe WHERE MaNLH=@MaNLH", conn);
                        cmdLH.Parameters.AddWithValue("@MaNLH", txtMaLienHe.Text);
                        OleDbDataReader rdLH = cmdLH.ExecuteReader();
                        if (rdLH.Read())
                        {
                            txtNguoiLienHe.Text = rdLH["TenNLH"].ToString();
                            txtDienThoaiLienHe.Text = rdLH["DienThoai"].ToString();
                        }
                        rdLH.Close();

                        // 4. Lấy hình thức thanh toán
                        

                        // 5. Lấy ngày giao hàng (và thông tin mặt hàng)
                        // Lấy chi tiết điều khoản mua hàng
                        string sql = @"
                                    SELECT ctk.MaHH, h.TenHH, h.DonViTinh, 
                                           ctk.SoLuongDat, h.DonGiaMua, 
                                           (ctk.SoLuongDat * h.DonGiaMua) AS ThanhTien,ctk.DienGiai,
                                           ctk.PhanTramVat, 
                                            (ctk.PhanTramVat / 100 * ThanhTien) AS TienVat,
                                            ctk.NgayGiaoHang, ctk.SoThangBaoHanh
                                    FROM ChiTietDieuKhoanMuaHang AS ctk
                                    INNER JOIN HangHoa AS h ON ctk.MaHH = h.MaHH
                                    WHERE ctk.MaHopDong = @MaHopDong AND ctk.Dot = @Dot";

                        OleDbDataAdapter daCT = new OleDbDataAdapter(sql, conn);
                        daCT.SelectCommand.Parameters.AddWithValue("@MaHopDong", maHD);
                        daCT.SelectCommand.Parameters.AddWithValue("@Dot", dot);

                        DataTable dtCT = new DataTable();
                        daCT.Fill(dtCT);

                        // Nếu có dữ liệu thì gán ngày đến hạn = ngày giao hàng đầu tiên
                        if (dtCT.Rows.Count > 0 && dtCT.Columns.Contains("NgayGiaoHang"))
                        {
                            var ngayGiao = dtCT.Rows[0]["NgayGiaoHang"];
                            if (ngayGiao != DBNull.Value)
                                dtNgayDenHan.Value = Convert.ToDateTime(ngayGiao);
                        }

                        // 6️⃣ Đổ vào DataGridView theo tên cột
                        dgv.Rows.Clear();
                        foreach (DataRow r in dtCT.Rows)
                        {
                            // Lấy giá trị có kiểm tra null
                            string maHH = r["MaHH"]?.ToString() ?? "";
                            string tenHH = r["TenHH"]?.ToString() ?? "";
                            string dvt = r["DonViTinh"]?.ToString() ?? "";
                            string sl = r["SoLuongDat"]?.ToString() ?? "0";
                            string donGia = r["DonGiaMua"]?.ToString() ?? "0";
                            string thanhTien = r["ThanhTien"]?.ToString() ?? "0";
                            string vat = r["PhanTramVat"]?.ToString() ?? "0";
                            string tienvat = r["TienVat"]?.ToString() ?? "0";
                            string dienGiai = r["DienGiai"]?.ToString() ?? "0";
                            string ngayGH = "";
                            if (r["NgayGiaoHang"] != DBNull.Value)
                            {
                                DateTime ngay = Convert.ToDateTime(r["NgayGiaoHang"]);
                                ngayGH = ngay.ToString("dd/MM/yyyy");   // ⬅ CHỈ LẤY NGÀY
                            }
                            string thangBH = r["SoThangBaoHanh"]?.ToString() ?? "";

                            // Thêm dòng mới theo đúng thứ tự cột DataGridView
                            dgv.Rows.Add(
                                maHH,       
                                tenHH,      
                                dvt,        
                                sl,         
                                donGia,  
                                thanhTien,  
                                dienGiai, 
                                vat,        
                                tienvat,         
                                ngayGH,     
                                thangBH   
                                      
                            );
                            
                        }
                        
                        MessageBox.Show("Đã lấy dữ liệu hợp đồng thành công!");
                    }
                }
            }
            CapNhatTongHop();
        }


    }
}
