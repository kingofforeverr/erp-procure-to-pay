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
    public partial class DanhMucNCC : Form
    {
        public DanhMucNCC()
        {
            InitializeComponent();
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Danh mục nhà cung cấp";
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
            // === Gắn sự kiện cho nút "Sửa" ===
            foreach (Control ctrl in pnlLeft.Controls)
            {
                if (ctrl is Button btn && btn.Text.Contains("Xem"))
                {
                    btn.Click += BtnXem_Click;
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
            this.Controls.Add(pnlMain);
            pnlMain.BringToFront();

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
            GroupBox grpLienHe = new GroupBox
            {
                Text = "LIÊN HỆ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 66, 168),
                Width = pnlMainContainer.ClientSize.Width - 40,
                Height = 300,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlMain.Controls.Add(grpLienHe);


            DataGridView dgvLienHe = new DataGridView
            {
                Location = new Point(20, 60),
                Width = grpLienHe.Width - 30,
                Height = grpLienHe.Height - 70,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            grpLienHe.Controls.Add(dgvLienHe);



            dgvLienHe.Columns.Add("Ten", "Tên");
            dgvLienHe.Columns.Add("GioiTinh", "Giới tính");
            dgvLienHe.Columns.Add("PhongBan", "Phòng ban");
            dgvLienHe.Columns.Add("ChucVu", "Chức vụ");
            dgvLienHe.Columns.Add("DienThoai", "Điện thoại");
            dgvLienHe.Columns.Add("Email", "Email");
            dgvLienHe.Columns.Add("DiaChi", "Địa chỉ");
            dgvLienHe.Columns.Add("NgaySinh", "Ngày sinh");
            dgvLienHe.Columns.Add("ThangSinh", "Tháng sinh");
            dgvLienHe.Columns.Add("NamSinh", "Năm sinh");

            // ===== NHÓM NGÂN HÀNG =====
            GroupBox grpNganHang = new GroupBox
            {
                Text = "NGÂN HÀNG",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 66, 168),
                Width = pnlMainContainer.ClientSize.Width - 40,
                Height = 300,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlMain.Controls.Add(grpNganHang);



            DataGridView dgvNganHang = new DataGridView
            {
                Location = new Point(20, 50),
                Width = grpNganHang.Width - 30,
                Height = grpNganHang.Height - 70,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            grpNganHang.Controls.Add(dgvNganHang);

            dgvNganHang.Columns.Add("MacDinh", "Mặc định");
            dgvNganHang.Columns.Add("SoTK", "Số TK ngân hàng");
            dgvNganHang.Columns.Add("TenTK", "Tên TK ngân hàng");
            dgvNganHang.Columns.Add("SwiftCode", "Swift Code");
            dgvNganHang.Columns.Add("NganHang", "Ngân hàng");
            dgvNganHang.Columns.Add("Tinh", "Tỉnh/Thành phố");
            dgvNganHang.Columns.Add("MaChiNhanh", "Mã chi nhánh");
            dgvNganHang.Columns.Add("TenChiNhanh", "Tên chi nhánh");


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
            int textBoxWidth = 188;
            int controlHeight = 32;
            int spacingX = 20;
            int spacingY = 20;
            int rowSpacing = 15;


            // Hàng 1: 
            string[] labels1 = { "Mã*", "Tên*", "Địa chỉ*", "Mã số thuế*" };
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
                    input = new ComboBox
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = textBoxWidth,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                }
                else
                {
                    input = new TextBox
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = label.Contains("Địa chỉ") ? textBoxWidth * 3 + spacingX * 2 : textBoxWidth * 2 + spacingX,
                    };
                }

                ;
                groupBox.Controls.Add(input);
                x += input.Width + spacingX;
            }

            // Hàng 2: Nhà cung cấp
            y += controlHeight + spacingY + rowSpacing;
            x = startX;

            string[] labels2 = { "Điện thoại", "Email", "Website", "Quốc gia", "Khu vực", "Tỉnh/TP" };

            foreach (string label in labels2)
            {
                Label lbl = new Label
                {
                    Text = label,
                    Location = new Point(x, y),
                    AutoSize = true
                };
                groupBox.Controls.Add(lbl);

                Control input;

                // Nếu là Quốc gia, Khu vực, Tỉnh/TP → dùng ComboBox (dropdown)
                if (label.Contains("Quốc gia") || label.Contains("Khu vực") || label.Contains("Tỉnh/TP"))
                {
                    ComboBox cbo = new ComboBox
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = textBoxWidth * 2 + spacingX,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };


                    if (label.Contains("Quốc gia"))
                    {
                        cbo.Items.AddRange(new string[] { "Việt Nam", "Thái Lan", "Singapore", "Malaysia" });
                    }
                    else if (label.Contains("Khu vực"))
                    {
                        cbo.Items.AddRange(new string[] { "Miền Bắc", "Miền Trung", "Miền Nam" });
                    }
                    else if (label.Contains("Tỉnh/TP"))
                    {
                        cbo.Items.AddRange(new string[] { "Hà Nội", "Đà Nẵng", "TP.HCM", "Cần Thơ" });
                    }

                    input = cbo;
                }
                else
                {

                    input = new TextBox
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = textBoxWidth
                    };
                }

                groupBox.Controls.Add(input);

                x += input.Width + spacingX;
            }

            // Hàng 3: 
            y += controlHeight + spacingY + rowSpacing;
            x = startX;



            // Hạn mức nợ
            Label lblHanMuc = new Label { Text = "Hạn mức nợ", Location = new Point(x, y), AutoSize = true };
            groupBox.Controls.Add(lblHanMuc);
            TextBox txtHanMuc = new TextBox { Location = new Point(x, y + 22), Width = textBoxWidth };
            groupBox.Controls.Add(txtHanMuc);

            // Số ngày nợ
            x += textBoxWidth + spacingX;
            Label lblNgayNo = new Label { Text = "Số ngày nợ", Location = new Point(x, y), AutoSize = true };
            groupBox.Controls.Add(lblNgayNo);
            TextBox txtNgayNo = new TextBox { Location = new Point(x, y + 22), Width = textBoxWidth };
            groupBox.Controls.Add(txtNgayNo);

            // Ghi chú
            x += textBoxWidth + spacingX;
            Label lblGhiChu = new Label { Text = "Ghi chú", Location = new Point(x, y), AutoSize = true };
            groupBox.Controls.Add(lblGhiChu);
            TextBox txtGhiChu = new TextBox
            {
                Location = new Point(x, y + 22),
                Width = textBoxWidth * 5
            };
            groupBox.Controls.Add(txtGhiChu);

            // CheckBox: Theo dõi công nợ
            x += txtGhiChu.Width + spacingX;
            CheckBox chkTheoDoi = new CheckBox
            {
                Text = "Theo dõi công nợ hóa đơn",
                Location = new Point(x, y + 25),
                AutoSize = true
            };
            groupBox.Controls.Add(chkTheoDoi);

            // CheckBox: Ngừng sử dụng
            x += 300;
            CheckBox chkNgung = new CheckBox
            {
                Text = "Ngừng sử dụng",
                Location = new Point(x, y + 25),
                AutoSize = true
            };
            groupBox.Controls.Add(chkNgung);



        }

        private void BtnXem_Click(object sender, EventArgs e)
        {
            DanhSachNCC ds = new DanhSachNCC();
            ds.ShowDialog();


        }
    }
}
