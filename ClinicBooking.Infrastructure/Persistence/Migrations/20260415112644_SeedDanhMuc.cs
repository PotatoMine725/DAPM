using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClinicBooking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedDanhMuc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ChuyenKhoa",
                columns: new[] { "IdChuyenKhoa", "GioDongDatLich", "GioMoDatLich", "HienThi", "MoTa", "TenChuyenKhoa", "ThoiGianSlotMacDinh" },
                values: new object[,]
                {
                    { 1, new TimeOnly(17, 0, 0), new TimeOnly(7, 0, 0), true, "Kham va dieu tri cac benh ly ve tim va mach mau", "Tim Mach", 20 },
                    { 2, new TimeOnly(17, 0, 0), new TimeOnly(7, 0, 0), true, "Kham va dieu tri cho tre em duoi 16 tuoi", "Nhi Khoa", 15 },
                    { 3, new TimeOnly(17, 0, 0), new TimeOnly(7, 0, 0), true, "Kham tong quat cac benh ly noi khoa", "Noi Tong Quat", 20 },
                    { 4, new TimeOnly(17, 0, 0), new TimeOnly(7, 0, 0), true, "Kham va dieu tri cac benh ly ngoai khoa", "Ngoai Tong Quat", 30 },
                    { 5, new TimeOnly(17, 0, 0), new TimeOnly(7, 0, 0), true, "Kham va dieu tri cac benh ly tai, mui, hong", "Tai Mui Hong", 15 },
                    { 6, new TimeOnly(17, 0, 0), new TimeOnly(7, 0, 0), true, "Kham va dieu tri cac benh ly ve da", "Da Lieu", 20 }
                });

            migrationBuilder.InsertData(
                table: "DinhNghiaCa",
                columns: new[] { "IdDinhNghiaCa", "GioBatDauMacDinh", "GioKetThucMacDinh", "MoTa", "TenCa", "TrangThai" },
                values: new object[,]
                {
                    { 1, new TimeOnly(7, 0, 0), new TimeOnly(12, 0, 0), "Ca sang: 07:00 - 12:00", "sang", true },
                    { 2, new TimeOnly(13, 0, 0), new TimeOnly(17, 0, 0), "Ca chieu: 13:00 - 17:00", "chieu", true },
                    { 3, new TimeOnly(17, 0, 0), new TimeOnly(21, 0, 0), "Ca toi: 17:00 - 21:00", "toi", true },
                    { 4, new TimeOnly(7, 0, 0), new TimeOnly(17, 0, 0), "Ca gop sang + chieu: 07:00 - 17:00", "sang_chieu", true }
                });

            migrationBuilder.InsertData(
                table: "MauThongBao",
                columns: new[] { "IdMau", "KenhGui", "LoaiThongBao", "NoiDungMau", "TieuDeMau" },
                values: new object[,]
                {
                    { 1, "Email", "XacNhanLich", "Xin chao {ten_benh_nhan}, lich hen cua ban ma {ma_lich_hen} vao ngay {ngay_kham} da duoc xac nhan. Vui long den truoc gio hen 15 phut.", "Xac nhan lich hen {ma_lich_hen}" },
                    { 2, "Email", "Nhac1Ngay", "Xin chao {ten_benh_nhan}, ban co lich hen kham vao ngay mai {ngay_kham} luc {gio_kham}. Ma lich hen: {ma_lich_hen}.", "Nhac lich kham ngay mai" },
                    { 3, "Sms", "Nhac2Gio", "{ten_benh_nhan}, ban co lich kham luc {gio_kham} hom nay. Ma: {ma_lich_hen}.", "Nhac lich kham 2 gio toi" },
                    { 4, "Email", "HuyLich", "Xin chao {ten_benh_nhan}, lich hen {ma_lich_hen} vao ngay {ngay_kham} da duoc huy. Ly do: {ly_do}.", "Huy lich hen {ma_lich_hen}" },
                    { 5, "TrongApp", "CheckIn", "Ban da check-in thanh cong. So thu tu cua ban la {so_thu_tu}. Vui long cho goi ten.", "Check-in thanh cong" },
                    { 6, "TrongApp", "DuyetCa", "Ca lam viec ngay {ngay_lam_viec} cua ban da duoc admin duyet.", "Ca lam viec da duoc duyet" },
                    { 7, "TrongApp", "TuChoiCa", "Ca lam viec ngay {ngay_lam_viec} cua ban da bi admin tu choi. Ly do: {ly_do}.", "Ca lam viec bi tu choi" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ChuyenKhoa",
                keyColumn: "IdChuyenKhoa",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ChuyenKhoa",
                keyColumn: "IdChuyenKhoa",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ChuyenKhoa",
                keyColumn: "IdChuyenKhoa",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "ChuyenKhoa",
                keyColumn: "IdChuyenKhoa",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "ChuyenKhoa",
                keyColumn: "IdChuyenKhoa",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "ChuyenKhoa",
                keyColumn: "IdChuyenKhoa",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "DinhNghiaCa",
                keyColumn: "IdDinhNghiaCa",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "DinhNghiaCa",
                keyColumn: "IdDinhNghiaCa",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "DinhNghiaCa",
                keyColumn: "IdDinhNghiaCa",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "DinhNghiaCa",
                keyColumn: "IdDinhNghiaCa",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 7);
        }
    }
}
