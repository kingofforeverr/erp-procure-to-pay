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
    public partial class DanhSachPhieuNhapKho : Form
    {
        TextBox txtMaCT, txtCTThamChieu, txtMaNCC, txtTenNCC, txtNguoiLienHe,
            txtSoCT, txtMaLienHe, txtDiaChi, txtDienThoaiLienHe, txtDienGiai, txtDienThoai, txtMaSoThue, txtKho;
        private bool isDirty = false;
        private Label lblTongSL, lblTongTien, lblTonHienTai, lblDuKienNhap;
        ComboBox cboMaKho;
        private DateTimePicker dtNgayCT;
        DataGridView dgv, dgvDanhSach;
        private string connectionString =
           DatabaseConfig.ConnectionString;
        public DanhSachPhieuNhapKho()
        {
            InitializeComponent();
            BuildUI();
            AttachControlChangeEvents();
        }
        private void BuildUI()
        {
            this.Text = "Danh sách phiếu nhập kho";
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
                "🗑️ Xóa",
                "👁️ Xem",
                "🖨️ In",
                "🔍 Tìm kiếm",
                "Lập phiếu xuất trả nhà cung cấp"
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
                    else if (btn.Text.Contains("xuất trả"))
                    {
                        btn.Click += BtnXuatTra_Click;
                    }
                    else if (btn.Text.Contains("Tìm"))
                    {
                        btn.Click += BtnTimKiem_Click;
                    }
                    else if (btn.Text.Contains("Xóa"))
                    {
                        btn.Click += BtnXoa_Click;
                    }
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
                Text = "DANH SÁCH PHIẾU NHẬP KHO",
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
            dgvDanhSach.Columns.Add("NguoiDK", "Mã kho");
            dgvDanhSach.Columns.Add("NgayCT", "Ngày chứng từ");

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
                Text = "Chi tiết phiếu nhập kho",
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

            // Tùy chỉnh header
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 235, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);

            // Thêm các cột với độ rộng hợp lý
            dgv.Columns.Add("MaHang", "Mã hàng");

            dgv.Columns.Add("TenHang", "Tên hàng");

            dgv.Columns.Add("DVT", "Đvt");

            dgv.Columns.Add("SL", "Số lượng đặt");

            dgv.Columns.Add("SLNhan", "Số lượng nhận");

            dgv.Columns.Add("DonGia", "Đơn giá");

            dgv.Columns.Add("TongTien", "Thành tiền");

            

          

            // Căn giữa header và dữ liệu số
            dgv.Columns["SL"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["DonGia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["TongTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["DVT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

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
            decimal tongTien = 0;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                decimal sl = 0, dongia = 0, tienvat = 0;

                decimal.TryParse(Convert.ToString(row.Cells["SL"].Value), out sl);
                decimal.TryParse(Convert.ToString(row.Cells["DonGia"].Value), out dongia);
                //decimal.TryParse(Convert.ToString(row.Cells["TienVat"].Value), out tienvat);

                tongSL += sl;
                tongTien += (sl * dongia);
            }

            // Cập nhật lên giao diện
            lblTongSL.Text = tongSL.ToString("N0");
            lblTongTien.Text = tongTien.ToString("N0");
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

        private void LoadDanhSach(DataGridView dgv)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"SELECT p.SoPNK, p.NgayChungTu, p.MaNCC, n.TenNCC, p.MaKho
                                    FROM PhieuNhapKho AS p
                                    LEFT JOIN NhaCungCap AS n
                                        ON p.MaNCC = n.MaNCC
                                    ORDER BY p.SoPNK DESC";
                    string maNCC = "";
                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgv.Rows.Clear();

                        foreach (DataRow row in dt.Rows)
                        {
                            dgv.Rows.Add(
                                row["SoPNK"].ToString(),
                                row["MaKho"].ToString(),
                                Convert.ToDateTime(row["NgayChungTu"]).ToString("dd/MM/yyyy")
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
                Text = "Chọn đơn mua hàng",
                Location = new Point(startX, 30),
                Size = new Size(200, controlHeight),
                BackColor = Color.MediumSeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            groupBox.Controls.Add(btnChon);

            // Hàng 1: 
            string[] labels1 = { "Số CT*", "Ngày CT*", "CT tham chiếu" };
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
                        Width = textBoxWidth * 3 + spacingX * 2,
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
                        Width = textBoxWidth * 2 + spacingX
                    };
                }
                ;
                groupBox.Controls.Add(input);
                if (label.Contains("Số CT"))
                    txtMaCT = (TextBox)input;
                else if (label.Contains("Ngày CT"))
                    dtNgayCT = (DateTimePicker)input;
                else if (label.Contains("CT tham chiếu"))
                    txtCTThamChieu = (TextBox)input;
                //else if (label.Contains("Số CT"))
                //    txtSoCT = (TextBox)input;
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
                    txtMaNCC = (TextBox)txt;
                else if (label.Contains("Tên nhà cung cấp"))
                    txtTenNCC = (TextBox)txt;
                else if (label.Contains("Địa chỉ"))
                    txtDiaChi = (TextBox)txt;
                else if (label.Contains("Mã số thuế"))
                    txtMaSoThue = (TextBox)txt;
                else if (label.Contains("Điện thoại"))
                    txtDienThoai = (TextBox)txt;
                x += txt.Width + spacingX;
            }

            // Hàng 3: Địa điểm giao hàng
            y += controlHeight + spacingY + rowSpacing;
            x = startX;

            string[] labels3 = { "Mã liên hệ", "Người liên hệ", "Điện thoại liên hệ", "Kho", "Nội dung" };

            foreach (string label in labels3)
            {
                Label lbl = new Label
                {
                    Text = label,
                    Location = new Point(x, y),
                    AutoSize = true
                };
                groupBox.Controls.Add(lbl);

                Control input;
                if (label.Contains("Kho"))
                {
                    cboMaKho = new ComboBox
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = textBoxWidth,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    LoadKho(cboMaKho);
                    input = cboMaKho;
                }

                else
                {
                    input = new TextBox
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = label.Contains("Mã liên hệ")
                    ? textBoxWidth
                    : label.Contains("Nội dung")
                        ? textBoxWidth * 3 + spacingX * 3
                        : textBoxWidth * 2
                    };
                }
                groupBox.Controls.Add(input);
                if (label.Contains("Mã liên hệ"))
                    txtMaLienHe = (TextBox)input;
                else if (label.Contains("Người liên hệ"))
                    txtNguoiLienHe = (TextBox)input;
                else if (label.Contains("Điện thoại liên hệ"))
                    txtDienThoaiLienHe = (TextBox)input;
                
                else if (label.Contains("Nội dung"))
                    txtDienGiai = (TextBox)input;
                x += input.Width + spacingX;
            }


            
        }

        private void DgvDanhSach_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDanhSach.CurrentRow == null) return;

            var cellValue = dgvDanhSach.CurrentRow.Cells["MaYC"].Value;
            if (cellValue == null) return;

            string maHD = cellValue.ToString();

            LoadThongTinPhieuNhapKho(maHD);

            LoadChiTietPhieuNhap(maHD);
            CapNhatTongHop();

        }
        private void LoadThongTinPhieuNhapKho(string maHD)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string sqlHD = @"SELECT * FROM PhieuNhapKho WHERE SoPNK = @SoPNK";
                OleDbCommand cmdHD = new OleDbCommand(sqlHD, conn);
                cmdHD.Parameters.AddWithValue("@SoPNK", maHD);

                string maNCC = "";
                string maNLH = "";
                using (OleDbDataReader rd = cmdHD.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        txtMaCT.Text = rd["SoPNK"].ToString();

                        if (rd["NgayChungTu"] != DBNull.Value)
                            dtNgayCT.Value = Convert.ToDateTime(rd["NgayChungTu"]);

                        txtDienGiai.Text = rd["NoiDung"].ToString();
                        txtCTThamChieu.Text = rd["ChungTuThamChieu"].ToString();
                        maNCC = rd["MaNCC"].ToString();
                        cboMaKho.Text = rd["MaKho"].ToString();
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
                            txtDienThoai.Text = rdNCC["DienThoai"].ToString();
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
                            txtMaLienHe.Text = maNCC;
                            txtNguoiLienHe.Text = rdNLH["TenNLH"].ToString();
                            txtDienThoaiLienHe.Text = rdNLH["DienThoai"].ToString();
                        }
                    }
                }


            }
        }
        private void LoadChiTietPhieuNhap(string maHD)
        {
            string sql = @"
                    SELECT 
                        ct.MaHH,
                        hh.TenHH,
                        hh.DonViTinh,
                        ct.SoLuongDat,
                        ct.DonGia,
                        (ct.SoLuongDat * ct.DonGia) AS ThanhTien
                    FROM  ChiTietPhieuNhapKho ct
                    LEFT JOIN HangHoa hh ON ct.MaHH = hh.MaHH
                    WHERE ct.SoPNK = @SoPNK
                ";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            using (OleDbDataAdapter da = new OleDbDataAdapter(sql, conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@SoPNK", maHD);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgv.Columns["MaHang"].DataPropertyName = "MaHH";
                dgv.Columns["TenHang"].DataPropertyName = "TenHH";
                dgv.Columns["DVT"].DataPropertyName = "DonViTinh";
                dgv.Columns["SL"].DataPropertyName = "SoLuongDat";
                dgv.Columns["DonGia"].DataPropertyName = "DonGia";
                dgv.Columns["TongTien"].DataPropertyName = "ThanhTien";
                

                dgv.DataSource = dt;
            }
        }


        private void AttachControlChangeEvents()
        {
            AttachEventsRecursive(this);
        }
        private void AttachEventsRecursive(Control parent)
        {
            foreach (Control ctl in parent.Controls)
            {
                if (ctl is TextBox txt && txt.Name != "txtMaCT")
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
        private void LoadKho(ComboBox input)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Kho";
                    using (OleDbDataAdapter da = new OleDbDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        input.DataSource = dt;
                        input.DisplayMember = "MaKho";
                        input.ValueMember = "MaKho";
                        input.SelectedIndex = -1;

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
                }
            }
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            if (!isDirty)
            {
                MessageBox.Show("Không có thay đổi để lưu.");
                return;
            }

            string soPNK = txtMaCT.Text.Trim();
            DateTime ngayct = dtNgayCT.Value;
            string chungtuthamchieu = txtCTThamChieu.Text;
            string dienGiai = txtDienGiai.Text.Trim();
            string kho = cboMaKho.SelectedValue.ToString();
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                // UPDATE bảng chính
                string sqlUpdate = @"
            UPDATE PhieuNhapKho
            SET NgayChungTu = ?, 
                NoiDung = ?,
                ChungTuThamChieu = ?,
                MaKho = ?
            WHERE SoPNK = ?
        ";

                using (OleDbCommand cmd = new OleDbCommand(sqlUpdate, conn))
                {
                    cmd.Parameters.Add("@NgayChungTu", OleDbType.Date).Value = ngayct;
                    cmd.Parameters.AddWithValue("@NoiDung", OleDbType.VarChar).Value = dienGiai;
                    cmd.Parameters.AddWithValue("@ChungTuThamChieu", OleDbType.VarChar).Value = chungtuthamchieu;
                    cmd.Parameters.AddWithValue("@kho", OleDbType.VarChar).Value = kho;
                    cmd.Parameters.AddWithValue("@SoPNK", OleDbType.VarChar).Value = soPNK;

                    cmd.ExecuteNonQuery();
                }

                // UPDATE chi tiết (dgv)
                UpdateChiTiet(conn, soPNK);
            }

            isDirty = false;
            MessageBox.Show(
                "Đã lưu thay đổi thành công!",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
        private void UpdateChiTiet(OleDbConnection conn, string maYC)
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                string maHH = row.Cells["MaHang"].Value?.ToString();
                decimal soLuongdat = Convert.ToDecimal(row.Cells["SL"].Value ?? 0);
                decimal soLuongnhan = Convert.ToDecimal(row.Cells["SLNhan"].Value ?? 0);
                decimal donGia = Convert.ToDecimal(row.Cells["DonGia"].Value ?? 0);

                // 1) Kiểm tra tồn tại
                string checkSQL = @"SELECT COUNT(*) FROM ChiTietPhieuNhapKho
                            WHERE SoPNK = ? AND MaHH = ?";

                bool exists = false;

                using (OleDbCommand cmdCheck = new OleDbCommand(checkSQL, conn))
                {
                    cmdCheck.Parameters.AddWithValue("@YC", maYC);
                    cmdCheck.Parameters.AddWithValue("@HH", maHH);

                    exists = (int)cmdCheck.ExecuteScalar() > 0;
                }

                if (exists)
                {
                    // 2) UPDATE
                    string updateSQL = @"
                UPDATE ChiTietPhieuNhapKho
                SET SoLuongDat = ?,SoLuongNhan = ?,DonGia = ?
                WHERE SoPNK = ? AND MaHH = ?
            ";

                    using (OleDbCommand cmdUp = new OleDbCommand(updateSQL, conn))
                    {
                        cmdUp.Parameters.AddWithValue("@SL", soLuongdat);
                        cmdUp.Parameters.AddWithValue("@SLNhan", soLuongnhan);
                        cmdUp.Parameters.AddWithValue("@DG", donGia);
                        cmdUp.Parameters.AddWithValue("@YC", maYC);
                        cmdUp.Parameters.AddWithValue("@HH", maHH);

                        cmdUp.ExecuteNonQuery();
                    }
                }
                else
                {
                    // 3) INSERT
                    string insertSQL = @"
                INSERT INTO ChiTietPhieuNhapKho
                (SoLuongDat ,SoLuongNhan ,DonGia  ,MaHH, SoPNK)
                VALUES (?, ?, ?, ?, ?,?,?)
            ";

                    using (OleDbCommand cmdIns = new OleDbCommand(insertSQL, conn))
                    {
                        cmdIns.Parameters.AddWithValue("@SL", soLuongdat);
                        cmdIns.Parameters.AddWithValue("@SLNhan", soLuongnhan);
                        cmdIns.Parameters.AddWithValue("@DG", donGia);
                        cmdIns.Parameters.AddWithValue("@YC", maYC);
                        cmdIns.Parameters.AddWithValue("@HH", maHH);

                        cmdIns.ExecuteNonQuery();
                    }
                }
            }
        }
        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (dgvDanhSach.SelectedRows.Count == 0)
            {
                MessageBox.Show("Không thể xóa phiếu nhập kho này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvDanhSach.SelectedRows[0];
            string maYeuCau = selectedRow.Cells["MaYC"].Value.ToString();


            DialogResult result = MessageBox.Show(
                $"Bạn có chắc muốn xóa phiếu nhập kho '{maYeuCau}' không?",
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

                    string sqlCT = "DELETE FROM ChiTietPhieuNhapKho WHERE SoPNK = ?";
                    using (OleDbCommand cmdCT = new OleDbCommand(sqlCT, conn))
                    {
                        cmdCT.Parameters.AddWithValue("@p1", maYeuCau);
                        cmdCT.ExecuteNonQuery();
                    }


                    string sqlMain = "DELETE FROM PhieuNhapKho WHERE SoPNK = ?";
                    using (OleDbCommand cmdMain = new OleDbCommand(sqlMain, conn))
                    {
                        cmdMain.Parameters.AddWithValue("@p1", maYeuCau);
                        cmdMain.ExecuteNonQuery();
                    }
                }

                dgvDanhSach.Rows.Remove(selectedRow);

                MessageBox.Show($"Đã xóa '{maYeuCau}' thành công.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void BtnXem_Click(object sender, EventArgs e)
        {
            DanhSachPhieuNhapKho ds = new DanhSachPhieuNhapKho();
            ds.ShowDialog();

        }
        private void BtnXuatTra_Click(object sender, EventArgs e)
        {
            PhieuXuatTraNCC ds = new PhieuXuatTraNCC();
            ds.ShowDialog();

        }
        private void BtnTimKiem_Click(object sender, EventArgs e)
        {

            FormTImKiemPhieuNhapKho formTimKiem = new FormTImKiemPhieuNhapKho();

            if (formTimKiem.ShowDialog() == DialogResult.OK)
            {

                DateTime? ngayCTTu = null;
                DateTime? ngayCTDen = null;

                if (formTimKiem.LocTheoNgay)
                {
                    ngayCTTu = formTimKiem.NgayTu;
                    ngayCTDen = formTimKiem.NgayDen;
                }

                string kho = formTimKiem.KhoNhap;
                string mahang = formTimKiem.MaHang;
                LocDanhSach(ngayCTTu, ngayCTDen, kho,mahang);
            }
        }
        private void LocDanhSach(DateTime? ngayCTTu, DateTime? ngayCTDen,
                 string kho, string mahang)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"
                        SELECT p.SoPNK, p.NgayChungTu, p.MaKho, ctk.MaHH
                        FROM PhieuNhapKho p
                        LEFT JOIN ChiTietPhieuNhapKho ctk ON p.SoPNK = ctk.SoPNK
                        WHERE 1 = 1
                        ";

                    OleDbCommand cmd = new OleDbCommand();
                    cmd.Connection = conn;

                    // ===== LỌC THEO NGÀY (NẾU CÓ) =====
                    if (ngayCTTu.HasValue)
                    {
                        query += " AND DateValue(NgayChungTu) >= DateValue(@Tu)";
                        cmd.Parameters.AddWithValue("@Tu", ngayCTTu.Value);
                    }

                    if (ngayCTDen.HasValue)
                    {
                        query += " AND DateValue(NgayChungTu) <= DateValue(@Den)";
                        cmd.Parameters.AddWithValue("@Den", ngayCTDen.Value);
                    }

                    if (!string.IsNullOrWhiteSpace(mahang))
                    {
                        query += " AND MaHH LIKE @MaHH";
                        cmd.Parameters.AddWithValue("@MaHH", "%"+ mahang + "%");
                    }

                    if (!string.IsNullOrWhiteSpace(kho))
                    {
                        query += " AND  MaKho LIKE @kho";
                        cmd.Parameters.AddWithValue("@kho", "%" + kho + "%");
                    }

                    

                    cmd.CommandText = query;

                    DataTable dt = new DataTable();
                    using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show(
                            "Không tìm thấy chứng từ phù hợp!",
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );

                        dgvDanhSach.Rows.Clear();
                        return;
                    }

                    dgvDanhSach.Rows.Clear();

                    foreach (DataRow row in dt.Rows)
                    {
                        dgvDanhSach.Rows.Add(
                            row["SoPNK"].ToString(),
                            row["MaKho"].ToString(),
                            Convert.ToDateTime(row["NgayChungTu"]).ToString("dd/MM/yyyy")
                        );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lọc: " + ex.Message);
                }
            }
        }
    }
}
