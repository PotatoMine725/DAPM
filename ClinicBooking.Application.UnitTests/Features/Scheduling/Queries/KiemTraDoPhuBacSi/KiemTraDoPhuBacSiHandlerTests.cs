using ClinicBooking.Application.Features.Scheduling.Queries.KiemTraDoPhuBacSi;
using ClinicBooking.Application.UnitTests.Common;
using ClinicBooking.Domain.Enums;
using FluentAssertions;

namespace ClinicBooking.Application.UnitTests.Features.Scheduling.Queries.KiemTraDoPhuBacSi;

public sealed class KiemTraDoPhuBacSiHandlerTests
{
    [Fact]
    public async Task Handle_TraDanhSachNgayThieuTheoChuyenKhoa()
    {
        using var factory = new TestDbContextFactory();
        using var db = factory.CreateContext();

        var ck = TestDataSeeder.SeedChuyenKhoa(db, "Khoa test do phu");
        var caDaDuyet = TestDataSeeder.SeedCaLamViec(db, ngayLamViec: new DateOnly(2026, 5, 5), trangThai: TrangThaiDuyetCa.DaDuyet);
        var caChoDuyet = TestDataSeeder.SeedCaLamViec(db, ngayLamViec: new DateOnly(2026, 5, 6), trangThai: TrangThaiDuyetCa.ChoDuyet);

        caDaDuyet.IdChuyenKhoa = ck.IdChuyenKhoa;
        caChoDuyet.IdChuyenKhoa = ck.IdChuyenKhoa;
        await db.SaveChangesAsync();

        var handler = new KiemTraDoPhuBacSiHandler(db);
        var result = await handler.Handle(new KiemTraDoPhuBacSiQuery(ck.IdChuyenKhoa, new DateOnly(2026, 5, 5), new DateOnly(2026, 5, 10)), CancellationToken.None);

        result.NgayThieu.Should().ContainSingle(x => x.Ngay == new DateOnly(2026, 5, 6) && x.SoCaChoDuyet == 1 && x.HoanToanTrong);
        result.NgayThieu.Should().NotContain(x => x.Ngay == new DateOnly(2026, 5, 5));
    }
}
