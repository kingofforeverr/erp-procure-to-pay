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
    public partial class DanhSachHeThongTaiKhoanKeToan : Form
    {
        public DanhSachHeThongTaiKhoanKeToan()
        {
            InitializeComponent();
            BuildUI();
        }
        private void BuildUI()
        {
            this.Text = "Danh sách hệ thống tài khoản kế toán";
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
                if (text.Contains("Thêm"))
                    btn.Click += BtnThem_Click;
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
            GroupBox grpDanhSach = new GroupBox
            {
                Text = "DANH SÁCH TÀI KHOẢN KẾ TOÁN",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 66, 168),
                Dock = DockStyle.Top,
                Height = 350,
                Padding = new Padding(10)
            };
            pnlMain.Controls.Add(grpDanhSach);

            // DataGridView hiển thị danh sách yêu cầu
            DataGridView dgvDanhSach = new DataGridView
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
            dgvDanhSach.Columns.Add("SoTK", "Số tài khoản ");
            dgvDanhSach.Columns.Add("TenNCC", "Tên tài khoản");
            dgvDanhSach.Columns.Add("ChiNhanh", "TK tổng hợp");
            dgvDanhSach.Columns.Add("Tinh", "Nhóm tài khoản");
            dgvDanhSach.Columns.Add("DiaChi", "Tính chất");
            dgvDanhSach.Columns.Add("SoDienThoai", "Diễn giải");
            dgvDanhSach.Columns.Add("SoDienThoai", "Trạng thái");

            // === Đổ dữ liệu mẫu ===
            // === Đổ dữ liệu mẫu ===
            dgvDanhSach.Rows.Add("1121", "Tiền gửi ngân hàng Vietcombank", "112", "Tài sản ngắn hạn", "Nợ", "Dùng để quản lý tiền gửi", "Đang hoạt động");
            dgvDanhSach.Rows.Add("131", "Phải thu khách hàng", "130", "Công nợ phải thu", "Nợ", "Theo dõi các khoản phải thu", "Đang hoạt động");
            dgvDanhSach.Rows.Add("331", "Phải trả người bán", "330", "Công nợ phải trả", "Có", "Theo dõi các khoản phải trả NCC", "Đang hoạt động");
            dgvDanhSach.Rows.Add("511", "Doanh thu bán hàng", "510", "Doanh thu", "Có", "Ghi nhận doanh thu bán hàng", "Đang hoạt động");
            dgvDanhSach.Rows.Add("632", "Giá vốn hàng bán", "630", "Chi phí", "Nợ", "Ghi nhận giá vốn", "Đang hoạt động");
            dgvDanhSach.Rows.Add("641", "Chi phí bán hàng", "640", "Chi phí hoạt động", "Nợ", "Ghi nhận chi phí bán hàng", "Đang hoạt động");
            dgvDanhSach.Rows.Add("642", "Chi phí quản lý doanh nghiệp", "640", "Chi phí hoạt động", "Nợ", "Ghi nhận chi phí quản lý", "Đang hoạt động");

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
                Text = "Chi tiết tài khoản kế toán",
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
            // === Căn chỉnh và làm đẹp ===
            dgvDanhSach.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDanhSach.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDanhSach.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvDanhSach.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dgvDanhSach.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvDanhSach.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 235, 250);
            dgvDanhSach.EnableHeadersVisualStyles = false;
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
            int startY = 70;
            int labelWidth = 110;
            int textBoxWidth = 200;
            int controlHeight = 32;
            int spacingX = 20;
            int spacingY = 20;
            int rowSpacing = 15;



            // Hàng 1: Mã CT, Ngày PO, Số PO, Loại tiền, Số hợp đồng, Ngày hợp đồng, Ngày đến hạn, Người lập
            string[] labels1 = { "Số tài khoản*", "Tên tài khoản", "TK tổng hợp", "Nhóm tài khoản" };
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
                        Width = textBoxWidth * 3 - 95,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                }
                else
                {
                    input = new TextBox
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = label.Contains("Địa chỉ")
                        ? textBoxWidth * 3 + spacingX * 2 : textBoxWidth * 2 + spacingX + 30,
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

            string[] labels2 = { "Tính chất", "Diễn giải", "Trạng thái" };

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
                    Width = (label.Contains("Địa chỉ") || label.Contains("Ghi chú") ? textBoxWidth * 4 + spacingX
                                    : textBoxWidth * 2 - 100)
                };
                groupBox.Controls.Add(txt);

                x += txt.Width + spacingX;
            }

        }
        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            FormTimKiemTkkt ds = new FormTimKiemTkkt();
            ds.ShowDialog();

        }
        private void BtnThem_Click(object sender, EventArgs e)
        {
            DanhMucTaiKhoan ds = new DanhMucTaiKhoan();
            ds.ShowDialog();

        }
    }
}
