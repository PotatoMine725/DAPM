using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using ClinicBooking.Infrastructure.Persistence;

namespace ClinicBooking.Application.UnitTests.Common;

/// <summary>
/// Helper seed cac entity tien quyet (TaiKhoan, BenhNhan, CaLamViec, DichVu) cho unit test.
/// Chi dung khi handler can FK that trong DB SQLite.
/// </summary>
internal static class TestDataSeeder
{
    public static TaiKhoan SeedTaiKhoan(
        AppDbContext db,
        VaiTro vaiTro = VaiTro.BenhNhan,
        string? tenDangNhap = null,
        string? email = null,
        string? soDienThoai = null)
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var tk = new TaiKhoan
        {
            TenDangNhap = tenDangNhap ?? $"user_{suffix}",
            Email = email ?? $"u_{suffix}@x.vn",
            SoDienThoai = soDienThoai ?? $"09{suffix[..8]}",
            MatKhau = "hash",
            VaiTro = vaiTro,
            TrangThai = true,
            NgayTao = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        db.TaiKhoan.Add(tk);
        db.SaveChanges();
        return tk;
    }

    public static BenhNhan SeedBenhNhan(
        AppDbContext db,
        int? idTaiKhoan = null,
        string hoTen = "Nguyen Van A",
        bool biHanChe = false)
    {
        var tkId = idTaiKhoan ?? SeedTaiKhoan(db, VaiTro.BenhNhan).IdTaiKhoan;
        var bn = new BenhNhan
        {
            IdTaiKhoan = tkId,
            HoTen = hoTen,
            BiHanChe = biHanChe,
            NgayTao = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        db.BenhNhan.Add(bn);
        db.SaveChanges();
        return bn;
    }

    public static ChuyenKhoa SeedChuyenKhoa(AppDbContext db, string? ten = null)
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];
        var ck = new ChuyenKhoa
        {
            TenChuyenKhoa = ten ?? $"CK_{suffix}",
            ThoiGianSlotMacDinh = 15,
            HienThi = true
        };
        db.ChuyenKhoa.Add(ck);
        db.SaveChanges();
        return ck;
    }

    public static DichVu SeedDichVu(AppDbContext db, int? idChuyenKhoa = null, string ten = "Kham tong quat")
    {
        var ckId = idChuyenKhoa ?? SeedChuyenKhoa(db).IdChuyenKhoa;
        var dv = new DichVu
        {
            IdChuyenKhoa = ckId,
            TenDichVu = ten,
            HienThi = true,
            NgayTao = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        db.DichVu.Add(dv);
        db.SaveChanges();
        return dv;
    }

    public static BacSi SeedBacSi(AppDbContext db, int? idChuyenKhoa = null)
    {
        var tk = SeedTaiKhoan(db, VaiTro.BacSi);
        var ckId = idChuyenKhoa ?? SeedChuyenKhoa(db).IdChuyenKhoa;
        var bs = new BacSi
        {
            IdTaiKhoan = tk.IdTaiKhoan,
            IdChuyenKhoa = ckId,
            HoTen = "BS Demo",
            LoaiHopDong = LoaiHopDong.NoiTru,
            TrangThai = TrangThaiBacSi.DangLam,
            NgayTao = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        db.BacSi.Add(bs);
        db.SaveChanges();
        return bs;
    }

    public static Phong SeedPhong(AppDbContext db)
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];
        var p = new Phong
        {
            MaPhong = $"P{suffix}",
            TenPhong = "Phong test",
            TrangThai = true
        };
        db.Phong.Add(p);
        db.SaveChanges();
        return p;
    }

    public static DinhNghiaCa SeedDinhNghiaCa(AppDbContext db)
    {
        var suffix = Guid.NewGuid().ToString("N")[..6];
        var dn = new DinhNghiaCa
        {
            TenCa = $"Ca_{suffix}",
            GioBatDauMacDinh = new TimeOnly(8, 0),
            GioKetThucMacDinh = new TimeOnly(11, 0),
            TrangThai = true
        };
        db.DinhNghiaCa.Add(dn);
        db.SaveChanges();
        return dn;
    }

    /// <summary>
    /// Seed mot CaLamViec day du graph (BacSi, Phong, ChuyenKhoa, DinhNghiaCa).
    /// Mac dinh TrangThaiDuyet = DaDuyet, NgayLamViec = 2026-05-05, 08:00-11:00, 10 slot, da dat 0.
    /// </summary>
    public static CaLamViec SeedCaLamViec(
        AppDbContext db,
        DateOnly? ngayLamViec = null,
        TrangThaiDuyetCa trangThai = TrangThaiDuyetCa.DaDuyet,
        int soSlotToiDa = 10,
        int soSlotDaDat = 0,
        TimeOnly? gioBatDau = null)
    {
        var ck = SeedChuyenKhoa(db);
        var bs = SeedBacSi(db, ck.IdChuyenKhoa);
        var phong = SeedPhong(db);
        var dnCa = SeedDinhNghiaCa(db);

        var ca = new CaLamViec
        {
            IdBacSi = bs.IdBacSi,
            IdPhong = phong.IdPhong,
            IdChuyenKhoa = ck.IdChuyenKhoa,
            IdDinhNghiaCa = dnCa.IdDinhNghiaCa,
            NgayLamViec = ngayLamViec ?? new DateOnly(2026, 5, 5),
            GioBatDau = gioBatDau ?? new TimeOnly(8, 0),
            GioKetThuc = new TimeOnly(11, 0),
            ThoiGianSlot = 15,
            SoSlotToiDa = soSlotToiDa,
            SoSlotDaDat = soSlotDaDat,
            TrangThaiDuyet = trangThai,
            NguonTaoCa = NguonTaoCa.TuDong,
            NgayTao = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };
        db.CaLamViec.Add(ca);
        db.SaveChanges();
        return ca;
    }

    /// <summary>
    /// Seed mot LichHen co kem CaLamViec + BenhNhan + DichVu. Tra ve entity da duoc save.
    /// </summary>
    public static LichHen SeedLichHen(
        AppDbContext db,
        int idBenhNhan,
        int idCaLamViec,
        int? idDichVu = null,
        int soSlot = 1,
        TrangThaiLichHen trangThai = TrangThaiLichHen.ChoXacNhan,
        string? maLichHen = null)
    {
        var dv = idDichVu ?? SeedDichVu(db).IdDichVu;
        var lh = new LichHen
        {
            MaLichHen = maLichHen ?? $"LH-{Guid.NewGuid().ToString("N")[..10]}",
            IdBenhNhan = idBenhNhan,
            IdCaLamViec = idCaLamViec,
            IdDichVu = dv,
            SoSlot = soSlot,
            HinhThucDat = HinhThucDat.TrucTuyen,
            TrangThai = trangThai,
            NgayTao = new DateTime(2026, 4, 1, 10, 0, 0, DateTimeKind.Utc),
            // SQLite khong auto-gen rowversion — set tay de qua NOT NULL constraint.
            RowVersion = new byte[] { 0, 0, 0, 0, 0, 0, 0, 1 }
        };
        db.LichHen.Add(lh);
        db.SaveChanges();
        return lh;
    }
}
