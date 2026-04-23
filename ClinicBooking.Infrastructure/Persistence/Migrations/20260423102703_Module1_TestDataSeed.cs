using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClinicBooking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Module1_TestDataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "TaiKhoan",
                columns: new[] { "IdTaiKhoan", "Email", "LanDangNhapCuoi", "MatKhau", "NgayTao", "SoDienThoai", "TenDangNhap", "TrangThai", "VaiTro" },
                values: new object[,]
                {
                    { 2001, "patient@test.vn", null, "$2a$11$encrypted_password", new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), "0912345678", "patient001", true, "BenhNhan" },
                    { 2002, "doctor@test.vn", null, "$2a$11$encrypted_password", new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), "0987654321", "doctor001", true, "BacSi" },
                    { 2003, "receptionist@test.vn", null, "$2a$11$encrypted_password", new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), "0911111111", "receptionist001", true, "LeTan" },
                    { 2004, "admin@test.vn", null, "$2a$11$encrypted_password", new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), "0988888888", "admin001", true, "Admin" }
                });

            migrationBuilder.InsertData(
                table: "BacSi",
                columns: new[] { "IdBacSi", "AnhDaiDien", "BangCap", "HoTen", "IdChuyenKhoa", "IdTaiKhoan", "LoaiHopDong", "NamKinhNghiem", "NgayTao", "TieuSu", "TrangThai" },
                values: new object[] { 2001, "https://via.placeholder.com/150", "Bac Si", "Dr. Nguyen Van A", 1, 2002, "NoiTru", 10, new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), "Bac si chuyen khoa tim mach", "DangLam" });

            migrationBuilder.InsertData(
                table: "BenhNhan",
                columns: new[] { "IdBenhNhan", "Cccd", "DiaChi", "GioiTinh", "HoTen", "IdTaiKhoan", "NgayHetHanChe", "NgaySinh", "NgayTao" },
                values: new object[] { 2001, "123456789012", "123 Duong ABC, TP. HCM", "Nu", "Tran Thi B", 2001, null, new DateOnly(1990, 5, 15), new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "CaLamViec",
                columns: new[] { "IdCaLamViec", "GioBatDau", "GioKetThuc", "IdAdminDuyet", "IdBacSi", "IdBacSiYeuCau", "IdChuyenKhoa", "IdDinhNghiaCa", "IdPhong", "LyDoTuChoi", "NgayDuyet", "NgayLamViec", "NgayTao", "NguonTaoCa", "SoSlotToiDa", "ThoiGianSlot", "TrangThaiDuyet" },
                values: new object[,]
                {
                    { 3001, new TimeOnly(7, 0, 0), new TimeOnly(12, 0, 0), 2004, 2001, null, 1, 1, 1, null, new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(2026, 4, 24), new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), "TuDong", 15, 20, "DaDuyet" },
                    { 3002, new TimeOnly(13, 0, 0), new TimeOnly(17, 0, 0), 2004, 2001, null, 1, 2, 1, null, new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(2026, 4, 24), new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), "TuDong", 12, 20, "DaDuyet" },
                    { 3003, new TimeOnly(7, 0, 0), new TimeOnly(12, 0, 0), 2004, 2001, null, 1, 1, 1, null, new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), new DateOnly(2026, 4, 30), new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), "TuDong", 15, 20, "DaDuyet" }
                });

            migrationBuilder.InsertData(
                table: "LichHen",
                columns: new[] { "IdLichHen", "BacSiMongMuonNote", "HinhThucDat", "IdBacSiMongMuon", "IdBenhNhan", "IdCaLamViec", "IdDichVu", "MaLichHen", "NgayTao", "SoSlot", "TrangThai", "TrieuChung" },
                values: new object[,]
                {
                    { 4001, null, "TrucTuyen", null, 2001, 3001, 1, "LH-20260424-001", new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), 1, "DaXacNhan", "Dau nguc nhe" },
                    { 4002, null, "TrucTuyen", null, 2001, 3001, 2, "LH-20260424-002", new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc), 2, "ChoXacNhan", null }
                });

            migrationBuilder.InsertData(
                table: "HangCho",
                columns: new[] { "IdHangCho", "IdCaLamViec", "IdLichHen", "NgayCheckIn", "SoThuTu", "TrangThai" },
                values: new object[] { 5001, 3001, 4001, new DateTime(2026, 4, 22, 22, 0, 0, 0, DateTimeKind.Utc), 1, "ChoKham" });

            migrationBuilder.InsertData(
                table: "LichSuLichHen",
                columns: new[] { "IdLichSu", "HanhDong", "IdLichHen", "IdLichHenTruoc", "IdNguoiThucHien", "LyDo", "NgayTao" },
                values: new object[,]
                {
                    { 6001, "DatMoi", 4001, null, null, null, new DateTime(2026, 4, 23, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6002, "XacNhan", 4001, null, 2004, "Admin xac nhan", new DateTime(2026, 4, 23, 0, 5, 0, 0, DateTimeKind.Utc) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CaLamViec",
                keyColumn: "IdCaLamViec",
                keyValue: 3002);

            migrationBuilder.DeleteData(
                table: "CaLamViec",
                keyColumn: "IdCaLamViec",
                keyValue: 3003);

            migrationBuilder.DeleteData(
                table: "HangCho",
                keyColumn: "IdHangCho",
                keyValue: 5001);

            migrationBuilder.DeleteData(
                table: "LichHen",
                keyColumn: "IdLichHen",
                keyValue: 4002);

            migrationBuilder.DeleteData(
                table: "LichSuLichHen",
                keyColumn: "IdLichSu",
                keyValue: 6001);

            migrationBuilder.DeleteData(
                table: "LichSuLichHen",
                keyColumn: "IdLichSu",
                keyValue: 6002);

            migrationBuilder.DeleteData(
                table: "TaiKhoan",
                keyColumn: "IdTaiKhoan",
                keyValue: 2003);

            migrationBuilder.DeleteData(
                table: "LichHen",
                keyColumn: "IdLichHen",
                keyValue: 4001);

            migrationBuilder.DeleteData(
                table: "BenhNhan",
                keyColumn: "IdBenhNhan",
                keyValue: 2001);

            migrationBuilder.DeleteData(
                table: "CaLamViec",
                keyColumn: "IdCaLamViec",
                keyValue: 3001);

            migrationBuilder.DeleteData(
                table: "BacSi",
                keyColumn: "IdBacSi",
                keyValue: 2001);

            migrationBuilder.DeleteData(
                table: "TaiKhoan",
                keyColumn: "IdTaiKhoan",
                keyValue: 2001);

            migrationBuilder.DeleteData(
                table: "TaiKhoan",
                keyColumn: "IdTaiKhoan",
                keyValue: 2004);

            migrationBuilder.DeleteData(
                table: "TaiKhoan",
                keyColumn: "IdTaiKhoan",
                keyValue: 2002);
        }
    }
}
