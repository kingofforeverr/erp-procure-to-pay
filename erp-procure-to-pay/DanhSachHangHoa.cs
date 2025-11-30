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
    public partial class DanhSachHangHoa : Form
    {
        private DataGridView dgvDanhSach;
        public DanhSachHangHoa()
        {
            InitializeComponent();
            BuildUI();
        }
        private void BuildUI()
        {
            this.Text = "Danh mục hàng hóa";
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

            int y = 10;
            GroupBox grpDanhSach = new GroupBox
            {
                Text = "DANH SÁCH HÀNG HÓA",
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
            dgvDanhSach.Columns.Add("MaHH", "Mã hàng hóa");
            dgvDanhSach.Columns.Add("TenHH", "Tên hàng hóa");
            dgvDanhSach.Columns.Add("Loai", "Loại");
            dgvDanhSach.Columns.Add("Kho", "Kho ngầm định");

            // === Đổ dữ liệu mẫu ===
            dgvDanhSach.Rows.Add("HH001", "Xi măng Nghi Sơn", "Vật liệu xây dựng", "Kho VLXD");
            dgvDanhSach.Rows.Add("HH002", "Thép Hòa Phát D16", "Vật liệu xây dựng", "Kho VLXD");
            dgvDanhSach.Rows.Add("HH003", "Sơn Jotun nội thất", "Sơn & Hóa chất", "Kho Hóa chất");
            dgvDanhSach.Rows.Add("HH004", "Ống nước Bình Minh Ø27", "Vật tư ống nước", "Kho Vật tư");
            dgvDanhSach.Rows.Add("HH005", "Gạch ốp lát Viglacera 60x60", "Trang trí nội thất", "Kho Gạch men");
            dgvDanhSach.Rows.Add("HH006", "Dây điện Cadivi 2x2.5", "Điện & Chiếu sáng", "Kho Điện");
            dgvDanhSach.Rows.Add("HH007", "Đèn LED âm trần Rạng Đông", "Điện & Chiếu sáng", "Kho Điện");
            dgvDanhSach.Rows.Add("HH008", "Cát xây tô loại 1", "Vật liệu xây dựng", "Kho VLXD");
            dgvDanhSach.Rows.Add("HH009", "Keo dán gạch Weber", "Hóa chất xây dựng", "Kho Hóa chất");
            dgvDanhSach.Rows.Add("HH010", "Bồn nước inox Sơn Hà 1000L", "Thiết bị gia dụng", "Kho Thiết bị");
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
                Text = "Chi tiết hàng hóa",
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

                        // Riêng combobox "Mã chứng từ" vẫn disable
                        DisableMaChungTu(pnlMain);
                    };
                }
            }


        }
        private void BtnXoa_Click(object sender, EventArgs e)
        {
            // Kiểm tra có chọn dòng nào chưa
            if (dgvDanhSach.SelectedRows.Count == 0)
            {
                MessageBox.Show("Không thể xóa yêu cầu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvDanhSach.SelectedRows[0];
            string trangThai = selectedRow.Cells[3].Value.ToString();
            string maYeuCau = selectedRow.Cells[0].Value.ToString();
            

            DialogResult result = MessageBox.Show(
                $"Bạn có chắc muốn xóa hàng hóa '{maYeuCau}' không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                dgvDanhSach.Rows.Remove(selectedRow);
                MessageBox.Show($"Đã xóa hàng hóa '{maYeuCau}' thành công.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            int textBoxWidth = 190;
            int controlHeight = 32;
            int spacingX = 45;
            int spacingY = 20;
            int rowSpacing = 25;



            // Hàng 1: 
            string[] labels1 = { "Mã(*)", "Tên(*)", "Loại(*)", "Mô tả", "Đơn vị tính(*)" };
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

                Control input = null;
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
                else if (label.Contains("Loại"))
                {
                    ComboBox cboLoai = new ComboBox
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = textBoxWidth * 2 + spacingX,
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        Name = "cboLoaiHangHoa"
                    };

                    LoadLoaiHangHoa(cboLoai); // load dữ liệu từ DB
                    groupBox.Controls.Add(cboLoai);

                    cboLoai.SelectedIndexChanged += (s, e) => LoadTaiKhoanTuLoai(cboLoai);

                    // tăng x theo Width của ComboBox
                    x += cboLoai.Width + spacingX;

                    continue;
                }
                else
                {
                    input = new TextBox
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = label.Contains("Tên") || label.Contains("Loại") || label.Contains("Mô tả")
                                    ? textBoxWidth * 2 + spacingX : textBoxWidth,
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

            string[] labels2 = { "Đơn vị chuyển đổi", "Tỉ lệ quy đổi", "Số lượng tối thiểu", "Nguồn gốc", "Đơn giá mua cố định" };

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
                    Width = (label.Contains("Nguồn gốc")) ? textBoxWidth * 3 + spacingX * 2 : textBoxWidth
                };
                groupBox.Controls.Add(txt);

                x += txt.Width + spacingX;
            }

            // Hàng 3: Địa điểm giao hàng
            y += controlHeight + spacingY + rowSpacing;
            x = startX;

            string[] labels3 = { "Kho ngầm định", "TK Kho", "TK doanh thu", "TK chi phí", "TK giá vốn", "Tk mua hàng", "Tỷ lệ CKMH (%)", "Thuế suất(%)" };

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
                    Width = (label.Contains("Địa chỉ")) ? textBoxWidth * 2 + spacingX : textBoxWidth
                };
                switch (label)
                {
                    case "TK Kho":
                        txt.Name = "txtTKKho";
                        break;
                    case "TK doanh thu":
                        txt.Name = "txtTKDoanhThu";
                        break;
                    case "TK chi phí":
                        txt.Name = "txtTKChiPhi";
                        break;
                    case "TK giá vốn":
                        txt.Name = "txtTKGiaVon";
                        break;
                    case "Tk mua hàng":
                        txt.Name = "txtTKMuaHang";
                        break;
                }

                groupBox.Controls.Add(txt);

                x += txt.Width + spacingX;
            }

            // Hàng 4: Thanh toán - giao hàng
            y += controlHeight + spacingY + rowSpacing;
            x = startX;

            CheckBox chkTheoDoi = new CheckBox
            {
                Text = "Quản lý tồn kho",
                Location = new Point(x, y),
                AutoSize = true
            };
            groupBox.Controls.Add((CheckBox)chkTheoDoi);

            // Hàng cuối – Nội dung
            y += controlHeight + spacingY;
            Label lblNoiDung = new Label
            {
                Text = "Ghi chú",
                Location = new Point(x, y),
                AutoSize = true
            };
            groupBox.Controls.Add(lblNoiDung);

            TextBox txtNoiDung = new TextBox
            {
                Location = new Point(startX, y + lblNoiDung.Height + 2),
                Width = groupBox.Width - 20,
                Height = 60,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            groupBox.Controls.Add(txtNoiDung);
        }
        private void LoadLoaiHangHoa(ComboBox input)
        {
            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=F:\reportkt\Database1.accdb;";
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT DISTINCT LoaiHangHoa FROM DinhKhoan";
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            input.Items.Clear();
                            while (reader.Read())
                            {
                                input.Items.Add(reader["LoaiHangHoa"].ToString());
                            }
                            input.SelectedIndex = -1; // mặc định rỗng
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải loại hàng hóa: " + ex.Message);
                }
            }
        }

        private void LoadTaiKhoanTuLoai(ComboBox cboLoai)
        {
            string selectedLoai = cboLoai.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedLoai)) return;

            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=F:\reportkt\Database1.accdb;";
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT TK_Kho, TK_ChiPhi, TK_MuaHang FROM DinhKhoan WHERE LoaiHangHoa = ?";
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@LoaiHangHoa", selectedLoai);

                        using (OleDbDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                SetTextBoxValue("txtTKKho", reader["TK_Kho"].ToString());
                                SetTextBoxValue("txtTKChiPhi", reader["TK_ChiPhi"].ToString());
                                SetTextBoxValue("txtTKMuaHang", reader["TK_MuaHang"].ToString());
                            }
                            else
                            {
                                // Không có dữ liệu thì để trống
                                SetTextBoxValue("txtTKKho", "");
                                SetTextBoxValue("txtTKDoanhThu", "");
                                SetTextBoxValue("txtTKChiPhi", "");
                                SetTextBoxValue("txtTKGiaVon", "");
                                SetTextBoxValue("txtTKMuaHang", "");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải TK: " + ex.Message);
                }
            }
        }
        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            FormTimKiemHangHoacs ds = new FormTimKiemHangHoacs();
            ds.ShowDialog();

        }

        // Hàm tìm TextBox theo Name và gán giá trị
        private void SetTextBoxValue(string name, string value)
        {
            foreach (Control ctrl in this.Controls)
            {
                TextBox txt = ctrl.Controls.Find(name, true).FirstOrDefault() as TextBox;
                if (txt != null)
                {
                    txt.Text = value;
                    break;
                }
            }
        }
    }
}
