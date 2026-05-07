using Microsoft.EntityFrameworkCore.Migrations;

namespace ClinicBooking.Infrastructure.Migrations;

public partial class FixPatientPassword : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            "UPDATE TaiKhoan SET MatKhau = '$2a$11$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj6ukx.LrUpm' WHERE TenDangNhap = 'patient001'");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            "UPDATE TaiKhoan SET MatKhau = '$2a$11$encrypted_password' WHERE TenDangNhap = 'patient001'");
    }
}
