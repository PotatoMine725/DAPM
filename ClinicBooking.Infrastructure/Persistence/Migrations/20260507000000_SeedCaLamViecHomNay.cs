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
            migrationBuilder.Sql(@"
                SET IDENTITY_INSERT CaLamViec ON;

                IF NOT EXISTS (SELECT 1 FROM CaLamViec WHERE IdCaLamViec = 3004)
                INSERT INTO CaLamViec (IdCaLamViec, GioBatDau, GioKetThuc, IdAdminDuyet, IdBacSi, IdBacSiYeuCau, IdChuyenKhoa, IdDinhNghiaCa, IdPhong, LyDoTuChoi, NgayDuyet, NgayLamViec, NgayTao, NguonTaoCa, SoSlotToiDa, ThoiGianSlot, TrangThaiDuyet)
                VALUES (3004, '07:00:00', '12:00:00', 2004, 2001, NULL, 1, 1, 1, NULL, '2026-05-07', CAST(GETDATE() AS DATE), GETUTCDATE(), 'TuDong', 15, 20, 'DaDuyet');

                IF NOT EXISTS (SELECT 1 FROM CaLamViec WHERE IdCaLamViec = 3005)
                INSERT INTO CaLamViec (IdCaLamViec, GioBatDau, GioKetThuc, IdAdminDuyet, IdBacSi, IdBacSiYeuCau, IdChuyenKhoa, IdDinhNghiaCa, IdPhong, LyDoTuChoi, NgayDuyet, NgayLamViec, NgayTao, NguonTaoCa, SoSlotToiDa, ThoiGianSlot, TrangThaiDuyet)
                VALUES (3005, '13:00:00', '17:00:00', 2004, 2001, NULL, 1, 2, 1, NULL, '2026-05-07', CAST(GETDATE() AS DATE), GETUTCDATE(), 'TuDong', 12, 20, 'DaDuyet');

                SET IDENTITY_INSERT CaLamViec OFF;
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "CaLamViec", keyColumn: "IdCaLamViec", keyValue: 3005);
            migrationBuilder.DeleteData(table: "CaLamViec", keyColumn: "IdCaLamViec", keyValue: 3004);
        }
    }
}
