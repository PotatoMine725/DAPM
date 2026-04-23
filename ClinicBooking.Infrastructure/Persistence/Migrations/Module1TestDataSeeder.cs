using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Infrastructure.Persistence.Migrations;

public static class Module1TestDataSeeder
{
    public static void SeedModule1TestData(ModelBuilder modelBuilder)
    {
        var now = new DateTime(2026, 4, 23, 0, 0, 0, DateTimeKind.Utc);
        var today = DateOnly.FromDateTime(now);
        var tomorrowDate = today.AddDays(1);
        var nextWeekDate = today.AddDays(7);

        modelBuilder.Entity<TaiKhoan>().HasData(
            new TaiKhoan
            {
                IdTaiKhoan = 2001,
                TenDangNhap = "patient001",
                Email = "patient@test.vn",
                SoDienThoai = "0912345678",
                MatKhau = "$2a$11$encrypted_password",
                VaiTro = VaiTro.BenhNhan,
                TrangThai = true,
                LanDangNhapCuoi = null,
                NgayTao = now
            },
            new TaiKhoan
            {
                IdTaiKhoan = 2002,
                TenDangNhap = "doctor001",
                Email = "doctor@test.vn",
                SoDienThoai = "0987654321",
                MatKhau = "$2a$11$encrypted_password",
                VaiTro = VaiTro.BacSi,
                TrangThai = true,
                LanDangNhapCuoi = null,
                NgayTao = now
            },
            new TaiKhoan
            {
                IdTaiKhoan = 2003,
                TenDangNhap = "receptionist001",
                Email = "receptionist@test.vn",
                SoDienThoai = "0911111111",
                MatKhau = "$2a$11$encrypted_password",
                VaiTro = VaiTro.LeTan,
                TrangThai = true,
                LanDangNhapCuoi = null,
                NgayTao = now
            },
            new TaiKhoan
            {
                IdTaiKhoan = 2004,
                TenDangNhap = "admin001",
                Email = "admin@test.vn",
                SoDienThoai = "0988888888",
                MatKhau = "$2a$11$encrypted_password",
                VaiTro = VaiTro.Admin,
                TrangThai = true,
                LanDangNhapCuoi = null,
                NgayTao = now
            });

        modelBuilder.Entity<BacSi>().HasData(
            new BacSi
            {
                IdBacSi = 2001,
                IdTaiKhoan = 2002,
                IdChuyenKhoa = 1,
                HoTen = "Dr. Nguyen Van A",
                AnhDaiDien = "https://via.placeholder.com/150",
                BangCap = "Bac Si",
                NamKinhNghiem = 10,
                TieuSu = "Bac si chuyen khoa tim mach",
                LoaiHopDong = LoaiHopDong.NoiTru,
                TrangThai = TrangThaiBacSi.DangLam,
                NgayTao = now
            });

        modelBuilder.Entity<CaLamViec>().HasData(
            new CaLamViec
            {
                IdCaLamViec = 3001,
                IdBacSi = 2001,
                IdPhong = 1,
                IdChuyenKhoa = 1,
                IdDinhNghiaCa = 1,
                NgayLamViec = tomorrowDate,
                GioBatDau = new TimeOnly(7, 0),
                GioKetThuc = new TimeOnly(12, 0),
                ThoiGianSlot = 20,
                SoSlotToiDa = 15,
                SoSlotDaDat = 0,
                TrangThaiDuyet = TrangThaiDuyetCa.DaDuyet,
                NguonTaoCa = NguonTaoCa.TuDong,
                IdBacSiYeuCau = null,
                IdAdminDuyet = 2004,
                LyDoTuChoi = null,
                NgayDuyet = now,
                NgayTao = now
            },
            new CaLamViec
            {
                IdCaLamViec = 3002,
                IdBacSi = 2001,
                IdPhong = 1,
                IdChuyenKhoa = 1,
                IdDinhNghiaCa = 2,
                NgayLamViec = tomorrowDate,
                GioBatDau = new TimeOnly(13, 0),
                GioKetThuc = new TimeOnly(17, 0),
                ThoiGianSlot = 20,
                SoSlotToiDa = 12,
                SoSlotDaDat = 0,
                TrangThaiDuyet = TrangThaiDuyetCa.DaDuyet,
                NguonTaoCa = NguonTaoCa.TuDong,
                IdBacSiYeuCau = null,
                IdAdminDuyet = 2004,
                LyDoTuChoi = null,
                NgayDuyet = now,
                NgayTao = now
            },
            new CaLamViec
            {
                IdCaLamViec = 3003,
                IdBacSi = 2001,
                IdPhong = 1,
                IdChuyenKhoa = 1,
                IdDinhNghiaCa = 1,
                NgayLamViec = nextWeekDate,
                GioBatDau = new TimeOnly(7, 0),
                GioKetThuc = new TimeOnly(12, 0),
                ThoiGianSlot = 20,
                SoSlotToiDa = 15,
                SoSlotDaDat = 0,
                TrangThaiDuyet = TrangThaiDuyetCa.DaDuyet,
                NguonTaoCa = NguonTaoCa.TuDong,
                IdBacSiYeuCau = null,
                IdAdminDuyet = 2004,
                LyDoTuChoi = null,
                NgayDuyet = now,
                NgayTao = now
            });

        modelBuilder.Entity<BenhNhan>().HasData(
            new BenhNhan
            {
                IdBenhNhan = 2001,
                IdTaiKhoan = 2001,
                HoTen = "Tran Thi B",
                Cccd = "123456789012",
                NgaySinh = new DateOnly(1990, 5, 15),
                GioiTinh = GioiTinh.Nu,
                DiaChi = "123 Duong ABC, TP. HCM",
                SoLanHuyMuon = 0,
                BiHanChe = false,
                NgayHetHanChe = null,
                NgayTao = now
            });

        modelBuilder.Entity<LichHen>().HasData(
            new LichHen
            {
                IdLichHen = 4001,
                MaLichHen = $"LH-{tomorrowDate:yyyyMMdd}-001",
                IdBenhNhan = 2001,
                IdCaLamViec = 3001,
                IdDichVu = 1,
                SoSlot = 1,
                HinhThucDat = HinhThucDat.TrucTuyen,
                BacSiMongMuonNote = null,
                IdBacSiMongMuon = null,
                TrieuChung = "Dau nguc nhe",
                TrangThai = TrangThaiLichHen.DaXacNhan,
                NgayTao = now,
                RowVersion = []
            },
            new LichHen
            {
                IdLichHen = 4002,
                MaLichHen = $"LH-{tomorrowDate:yyyyMMdd}-002",
                IdBenhNhan = 2001,
                IdCaLamViec = 3001,
                IdDichVu = 2,
                SoSlot = 2,
                HinhThucDat = HinhThucDat.TrucTuyen,
                BacSiMongMuonNote = null,
                IdBacSiMongMuon = null,
                TrieuChung = null,
                TrangThai = TrangThaiLichHen.ChoXacNhan,
                NgayTao = now,
                RowVersion = []
            });

        modelBuilder.Entity<HangCho>().HasData(
            new HangCho
            {
                IdHangCho = 5001,
                IdCaLamViec = 3001,
                IdLichHen = 4001,
                SoThuTu = 1,
                TrangThai = TrangThaiHangCho.ChoKham,
                NgayCheckIn = now.AddHours(-2)
            });

        modelBuilder.Entity<LichSuLichHen>().HasData(
            new LichSuLichHen
            {
                IdLichSu = 6001,
                IdLichHen = 4001,
                HanhDong = HanhDongLichSu.DatMoi,
                IdNguoiThucHien = null,
                LyDo = null,
                IdLichHenTruoc = null,
                DanhDauHuyMuon = false,
                NgayTao = now
            },
            new LichSuLichHen
            {
                IdLichSu = 6002,
                IdLichHen = 4001,
                HanhDong = HanhDongLichSu.XacNhan,
                IdNguoiThucHien = 2004,
                LyDo = "Admin xac nhan",
                IdLichHenTruoc = null,
                DanhDauHuyMuon = false,
                NgayTao = now.AddMinutes(5)
            });
    }
}
