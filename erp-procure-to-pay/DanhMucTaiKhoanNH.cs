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
    public partial class DanhMucTaiKhoanNH : Form
    {
        public DanhMucTaiKhoanNH()
        {
            InitializeComponent();
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Danh mục tài khoản ngân hàng";
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
                if (ctrl is Button btn && btn.Text.Contains("Xem"))
                {
                    btn.Click += BtnSua_Click;
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
                    ForeColor = Color.White, // cho chữ nổi bật hơn
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

          

            // Hàng 1: Mã CT, Ngày PO, Số PO, Loại tiền, Số hợp đồng, Ngày hợp đồng, Ngày đến hạn, Người lập
            string[] labels1 = { "Số tài khoản*", "Ngân hàng*", "Chi nhánh", "Tỉnh/TP" };
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
                else if (label.Contains("Ngân hàng") || label.Contains("Chi nhánh") || label.Contains("Tỉnh/TP"))
                {
                    input = new ComboBox
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = textBoxWidth *3 -95,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                }
                else
                {
                    input = new TextBox
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = label.Contains("Địa chỉ") 
                        ? textBoxWidth * 3 + spacingX * 2 : textBoxWidth * 2 + spacingX,
                    };
                }
                // Gọi hàm bo góc
                ;
                groupBox.Controls.Add(input);
                x += input.Width + spacingX;
            }

            // Hàng 2: Nhà cung cấp
            y += controlHeight + spacingY + rowSpacing;
            x = startX;

            string[] labels2 = {  "Địa chỉ chi nhánh", "Chủ tài khoản","Ghi chú" };

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
                    Width = (label.Contains("Địa chỉ") || label.Contains("Ghi chú")  ? textBoxWidth * 4 + spacingX
                                    : textBoxWidth *2 -100)
                };
                groupBox.Controls.Add(txt);

                x += txt.Width + spacingX;
            }

        }
        private void BtnSua_Click(object sender, EventArgs e)
        {
            DanhSachTaiKhoanNH ds = new DanhSachTaiKhoanNH();
            ds.ShowDialog();

        }
    }
}
