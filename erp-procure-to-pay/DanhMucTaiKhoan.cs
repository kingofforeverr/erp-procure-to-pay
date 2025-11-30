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
    public partial class DanhMucTaiKhoan : Form
    {
        public DanhMucTaiKhoan()
        {
            InitializeComponent();
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Thêm Tài khoản";
            this.Size = new Size(650, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(230, 233, 246);

            // === HEADER ===
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

            // === THANH CÔNG CỤ ===
            

            // === PANEL CHÍNH ===
            Panel pnlMain = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(230, 233, 246)
            };
            this.Controls.Add(pnlMain);
            pnlMain.BringToFront();

            // === GROUPBOX: THÔNG TIN CHUNG ===
            GroupBox grpThongTin = new GroupBox
            {
                Text = "Thông tin chung",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 66, 168),
                Dock = DockStyle.Top,
                Padding = new Padding(30),
                Height = 420
            };
            pnlMain.Controls.Add(grpThongTin);

            int xLabel = 10, xControl = 180, y = 40, spacingY = 45, textWidth = 300;

            // Số tài khoản
            grpThongTin.Controls.Add(new Label { Text = "Số tài khoản (*)", Location = new Point(xLabel, y + 3), AutoSize = true });
            TextBox txtSoTK = new TextBox { Location = new Point(xControl, y), Width = textWidth };
            grpThongTin.Controls.Add(txtSoTK);

            y += spacingY;
            grpThongTin.Controls.Add(new Label { Text = "Tên tài khoản (*)", Location = new Point(xLabel, y + 3), AutoSize = true });
            TextBox txtTenTK = new TextBox { Location = new Point(xControl, y), Width = textWidth };
            grpThongTin.Controls.Add(txtTenTK);

            y += spacingY;
            grpThongTin.Controls.Add(new Label { Text = "TK tổng hợp", Location = new Point(xLabel, y + 3), AutoSize = true });
            ComboBox cboTongHop = new ComboBox { Location = new Point(xControl, y), Width = textWidth, DropDownStyle = ComboBoxStyle.DropDownList };
            grpThongTin.Controls.Add(cboTongHop);

            y += spacingY;
            grpThongTin.Controls.Add(new Label { Text = "Nhóm tài khoản", Location = new Point(xLabel, y + 3), AutoSize = true });
            ComboBox cboNhomTk = new ComboBox { Location = new Point(xControl, y), Width = textWidth, DropDownStyle = ComboBoxStyle.DropDownList };
            grpThongTin.Controls.Add(cboNhomTk);

            y += spacingY;
            grpThongTin.Controls.Add(new Label { Text = "Tính chất", Location = new Point(xLabel, y + 3), AutoSize = true });
            ComboBox cboTinhChat = new ComboBox { Location = new Point(xControl, y), Width = textWidth, DropDownStyle = ComboBoxStyle.DropDownList };
            cboTinhChat.Items.AddRange(new string[] { "Dư Nợ", "Dư Có", "Lưỡng tính" });
            grpThongTin.Controls.Add(cboTinhChat);

            y += spacingY;
            grpThongTin.Controls.Add(new Label { Text = "Diễn giải", Location = new Point(xLabel, y + 3), AutoSize = true });
            TextBox txtDienGiai = new TextBox { Location = new Point(xControl, y), Width = textWidth, Height = 60, Multiline = true };
            grpThongTin.Controls.Add(txtDienGiai);

            y += spacingY;
            grpThongTin.Controls.Add(new Label { Text = "Trạng thái", Location = new Point(xLabel, y + 25), AutoSize = true });

            CheckBox chkHoatDong = new CheckBox { Text = "Đang hoạt động", Location = new Point(xControl, y + 25), AutoSize = true };
            CheckBox chkNgungHD = new CheckBox { Text = "Ngừng hoạt động", Location = new Point(chkHoatDong.Right + 70, y + 25), AutoSize = true };
            grpThongTin.Controls.Add(chkHoatDong);
            grpThongTin.Controls.Add(chkNgungHD);

            // === PANEL CHỨA NÚT LƯU & HUỶ ===
            Panel pnlButtons = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                BackColor = Color.Transparent
            };
            this.Controls.Add(pnlButtons);

            // Nút Lưu
            Button btnLuu = new Button
            {
                Text = "💾 Lưu",
                Width = 140,
                Height = 45,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.LightSkyBlue,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(180, 15)
            };
            btnLuu.FlatAppearance.BorderSize = 0;

            // Nút Huỷ
            Button btnHuy = new Button
            {
                Text = "❌ Huỷ",
                Width = 140,
                Height = 45,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.LightCoral,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(330, 15)
            };
            btnHuy.FlatAppearance.BorderSize = 0;

            btnHuy.Click += (s, e) => this.Close();

            pnlButtons.Controls.Add(btnLuu);
            pnlButtons.Controls.Add(btnHuy);
        }
    }
}
