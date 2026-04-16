using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClinicBooking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ThemRefreshToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefreshToken",
                columns: table => new
                {
                    IdRefreshToken = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdTaiKhoan = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    HetHan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DaThuHoi = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    NgayThuHoi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LyDoThuHoi = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ThayTheBangToken = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshToken", x => x.IdRefreshToken);
                    table.ForeignKey(
                        name: "FK_RefreshToken_TaiKhoan_IdTaiKhoan",
                        column: x => x.IdTaiKhoan,
                        principalTable: "TaiKhoan",
                        principalColumn: "IdTaiKhoan",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DichVu",
                columns: new[] { "IdDichVu", "HienThi", "IdChuyenKhoa", "MoTa", "NgayTao", "TenDichVu", "ThoiGianUocTinh" },
                values: new object[,]
                {
                    { 1, true, 1, "Kham lam sang tim mach tong quat", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kham Tim Mach Tong Quat", 20 },
                    { 2, true, 1, "Do dien tim 12 chuyen dao", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Dien Tim (ECG)", 15 },
                    { 3, true, 1, "Sieu am tim qua thanh nguc", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Sieu Am Tim", 30 },
                    { 4, true, 2, "Kham tong quat cho tre em", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kham Nhi Tong Quat", 15 },
                    { 5, true, 2, "Tiem vaccine theo lich", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tiem Chung", 10 },
                    { 6, true, 3, "Kham tong quat cac benh ly noi khoa", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kham Noi Tong Quat", 20 },
                    { 7, true, 4, "Kham lam sang ngoai khoa", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kham Ngoai Tong Quat", 20 },
                    { 8, true, 4, "Tieu phau cac ca don gian", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tieu Phau", 45 },
                    { 9, true, 5, "Kham lam sang tai, mui, hong", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kham Tai Mui Hong", 15 },
                    { 10, true, 5, "Noi soi chan doan tai, mui, hong", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Noi Soi Tai Mui Hong", 30 },
                    { 11, true, 6, "Kham cac benh ly ve da", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Kham Da Lieu", 20 },
                    { 12, true, 6, "Soi da chan doan bang dermoscope", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Soi Da Bang Dermoscope", 20 }
                });

            migrationBuilder.InsertData(
                table: "Phong",
                columns: new[] { "IdPhong", "MaPhong", "SucChua", "TenPhong", "TrangBi", "TrangThai" },
                values: new object[,]
                {
                    { 1, "P101", 10, "Phong Kham 101", "Giuong kham, may do huyet ap, ong nghe", true },
                    { 2, "P102", 10, "Phong Kham 102", "Giuong kham, may do huyet ap, ong nghe", true },
                    { 3, "P201", 8, "Phong Kham Nhi 201", "Giuong kham tre em, can do chuyen dung", true },
                    { 4, "P202", 8, "Phong Kham 202", "Giuong kham, may do huyet ap", true },
                    { 5, "P301", 5, "Phong Sieu Am", "May sieu am 4D", true },
                    { 6, "P302", 5, "Phong X-quang", "May X-quang ky thuat so", true }
                });

            migrationBuilder.InsertData(
                table: "TaiKhoan",
                columns: new[] { "IdTaiKhoan", "Email", "LanDangNhapCuoi", "MatKhau", "NgayTao", "SoDienThoai", "TenDangNhap", "TrangThai", "VaiTro" },
                values: new object[] { 1, "admin@phongkham.local", null, "$2a$11$GiaLapHashThayDoiTruocKhiDeployProdXXXXXXXXXXXXXXXXXXXX", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "0900000000", "admin", true, "Admin" });

            migrationBuilder.InsertData(
                table: "Thuoc",
                columns: new[] { "IdThuoc", "DonVi", "GhiChu", "HoatChat", "TenThuoc" },
                values: new object[,]
                {
                    { 1, "Vien", "Giam dau, ha sot", "Paracetamol", "Paracetamol 500mg" },
                    { 2, "Vien", "Khang sinh nhom Beta-lactam", "Amoxicillin", "Amoxicillin 500mg" },
                    { 3, "Vien", "Khang viem, giam dau NSAID", "Ibuprofen", "Ibuprofen 400mg" },
                    { 4, "Vien", "Uc che bom proton, dieu tri viem loet da day", "Omeprazole", "Omeprazole 20mg" },
                    { 5, "Vien", "Khang histamin H1, di ung", "Cetirizine", "Cetirizine 10mg" },
                    { 6, "Vien", "Bo sung vitamin C", "Ascorbic Acid", "Vitamin C 500mg" },
                    { 7, "Chai", "Rua mat, rua mui", "NaCl 0.9%", "Nuoc Muoi Sinh Ly 0.9%" },
                    { 8, "Vien", "Khang histamin H1 the he 2", "Loratadine", "Loratadine 10mg" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_IdTaiKhoan",
                table: "RefreshToken",
                column: "IdTaiKhoan");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshToken_Token",
                table: "RefreshToken",
                column: "Token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshToken");

            migrationBuilder.DeleteData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Phong",
                keyColumn: "IdPhong",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Phong",
                keyColumn: "IdPhong",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Phong",
                keyColumn: "IdPhong",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Phong",
                keyColumn: "IdPhong",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Phong",
                keyColumn: "IdPhong",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Phong",
                keyColumn: "IdPhong",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TaiKhoan",
                keyColumn: "IdTaiKhoan",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 8);
        }
    }
}
