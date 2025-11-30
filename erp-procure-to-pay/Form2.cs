using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace TestAccess
{
    public partial class Form2 : Form

    {
        private Label lblTongSL, lblTongTien, lblTonHienTai, lblDuKienNhap;
        private TextBox txtSoChungTu, txtLoaiTien, txtTrangThai, txtNoiDung;
        private DateTimePicker dtNgayChungTu, dtNgayCan;
        private ComboBox cboMaChungTu, cboLoaiTien;
        private DataGridView dgvChiTiet;

        private string connectionString = DatabaseConfig.ConnectionString;
        private DataTable dtHangHoa;
        public Form2()
        {
            InitializeComponent();
            BuildUI();
            LoadHangHoa();
            SetDefaultNguoiDangKy();
        }
        private void LoadHangHoa()
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                string query = "SELECT MaHH, TenHH, DonViTinh, DonGiaMua FROM HangHoa";
                using (OleDbDataAdapter da = new OleDbDataAdapter(query, conn))
                {
                    dtHangHoa = new DataTable();
                    da.Fill(dtHangHoa);
                }
            }
        }
        private void Form2_Load(object sender, EventArgs e)
        {

        }
        private void BuildUI()
        {
            this.Text = "Lập yêu cầu";
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
            // === Gắn sự kiện cho nút "Tìm kiếm" ===
            foreach (Control ctrl in pnlLeft.Controls)
            {
                //if (ctrl is Button btn && btn.Text.Contains("Tìm"))
                //{
                //    btn.Click += BtnTimKiem_Click;
                //}
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
                // Gắn sự kiện cho nút Lưu
                if (text.Contains("Lưu"))
                    btn.Click += BtnLuu_Click;
            }

            // === Panel chứa nội dung chính ===
            Panel pnlMain = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(10,10,10,10)
            };
            this.Controls.Add(pnlMain);
            pnlMain.BringToFront();

            int y = 10;

            // --- Nhóm NGƯỜI ĐĂNG KÝ ---
            GroupBox grpNguoiDK = new GroupBox
            {
                Text = "NGƯỜI ĐĂNG KÝ",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(50, 66, 168),
                Location = new Point(10, y),
                Width = pnlMain.Width - 40,
                Height = 120,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlMain.Controls.Add(grpNguoiDK);

            AddTextBoxRow(grpNguoiDK, new[] { "Người đăng ký*","Email", "Đơn vị", "Phòng ban", "Bộ phận", "Chức danh" }, 10, 30);

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

            AddTextBoxRow(grpThongTin, new[] { "Mã chứng từ*", "Ngày chứng từ*", "Số chứng từ*", "Loại tiền", "Ngày cần*", "Trạng thái" }, 10, 30);


            // Nội dung
            Label lblNoiDung = new Label { Text = "Nội dung:", Location = new Point(4, 125), Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true, };
            txtNoiDung = new TextBox
            {
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
            string[] thongTin = { "Tổng SL", "Tổng tiền"};
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

            DataGridView dgv = new DataGridView
            {
                Location = new Point(10, grpTongHop.Bottom + 10), // đặt phía dưới nhóm tổng hợp
                Width = grpChiTiet.Width - 20,
                Height = grpChiTiet.Height - grpTongHop.Bottom - 25,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackgroundColor = Color.White,
                AllowUserToAddRows = true,
                ColumnHeadersHeight = 35,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                BorderStyle = BorderStyle.FixedSingle,
                EnableHeadersVisualStyles = false
            };
            grpChiTiet.Controls.Add(dgv);
            dgvChiTiet = dgv;
            dgv.CellEndEdit += Dgv_CellEndEdit;
           

            // Tùy chỉnh header
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 235, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);

            // Thêm các cột với độ rộng hợp lý
            dgv.Columns.Add("MaHang", "Mã hàng*");
            dgv.Columns["MaHang"].Width = 220;

            dgv.Columns.Add("TenHang", "Tên hàng*");
            dgv.Columns["TenHang"].Width = 400;

            dgv.Columns.Add("DVT", "Đvt");
            dgv.Columns["DVT"].Width = 100;

            dgv.Columns.Add("SL", "Số lượng*");
            dgv.Columns["SL"].Width = 100;

            dgv.Columns.Add("DonGia", "Đơn giá");
            dgv.Columns["DonGia"].Width = 250;

            dgv.Columns.Add("TongTien", "Thành tiền");
            dgv.Columns["TongTien"].Width = 340;
            dgv.Columns["TongTien"].ReadOnly = true;

            dgv.Columns.Add("DienGiai", "Diễn giải");
            dgv.Columns["DienGiai"].Width = 410;

            // Căn giữa header và dữ liệu số
            dgv.Columns["SL"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["DonGia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["TongTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["DVT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgv.EditingControlShowing += (s, e) =>
            {
                if (dgv.CurrentCell.OwningColumn.Name == "TenHang")
                {
                    DataGridViewTextBoxEditingControl txt = e.Control as DataGridViewTextBoxEditingControl;
                    if (txt != null)
                    {
                        txt.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                        txt.AutoCompleteSource = AutoCompleteSource.CustomSource;

                        AutoCompleteStringCollection collection = new AutoCompleteStringCollection();
                        foreach (DataRow row in dtHangHoa.Rows)
                        {
                            collection.Add(row["TenHH"].ToString());
                        }
                        txt.AutoCompleteCustomSource = collection;

                        txt.Leave -= TxtTenHang_Leave;
                        txt.Leave += TxtTenHang_Leave;
                    }
                }
            };

        }
        private void CapNhatTongHop()
        {
            decimal tongSL = 0;
            decimal tongTien = 0;

            foreach (DataGridViewRow row in dgvChiTiet.Rows)
            {
                if (row.IsNewRow) continue;

                decimal sl = 0, dongia = 0;

                decimal.TryParse(Convert.ToString(row.Cells["SL"].Value), out sl);
                decimal.TryParse(Convert.ToString(row.Cells["DonGia"].Value), out dongia);

                tongSL += sl;
                tongTien += (sl * dongia);
            }

            // Cập nhật lên giao diện
            lblTongSL.Text = tongSL.ToString("N0");
            lblTongTien.Text = tongTien.ToString("N0");
        }

        private void Dgv_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var dgv = sender as DataGridView;
            if (dgv == null) return;

            if (dgv.Columns[e.ColumnIndex].Name == "SL" || dgv.Columns[e.ColumnIndex].Name == "DonGia")
            {
                DataGridViewRow row = dgv.Rows[e.RowIndex];

                // Lấy giá trị số lượng & đơn giá
                decimal soLuong = 0, donGia = 0;
                decimal.TryParse(Convert.ToString(row.Cells["SL"].Value), out soLuong);
                decimal.TryParse(Convert.ToString(row.Cells["DonGia"].Value), out donGia);

                // Tính thành tiền
                decimal thanhTien = soLuong * donGia;

                // Gán vào cột Thành tiền
                row.Cells["TongTien"].Value = thanhTien.ToString("N0");
                CapNhatTongHop();
            }
        }

        private void TxtTenHang_Leave(object sender, EventArgs e)
        {
            var txt = sender as DataGridViewTextBoxEditingControl;
            if (txt == null) return;

            DataGridView dgv = txt.EditingControlDataGridView;
            if (dgv == null) return;

            DataGridViewRow currentRow = dgv.CurrentRow;
            if (currentRow == null) return;

            string tenHang = txt.Text.Trim().Replace("'", "''");
            DataRow[] found = dtHangHoa.Select($"TenHH = '{tenHang}'");
            if (found.Length > 0)
            {
                currentRow.Cells["MaHang"].Value = found[0]["MaHH"].ToString();
                currentRow.Cells["DVT"].Value = found[0]["DonViTinh"].ToString();
                currentRow.Cells["DonGia"].Value = found[0]["DonGiaMua"].ToString();
            }
        }




        private void SetDefaultNguoiDangKy()
        {
            // Lấy group NGƯỜI ĐĂNG KÝ
            GroupBox grp = this.Controls
                .OfType<Panel>()
                .SelectMany(p => p.Controls.OfType<GroupBox>())
                .FirstOrDefault(g => g.Text == "NGƯỜI ĐĂNG KÝ");

            if (grp == null) return;

            // Lấy tất cả textbox trong group
            var textboxes = grp.Controls.OfType<TextBox>().ToList();

            if (textboxes.Count >= 6)
            {
                textboxes[0].Text = "Trần Văn Tuấn";     
                textboxes[1].Text = "tuan@example.com"; 
                textboxes[2].Text = "Công ty ABC";      
                textboxes[3].Text = "Phòng Kế toán";    
                textboxes[4].Text = "Bộ phận Mua hàng"; 
                textboxes[5].Text = "Nhân viên";      
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
                    if (label.Contains("Ngày chứng từ"))
                        dtNgayChungTu = (DateTimePicker)inputControl;
                    else if (label.Contains("Ngày cần"))
                        dtNgayCan = (DateTimePicker)inputControl;
                }
                else if(label.Contains("Mã chứng từ"))
                {
                    ComboBox cbo = new ComboBox
                    {
                        Location = new Point(x, startY + lbl.Height + spacingY),
                        Width = textBoxWidth,
                        DropDownStyle = ComboBoxStyle.DropDownList,
                        DrawMode = DrawMode.OwnerDrawFixed // 👈 Quan trọng
                    };

                    parent.Controls.Add(lbl);
                    parent.Controls.Add(cbo);

                    LoadMaChungTu(cbo);
                    cboMaChungTu = cbo;

                    cbo.DrawItem += (s, e) =>
                    {
                        if (e.Index < 0) return;
                        ComboBox combo = (ComboBox)s;
                        DataRowView row = (DataRowView)combo.Items[e.Index];

                        string ma = row["MaCT"].ToString();
                        string ten = row["TenNghiepVu"].ToString();

                        e.DrawBackground();
                        using (Brush brush = new SolidBrush(e.ForeColor))
                        {
                            e.Graphics.DrawString($"{ma} - {ten}", e.Font, brush, e.Bounds);
                        }
                        e.DrawFocusRectangle();
                    };

                    // --- chỉ hiển thị mã sau khi chọn ---
                    cbo.SelectionChangeCommitted += (s, e) =>
                    {
                        if (cbo.SelectedValue != null)
                        {
                            cbo.Text = cbo.SelectedValue.ToString(); // chỉ hiển thị mã
                        }
                    };

                    x += textBoxWidth + spacingX;
                    continue;
                }
                else if (label.Contains("Loại tiền") || label.Contains("Người lập"))
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
                    // Gán biến tương ứng
                    if (label.Contains("Số chứng từ"))
                        txtSoChungTu = (TextBox)inputControl;
                    else if (label.Contains("Loại tiền"))
                        txtLoaiTien = (TextBox)inputControl;
                    else if (label.Contains("Trạng thái"))
                        txtTrangThai = (TextBox)inputControl;
                }

                parent.Controls.Add(inputControl);

                // Di chuyển sang cột kế tiếp
                x += textBoxWidth + spacingX;
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

        bool _isSelecting = false;
        private void LoadMaChungTu(ComboBox cbo)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT MaCT, TenNghiepVu FROM NghiepVu WHERE LoaiNghiepVu='Yêu cầu mua hàng'";
                    using (OleDbDataAdapter da = new OleDbDataAdapter(query, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);


                        cbo.DataSource = dt;
                        cbo.DisplayMember = "TenNghiepVu";
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

        // Xử lý khi chọn item
        private void Cbo_SelectedIndexChanged(object ? sender, EventArgs e)
        {
            if (_isSelecting) return;

            ComboBox cbo = sender as ComboBox;
            if (cbo != null && cbo.SelectedValue != null && cbo.SelectedIndex >= 0)
            {
                _isSelecting = true;
                cbo.Text = cbo.SelectedValue.ToString();  
                _isSelecting = false;
            }
        }

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            string soYeuCau = $"{cboMaChungTu.SelectedValue}{txtSoChungTu.Text.Trim()}";
            string maNgoaiTe = cboLoaiTien.Text.Trim();
            DateTime ngayChungTu = dtNgayChungTu.Value;
            DateTime ngayCan = dtNgayCan.Value;
            string trangThai = "Chờ xử lý";
            string maNhanVienTao = "NV011";
            string noidung = txtNoiDung.Text.Trim();
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();
                OleDbTransaction tran = conn.BeginTransaction();

                try
                {
                    string queryYCMH = @"INSERT INTO YeuCauMuaHang 
                (SoYeuCau, NgayChungTu, NgayCan, TrangThai, MaNgoaiTe, MaNhanVienTao)
                VALUES (@SoYeuCau, @NgayChungTu, @NgayCan, @TrangThai, @MaNgoaiTe, @MaNhanVienTao)";
                    using (OleDbCommand cmd = new OleDbCommand(queryYCMH, conn, tran))
                    {
                        cmd.Parameters.Add("@SoYeuCau", OleDbType.VarChar).Value = soYeuCau;
                        cmd.Parameters.Add("@NgayChungTu", OleDbType.Date).Value = ngayChungTu;
                        cmd.Parameters.Add("@NgayCan", OleDbType.Date).Value = ngayCan;
                        cmd.Parameters.Add("@TrangThai", OleDbType.VarChar).Value = trangThai;
                        cmd.Parameters.Add("@MaNgoaiTe", OleDbType.VarChar).Value = maNgoaiTe;
                        cmd.Parameters.Add("@MaNhanVienTao", OleDbType.VarChar).Value = maNhanVienTao;
                        cmd.ExecuteNonQuery();
                    }

                    // --- Lưu chi tiết ---
                    foreach (DataGridViewRow row in dgvChiTiet.Rows)
                    {
                        if (row.IsNewRow) continue;

                        string maHH = Convert.ToString(row.Cells["MaHang"].Value);
                        if (string.IsNullOrEmpty(maHH)) continue;

                        double soLuongDat = 0;
                        double.TryParse(Convert.ToString(row.Cells["SL"].Value), out soLuongDat);

                        double donGia = 0;
                        double.TryParse(Convert.ToString(row.Cells["DonGia"].Value), out donGia);

                        string dienGiai = Convert.ToString(row.Cells["DienGiai"].Value) ?? "";
                        string trangThaiDuyet = "Chờ xử lý";

                        string queryCT = @"INSERT INTO ChiTietYeuCauMuaHang 
                    (SoYeuCau, MaHH, SoLuongDat, DonGia, DienGiai, TrangThaiDuyet)
                    VALUES (@SoYeuCau, @MaHH, @SoLuongDat, @DonGia, @DienGiai, @TrangThaiDuyet)";
                        using (OleDbCommand cmdCT = new OleDbCommand(queryCT, conn, tran))
                        {
                            cmdCT.Parameters.Add("@SoYeuCau", OleDbType.VarChar).Value = soYeuCau;
                            cmdCT.Parameters.Add("@MaHH", OleDbType.VarChar).Value = maHH;
                            cmdCT.Parameters.Add("@SoLuongDat", OleDbType.Double).Value = soLuongDat;
                            cmdCT.Parameters.Add("@DonGia", OleDbType.Currency).Value = donGia;
                            cmdCT.Parameters.Add("@DienGiai", OleDbType.LongVarChar).Value = dienGiai;
                            cmdCT.Parameters.Add("@TrangThaiDuyet", OleDbType.VarChar).Value = trangThaiDuyet;
                            cmdCT.ExecuteNonQuery();
                        }



                    }

                    tran.Commit();
                    MessageBox.Show("Đã lưu yêu cầu mua hàng thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message);
                }
            }
        }



    }
}
