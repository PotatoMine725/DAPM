using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicBooking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Module1_ThemRangBuocLichHen : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LichHen_IdBenhNhan",
                table: "LichHen");

            migrationBuilder.DropIndex(
                name: "IX_LichHen_IdCaLamViec",
                table: "LichHen");

            migrationBuilder.DropIndex(
                name: "IX_HangCho_IdCaLamViec",
                table: "HangCho");

            migrationBuilder.DropIndex(
                name: "IX_GiuCho_IdCaLamViec",
                table: "GiuCho");

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "LichHen",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "MaLichHen",
                table: "LichHen",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "LichHen",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "HangCho",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_IdBenhNhan_TrangThai",
                table: "LichHen",
                columns: new[] { "IdBenhNhan", "TrangThai" });

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_IdCaLamViec_SoSlot",
                table: "LichHen",
                columns: new[] { "IdCaLamViec", "SoSlot" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_IdCaLamViec_TrangThai",
                table: "LichHen",
                columns: new[] { "IdCaLamViec", "TrangThai" });

            migrationBuilder.CreateIndex(
                name: "IX_HangCho_IdCaLamViec_SoThuTu",
                table: "HangCho",
                columns: new[] { "IdCaLamViec", "SoThuTu" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HangCho_IdCaLamViec_TrangThai_SoThuTu",
                table: "HangCho",
                columns: new[] { "IdCaLamViec", "TrangThai", "SoThuTu" });

            migrationBuilder.CreateIndex(
                name: "IX_GiuCho_IdCaLamViec_DaGiaiPhong_GioHetHan",
                table: "GiuCho",
                columns: new[] { "IdCaLamViec", "DaGiaiPhong", "GioHetHan" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LichHen_IdBenhNhan_TrangThai",
                table: "LichHen");

            migrationBuilder.DropIndex(
                name: "IX_LichHen_IdCaLamViec_SoSlot",
                table: "LichHen");

            migrationBuilder.DropIndex(
                name: "IX_LichHen_IdCaLamViec_TrangThai",
                table: "LichHen");

            migrationBuilder.DropIndex(
                name: "IX_HangCho_IdCaLamViec_SoThuTu",
                table: "HangCho");

            migrationBuilder.DropIndex(
                name: "IX_HangCho_IdCaLamViec_TrangThai_SoThuTu",
                table: "HangCho");

            migrationBuilder.DropIndex(
                name: "IX_GiuCho_IdCaLamViec_DaGiaiPhong_GioHetHan",
                table: "GiuCho");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "LichHen");

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "LichHen",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "MaLichHen",
                table: "LichHen",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32);

            migrationBuilder.AlterColumn<string>(
                name: "TrangThai",
                table: "HangCho",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_IdBenhNhan",
                table: "LichHen",
                column: "IdBenhNhan");

            migrationBuilder.CreateIndex(
                name: "IX_LichHen_IdCaLamViec",
                table: "LichHen",
                column: "IdCaLamViec");

            migrationBuilder.CreateIndex(
                name: "IX_HangCho_IdCaLamViec",
                table: "HangCho",
                column: "IdCaLamViec");

            migrationBuilder.CreateIndex(
                name: "IX_GiuCho_IdCaLamViec",
                table: "GiuCho",
                column: "IdCaLamViec");
        }
    }
}
