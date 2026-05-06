using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicBooking.Infrastructure.Persistence.Migrations
{
    public partial class SeedCaLamViecHomNay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // CaLamViec 3004/3005: hom nay (today placeholder).
            // DatabaseSeeder.RefreshCaLamViecDatesAsync cap nhat NgayLamViec sang today() moi lan startup.
            migrationBuilder.InsertData(
                table: "CaLamViec",
                columns: new[] { "IdCaLamViec", "GioBatDau", "GioKetThuc", "IdAdminDuyet", "IdBacSi", "IdBacSiYeuCau", "IdChuyenKhoa", "IdDinhNghiaCa", "IdPhong", "LyDoTuChoi", "NgayDuyet", "NgayLamViec", "NgayTao", "NguonTaoCa", "SoSlotToiDa", "ThoiGianSlot", "TrangThaiDuyet" },
                values: new object[,]
                {
                    { 3004, new TimeOnly(7, 0, 0), new TimeOnly(12, 0, 0), 2004, 2001, null, 1, 1, 1, null, new DateTime(2026, 5, 7, 0, 0, 0, DateTimeKind.Utc), new DateOnly(2026, 5, 7), new DateTime(2026, 5, 7, 0, 0, 0, DateTimeKind.Utc), "TuDong", 15, 20, "DaDuyet" },
                    { 3005, new TimeOnly(13, 0, 0), new TimeOnly(17, 0, 0), 2004, 2001, null, 1, 2, 1, null, new DateTime(2026, 5, 7, 0, 0, 0, DateTimeKind.Utc), new DateOnly(2026, 5, 7), new DateTime(2026, 5, 7, 0, 0, 0, DateTimeKind.Utc), "TuDong", 12, 20, "DaDuyet" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "CaLamViec", keyColumn: "IdCaLamViec", keyValue: 3005);
            migrationBuilder.DeleteData(table: "CaLamViec", keyColumn: "IdCaLamViec", keyValue: 3004);
        }
    }
}
