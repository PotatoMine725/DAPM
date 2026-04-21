using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using ClinicBooking.Infrastructure.Persistence;

namespace ClinicBooking.Application.UnitTests.Common;

public static class HoSoKhamTestDataSeeder
{
    public sealed record DuLieuCoBan(
        int IdTaiKhoanBacSi,
        int IdBacSi,
        int IdTaiKhoanBenhNhan,
        int IdBenhNhan,
        int IdTaiKhoanBenhNhanKhac,
        int IdBenhNhanKhac,
        int IdLichHen);

    public static async Task<DuLieuCoBan> TaoAsync(AppDbContext db, DateTime now)
    {
        var chuyenKhoa = new ChuyenKhoa
        {
            TenChuyenKhoa = $"Noi Tong Quat {Guid.NewGuid():N}",
            ThoiGianSlotMacDinh = 20,
            HienThi = true
        };
        db.ChuyenKhoa.Add(chuyenKhoa);
        await db.SaveChangesAsync();

        var phong = new Phong
        {
            MaPhong = $"P{Random.Shared.Next(100, 999)}",
            TenPhong = "Phong 1",
            TrangThai = true
        };
        db.Phong.Add(phong);

        var dinhNghiaCa = new DinhNghiaCa
        {
            TenCa = $"Ca Sang {Guid.NewGuid():N}",
            GioBatDauMacDinh = new TimeOnly(8, 0),
            GioKetThucMacDinh = new TimeOnly(11, 0),
            TrangThai = true
        };
        db.DinhNghiaCa.Add(dinhNghiaCa);
        await db.SaveChangesAsync();

        var dichVu = new DichVu
        {
            IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
            TenDichVu = $"Kham tong quat {Guid.NewGuid():N}",
            HienThi = true,
            NgayTao = now
        };
        db.DichVu.Add(dichVu);

        var taiKhoanBacSi = new TaiKhoan
        {
            TenDangNhap = $"bacsi_{Guid.NewGuid():N}",
            Email = $"bacsi_{Guid.NewGuid():N}@example.com",
            SoDienThoai = $"09{Random.Shared.Next(10000000, 99999999)}",
            MatKhau = "hash",
            VaiTro = VaiTro.BacSi,
            TrangThai = true,
            NgayTao = now
        };
        db.TaiKhoan.Add(taiKhoanBacSi);

        var taiKhoanBenhNhan = new TaiKhoan
        {
            TenDangNhap = $"benhnhan_{Guid.NewGuid():N}",
            Email = $"benhnhan_{Guid.NewGuid():N}@example.com",
            SoDienThoai = $"09{Random.Shared.Next(10000000, 99999999)}",
            MatKhau = "hash",
            VaiTro = VaiTro.BenhNhan,
            TrangThai = true,
            NgayTao = now
        };
        db.TaiKhoan.Add(taiKhoanBenhNhan);

        var taiKhoanBenhNhanKhac = new TaiKhoan
        {
            TenDangNhap = $"benhnhan_{Guid.NewGuid():N}",
            Email = $"benhnhan_{Guid.NewGuid():N}@example.com",
            SoDienThoai = $"09{Random.Shared.Next(10000000, 99999999)}",
            MatKhau = "hash",
            VaiTro = VaiTro.BenhNhan,
            TrangThai = true,
            NgayTao = now
        };
        db.TaiKhoan.Add(taiKhoanBenhNhanKhac);

        await db.SaveChangesAsync();

        var bacSi = new BacSi
        {
            IdTaiKhoan = taiKhoanBacSi.IdTaiKhoan,
            IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
            HoTen = "Bac Si A",
            LoaiHopDong = LoaiHopDong.HopDong,
            TrangThai = TrangThaiBacSi.DangLam,
            NgayTao = now
        };
        db.BacSi.Add(bacSi);

        var benhNhan = new BenhNhan
        {
            IdTaiKhoan = taiKhoanBenhNhan.IdTaiKhoan,
            HoTen = "Benh Nhan A",
            NgayTao = now
        };
        db.BenhNhan.Add(benhNhan);

        var benhNhanKhac = new BenhNhan
        {
            IdTaiKhoan = taiKhoanBenhNhanKhac.IdTaiKhoan,
            HoTen = "Benh Nhan B",
            NgayTao = now
        };
        db.BenhNhan.Add(benhNhanKhac);
        await db.SaveChangesAsync();

        var caLamViec = new CaLamViec
        {
            IdBacSi = bacSi.IdBacSi,
            IdPhong = phong.IdPhong,
            IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
            IdDinhNghiaCa = dinhNghiaCa.IdDinhNghiaCa,
            NgayLamViec = DateOnly.FromDateTime(now),
            GioBatDau = new TimeOnly(8, 0),
            GioKetThuc = new TimeOnly(11, 0),
            ThoiGianSlot = 20,
            SoSlotToiDa = 12,
            SoSlotDaDat = 1,
            TrangThaiDuyet = TrangThaiDuyetCa.DaDuyet,
            NguonTaoCa = NguonTaoCa.TuDong,
            NgayTao = now
        };
        db.CaLamViec.Add(caLamViec);
        await db.SaveChangesAsync();

        var lichHen = new LichHen
        {
            MaLichHen = $"LH-{now:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}",
            IdBenhNhan = benhNhan.IdBenhNhan,
            IdCaLamViec = caLamViec.IdCaLamViec,
            IdDichVu = dichVu.IdDichVu,
            SoSlot = 1,
            HinhThucDat = HinhThucDat.TrucTuyen,
            TrangThai = TrangThaiLichHen.DangKham,
            NgayTao = now
        };
        db.LichHen.Add(lichHen);
        await db.SaveChangesAsync();

        return new DuLieuCoBan(
            taiKhoanBacSi.IdTaiKhoan,
            bacSi.IdBacSi,
            taiKhoanBenhNhan.IdTaiKhoan,
            benhNhan.IdBenhNhan,
            taiKhoanBenhNhanKhac.IdTaiKhoan,
            benhNhanKhac.IdBenhNhan,
            lichHen.IdLichHen);
    }
}
