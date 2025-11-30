using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestAccess
{
    public partial class DanhSachDonMuaHang : Form

    {
        private bool isDirty = false;
        private DataGridView dgvDanhSach;
        private Label lblTongSL, lblTongTien, lblTonHienTai, lblDuKienNhap;
        private string connectionString = DatabaseConfig.ConnectionString;
        TextBox txtSoHopDong, txtMaNCC, txtTenNCC, txtDiaChi, txtSoChungTu, txtPhuongThucGiaoHang, txtTrangThai, txtDot;
        TextBox txtMaLienHe, txtNguoiLienHe, txtDienThoaiLienHe, txtDienThoai, txtNoiDung;
        TextBox txtNguoiLap;
        ComboBox cboHinhThucThanhToan, cboMaChungTu, cboLoaiTien, cboPhuongThucThanhToan;
        DateTimePicker dtNgayHopDong, dtNgayDenHan, dtNgayChungTu, dtThoiGianGiaoHang;
        TextBox txtDienThoaiGiao, txtDiaDiemGiao, txtTenDiaDiemGiao;
        DataGridView dgv;
        public DanhSachDonMuaHang()
        {
            InitializeComponent();
            BuildUI();
            AttachControlChangeEvents();
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
                    //btn.Click += BtnIn_Click;
                }
                if (text.Contains("Xoá"))
                    btn.Click += BtnXoa_Click;
                if (text.Contains("Tìm"))
                    btn.Click += BtnTimKiem_Click;
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
            GroupBox grpDanhSach = new GroupBox
            {
                Text = "DANH SÁCH ĐƠN MUA HÀNG",
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
            dgvDanhSach.Columns.Add("MaYC", "Mã đơn hàng");
            dgvDanhSach.Columns.Add("NguoiDK", "Người đăng ký");
            dgvDanhSach.Columns.Add("NgayCT", "Ngày chứng từ");
            dgvDanhSach.Columns.Add("TrangThai", "Trạng thái");

            LoadDanhSachDonMuaHang(dgvDanhSach);
            dgvDanhSach.SelectionChanged += DgvDanhSach_SelectionChanged;

            // === TẠO DỮ LIỆU MẪU CHO DANH SÁCH ===

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
                Text = "Chi tiết đơn mua hàng",
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
                decimal.TryParse(Convert.ToString(row.Cells["TienVat"].Value), out tienvat);

                tongSL += sl;
                tongTien += (sl * dongia) + tienvat;
            }

            // Cập nhật lên giao diện
            lblTongSL.Text = tongSL.ToString("N0");
            lblTongTien.Text = tongTien.ToString("N0");
        }
        //private void LoadMaChungTu(ComboBox input)
        //{
        //    using (OleDbConnection conn = new OleDbConnection(connectionString))
        //    {
        //        try
        //        {
        //            conn.Open();
        //            string query = "SELECT MaCT, TenCT FROM MaChungTuMuaHang";
        //            using (OleDbDataAdapter da = new OleDbDataAdapter(query, conn))
        //            {
        //                DataTable dt = new DataTable();
        //                da.Fill(dt);
        //                input.DataSource = dt;
        //                input.DisplayMember = "MaCT";
        //                input.ValueMember = "MaCT";
        //                input.SelectedIndex = -1;

        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
        //        }
        //    }
        //}

        private void LoadDanhSachDonMuaHang(DataGridView dgv)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"SELECT SoDonDatHang, NgayChungTu,MaNhanVienLap, TrangThai
                             FROM DonMuaHang";

                    using (OleDbCommand cmd = new OleDbCommand(query, conn))
                    using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgv.Rows.Clear();

                        foreach (DataRow row in dt.Rows)
                        {
                            dgv.Rows.Add(
                                row["SoDonDatHang"].ToString(),
                                Convert.ToDateTime(row["NgayChungTu"]).ToString("dd/MM/yyyy"),
                                row["MaNhanVienLap"].ToString(),
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
        private void DgvDanhSach_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDanhSach.CurrentRow == null) return;

            var cellValue = dgvDanhSach.CurrentRow.Cells["MaYC"].Value;
            if (cellValue == null) return;

            string maHD = cellValue.ToString();

            LoadThongTinDonMuaHang(maHD);

            LoadChiTietDonMuaHang(maHD);

            CapNhatTongHop();
            //LoadChiTietThanhToan(maHD);
        }

        private void LoadThongTinDonMuaHang(string maHD)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string sqlHD = @"SELECT * FROM DonMuaHang WHERE SoDonDatHang = @SoDonDatHang";
                OleDbCommand cmdHD = new OleDbCommand(sqlHD, conn);
                cmdHD.Parameters.AddWithValue("@SoDonDatHang", maHD);

                string maNCC = "";
                string manlh = "";
                using (OleDbDataReader rd = cmdHD.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        txtSoChungTu.Text = rd["SoDonDatHang"].ToString();

                        if (rd["NgayChungTu"] != DBNull.Value)
                            dtNgayChungTu.Value = Convert.ToDateTime(rd["NgayChungTu"]);

                        cboLoaiTien.Text = rd["MaNgoaiTe"].ToString();

                        txtSoHopDong.Text = rd["MaHopDong"].ToString();

                        txtNoiDung.Text = rd["NoiDung"].ToString();
                        txtTrangThai.Text = rd["TrangThai"].ToString();


                        maNCC = rd["MaNCC"].ToString();

                        txtNguoiLap.Text = rd["MaNhanVienLap"].ToString();

                        manlh = rd["MaNLH"].ToString();
                        txtMaLienHe.Text = manlh;
                        cboHinhThucThanhToan.Text = rd["HinhThucThanhToan"].ToString();
                        cboPhuongThucThanhToan.Text = rd["PhuongThucThanhToan"].ToString();
                        txtTenDiaDiemGiao.Text = rd["TenDiaDiemGiao"].ToString();
                        txtDiaDiemGiao.Text = rd["DiaChiGiao"].ToString();
                        txtDienThoaiGiao.Text = rd["DienThoaiGiao"].ToString();
                        txtDot.Text = rd["Dot"].ToString();
                    }
                }
                if (!string.IsNullOrEmpty(manlh))
                {
                    string sqlNLH = @"SELECT * 
                              FROM NguoiLienHe 
                              WHERE MaNLH = @MaNLH";

                    OleDbCommand cmdNLH = new OleDbCommand(sqlNLH, conn);
                    cmdNLH.Parameters.AddWithValue("@MaNLH", manlh);

                    using (OleDbDataReader rdNLH = cmdNLH.ExecuteReader())
                    {
                        if (rdNLH.Read())
                        {

                            txtNguoiLienHe.Text = rdNLH["TenNLH"].ToString();
                            txtDienThoaiLienHe.Text = rdNLH["DienThoai"].ToString();
                        }
                    }
                }

                // 2) Lấy thông tin nhà cung cấp
                if (!string.IsNullOrEmpty(maNCC))
                {
                    string sqlNCC = @"SELECT TenNCC, DiaChi, MaSoThue,DienThoai 
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
                            txtDienThoai.Text = rdNCC["DienThoai"].ToString();
                        }
                    }
                }

            }
        }
        private void LoadChiTietDonMuaHang(string maHD)
        {
            string sql = @"
                    SELECT 
                        ct.MaHH,
                        hh.TenHH,
                        hh.DonViTinh,
                        ct.SoLuong,
                        ct.DonGia,
                        (ct.SoLuong * ct.DonGia) AS ThanhTien,
                        ct.DienGiai,
                        ct.PhanTramVAT,
                        (ct.SoLuong * ct.DonGia * ct.PhanTramVAT / 100) AS TienVAT,
                        ct.NgayGiaoHang,
                        ct.SoThangBaoHanh
                    FROM  ChiTietDonMua ct
                    LEFT JOIN HangHoa hh ON ct.MaHH = hh.MaHH
                    WHERE ct.SoDonDatHang = @MaHopDong
                ";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            using (OleDbDataAdapter da = new OleDbDataAdapter(sql, conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@MaHopDong", maHD);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgv.Columns["MaHang"].DataPropertyName = "MaHH";
                dgv.Columns["TenHang"].DataPropertyName = "TenHH";
                dgv.Columns["DVT"].DataPropertyName = "DonViTinh";
                dgv.Columns["SL"].DataPropertyName = "SoLuong";
                dgv.Columns["DonGia"].DataPropertyName = "DonGia";
                dgv.Columns["TongTien"].DataPropertyName = "ThanhTien";
                dgv.Columns["DienGiai"].DataPropertyName = "DienGiai";
                dgv.Columns["Vat"].DataPropertyName = "PhanTramVAT";
                dgv.Columns["TienVAT"].DataPropertyName = "TienVAT";
                dgv.Columns["NgayGH"].DataPropertyName = "NgayGiaoHang";
                dgv.Columns["ThangBH"].DataPropertyName = "SoThangBaoHanh";

                dgv.DataSource = dt;
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
            

            // Hàng 1: Mã CT, Ngày PO, Số PO, Loại tiền, Số hợp đồng, Ngày hợp đồng, Ngày đến hạn, Người lập
            string[] labels1 = { "Số chứng từ đơn hàng*", "Ngày chứng từ*",  "Loại tiền*", "Số hợp đồng", "Ngày hợp đồng", "Ngày đến hạn", "Người lập" };
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
                else if (label.Contains("Loại tiền"))
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
                else if (label.Contains("Điện thoại"))
                    txtDienThoai = txt;
                else if (label.Contains("ĐT liên hệ"))
                    txtDienThoaiLienHe = (TextBox)txt;
                x += txt.Width + spacingX;
            }

            // Hàng 3: Địa điểm giao hàng
            y += controlHeight + spacingY + rowSpacing;
            x = startX;

            string[] labels3 = { "Tên địa điểm giao/nhận", "Địa chỉ giao/nhận", "Điện thoại" };

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
                    txtTenDiaDiemGiao = (TextBox)txt;
                else if (label.Contains("Địa chỉ giao"))
                    txtDiaDiemGiao = (TextBox)txt;
                else if (label.Contains("Điện thoại"))
                    txtDienThoaiGiao = (TextBox)txt;
                x += txt.Width + spacingX;
            }

            // Hàng 4: Thanh toán - giao hàng
            y += controlHeight + spacingY + rowSpacing;
            x = startX;

            string[] labels4 = { "Hình thức thanh toán", "Phương thức thanh toán", "Phương thức giao hàng", "Thời gian giao hàng", "Đợt", "Trạng thái" };

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
                    cboHinhThucThanhToan = new ComboBox
                    {
                        Location = new Point(x, y + lbl.Height + 2),
                        Width = textBoxWidth,
                        DropDownStyle = ComboBoxStyle.DropDownList
                    };
                    cboHinhThucThanhToan.Items.Add("Thanh toán ngay");
                    cboHinhThucThanhToan.Items.Add("Đặt cọc trước – trả sau");
                    cboHinhThucThanhToan.Items.Add("Trả sau");
                    cboHinhThucThanhToan.Items.Add("Trả theo tiến độ");
                    cboHinhThucThanhToan.SelectedIndex = -1; // mặc định rỗng
                    groupBox.Controls.Add(cboHinhThucThanhToan);

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
                        Width = textBoxWidth * 2 + spacingX,
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

        private void BtnLuu_Click(object sender, EventArgs e)
        {
            if (!isDirty)
            {
                MessageBox.Show("Không có thay đổi để lưu.");
                return;
            }

            string soDonMua = txtSoChungTu.Text.Trim();
            string tendiadiemgiao = txtTenDiaDiemGiao.Text.Trim();
            string diaChiGiao = txtDiaDiemGiao.Text.Trim();
            string dienthoaigiao = txtDienThoaiGiao.Text.Trim();
            DateTime ngayct = dtNgayChungTu.Value;
            DateTime thoigiangiaohang = dtThoiGianGiaoHang.Value;
            string dienGiai = txtNoiDung.Text.Trim();
            string loaitien = cboLoaiTien.Text;
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                // UPDATE bảng chính
                string sqlUpdate = @"
            UPDATE DonMuaHang
            SET NgayChungTu = ?, 
                TenDiaDiemGiao = ?,
                DiaChiGiao = ?,
                DienThoaiGiao = ?,
                NoiDung = ?,
                ThoiGianGiaoHang = ?,
                MaNgoaiTe = ?
            WHERE SoDonDatHang = ?
        ";

                using (OleDbCommand cmd = new OleDbCommand(sqlUpdate, conn))
                {
                    cmd.Parameters.Add("@NgayChungTu", OleDbType.Date).Value = ngayct;
                    cmd.Parameters.AddWithValue("@TenDiaDiemGiao", OleDbType.VarChar).Value = tendiadiemgiao;
                    cmd.Parameters.AddWithValue("@DiaChiGiao", OleDbType.VarChar).Value = diaChiGiao;
                    cmd.Parameters.AddWithValue("@DienThoaiGiao", OleDbType.VarChar).Value = dienthoaigiao;
                    cmd.Parameters.AddWithValue("@DienGiai", OleDbType.VarChar).Value = dienGiai;
                    cmd.Parameters.Add("@ThoiGianGiao", OleDbType.Date).Value = thoigiangiaohang;
                    cmd.Parameters.AddWithValue("@MaNgoaiTe", OleDbType.VarChar).Value = loaitien;
                    cmd.Parameters.AddWithValue("@SoDonDatHang", OleDbType.VarChar).Value = soDonMua;

                    cmd.ExecuteNonQuery();
                }

                // UPDATE chi tiết (dgv)
                //UpdateChiTiet(conn, maYC);
            }

            isDirty = false; // đã lưu xong
            MessageBox.Show(
                "Đã lưu thay đổi thành công!",
                "Thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (dgvDanhSach.SelectedRows.Count == 0)
            {
                MessageBox.Show("Không thể xóa đơn mua hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvDanhSach.SelectedRows[0];
            string trangThai = selectedRow.Cells["TrangThai"].Value.ToString();
            string maYeuCau = selectedRow.Cells["MaYC"].Value.ToString();

            if (trangThai == "Đã xử lý")
            {
                MessageBox.Show("Không thể xóa đơn mua hàng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Bạn có chắc muốn xóa đơn mua hàng '{maYeuCau}' không?",
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
                    string sqlCT = "DELETE FROM ChiTietDonMua WHERE SoDonDatHang = ?";
                    using (OleDbCommand cmdCT = new OleDbCommand(sqlCT, conn))
                    {
                        cmdCT.Parameters.AddWithValue("@p1", maYeuCau);
                        cmdCT.ExecuteNonQuery();
                    }

                 
                    // 2) Xóa bảng chính
                    string sqlMain = "DELETE FROM DonMuaHang WHERE SoDonDatHang = ?";
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

        private void BtnTimKiem_Click(object sender, EventArgs e)
        {

            FormTimKiemDonMuaHang formTimKiem = new FormTimKiemDonMuaHang();

            if (formTimKiem.ShowDialog() == DialogResult.OK)
            {

                DateTime? ngayCTTu = null;
                DateTime? ngayCTDen = null;

                if (formTimKiem.LocTheoNgay)
                {
                    ngayCTTu = formTimKiem.NgayTu;
                    ngayCTDen = formTimKiem.NgayDen;
                }

                string trangThai = formTimKiem.TrangThai;
                string maChungTu = formTimKiem.MaChungTu;
                string nguoidangki = formTimKiem.NguoiDangKi;
                LocDanhSach(ngayCTTu, ngayCTDen, trangThai, maChungTu, nguoidangki);
            }
        }
        private void LocDanhSach(DateTime? ngayCTTu, DateTime? ngayCTDen,
                 string trangThai, string maChungTu, string nguoidangki)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"
                SELECT SoDonDatHang, NgayChungTu, MaNhanVienLap, TrangThai
                FROM DonMuaHang
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
                        query += " AND  SoDonDatHang LIKE @MaCT";
                        cmd.Parameters.AddWithValue("@MaCT", "%" + maChungTu + "%");
                    }

                    if (!string.IsNullOrWhiteSpace(nguoidangki))
                    {
                        query += " AND MaNhanVienLap LIKE @MaNV";
                        cmd.Parameters.AddWithValue("@MaNV", "%" + nguoidangki + "%");
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
                            row["SoDonDatHang"].ToString(),
                            row["MaNhanVienLap"].ToString(),
                            Convert.ToDateTime(row["NgayChungTu"]).ToString("dd/MM/yyyy"),
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

    }
}
