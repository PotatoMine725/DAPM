using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.DanhMuc.Commands.XoaDinhNghiaCa;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using FluentAssertions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.UnitTests.Features.DanhMuc.Commands.TaoDinhNghiaCa;

public sealed class XoaDinhNghiaCaHandlerTests
{
    [Fact]
    public async Task Handle_KhongDuocSuDung_XoaThanhCong()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var entity = new DinhNghiaCa
        {
            TenCa = "Ca de xoa",
            GioBatDauMacDinh = new TimeOnly(9, 0),
            GioKetThucMacDinh = new TimeOnly(11, 0),
            TrangThai = true
        };

        db.DinhNghiaCa.Add(entity);
        await db.SaveChangesAsync();

        var handler = new XoaDinhNghiaCaHandler(db);
        var result = await handler.Handle(new XoaDinhNghiaCaCommand(entity.IdDinhNghiaCa), CancellationToken.None);

        result.Should().Be(Unit.Value);
        var conTonTai = await db.DinhNghiaCa.AnyAsync(x => x.IdDinhNghiaCa == entity.IdDinhNghiaCa);
        conTonTai.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_DangDuocSuDung_ThrowConflictException()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var chuyenKhoa = new ChuyenKhoa
        {
            TenChuyenKhoa = "CK Test",
            ThoiGianSlotMacDinh = 20,
            HienThi = true
        };

        var phong = new Phong
        {
            MaPhong = "P-UT-01",
            TenPhong = "Phong Test",
            TrangThai = true
        };

        var dinhNghiaCa = new DinhNghiaCa
        {
            TenCa = "Ca su dung",
            GioBatDauMacDinh = new TimeOnly(7, 0),
            GioKetThucMacDinh = new TimeOnly(11, 0),
            TrangThai = true
        };

        var taiKhoan = new TaiKhoan
        {
            TenDangNhap = TestSeedSafeValues.TenDangNhapBacSi,
            Email = TestSeedSafeValues.EmailBacSi,
            SoDienThoai = TestSeedSafeValues.SoDienThoaiBacSi,
            MatKhau = "hash",
            VaiTro = VaiTro.BacSi,
            TrangThai = true,
            NgayTao = DateTime.UtcNow
        };

        db.ChuyenKhoa.Add(chuyenKhoa);
        db.Phong.Add(phong);
        db.DinhNghiaCa.Add(dinhNghiaCa);
        db.TaiKhoan.Add(taiKhoan);
        await db.SaveChangesAsync();

        var bacSi = new BacSi
        {
            IdTaiKhoan = taiKhoan.IdTaiKhoan,
            IdChuyenKhoa = chuyenKhoa.IdChuyenKhoa,
            HoTen = "Bac si test",
            LoaiHopDong = LoaiHopDong.NoiTru,
            TrangThai = TrangThaiBacSi.DangLam,
            NgayTao = DateTime.UtcNow
        };

        db.BacSi.Add(bacSi);
        await db.SaveChangesAsync();

        db.LichNoiTru.Add(new LichNoiTru
        {
            IdBacSi = bacSi.IdBacSi,
            IdPhong = phong.IdPhong,
            IdDinhNghiaCa = dinhNghiaCa.IdDinhNghiaCa,
            NgayTrongTuan = 1,
            NgayApDung = new DateOnly(2026, 4, 20),
            TrangThai = true
        });
        await db.SaveChangesAsync();

        var handler = new XoaDinhNghiaCaHandler(db);

        var act = async () => await handler.Handle(
            new XoaDinhNghiaCaCommand(dinhNghiaCa.IdDinhNghiaCa),
            CancellationToken.None);

        await act.Should().ThrowAsync<ConflictException>()
            .WithMessage("Khong the xoa dinh nghia ca dang duoc su dung.");
    }
}