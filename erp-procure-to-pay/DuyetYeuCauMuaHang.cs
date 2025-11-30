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
    public partial class DuyetYeuCauMuaHang : Form
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
        private string connectionString = DatabaseConfig.ConnectionString;

        public DuyetYeuCauMuaHang()
        {
            InitializeComponent();
            BuildUI();
        }


        private void BuildUI()
        {
            this.Text = "Duyệt yêu cầu mua hàng";
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
                ColumnHeadersHeight = 65,
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
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                BorderStyle = BorderStyle.FixedSingle,
                EnableHeadersVisualStyles = false
            };
            grpChiTiet.Controls.Add(dgv);

            // --- Thêm 2 nút xử lý ---
            Button btnPheDuyet = new Button
            {
                Text = "✅ Phê duyệt",
                Width = 180,
                Height = 45,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.LightGreen,
                Location = new Point(1500, dgv.Bottom + 10)
            };
            Button btnTuChoi = new Button
            {
                Text = "❌ Từ chối",
                Width = 180,
                Height = 45,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.LightCoral,
                Location = new Point(btnPheDuyet.Right + 10, dgv.Bottom + 10)
            };

            grpChiTiet.Controls.Add(btnPheDuyet);
            grpChiTiet.Controls.Add(btnTuChoi);

            // --- Sự kiện xử lý ---
            btnPheDuyet.Click += (s, e) =>
            {
                string maYC = txtMaChungTu.Text.Trim();

                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["Chon"].Value) == true)
                    {
                        row.Cells["TrangThai"].Value = "Đã phê duyệt";

                        string maHH = row.Cells["MaHang"].Value.ToString();
                        UpdateTrangThaiChiTiet(maYC, maHH, "Đã phê duyệt");
                    }
                }

                // Cập nhật bảng cha
                UpdateTrangThaiYeuCau(maYC);

                // Set textbox "Chờ xử lý" -> "Đã xử lý"
                SetProcessedTextBox(pnlMain);

                MessageBox.Show("Đã phê duyệt mặt hàng.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            btnTuChoi.Click += (s, e) =>
            {

                string maYC = txtMaChungTu.Text.Trim();
                // Kiểm tra có hàng nào được chọn không
                var rowsChon = dgv.Rows.Cast<DataGridViewRow>()
                                 .Where(r => r.Cells["Chon"].Value != null && (bool)r.Cells["Chon"].Value)
                                 .ToList();

                if (rowsChon.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn ít nhất một dòng để từ chối.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Hiển thị hộp thoại nhập lý do từ chối
                using (Form frmLyDo = new Form())
                {
                    frmLyDo.Text = "Nhập lý do từ chối";
                    frmLyDo.StartPosition = FormStartPosition.CenterParent;
                    frmLyDo.Size = new Size(400, 250);
                    frmLyDo.FormBorderStyle = FormBorderStyle.FixedDialog;
                    frmLyDo.MaximizeBox = false;
                    frmLyDo.MinimizeBox = false;

                    Label lbl = new Label
                    {
                        Text = "Vui lòng nhập lý do từ chối:",
                        Location = new Point(20, 20),
                        AutoSize = true
                    };

                    TextBox txtLyDo = new TextBox
                    {
                        Multiline = true,
                        Location = new Point(20, 50),
                        Size = new Size(340, 100)
                    };

                    Button btnOK = new Button
                    {
                        Text = "OK",
                        Location = new Point(170, 170),
                        Width = 90,
                        Height = 30,
                        DialogResult = DialogResult.OK
                    };

                    Button btnCancel = new Button
                    {
                        Text = "Hủy",
                        Location = new Point(280, 170),
                        Width = 90,
                        Height = 30,
                        DialogResult = DialogResult.Cancel
                    };

                    frmLyDo.Controls.Add(lbl);
                    frmLyDo.Controls.Add(txtLyDo);
                    frmLyDo.Controls.Add(btnOK);
                    frmLyDo.Controls.Add(btnCancel);

                    frmLyDo.AcceptButton = btnOK;
                    frmLyDo.CancelButton = btnCancel;

                    // Nếu người dùng nhấn OK
                    if (frmLyDo.ShowDialog() == DialogResult.OK)
                    {
                        string lyDo = txtLyDo.Text.Trim();
                        if (string.IsNullOrEmpty(lyDo))
                        {
                            MessageBox.Show("Bạn phải nhập lý do từ chối.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        // Cập nhật trạng thái cho các dòng được chọn
                        foreach (var row in rowsChon)
                        {
                            row.Cells["TrangThai"].Value = "Từ chối";
                            string maHH = row.Cells["MaHang"].Value.ToString();
                            //updatedb

                            UpdateTrangThaiChiTiet(maYC, maHH, "Từ chối", "Lý do: " + lyDo);

                            if (dgv.Columns.Contains("DienGiai"))
                            {
                                row.Cells["DienGiai"].Value = "Lý do: " + lyDo;
                            }
                        }
                        UpdateTrangThaiYeuCau(maYC);
                        SetProcessedTextBox(pnlMain);
                        MessageBox.Show("Đã từ chối mặt hàng này.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            };



            // Tùy chỉnh header
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 235, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);

            dgv.Columns.Add("MaHang", "Mã hàng*");
            dgv.Columns["MaHang"].Width = 220;

            dgv.Columns.Add("TenHang", "Tên hàng*");
            dgv.Columns["TenHang"].Width = 350;

            dgv.Columns.Add("DVT", "Đvt");
            dgv.Columns["DVT"].Width = 100;

            dgv.Columns.Add("SL", "Số lượng*");
            dgv.Columns["SL"].Width = 100;

            dgv.Columns.Add("DonGia", "Đơn giá");
            dgv.Columns["DonGia"].Width = 250;

            dgv.Columns.Add("TongTien", "Thành tiền");
            dgv.Columns["TongTien"].Width = 240;

            dgv.Columns.Add("DienGiai", "Diễn giải");
            dgv.Columns["DienGiai"].Width = 310;


            // Cột checkbox — đặt Name rõ ràng
            DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn
            {
                Name = "Chon",
                HeaderText = "Chọn",
                Width = 80
            };
            dgv.Columns.Insert(0, chk); // chèn đầu tiên
            
            // Cột trạng thái — gán Name khác so với HeaderText
            DataGridViewTextBoxColumn trangThaiCol = new DataGridViewTextBoxColumn
            {
                Name = "TrangThai",           // <-- dùng Name này để truy xuất
                HeaderText = "Trạng thái",
                Width = 180
            };
            dgv.Columns.Add(trangThaiCol);

            
            // Căn giữa header và dữ liệu số
            dgv.Columns["SL"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["DonGia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["TongTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["DVT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
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
        private void UpdateTrangThaiYeuCau(string maYC)
        {
            string sql = "UPDATE YeuCauMuaHang SET TrangThai = ? WHERE SoYeuCau = ?";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            using (OleDbCommand cmd = new OleDbCommand(sql, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@TrangThai", "Đã xử lý");
                cmd.Parameters.AddWithValue("@SoYeuCau", maYC);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateTrangThaiChiTiet(string maYC, string maHH, string trangThai, string dienGiai = "")
        {
            string sql = @"
        UPDATE ChiTietYeuCauMuaHang
        SET TrangThaiDuyet = ?, DienGiai = ?
        WHERE SoYeuCau = ? AND MaHH = ?";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            using (OleDbCommand cmd = new OleDbCommand(sql, conn))
            {
                conn.Open();
                cmd.Parameters.AddWithValue("@TrangThai", trangThai);
                cmd.Parameters.AddWithValue("@DienGiai", dienGiai);
                cmd.Parameters.AddWithValue("@SoYeuCau", maYC);
                cmd.Parameters.AddWithValue("@MaHH", maHH);
                cmd.ExecuteNonQuery();
            }
        }

        private void SetProcessedTextBox(Control container)
        {
            foreach (Control ctrl in container.Controls)
            {
                if (ctrl is TextBox tb && tb.Text.Equals("Chờ xử lý", StringComparison.OrdinalIgnoreCase))
                {
                    tb.Text = "Đã xử lý";
                }

                // Nếu có container con, đệ quy kiểm tra
                if (ctrl.HasChildren)
                {
                    SetProcessedTextBox(ctrl);
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
                             FROM YeuCauMuaHang
                             WHERE TrangThai = 'Chờ xử lý'";

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
                        Width = textBoxWidth,
                        Format = DateTimePickerFormat.Short
                    };
                    if (label.Contains("Ngày chứng từ"))
                        dtNgayChungTu = (DateTimePicker)inputControl;
                    else if (label.Contains("Ngày cần"))
                        dtNgayCan = (DateTimePicker)inputControl;
                }

                else if (label.Contains("Loại tiền"))
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
            string[] labels1 = { "Người đăng ký*", "Email", "Đơn vị", "Phòng ban", "Bộ phận","Chức danh" };
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
        private void DgvDanhSach_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDanhSach.CurrentRow == null) return;

            var cellValue = dgvDanhSach.CurrentRow.Cells["MaYC"].Value;
            if (cellValue == null) return;

            string maYC = cellValue.ToString();

            LoadThongTinYeuCau(maYC);

            LoadChiTietYeuCau(maYC);
            CapNhatTongHop();
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
                    cmdNN.Parameters.AddWithValue("@p1", manhanvien);

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
    }
}
