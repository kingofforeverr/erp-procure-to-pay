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
    public partial class DanhSachYeuCauMuaHang : Form

    {
        private Label lblTongSL, lblTongTien, lblTonHienTai, lblDuKienNhap;

        private bool isDirty = false;
        private TextBox txtSoChungTu, txtTrangThai, txtNoiDung, txtMaChungTu;
        private DateTimePicker dtNgayChungTu, dtNgayCan;
        private ComboBox cboLoaiTien;
        private DataGridView dgvChiTiet;
        private DataGridView dgvDanhSach; 
        private DataGridView dgv;
        private TextBox txtNguoiDangKy, txtEmail, txtDonVi, txtPhongBan, txtBoPhan, txtChucDanh;
        private FlowLayoutPanel pnlMain, pnlLeft;
        private string connectionString = DatabaseConfig.ConnectionString;

        public DanhSachYeuCauMuaHang()
        {
            InitializeComponent();
            BuildUI();
            AttachControlChangeEvents();
        }
        private void BuildUI()
        {
            this.Text = "Danh sách yêu cầu mua hàng";
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
            pnlLeft = new FlowLayoutPanel
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
                if (text.Contains("Thêm"))
                    btn.Click += BtnThem_Click;
            }

            // === Gắn sự kiện cho nút "Tìm kiếm" ===
            foreach (Control ctrl in pnlLeft.Controls)
            {
                if (ctrl is Button btn && btn.Text.Contains("Tìm"))
                {
                    btn.Click += BtnTimKiem_Click;
                }
            }
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
                    btn.Click += BtnLuu_Click;
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

            pnlMain = new FlowLayoutPanel
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
            

            // === DANH SÁCH YÊU CẦU (thay cho thanh công cụ) ===
            GroupBox grpDanhSach = new GroupBox
            {
                Text = "DANH SÁCH YÊU CẦU MUA HÀNG",
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
            dgvDanhSach.Columns.Add("MaYC", "Mã yêu cầu");
            dgvDanhSach.Columns.Add("NguoiDK", "Người đăng ký");
            dgvDanhSach.Columns.Add("NgayCT", "Ngày chứng từ");
            dgvDanhSach.Columns.Add("TrangThai", "Trạng thái");

            LoadDanhSachYeuCau(dgvDanhSach);
            // Gắn sự kiện chọn dòng
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
                Text = "Chi tiết yêu cầu mua hàng",
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
                Text = "Thông tin",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 66, 168),
                //Location = new Point(10, y),
                Width = pnlMain.Width - 40,
                Height = 120,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlMain.Controls.Add(grpNguoiDK);
            // === Gọi hàm dựng layout chi tiết ===
            TaoFormThongTin(grpNguoiDK);

            y += grpNguoiDK.Height + 10;

            // --- Nhóm THÔNG TIN ---
            GroupBox grpThongTin = new GroupBox
            {
                Text = "THÔNG TIN",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 66, 168),
                Location = new Point(10, y),
                Width = pnlMain.Width - 40,
                Height = 250,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlMain.Controls.Add(grpThongTin);

            AddTextBoxRow(grpThongTin, new[] { "Số yêu cầu", "Ngày chứng từ*", "Loại tiền", "Ngày cần*", "Trạng thái" }, 10, 30);

            // Nội dung
            Label lblNoiDung = new Label { Text = "Nội dung:", Location = new Point(4, 125), Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true, };
            txtNoiDung = new TextBox
            {
                Name = "txtNoiDung",
                Multiline = true,
                Location = new Point(7, 160),
                Width = grpThongTin.Width - 40,
                Height = 80,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            grpThongTin.Controls.Add(lblNoiDung);
            grpThongTin.Controls.Add(txtNoiDung);

            y += grpThongTin.Height + 10;

            // --- Nhóm CHI TIẾT MẶT HÀNG ---
            GroupBox grpChiTiet = new GroupBox
            {
                Text = "CHI TIẾT MẶT HÀNG / DỊCH VỤ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 66, 168),
                Location = new Point(10, y),
                Width = pnlMain.Width - 40,
                Height = 400,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            pnlMain.Controls.Add(grpChiTiet);

            // === Nhóm nhỏ: Tổng hợp (ở góc trên bên phải) ===
            GroupBox grpTongHop = new GroupBox
            {
                Text = "TỔNG HỢP",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue,
                Width = 600,
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
                

                Location = new Point(10, grpTongHop.Bottom + 10), 
                Width = grpChiTiet.Width - 20,
                Height = 200,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackgroundColor = Color.White,
                AllowUserToAddRows = true,
                ColumnHeadersHeight = 35,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                EnableHeadersVisualStyles = false,
                //ReadOnly = true,
            };
            grpChiTiet.Controls.Add(dgv);

 
            // Tùy chỉnh header
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 235, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);

            // Thêm các cột với độ rộng hợp lý
            dgv.Columns.Add("MaHang", "Mã hàng*");
            dgv.Columns.Add("TenHang", "Tên hàng*");
            dgv.Columns.Add("DVT", "Đvt");
            dgv.Columns.Add("SL", "Số lượng*");
            dgv.Columns.Add("DonGia", "Đơn giá");
            dgv.Columns.Add("TongTien", "Thành tiền");
            dgv.Columns.Add("DienGiai", "Diễn giải");

            dgv.CellEndEdit += Dgv_CellEndEdit;
            dgv.DataError += dgv_DataError;


            // Cột trạng thái — gán Name khác so với HeaderText
            DataGridViewTextBoxColumn trangThaiCol = new DataGridViewTextBoxColumn
            {
                Name = "TrangThai",           
                HeaderText = "Trạng thái",
                Width = 180
            };
            dgv.Columns.Add(trangThaiCol);

           
            dgv.Columns["SL"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["DonGia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["TongTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["DVT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Gọi hàm disable toàn bộ input khi khởi tạo
            ToggleInputs(pnlMain, false);

            // Gắn sự kiện cho nút Xem
            foreach (Control ctrl in pnlLeft.Controls)
            {
                if (ctrl is Button btn && btn.Text.Contains("Chỉnh sửa"))
                {
                    btn.Click += (s, e) =>
                    {
                        if (IsAnyTextBoxProcessed(pnlMain))
                        {
                            MessageBox.Show("Không thể chỉnh sửa chứng từ này",
                                            "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        ToggleInputs(pnlMain, true);

                        DisableMaChungTu(pnlMain);
                    };
                    
                }
            }

        }
        private bool IsAnyTextBoxProcessed(Control container)
        {
            foreach (Control ctrl in container.Controls)
            {
                if (ctrl is TextBox tb && tb.Text.Equals("Đã xử lý", StringComparison.OrdinalIgnoreCase))
                {
                    return true; 
                }

                if (ctrl.HasChildren)
                {
                    if (IsAnyTextBoxProcessed(ctrl))
                        return true;
                }
            }
            return false; 
        }
        private decimal SafeDecimal(object v)
        {
            if (v == null || v == DBNull.Value) return 0;

            string s = v.ToString()
                         .Replace(",", "")
                         .Replace(".", "")
                         .Trim();

            if (decimal.TryParse(s, out decimal result))
                return result;

            return 0;
        }
        private void dgvDanhSachCT_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;  
        }
        private void Dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var row = dgv.Rows[e.RowIndex];

            if (row.IsNewRow) return;

            decimal sl = SafeDecimal(row.Cells["SL"].Value);
            decimal donGia = SafeDecimal(row.Cells["DonGia"].Value);

            // Tính toán
            decimal thanhTien = sl * donGia;

            // Gán lại giá trị
            row.Cells["TongTien"].Value = thanhTien;

            // Định dạng đẹp lại
            row.Cells["DonGia"].Value = donGia.ToString("N0");
            row.Cells["TongTien"].Value = thanhTien.ToString("N0");

            CapNhatTongHop();
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
                tongTien += (sl * dongia) ;
            }

            // Cập nhật lên giao diện
            lblTongSL.Text = tongSL.ToString("N0");
            lblTongTien.Text = tongTien.ToString("N0");
        }
        
        private void AddTextBoxRow(Control parent, string[] labels, int startX, int startY)
        {
            int x = startX;
            int labelWidth = 150;
            int textBoxWidth = 260;
            int controlHeight = 75;
            int spacingX = 55;   // khoảng cách giữa các cột
            int spacingY = 5;    // khoảng cách giữa label và textbox

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

                // Nếu có chữ "Ngày" thì dùng DateTimePicker
                Control inputControl;
                if (label.Contains("Ngày"))
                {
                    inputControl = new DateTimePicker
                    {
                        Location = new Point(x, startY + lbl.Height + spacingY),
                        Width = textBoxWidth ,
                        Format = DateTimePickerFormat.Short
                    };
                    if (label.Contains("Ngày chứng từ"))
                        dtNgayChungTu = (DateTimePicker)inputControl;
                    else if (label.Contains("Ngày cần"))
                        dtNgayCan = (DateTimePicker)inputControl;
                }
                
                else if (label.Contains("Loại tiền") )
                {
                    cboLoaiTien = new ComboBox
                    {
                        Location = new Point(x, startY + lbl.Height + spacingY),
                        Width = textBoxWidth,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    LoadNgoaiTe((ComboBox)cboLoaiTien);
                    inputControl = cboLoaiTien;
                }
                else
                {
                    inputControl = new TextBox
                    {
                        Location = new Point(x, startY + lbl.Height + spacingY),
                        Width = textBoxWidth,
                        Height = controlHeight
                    };
                    if (label.Contains("Số yêu cầu"))
                        txtMaChungTu = (TextBox)inputControl;
                    else if (label.Contains("Số chứng từ"))
                        txtSoChungTu = (TextBox)inputControl;
                    else if (label.Contains("Trạng thái"))
                        txtTrangThai = (TextBox)inputControl;
                    
                }

                parent.Controls.Add(inputControl);

                // Di chuyển sang cột kế tiếp
                x += textBoxWidth + spacingX;
            }
        }
        private void LoadChiTietYeuCau(string maYC)
        {
            string sql = @"
                    SELECT 
                        ct.MaHH,
                        hh.TenHH,
                        hh.DonViTinh,
                        ct.SoLuongDat,
                        ct.DonGia,
                        (ct.SoLuongDat * ct.DonGia) AS ThanhTien,
                        ct.DienGiai,
                        ct.TrangThaiDuyet
                    FROM ChiTietYeuCauMuaHang ct
                    LEFT JOIN HangHoa hh ON ct.MaHH = hh.MaHH
                    WHERE ct.SoYeuCau = ?
    ";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            using (OleDbDataAdapter da = new OleDbDataAdapter(sql, conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@MaYC", maYC);

                DataTable dt = new DataTable();
                da.Fill(dt);
                dgv.Columns["MaHang"].DataPropertyName = "MaHH";
                dgv.Columns["TenHang"].DataPropertyName = "TenHH";
                dgv.Columns["DVT"].DataPropertyName = "DonViTinh";
                dgv.Columns["SL"].DataPropertyName = "SoLuongDat";
                dgv.Columns["DonGia"].DataPropertyName = "DonGia";
                dgv.Columns["TongTien"].DataPropertyName = "ThanhTien";
                dgv.Columns["DienGiai"].DataPropertyName = "DienGiai";
                dgv.Columns["TrangThai"].DataPropertyName = "TrangThaiDuyet";
                dgv.DataSource = dt;
            }
            

        }
        private void dgv_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;  // không cho popup lỗi
        }
        private void LoadThongTinYeuCau(string maYC)
        {
            string sql = @"SELECT * FROM YeuCauMuaHang WHERE SoYeuCau = @MaYC";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            using (OleDbCommand cmd = new OleDbCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MaYC", maYC);

                conn.Open();
                string manhanvien = "";
                using (OleDbDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                    

                        manhanvien = rd["MaNhanVienTao"].ToString();
                        txtMaChungTu.Text = rd["SoYeuCau"].ToString();

                        if (rd["NgayChungTu"] != DBNull.Value)
                            dtNgayChungTu.Value = Convert.ToDateTime(rd["NgayChungTu"]);

                        cboLoaiTien.Text = rd["MaNgoaiTe"].ToString();

                        if (rd["NgayCan"] != DBNull.Value)
                            dtNgayCan.Value = Convert.ToDateTime(rd["NgayCan"]);

                        txtNoiDung.Text = rd["NoiDung"].ToString();
                        txtTrangThai.Text = rd["TrangThai"].ToString();
                    }
                }
                if (!string.IsNullOrEmpty(manhanvien))
                {
                    string sqlNN = @"SELECT *
                             FROM NhanVien 
                             WHERE MaNhanVien = ?";

                    OleDbCommand cmdNN = new OleDbCommand(sqlNN, conn);
                    cmdNN.Parameters.AddWithValue("@p1",manhanvien);

                    using (OleDbDataReader rdNN = cmdNN.ExecuteReader())
                    {
                        if (rdNN.Read())
                        {
                            //txtMaNhanVienNop.Text = maNhanVienNop;
                            txtNguoiDangKy.Text = rdNN["HoTen"].ToString();
                            txtEmail.Text = rdNN["Email"].ToString();
                            txtDonVi.Text = rdNN["DonVi"].ToString();
                            txtPhongBan.Text = rdNN["PhongBan"].ToString();
                            txtBoPhan.Text = rdNN["BoPhan"].ToString();
                            txtChucDanh.Text = rdNN["ChucDanh"].ToString();

                        }
                    }
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


        private void LoadMaChungTu(ComboBox cbo)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT MaCT, TenCT FROM MaChungTu";
                    using (OleDbDataAdapter da = new OleDbDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);


                        cbo.DataSource = dt;
                        cbo.DisplayMember = "TenCT";
                        cbo.ValueMember = "MaCT";
                        cbo.SelectedIndex = -1;

                        

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
                }
            }
        }

        private void LoadDanhSachYeuCau(DataGridView dgv)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"SELECT SoYeuCau, NgayChungTu,MaNhanVienTao, TrangThai
                             FROM YeuCauMuaHang";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgv.Rows.Clear();

                        foreach (DataRow row in dt.Rows)
                        {
                            dgv.Rows.Add(
                                row["SoYeuCau"].ToString(),
                                Convert.ToDateTime(row["NgayChungTu"]).ToString("dd/MM/yyyy"),
                                row["MaNhanVienTao"].ToString(),
                                row["TrangThai"].ToString()
                            );
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi tải danh sách yêu cầu: " + ex.Message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            

        }

        private void TaoFormThongTin(GroupBox groupBox)
        {
            groupBox.Text = "Người đăng ký";
            groupBox.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            groupBox.ForeColor = Color.FromArgb(50, 66, 168);
            groupBox.Padding = new Padding(10, 20, 10, 10);
            groupBox.AutoSize = true;

            int startX = 15;
            int startY = 70;
            int labelWidth = 110;
            int textBoxWidth = 286;
            int controlHeight = 32;
            int spacingX = 30;
            int spacingY = 20;
            int rowSpacing = 25;



            // Hàng 1: Mã CT, Ngày PO, Số PO, Loại tiền, Số hợp đồng, Ngày hợp đồng, Ngày đến hạn, Người lập
            string[] labels1 = { "Người đăng ký*", "Email", "Đơn vị", "Phòng ban", "Bộ phận", "Chức danh" };
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
                        Width = textBoxWidth
                    };
                }
                ;
                groupBox.Controls.Add(input);
                if (label.Contains("Người đăng ký"))
                    txtNguoiDangKy = (TextBox)input;
                else if (label.Contains("Email"))
                    txtEmail = (TextBox)input;
                else if (label.Contains("Đơn vị"))
                    txtDonVi = (TextBox)input;
                else if (label.Contains("Phòng ban"))
                    txtPhongBan = (TextBox)input;
                else if (label.Contains("Bộ phận"))
                    txtBoPhan = (TextBox)input;
                else if (label.Contains("Chức danh"))
                    txtChucDanh = (TextBox)input;
                x += textBoxWidth + 30;
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


        // Hàm bật/tắt tất cả input control trong form
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

        private void DisableMaChungTu(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is ComboBox cbo && cbo.Name.Contains("MaChungTu", StringComparison.OrdinalIgnoreCase))
                {
                    cbo.Enabled = false;
                }

                if (ctrl.HasChildren)
                {
                    DisableMaChungTu(ctrl);
                }
            }
        }
        
        private void DgvDanhSach_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDanhSach.CurrentRow == null) return;

            var cellValue = dgvDanhSach.CurrentRow.Cells["MaYC"].Value;
            if (cellValue == null) return;

            string maYC = cellValue.ToString();

            LoadThongTinYeuCau(maYC);

            LoadChiTietYeuCau(maYC);

            CapNhatTongHop();

            ToggleInputs(pnlMain, false);
            
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            if (!isDirty)
            {
                MessageBox.Show(
                        "Không có thay đổi nào để lưu.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                return;
            }

            //string soYeuCau = $"{cboMaChungTu.SelectedValue}{txtSoChungTu.Text.Trim()}";
            string maNgoaiTe = cboLoaiTien.Text.Trim();
            string maYC = txtMaChungTu.Text.Trim();
            DateTime ngayChungTu = dtNgayChungTu.Value;
            string noidung = txtNoiDung.Text.Trim();
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                // UPDATE bảng chính
                string sqlUpdate = @"
            UPDATE YeuCauMuaHang
            SET NgayCan = ?, 
                MaNgoaiTe = ?, 
                NoiDung = ?
            WHERE SoYeuCau = ?
        ";

                using (OleDbCommand cmd = new OleDbCommand(sqlUpdate, conn))
                {
                    cmd.Parameters.Add("@NgayChungTu", OleDbType.Date).Value = ngayChungTu;
                    cmd.Parameters.Add("@MaNgoaiTe", OleDbType.VarChar).Value = maNgoaiTe;
                    cmd.Parameters.AddWithValue("@DienGiai", OleDbType.VarChar).Value = noidung;
                    cmd.Parameters.AddWithValue("@SoYeuCau", OleDbType.VarChar).Value = maYC;

                    cmd.ExecuteNonQuery();
                }

                // UPDATE chi tiết (dgv)
                UpdateChiTiet(conn, maYC);
            }

            isDirty = false; // đã lưu xong
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
                decimal soLuong = Convert.ToDecimal(row.Cells["SL"].Value ?? 0);
                decimal donGia = Convert.ToDecimal(row.Cells["DonGia"].Value ?? 0);
                string dienGiai = row.Cells["DienGiai"].Value?.ToString() ?? "";

                // 1) Kiểm tra tồn tại
                string checkSQL = @"SELECT COUNT(*) FROM ChiTietYeuCauMuaHang 
                            WHERE SoYeuCau = ? AND MaHH = ?";

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
                UPDATE ChiTietYeuCauMuaHang
                SET SoLuongDat = ?, DonGia = ?, DienGiai = ?
                WHERE SoYeuCau = ? AND MaHH = ?
            ";

                    using (OleDbCommand cmdUp = new OleDbCommand(updateSQL, conn))
                    {
                        cmdUp.Parameters.AddWithValue("@SL", soLuong);
                        cmdUp.Parameters.AddWithValue("@DG", donGia);
                        cmdUp.Parameters.AddWithValue("@DGiai", dienGiai);
                        cmdUp.Parameters.AddWithValue("@YC", maYC);
                        cmdUp.Parameters.AddWithValue("@HH", maHH);

                        cmdUp.ExecuteNonQuery();
                    }
                }
                else
                {
                    // 3) INSERT
                    string insertSQL = @"
                INSERT INTO ChiTietYeuCauMuaHang
                (SoYeuCau, MaHH, SoLuongDat, DonGia, DienGiai)
                VALUES (?, ?, ?, ?, ?)
            ";

                    using (OleDbCommand cmdIns = new OleDbCommand(insertSQL, conn))
                    {
                        cmdIns.Parameters.AddWithValue("@YC", maYC);
                        cmdIns.Parameters.AddWithValue("@HH", maHH);
                        cmdIns.Parameters.AddWithValue("@SL", soLuong);
                        cmdIns.Parameters.AddWithValue("@DG", donGia);
                        cmdIns.Parameters.AddWithValue("@DGiai", dienGiai);

                        cmdIns.ExecuteNonQuery();
                    }
                }
            }
        }



        private void BtnTimKiem_Click(object sender, EventArgs e)
        {
            
            FormTimKiemYeuCauMuaHang formTimKiem = new FormTimKiemYeuCauMuaHang();

            if (formTimKiem.ShowDialog() == DialogResult.OK)
            {

                DateTime? ngayCTTu = null;
                DateTime? ngayCTDen = null;

                // Chỉ lấy ngày khi checkbox được tick
                if (formTimKiem.LocTheoNgay)
                {
                    ngayCTTu = formTimKiem.NgayTu;
                    ngayCTDen = formTimKiem.NgayDen;
                }

                string trangThai = formTimKiem.TrangThai;
                string maChungTu = formTimKiem.MaChungTu;

                LocDanhSach(ngayCTTu, ngayCTDen, trangThai, maChungTu);
            }
        }
        private void LocDanhSach(DateTime? ngayCTTu, DateTime? ngayCTDen,
                 string trangThai, string maChungTu)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"
                SELECT SoYeuCau, NgayChungTu, MaNhanVienTao, TrangThai
                FROM YeuCauMuaHang
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

                    // ===== TRẠNG THÁI =====
                    if (!string.IsNullOrWhiteSpace(trangThai))
                    {
                        query += " AND TrangThai = @TrangThai";
                        cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                    }

                    // ===== MÃ CHỨNG TỪ =====
                    if (!string.IsNullOrWhiteSpace(maChungTu))
                    {
                        query += " AND SoYeuCau LIKE @MaCT";
                        cmd.Parameters.AddWithValue("@MaCT", "%" + maChungTu + "%");
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
                            "Không tìm thấy yêu cầu phù hợp!",
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
                            row["SoYeuCau"].ToString(),
                            Convert.ToDateTime(row["NgayChungTu"]).ToString("dd/MM/yyyy"),
                            row["MaNhanVienTao"].ToString(),
                            row["TrangThai"].ToString()
                        );
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi lọc: " + ex.Message);
                }
            }
        }
        private void BtnThem_Click(object sender, EventArgs e)
        {
            Form2 ds = new Form2();
            ds.ShowDialog();
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (dgvDanhSach.SelectedRows.Count == 0)
            {
                MessageBox.Show("Không thể xóa yêu cầu!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvDanhSach.SelectedRows[0];
            string trangThai = selectedRow.Cells["TrangThai"].Value.ToString();
            string maYeuCau = selectedRow.Cells["MaYC"].Value.ToString();

            if (trangThai == "Đã xử lý")
            {
                MessageBox.Show("Không thể xóa yêu cầu đã xử lý!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Bạn có chắc muốn xóa yêu cầu '{maYeuCau}' không?",
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

                    // 1) Xóa chi tiết trước
                    string sqlCT = "DELETE FROM ChiTietYeuCauMuaHang WHERE SoYeuCau = ?";
                    using (OleDbCommand cmdCT = new OleDbCommand(sqlCT, conn))
                    {
                        cmdCT.Parameters.AddWithValue("@p1", maYeuCau);
                        cmdCT.ExecuteNonQuery();
                    }

                    // 2) Xóa bảng chính
                    string sqlMain = "DELETE FROM YeuCauMuaHang WHERE SoYeuCau = ?";
                    using (OleDbCommand cmdMain = new OleDbCommand(sqlMain, conn))
                    {
                        cmdMain.Parameters.AddWithValue("@p1", maYeuCau);
                        cmdMain.ExecuteNonQuery();
                    }
                }

                // 3) Xóa khỏi lưới
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

    }

}
