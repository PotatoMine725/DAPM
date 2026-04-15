using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicBooking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChuyenKhoa",
                columns: table => new
                {
                    IdChuyenKhoa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenChuyenKhoa = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGianSlotMacDinh = table.Column<int>(type: "int", nullable: false),
                    GioMoDatLich = table.Column<TimeOnly>(type: "time", nullable: true),
                    GioDongDatLich = table.Column<TimeOnly>(type: "time", nullable: true),
                    HienThi = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuyenKhoa", x => x.IdChuyenKhoa);
                });

            migrationBuilder.CreateTable(
                name: "DinhNghiaCa",
                columns: table => new
                {
                    IdDinhNghiaCa = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenCa = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GioBatDauMacDinh = table.Column<TimeOnly>(type: "time", nullable: false),
                    GioKetThucMacDinh = table.Column<TimeOnly>(type: "time", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DinhNghiaCa", x => x.IdDinhNghiaCa);
                });

            migrationBuilder.CreateTable(
                name: "MauThongBao",
                columns: table => new
                {
                    IdMau = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LoaiThongBao = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TieuDeMau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiDungMau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KenhGui = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MauThongBao", x => x.IdMau);
                });

            migrationBuilder.CreateTable(
                name: "Phong",
                columns: table => new
                {
                    IdPhong = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaPhong = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenPhong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SucChua = table.Column<int>(type: "int", nullable: true),
                    TrangBi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phong", x => x.IdPhong);
                });

            migrationBuilder.CreateTable(
                name: "TaiKhoan",
                columns: table => new
                {
                    IdTaiKhoan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenDangNhap = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    MatKhau = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VaiTro = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    LanDangNhapCuoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaiKhoan", x => x.IdTaiKhoan);
                });

            migrationBuilder.CreateTable(
                name: "Thuoc",
                columns: table => new
                {
                    IdThuoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenThuoc = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HoatChat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DonVi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Thuoc", x => x.IdThuoc);
                });

            migrationBuilder.CreateTable(
                name: "DichVu",
                columns: table => new
                {
                    IdDichVu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdChuyenKhoa = table.Column<int>(type: "int", nullable: false),
                    TenDichVu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ThoiGianUocTinh = table.Column<int>(type: "int", nullable: true),
                    HienThi = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DichVu", x => x.IdDichVu);
                    table.ForeignKey(
                        name: "FK_DichVu_ChuyenKhoa_IdChuyenKhoa",
                        column: x => x.IdChuyenKhoa,
                        principalTable: "ChuyenKhoa",
                        principalColumn: "IdChuyenKhoa",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BacSi",
                columns: table => new
                {
                    IdBacSi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    IdChuyenKhoa = table.Column<int>(type: "int", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AnhDaiDien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BangCap = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NamKinhNghiem = table.Column<int>(type: "int", nullable: true),
                    TieuSu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LoaiHopDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BacSi", x => x.IdBacSi);
                    table.ForeignKey(
                        name: "FK_BacSi_ChuyenKhoa_IdChuyenKhoa",
                        column: x => x.IdChuyenKhoa,
                        principalTable: "ChuyenKhoa",
                        principalColumn: "IdChuyenKhoa",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BacSi_TaiKhoan_IdTaiKhoan",
                        column: x => x.IdTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "IdTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BenhNhan",
                columns: table => new
                {
                    IdBenhNhan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cccd = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NgaySinh = table.Column<DateOnly>(type: "date", nullable: true),
                    GioiTinh = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoLanHuyMuon = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    BiHanChe = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NgayHetHanChe = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BenhNhan", x => x.IdBenhNhan);
                    table.ForeignKey(
                        name: "FK_BenhNhan_TaiKhoan_IdTaiKhoan",
                        column: x => x.IdTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "IdTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LeTan",
                columns: table => new
                {
                    IdLeTan = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeTan", x => x.IdLeTan);
                    table.ForeignKey(
                        name: "FK_LeTan_TaiKhoan_IdTaiKhoan",
                        column: x => x.IdTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "IdTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OtpLog",
                columns: table => new
                {
                    IdOtpLog = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    MaOtp = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MucDich = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GioHetHan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DaSuDung = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    SoLanThu = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    IdPhien = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DiaChiIp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtpLog", x => x.IdOtpLog);
                    table.ForeignKey(
                        name: "FK_OtpLog_TaiKhoan_IdTaiKhoan",
                        column: x => x.IdTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "IdTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ThongBao",
                columns: table => new
                {
                    IdThongBao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    IdMau = table.Column<int>(type: "int", nullable: false),
                    KenhGui = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TieuDe = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdThamChieu = table.Column<int>(type: "int", nullable: true),
                    LoaiThamChieu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DaDoc = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NgayGui = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ThongBao", x => x.IdThongBao);
                    table.ForeignKey(
                        name: "FK_ThongBao_MauThongBao_IdMau",
                        column: x => x.IdMau,
                        principalTable: "MauThongBao",
                        principalColumn: "IdMau",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ThongBao_TaiKhoan_IdTaiKhoan",
                        column: x => x.IdTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "IdTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CaLamViec",
                columns: table => new
                {
                    IdCaLamViec = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdBacSi = table.Column<int>(type: "int", nullable: false),
                    IdPhong = table.Column<int>(type: "int", nullable: false),
                    IdChuyenKhoa = table.Column<int>(type: "int", nullable: false),
                    IdDinhNghiaCa = table.Column<int>(type: "int", nullable: false),
                    NgayLamViec = table.Column<DateOnly>(type: "date", nullable: false),
                    GioBatDau = table.Column<TimeOnly>(type: "time", nullable: false),
                    GioKetThuc = table.Column<TimeOnly>(type: "time", nullable: false),
                    ThoiGianSlot = table.Column<int>(type: "int", nullable: false),
                    SoSlotToiDa = table.Column<int>(type: "int", nullable: false),
                    SoSlotDaDat = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TrangThaiDuyet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NguonTaoCa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdBacSiYeuCau = table.Column<int>(type: "int", nullable: true),
                    IdAdminDuyet = table.Column<int>(type: "int", nullable: true),
                    LyDoTuChoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayDuyet = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaLamViec", x => x.IdCaLamViec);
                    table.ForeignKey(
                        name: "FK_CaLamViec_BacSi_IdBacSi",
                        column: x => x.IdBacSi,
                        principalTable: "BacSi",
                        principalColumn: "IdBacSi",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaLamViec_BacSi_IdBacSiYeuCau",
                        column: x => x.IdBacSiYeuCau,
                        principalTable: "BacSi",
                        principalColumn: "IdBacSi",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaLamViec_ChuyenKhoa_IdChuyenKhoa",
                        column: x => x.IdChuyenKhoa,
                        principalTable: "ChuyenKhoa",
                        principalColumn: "IdChuyenKhoa",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaLamViec_DinhNghiaCa_IdDinhNghiaCa",
                        column: x => x.IdDinhNghiaCa,
                        principalTable: "DinhNghiaCa",
                        principalColumn: "IdDinhNghiaCa",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaLamViec_Phong_IdPhong",
                        column: x => x.IdPhong,
                        principalTable: "Phong",
                        principalColumn: "IdPhong",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CaLamViec_TaiKhoan_IdAdminDuyet",
                        column: x => x.IdAdminDuyet,
                        principalTable: "TaiKhoan",
                        principalColumn: "IdTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LichNoiTru",
                columns: table => new
                {
                    IdLichNoiTru = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdBacSi = table.Column<int>(type: "int", nullable: false),
                    IdPhong = table.Column<int>(type: "int", nullable: false),
                    IdDinhNghiaCa = table.Column<int>(type: "int", nullable: false),
                    NgayTrongTuan = table.Column<int>(type: "int", nullable: false),
                    NgayApDung = table.Column<DateOnly>(type: "date", nullable: false),
                    NgayKetThuc = table.Column<DateOnly>(type: "date", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichNoiTru", x => x.IdLichNoiTru);
                    table.ForeignKey(
                        name: "FK_LichNoiTru_BacSi_IdBacSi",
                        column: x => x.IdBacSi,
                        principalTable: "BacSi",
                        principalColumn: "IdBacSi",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichNoiTru_DinhNghiaCa_IdDinhNghiaCa",
                        column: x => x.IdDinhNghiaCa,
                        principalTable: "DinhNghiaCa",
                        principalColumn: "IdDinhNghiaCa",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichNoiTru_Phong_IdPhong",
                        column: x => x.IdPhong,
                        principalTable: "Phong",
                        principalColumn: "IdPhong",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DonNghiPhep",
                columns: table => new
                {
                    IdDonNghiPhep = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdBacSi = table.Column<int>(type: "int", nullable: false),
                    IdCaLamViec = table.Column<int>(type: "int", nullable: false),
                    LoaiNghiPhep = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrangThaiDuyet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdNguoiDuyet = table.Column<int>(type: "int", nullable: true),
                    LyDoTuChoi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayGuiDon = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayXuLy = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DonNghiPhep", x => x.IdDonNghiPhep);
                    table.ForeignKey(
                        name: "FK_DonNghiPhep_BacSi_IdBacSi",
                        column: x => x.IdBacSi,
                        principalTable: "BacSi",
                        principalColumn: "IdBacSi",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonNghiPhep_CaLamViec_IdCaLamViec",
                        column: x => x.IdCaLamViec,
                        principalTable: "CaLamViec",
                        principalColumn: "IdCaLamViec",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DonNghiPhep_TaiKhoan_IdNguoiDuyet",
                        column: x => x.IdNguoiDuyet,
                        principalTable: "TaiKhoan",
                        principalColumn: "IdTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GiuCho",
                columns: table => new
                {
                    IdGiuCho = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCaLamViec = table.Column<int>(type: "int", nullable: false),
                    SoSlot = table.Column<int>(type: "int", nullable: false),
                    IdBenhNhan = table.Column<int>(type: "int", nullable: false),
                    GioHetHan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DaGiaiPhong = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiuCho", x => x.IdGiuCho);
                    table.ForeignKey(
                        name: "FK_GiuCho_BenhNhan_IdBenhNhan",
                        column: x => x.IdBenhNhan,
                        principalTable: "BenhNhan",
                        principalColumn: "IdBenhNhan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GiuCho_CaLamViec_IdCaLamViec",
                        column: x => x.IdCaLamViec,
                        principalTable: "CaLamViec",
                        principalColumn: "IdCaLamViec",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LichHen",
                columns: table => new
                {
                    IdLichHen = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaLichHen = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IdBenhNhan = table.Column<int>(type: "int", nullable: false),
                    IdCaLamViec = table.Column<int>(type: "int", nullable: false),
                    IdDichVu = table.Column<int>(type: "int", nullable: false),
                    SoSlot = table.Column<int>(type: "int", nullable: false),
                    HinhThucDat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BacSiMongMuonNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdBacSiMongMuon = table.Column<int>(type: "int", nullable: true),
                    TrieuChung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichHen", x => x.IdLichHen);
                    table.ForeignKey(
                        name: "FK_LichHen_BacSi_IdBacSiMongMuon",
                        column: x => x.IdBacSiMongMuon,
                        principalTable: "BacSi",
                        principalColumn: "IdBacSi",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichHen_BenhNhan_IdBenhNhan",
                        column: x => x.IdBenhNhan,
                        principalTable: "BenhNhan",
                        principalColumn: "IdBenhNhan",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichHen_CaLamViec_IdCaLamViec",
                        column: x => x.IdCaLamViec,
                        principalTable: "CaLamViec",
                        principalColumn: "IdCaLamViec",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichHen_DichVu_IdDichVu",
                        column: x => x.IdDichVu,
                        principalTable: "DichVu",
                        principalColumn: "IdDichVu",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HangCho",
                columns: table => new
                {
                    IdHangCho = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCaLamViec = table.Column<int>(type: "int", nullable: false),
                    IdLichHen = table.Column<int>(type: "int", nullable: false),
                    SoThuTu = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayCheckIn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HangCho", x => x.IdHangCho);
                    table.ForeignKey(
                        name: "FK_HangCho_CaLamViec_IdCaLamViec",
                        column: x => x.IdCaLamViec,
                        principalTable: "CaLamViec",
                        principalColumn: "IdCaLamViec",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HangCho_LichHen_IdLichHen",
                        column: x => x.IdLichHen,
                        principalTable: "LichHen",
                        principalColumn: "IdLichHen",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HoSoKham",
                columns: table => new
                {
                    IdHoSoKham = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdLichHen = table.Column<int>(type: "int", nullable: false),
                    IdBacSi = table.Column<int>(type: "int", nullable: false),
                    ChanDoan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KetQuaKham = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayKham = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSoKham", x => x.IdHoSoKham);
                    table.ForeignKey(
                        name: "FK_HoSoKham_BacSi_IdBacSi",
                        column: x => x.IdBacSi,
                        principalTable: "BacSi",
                        principalColumn: "IdBacSi",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HoSoKham_LichHen_IdLichHen",
                        column: x => x.IdLichHen,
                        principalTable: "LichHen",
                        principalColumn: "IdLichHen",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LichSuLichHen",
                columns: table => new
                {
                    IdLichSu = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdLichHen = table.Column<int>(type: "int", nullable: false),
                    HanhDong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdNguoiThucHien = table.Column<int>(type: "int", nullable: true),
                    LyDo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdLichHenTruoc = table.Column<int>(type: "int", nullable: true),
                    DanhDauHuyMuon = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuLichHen", x => x.IdLichSu);
                    table.ForeignKey(
                        name: "FK_LichSuLichHen_LichHen_IdLichHen",
                        column: x => x.IdLichHen,
                        principalTable: "LichHen",
                        principalColumn: "IdLichHen",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichSuLichHen_LichHen_IdLichHenTruoc",
                        column: x => x.IdLichHenTruoc,
                        principalTable: "LichHen",
                        principalColumn: "IdLichHen",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LichSuLichHen_TaiKhoan_IdNguoiThucHien",
                        column: x => x.IdNguoiThucHien,
                        principalTable: "TaiKhoan",
                        principalColumn: "IdTaiKhoan",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ToaThuoc",
                columns: table => new
                {
                    IdToaThuoc = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdHoSoKham = table.Column<int>(type: "int", nullable: false),
                    IdThuoc = table.Column<int>(type: "int", nullable: false),
                    LieuLuong = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CachDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SoNgayDung = table.Column<int>(type: "int", nullable: true),
                    GhiChu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToaThuoc", x => x.IdToaThuoc);
                    table.ForeignKey(
                        name: "FK_ToaThuoc_HoSoKham_IdHoSoKham",
                        column: x => x.IdHoSoKham,
                        principalTable: "HoSoKham",
                        principalColumn: "IdHoSoKham",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ToaThuoc_Thuoc_IdThuoc",
                        column: x => x.IdThuoc,
                        principalTable: "Thuoc",
                        principalColumn: "IdThuoc",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BacSi_IdChuyenKhoa",
                table: "BacSi",
                column: "IdChuyenKhoa");

            migrationBuilder.CreateIndex(
                name: "IX_BacSi_IdTaiKhoan",
                table: "BacSi",
                column: "IdTaiKhoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BenhNhan_Cccd",
                table: "BenhNhan",
                column: "Cccd",
                unique: true,
                filter: "[Cccd] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BenhNhan_IdTaiKhoan",
                table: "BenhNhan",
                column: "IdTaiKhoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CaLamViec_IdAdminDuyet",
                table: "CaLamViec",
                column: "IdAdminDuyet");

            migrationBuilder.CreateIndex(
                name: "IX_CaLamViec_IdBacSi",
                table: "CaLamViec",
                column: "IdBacSi");

            migrationBuilder.CreateIndex(
                name: "IX_CaLamViec_IdBacSiYeuCau",
                table: "CaLamViec",
                column: "IdBacSiYeuCau");

            migrationBuilder.CreateIndex(
                name: "IX_CaLamViec_IdChuyenKhoa",
                table: "CaLamViec",
                column: "IdChuyenKhoa");

            migrationBuilder.CreateIndex(
                name: "IX_CaLamViec_IdDinhNghiaCa",
                table: "CaLamViec",
                column: "IdDinhNghiaCa");

            migrationBuilder.CreateIndex(
                name: "IX_CaLamViec_IdPhong",
                table: "CaLamViec",
                column: "IdPhong");

            migrationBuilder.CreateIndex(
                name: "IX_ChuyenKhoa_TenChuyenKhoa",
                table: "ChuyenKhoa",
                column: "TenChuyenKhoa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DichVu_IdChuyenKhoa",
                table: "DichVu",
                column: "IdChuyenKhoa");

            migrationBuilder.CreateIndex(
                name: "IX_DinhNghiaCa_TenCa",
                table: "DinhNghiaCa",
                column: "TenCa",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DonNghiPhep_IdBacSi",
                table: "DonNghiPhep",
                column: "IdBacSi");

            migrationBuilder.CreateIndex(
                name: "IX_DonNghiPhep_IdCaLamViec",
                table: "DonNghiPhep",
                column: "IdCaLamViec");

            migrationBuilder.CreateIndex(
                name: "IX_DonNghiPhep_IdNguoiDuyet",
                table: "DonNghiPhep",
                column: "IdNguoiDuyet");

            migrationBuilder.CreateIndex(
                name: "IX_GiuCho_IdBenhNhan",
                table: "GiuCho",
                column: "IdBenhNhan");

            migrationBuilder.CreateIndex(
                name: "IX_GiuCho_IdCaLamViec",
                table: "GiuCho",
                column: "IdCaLamViec");

            migrationBuilder.CreateIndex(
                name: "IX_HangCho_IdCaLamViec",
                table: "HangCho",
                column: "IdCaLamViec");

            migrationBuilder.CreateIndex(
                name: "IX_HangCho_IdLichHen",
                table: "HangCho",
                column: "IdLichHen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoSoKham_IdBacSi",
                table: "HoSoKham",
                column: "IdBacSi");

            migrationBuilder.CreateIndex(
                name: "IX_HoSoKham_IdLichHen",
                table: "HoSoKham",
                column: "IdLichHen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeTan_IdTaiKhoan",
                table: "LeTan",
                column: "IdTaiKhoan",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_IdBacSiMongMuon",
                table: "LichHen",
                column: "IdBacSiMongMuon");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_IdBenhNhan",
                table: "LichHen",
                column: "IdBenhNhan");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_IdCaLamViec",
                table: "LichHen",
                column: "IdCaLamViec");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_IdDichVu",
                table: "LichHen",
                column: "IdDichVu");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_MaLichHen",
                table: "LichHen",
                column: "MaLichHen",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichNoiTru_IdBacSi",
                table: "LichNoiTru",
                column: "IdBacSi");

            migrationBuilder.CreateIndex(
                name: "IX_LichNoiTru_IdDinhNghiaCa",
                table: "LichNoiTru",
                column: "IdDinhNghiaCa");

            migrationBuilder.CreateIndex(
                name: "IX_LichNoiTru_IdPhong",
                table: "LichNoiTru",
                column: "IdPhong");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuLichHen_IdLichHen",
                table: "LichSuLichHen",
                column: "IdLichHen");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuLichHen_IdLichHenTruoc",
                table: "LichSuLichHen",
                column: "IdLichHenTruoc");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuLichHen_IdNguoiThucHien",
                table: "LichSuLichHen",
                column: "IdNguoiThucHien");

            migrationBuilder.CreateIndex(
                name: "IX_MauThongBao_LoaiThongBao",
                table: "MauThongBao",
                column: "LoaiThongBao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OtpLog_IdTaiKhoan_MucDich_NgayTao",
                table: "OtpLog",
                columns: new[] { "IdTaiKhoan", "MucDich", "NgayTao" });

            migrationBuilder.CreateIndex(
                name: "IX_Phong_MaPhong",
                table: "Phong",
                column: "MaPhong",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_Email",
                table: "TaiKhoan",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_SoDienThoai",
                table: "TaiKhoan",
                column: "SoDienThoai",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaiKhoan_TenDangNhap",
                table: "TaiKhoan",
                column: "TenDangNhap",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ThongBao_IdMau",
                table: "ThongBao",
                column: "IdMau");

            migrationBuilder.CreateIndex(
                name: "IX_ThongBao_IdTaiKhoan",
                table: "ThongBao",
                column: "IdTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_Thuoc_TenThuoc",
                table: "Thuoc",
                column: "TenThuoc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuoc_IdHoSoKham",
                table: "ToaThuoc",
                column: "IdHoSoKham");

            migrationBuilder.CreateIndex(
                name: "IX_ToaThuoc_IdThuoc",
                table: "ToaThuoc",
                column: "IdThuoc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DonNghiPhep");

            migrationBuilder.DropTable(
                name: "GiuCho");

            migrationBuilder.DropTable(
                name: "HangCho");

            migrationBuilder.DropTable(
                name: "LeTan");

            migrationBuilder.DropTable(
                name: "LichNoiTru");

            migrationBuilder.DropTable(
                name: "LichSuLichHen");

            migrationBuilder.DropTable(
                name: "OtpLog");

            migrationBuilder.DropTable(
                name: "ThongBao");

            migrationBuilder.DropTable(
                name: "ToaThuoc");

            migrationBuilder.DropTable(
                name: "MauThongBao");

            migrationBuilder.DropTable(
                name: "HoSoKham");

            migrationBuilder.DropTable(
                name: "Thuoc");

            migrationBuilder.DropTable(
                name: "LichHen");

            migrationBuilder.DropTable(
                name: "BenhNhan");

            migrationBuilder.DropTable(
                name: "CaLamViec");

            migrationBuilder.DropTable(
                name: "DichVu");

            migrationBuilder.DropTable(
                name: "BacSi");

            migrationBuilder.DropTable(
                name: "DinhNghiaCa");

            migrationBuilder.DropTable(
                name: "Phong");

            migrationBuilder.DropTable(
                name: "ChuyenKhoa");

            migrationBuilder.DropTable(
                name: "TaiKhoan");
        }
    }
}
