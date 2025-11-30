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
    public partial class DanhSachPhieuXuatTraNCC : Form
    {
        private Label lblTongSL, lblTongTien, lblTonHienTai, lblDuKienNhap;

        TextBox txtMaCT;
        TextBox txtSoCT;
        TextBox txtLoaiTien;
        TextBox txtMauHoaDon;
        TextBox txtSoSeri;
        TextBox txtSoHoaDon;
        TextBox txtCTThamChieu;
        TextBox txtMaNCC;
        TextBox txtTenNCC;
        TextBox txtDiaChi;
        TextBox txtMaSoThue;
        TextBox txtDienThoaiNCC, txtNguoiLienHe, txtMaLienHe, txtDienThoaiLienHe, txtSoNgayNo, txtNoiDung;
        ComboBox cboLoaiTien, cboMaCT;
        // ===== Khai báo DateTimePicker =====
        DateTimePicker dtNgayCT;
        DateTimePicker dtNgayHoaDon, dtNgayDenHan;
        DataGridView dgv, dgvDanhSach,dgvthue;
        private bool isDirty = false;

        private string connectionString =
           DatabaseConfig.ConnectionString;
        public DanhSachPhieuXuatTraNCC()
        {
            InitializeComponent();
            BuildUI();
            AttachControlChangeEvents();
        }
        private void BuildUI()
        {
            this.Text = "Danh sách phiếu xuất trả nhà cung cấp";
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
                //if (text.Contains("Xoá"))
                //    btn.Click += BtnXoa_Click;

            }
            foreach (Control ctrl in pnlLeft.Controls)
            {
                if (ctrl is Button btn)
                {
                    if (btn.Text.Contains("Tìm"))
                    {
                        btn.Click += BtnTimKiem_Click;
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
                    BackColor = text.Contains("Lưu") ? Color.LightSkyBlue : Color.FromArgb(242, 52, 52),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                pnlRight.Controls.Add(btn);
                if (text.Contains("Lưu"))
                {
                    btn.Click += BtnLuu_Click;
                }
            }

            // === Panel chứa nội dung chính ===
            //Panel pnlMain = new Panel
            //{
            //    Dock = DockStyle.Fill,
            //    AutoScroll = true,
            //    Padding = new Padding(10, 10, 10, 10)
            //};
            //this.Controls.Add(pnlMain);
            //pnlMain.BringToFront();


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
                Text = "DANH SÁCH PHIẾU XUẤT TRẢ NHÀ CUNG CẤP",
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
            dgvDanhSach.Columns.Add("NgayCT", "Ngày chứng từ");
            dgvDanhSach.Columns.Add("SoHoaDon", "Số hóa đơn");
            dgvDanhSach.Columns.Add("NguoiDK", "Tên nhà cung cấp");

            LoadDanhSach(dgvDanhSach);
            dgvDanhSach.SelectionChanged += DgvDanhSach_SelectionChanged;
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
                Height = 900,
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
                Height = 350,
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
            dgv.Columns["MaHang"].Width = 180;

            dgv.Columns.Add("TenHang", "Tên hàng");
            dgv.Columns["TenHang"].Width = 300;

            dgv.Columns.Add("DienGiai", "Diễn giải");
            dgv.Columns["DienGiai"].Width = 310;

            dgv.Columns.Add("DVT", "Đvt");
            dgv.Columns["DVT"].Width = 100;

            dgv.Columns.Add("SL", "Số lượng");
            dgv.Columns["SL"].Width = 100;

            dgv.Columns.Add("DonGia", "Đơn giá");
            dgv.Columns["DonGia"].Width = 200;

            dgv.Columns.Add("TongTien", "Thành tiền");
            dgv.Columns["TongTien"].Width = 220;

            dgv.Columns.Add("TKNO", "TK Nợ");
            dgv.Columns["TKNO"].Width = 200;

            dgv.Columns.Add("TKCO", "TK Có");
            dgv.Columns["TKCO"].Width = 200;



            // Căn giữa header và dữ liệu số
            dgv.Columns["SL"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["DonGia"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["TongTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["DVT"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            Label lbl2 = new Label
            {
                Text = "CHI TIẾT THUẾ",
                Location = new Point(10, dgv.Bottom + 10),
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue
            };
            grpChiTiet.Controls.Add(lbl2);
            dgvthue = new DataGridView
            {


                Location = new Point(10, lbl2.Bottom + 10),
                Width = grpChiTiet.Width - 20,
                Height = grpChiTiet.Height - grpTongHop.Bottom - 205,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackgroundColor = Color.White,
                AllowUserToAddRows = true,
                ColumnHeadersHeight = 35,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BorderStyle = BorderStyle.FixedSingle,
                EnableHeadersVisualStyles = false
            };
            grpChiTiet.Controls.Add(dgvthue);
            //dgvthue.CellEndEdit += DgvThue_CellEndEdit;

            dgvthue.Columns.Add("MaHang", "Mã hàng");
            dgvthue.Columns.Add("TenHang", "Tên hàng");
            dgvthue.Columns.Add("Vat", "%Vat");
            dgvthue.Columns.Add("TongVat", "Tổng tiền thuế");
            dgvthue.Columns.Add("TKNO", "TK Nợ");
            dgvthue.Columns.Add("TKCO", "TK Có");



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

        // Hàm riêng để disable ComboBox Mã chứng từ
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
        private void LoadDanhSach(DataGridView dgv)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"SELECT p.SoPhieuTra, p.NgayChungTu, p.MaNhanVien, p.MaNCC, n.TenNCC,p.SoChungTuHoaDon
                                    FROM PhieuTraHang AS p
                                    LEFT JOIN NhaCungCap AS n
                                        ON p.MaNCC = n.MaNCC
                                    ORDER BY p.SoPhieuTra DESC";
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
                                row["SoPhieuTra"].ToString(),
                                Convert.ToDateTime(row["NgayChungTu"]).ToString("dd/MM/yyyy"),
                                row["SoChungTuHoaDon"].ToString(),
                                row["TenNCC"].ToString()
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
        private void DgvDanhSach_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDanhSach.CurrentRow == null) return;

            var cellValue = dgvDanhSach.CurrentRow.Cells["MaYC"].Value;
            if (cellValue == null) return;

            string maHD = cellValue.ToString();

            LoadThongTinPhieuTra(maHD);

            LoadChiTietPhieuTra(maHD);
            LoadChiTietThuePhieuTra(maHD);
            CapNhatTongHop();
        }
        private void CapNhatTongHop()
        {
            decimal tongSL = 0;
            decimal tongTienHang = 0;
            decimal tongTienthue = 0;
            decimal tongtien = 0;
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.IsNewRow) continue;

                decimal sl = 0, dongia = 0;

                decimal.TryParse(Convert.ToString(row.Cells["SL"].Value), out sl);
                decimal.TryParse(Convert.ToString(row.Cells["DonGia"].Value), out dongia);
                //decimal.TryParse(Convert.ToString(row.Cells["TienVat"].Value), out tienvat);

                tongSL += sl;
                tongTienHang += (sl * dongia);
            }
            foreach (DataGridViewRow row in dgvthue.Rows)
            {
                if (row.IsNewRow) continue;

                decimal tongvat = 0;

                decimal.TryParse(Convert.ToString(row.Cells["TongVat"].Value), out tongvat);

                tongTienthue += tongvat;
            }
            tongtien = tongTienHang + tongTienthue;
            // Cập nhật lên giao diện
            lblTongSL.Text = tongSL.ToString("N0");
            lblTongTien.Text = tongtien.ToString("N0");
        }

        private void LoadThongTinPhieuTra(string maHD)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                string sqlHD = @"SELECT * FROM PhieuTraHang WHERE SoPhieuTra = @SoPhieuTra";
                OleDbCommand cmdHD = new OleDbCommand(sqlHD, conn);
                cmdHD.Parameters.AddWithValue("@SoPhieuTra", maHD);

                string maNCC = "";
                string maNLH = "";
                string mahoadon = "";
                using (OleDbDataReader rd = cmdHD.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        txtMaCT.Text = rd["SoPhieuTra"].ToString();

                        if (rd["NgayChungTu"] != DBNull.Value)
                            dtNgayCT.Value = Convert.ToDateTime(rd["NgayChungTu"]);

                        txtNoiDung.Text = rd["NoiDung"].ToString();
                        cboLoaiTien.Text = rd["MaNgoaiTe"].ToString();
                        //txtSoHoaDon.Text = rd["SoChungTuHoaDon"].ToString();
                        mahoadon = rd["SoChungTuHoaDon"].ToString();
                        txtCTThamChieu.Text = mahoadon;
                        maNCC = rd["MaNCC"].ToString();


                        maNLH = rd["MaNLH"].ToString();
                    }
                }

                //lấy thông tin hóa đơn
                // 2) Lấy thông tin nhà cung cấp
                if (!string.IsNullOrEmpty(mahoadon))
                {
                    string sqlhoadon = @"SELECT *
                              FROM HoaDonMuaHang 
                              WHERE SoChungTuHoaDon = @SoChungTuHoaDon";

                    OleDbCommand cmdhoadon = new OleDbCommand(sqlhoadon, conn);
                    cmdhoadon.Parameters.AddWithValue("@SoChungTuHoaDon", mahoadon);

                    using (OleDbDataReader rdNCC = cmdhoadon.ExecuteReader())
                    {
                        if (rdNCC.Read())
                        {
                            txtSoSeri.Text = rdNCC["SoSeri"].ToString() ;
                            txtMauHoaDon.Text = rdNCC["MauHoaDon"].ToString() ;
                            txtSoHoaDon.Text = rdNCC["SoHoaDon"].ToString();
                        }
                    }
                }

                // 2) Lấy thông tin nhà cung cấp
                if (!string.IsNullOrEmpty(maNCC))
                {
                    string sqlNCC = @"SELECT *
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
                            txtDienThoaiNCC.Text = rdNCC["DienThoai"].ToString();
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
        private void LoadChiTietPhieuTra(string maHD)
        {
            string sql = @"
                    SELECT 
                        ct.MaHH,
                        hh.TenHH,
                        ct.DienGiai,
                        hh.DonViTinh,
                        ct.SoLuongTra,
                        ct.DonGia,
                        (ct.SoLuongTra * ct.DonGia) AS ThanhTien,
                        ct.TKNo,
                        ct.TKCo
                    FROM  ChiTietPhieuTraHang ct
                    LEFT JOIN HangHoa hh ON ct.MaHH = hh.MaHH
                    WHERE ct.SoPhieuTra = @SoPhieuTra
                ";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            using (OleDbDataAdapter da = new OleDbDataAdapter(sql, conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@SoPhieuTra", maHD);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgv.Columns["MaHang"].DataPropertyName = "MaHH";
                dgv.Columns["TenHang"].DataPropertyName = "TenHH";
                dgv.Columns["DienGiai"].DataPropertyName = "DienGiai";
                dgv.Columns["DVT"].DataPropertyName = "DonViTinh";
                dgv.Columns["SL"].DataPropertyName = "SoLuongTra";
                dgv.Columns["DonGia"].DataPropertyName = "DonGia";
                dgv.Columns["TongTien"].DataPropertyName = "ThanhTien";
                dgv.Columns["TKNO"].DataPropertyName = "TKNo";
                dgv.Columns["TKCO"].DataPropertyName = "TKCo";

                dgv.DataSource = dt;
            }
        }

        private void LoadChiTietThuePhieuTra(string maHD)
        {
            string sql = @"
                        SELECT 
                            ChiTietThuePhieuTra.PhanTramVAT,
                            ChiTietThuePhieuTra.GiaTriThue,
                            ChiTietThuePhieuTra.TKNo,
                            ChiTietThuePhieuTra.TKCo,
                            ChiTietPhieuTraHang.MaHH,
                            HangHoa.TenHH
                        FROM 
                            (ChiTietThuePhieuTra
                            INNER JOIN ChiTietPhieuTraHang
                                ON ChiTietThuePhieuTra.MaCTPTH = ChiTietPhieuTraHang.MaCTPTH)
                            LEFT JOIN HangHoa 
                                ON ChiTietPhieuTraHang.MaHH = HangHoa.MaHH
                        WHERE 
                            ChiTietPhieuTraHang.SoPhieuTra = ?
                    ";

            using (OleDbConnection conn = new OleDbConnection(connectionString))
            using (OleDbDataAdapter da = new OleDbDataAdapter(sql, conn))
            {
                da.SelectCommand.Parameters.AddWithValue("@SoChungTuHoaDon", maHD);

                DataTable dt = new DataTable();
                da.Fill(dt);

                dgvthue.Columns["MaHang"].DataPropertyName = "MaHH";
                dgvthue.Columns["TenHang"].DataPropertyName = "TenHH";
                dgvthue.Columns["Vat"].DataPropertyName = "PhanTramVAT";
                dgvthue.Columns["TongVat"].DataPropertyName = "GiaTriThue";
                dgvthue.Columns["TKNO"].DataPropertyName = "TKCo";
                dgvthue.Columns["TKCO"].DataPropertyName = "TKNo";

                dgvthue.DataSource = dt;
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
                Text = "Chọn",
                Location = new Point(startX, 30),
                Size = new Size(80, controlHeight),
                BackColor = Color.MediumSeaGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            groupBox.Controls.Add(btnChon);

            // Hàng 1: Mã CT, Ngày PO, Số PO, Loại tiền, Số hợp đồng, Ngày hợp đồng, Ngày đến hạn, Người lập
            string[] labels1 = { "Số CT*", "Ngày CT*", "Loại tiền*", "Mẫu hóa đơn", "Số seri", "Số hóa đơn", "Ngày hóa đơn", "CT tham chiếu" };
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
                // Gọi hàm bo góc
                ;
                groupBox.Controls.Add(input);
                if (label.Contains("Ngày CT*"))
                    dtNgayCT = (DateTimePicker)input;
                else if (label.Contains("Số CT*"))
                    txtMaCT = (TextBox)input;

                else if (label.Contains("Loại tiền*"))
                    cboLoaiTien = (ComboBox)input;

                else if (label.Contains("Mẫu hóa đơn"))
                    txtMauHoaDon = (TextBox)input;

                else if (label.Contains("Số seri"))
                    txtSoSeri = (TextBox)input;

                else if (label.Contains("Số hóa đơn"))
                    txtSoHoaDon = (TextBox)input;

                else if (label.Contains("Ngày hóa đơn"))
                    dtNgayHoaDon = (DateTimePicker)input;

                else if (label.Contains("CT tham chiếu"))
                    txtCTThamChieu = (TextBox)input;
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
                    Width = (label.Contains("NCC")) ? textBoxWidth : textBoxWidth * 2 + spacingX
                };
                groupBox.Controls.Add(txt);
                if (label.Contains("Mã NCC"))
                    txtMaNCC = txt;
                else if (label.Contains("Tên nhà cung cấp"))
                    txtTenNCC = txt;
                else if (label.Contains("Địa chỉ"))
                    txtDiaChi = txt;
                else if (label.Contains("Mã số thuế"))
                    txtMaSoThue = txt;
                else if (label.Contains("Điện thoại"))
                    txtDienThoaiNCC = txt;
                x += txt.Width + spacingX;
            }

            // Hàng 3: Địa điểm giao hàng
            y += controlHeight + spacingY + rowSpacing;
            x = startX;

            string[] labels3 = { "Mã liên hệ", "Người liên hệ", "Điện thoại liên hệ", "Nội dung" };

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
                    Width = (label.Contains("Nội dung")
                                ? textBoxWidth * 5 + spacingX * 4
                                : label.Contains("Người liên hệ")
                                       ? textBoxWidth * 2 + spacingX : textBoxWidth)
                };
                groupBox.Controls.Add(txt);
                if (label.Contains("Mã liên hệ"))
                    txtMaLienHe = (TextBox)txt;
                else if (label.Contains("Người liên hệ"))
                    txtNguoiLienHe = (TextBox)txt;
                else if (label.Contains("Điện thoại liên hệ"))
                    txtDienThoaiLienHe = (TextBox)txt;
                else if (label.Contains("Nội dung"))
                    txtNoiDung = (TextBox)txt;
                x += txt.Width + spacingX;
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

            string soDonMua = txtMaCT.Text.Trim();
            DateTime ngayct = dtNgayCT.Value;
            string loaitien = cboLoaiTien.Text;
            string sohoadon = txtSoHoaDon.Text;
            string chungtuthamchieu = txtCTThamChieu.Text;
            string dienGiai = txtNoiDung.Text.Trim();
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                conn.Open();

                // UPDATE bảng chính
                string sqlUpdate = @"
            UPDATE PhieuTraHang
            SET NgayChungTu = ?, 
                SoChungTuHoaDon = ?,
                NoiDung = ?,
                MaNgoaiTe = ?
            WHERE SoPhieuTra = ?
        ";

                using (OleDbCommand cmd = new OleDbCommand(sqlUpdate, conn))
                {
                    cmd.Parameters.Add("@NgayChungTu", OleDbType.Date).Value = ngayct;
                    cmd.Parameters.AddWithValue("@ChungTuThamChieu", OleDbType.VarChar).Value = chungtuthamchieu;
                    cmd.Parameters.AddWithValue("@NoiDung", OleDbType.VarChar).Value = dienGiai;
                    cmd.Parameters.AddWithValue("@MaNgoaiTe", OleDbType.VarChar).Value = loaitien;
                    cmd.Parameters.AddWithValue("@SoPhieuTra", OleDbType.VarChar).Value = soDonMua;

                    cmd.ExecuteNonQuery();
                }

                // UPDATE chi tiết (dgv)
                UpdateChiTiet(conn, soDonMua);
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
                decimal soLuong = Convert.ToDecimal(row.Cells["SL"].Value ?? 0);
                decimal donGia = Convert.ToDecimal(row.Cells["DonGia"].Value ?? 0);
                string dienGiai = row.Cells["DienGiai"].Value?.ToString() ?? "";
                string no = row.Cells["TKNO"].Value?.ToString() ?? "";
                string co = row.Cells["TKCO"].Value?.ToString() ?? "";

                // 1) Kiểm tra tồn tại
                string checkSQL = @"SELECT COUNT(*) FROM ChiTietPhieuTraHang 
                            WHERE SoPhieuTra = ? AND MaHH = ?";

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
                UPDATE ChiTietPhieuTraHang
                SET SoLuongTra = ?, DonGia = ?, DienGiai = ?, TKNo = ?, TKCo = ?
                WHERE SoPhieuTra = ? AND MaHH = ?
            ";

                    using (OleDbCommand cmdUp = new OleDbCommand(updateSQL, conn))
                    {
                        cmdUp.Parameters.AddWithValue("@SL", soLuong);
                        cmdUp.Parameters.AddWithValue("@DG", donGia);
                        cmdUp.Parameters.AddWithValue("@DGiai", dienGiai);
                        cmdUp.Parameters.AddWithValue("@No", no);
                        cmdUp.Parameters.AddWithValue("@Co", co);
                        cmdUp.Parameters.AddWithValue("@YC", maYC);
                        cmdUp.Parameters.AddWithValue("@HH", maHH);

                        cmdUp.ExecuteNonQuery();
                    }
                }
                else
                {
                    // 3) INSERT
                    string insertSQL = @"
                INSERT INTO ChiTietPhieuTraHang
                (SoLuongTra, DonGia, DienGiai, TKNo, TKCo ,MaHH, SoPhieuTra)
                VALUES (?, ?, ?, ?, ?,?,?)
            ";

                    using (OleDbCommand cmdIns = new OleDbCommand(insertSQL, conn))
                    {
                        cmdIns.Parameters.AddWithValue("@SL", soLuong);
                        cmdIns.Parameters.AddWithValue("@DG", donGia);
                        cmdIns.Parameters.AddWithValue("@DGiai", dienGiai);
                        cmdIns.Parameters.AddWithValue("@no", no);
                        cmdIns.Parameters.AddWithValue("@co", co);
                        cmdIns.Parameters.AddWithValue("@HH", maHH);
                        cmdIns.Parameters.AddWithValue("@YC", maYC);

                        cmdIns.ExecuteNonQuery();
                    }
                }
            }
        }

        private void BtnXoa_Click(object sender, EventArgs e)
        {
            if (dgvDanhSach.SelectedRows.Count == 0)
            {
                MessageBox.Show("Không thể xóa phiếu xuất trả này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvDanhSach.SelectedRows[0];
            string maYeuCau = selectedRow.Cells["MaYC"].Value.ToString();


            DialogResult result = MessageBox.Show(
                $"Bạn có chắc muốn xóa phiếu xuất trả '{maYeuCau}' không?",
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
                    string sqlCT = "DELETE FROM ChiTietPhieuTraHang WHERE SoPhieuTra = ?";
                    using (OleDbCommand cmdCT = new OleDbCommand(sqlCT, conn))
                    {
                        cmdCT.Parameters.AddWithValue("@p1", maYeuCau);
                        cmdCT.ExecuteNonQuery();
                    }


                    // 2) Xóa bảng chính
                    string sqlMain = "DELETE FROM PhieuTraHang WHERE SoPhieuTra = ?";
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
        private void BtnTimKiem_Click(object sender, EventArgs e)
        {

            FormTimKiemPhieuXuatTraNCC formTimKiem = new FormTimKiemPhieuXuatTraNCC();

            if (formTimKiem.ShowDialog() == DialogResult.OK)
            {

                DateTime? ngayCTTu = null;
                DateTime? ngayCTDen = null;

                if (formTimKiem.LocTheoNgay)
                {
                    ngayCTTu = formTimKiem.NgayTu;
                    ngayCTDen = formTimKiem.NgayDen;
                }

                string sohoadon = formTimKiem.SoHoaDon;
                string maChungTu = formTimKiem.MaChungTu;
                string nguoidangki = formTimKiem.NhaCungCap;
                LocDanhSach(ngayCTTu, ngayCTDen, sohoadon, maChungTu, nguoidangki);
            }
        }
        private void LocDanhSach(DateTime? ngayCTTu, DateTime? ngayCTDen,
                 string sohoadon, string maChungTu, string nguoidangki)
        {
            using (OleDbConnection conn = new OleDbConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"
                        SELECT pth.SoPhieuTra, pth.NgayChungTu, pth.MaNCC,ncc.TenNCC, SoHoaDon
                        FROM PhieuTraHang pth
                        LEFT JOIN NhaCungCap ncc ON pth.MaNCC = ncc.MaNCC
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
                    if (!string.IsNullOrWhiteSpace(sohoadon))
                    {
                        query += " AND SoHoaDon = @SoHoaDon";
                        cmd.Parameters.AddWithValue("@SoHoaDon", sohoadon);
                    }

                    // ===== MÃ CHỨNG TỪ =====
                    if (!string.IsNullOrWhiteSpace(maChungTu))
                    {
                        query += " AND  SoPhieuTra LIKE @SoPhieuTra";
                        cmd.Parameters.AddWithValue("@SoPhieuTra", "%" + maChungTu + "%");
                    }

                    if (!string.IsNullOrWhiteSpace(nguoidangki))
                    {
                        query += " AND TenNCC LIKE @MaNV";
                        cmd.Parameters.AddWithValue("@MaNV", "%" + nguoidangki + "%");
                    }

                    cmd.CommandText = query;

                    DataTable dt = new DataTable();
                    using (OleDbDataAdapter da = new OleDbDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }

                    dgvDanhSach.Rows.Clear();

                    foreach (DataRow row in dt.Rows)
                    {
                        dgvDanhSach.Rows.Add(
                            row["SoPhieuTra"].ToString(),
                            Convert.ToDateTime(row["NgayChungTu"]).ToString("dd/MM/yyyy"),
                            row["SoHoaDon"].ToString(),
                            row["TenNCC"].ToString()
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
