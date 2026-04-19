using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Scheduling;
using ClinicBooking.Application.Abstractions.Scheduling.Dtos;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Infrastructure.Services.Scheduling;

/// <summary>
/// Stub tam thoi cho <see cref="ICaLamViecQueryService"/>.
/// Doc truc tiep tu <see cref="IAppDbContext"/> de Module 1 chay duoc truoc khi Module 2 ship.
/// TODO: Thay the bang implementation that cua Module 2 khi code duoc day len.
/// </summary>
public class CaLamViecQueryServiceStub : ICaLamViecQueryService
{
    private readonly IAppDbContext _db;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CaLamViecQueryServiceStub(IAppDbContext db, IDateTimeProvider dateTimeProvider)
    {
        _db = db;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ThongTinCaLamViecDto?> LayThongTinCaAsync(
        int idCaLamViec, CancellationToken cancellationToken = default)
    {
        return await _db.CaLamViec
            .Where(c => c.IdCaLamViec == idCaLamViec)
            .Select(c => new ThongTinCaLamViecDto(
                c.IdCaLamViec,
                c.IdBacSi,
                c.IdPhong,
                c.IdChuyenKhoa,
                c.IdDinhNghiaCa,
                c.NgayLamViec,
                c.GioBatDau,
                c.GioKetThuc,
                c.ThoiGianSlot,
                c.SoSlotToiDa,
                c.SoSlotDaDat,
                c.TrangThaiDuyet))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<KetQuaKiemTraSlotDto> KiemTraSlotTrongAsync(
        int idCaLamViec, CancellationToken cancellationToken = default)
    {
        var ca = await LayThongTinCaAsync(idCaLamViec, cancellationToken);

        if (ca is null)
        {
            return new KetQuaKiemTraSlotDto(
                CoTheDat: false,
                SoSlotToiDa: 0,
                SoSlotDaDat: 0,
                SoGiuChoHieuLuc: 0,
                LyDoTuChoi: LyDoKhongDatDuoc.KhongTonTai);
        }

        if (ca.TrangThaiDuyet != TrangThaiDuyetCa.DaDuyet)
        {
            return new KetQuaKiemTraSlotDto(
                CoTheDat: false,
                SoSlotToiDa: ca.SoSlotToiDa,
                SoSlotDaDat: ca.SoSlotDaDat,
                SoGiuChoHieuLuc: 0,
                LyDoTuChoi: LyDoKhongDatDuoc.CaChuaDuyet);
        }

        var now = _dateTimeProvider.UtcNow;
        var ngayGioBatDau = ca.NgayLamViec.ToDateTime(ca.GioBatDau);
        if (ngayGioBatDau < now)
        {
            return new KetQuaKiemTraSlotDto(
                CoTheDat: false,
                SoSlotToiDa: ca.SoSlotToiDa,
                SoSlotDaDat: ca.SoSlotDaDat,
                SoGiuChoHieuLuc: 0,
                LyDoTuChoi: LyDoKhongDatDuoc.CaDaDiQua);
        }

        // Dem giu cho tam con hieu luc (chua giai phong VA chua het han)
        // Dong thoi opportunistic cleanup: danh dau giu cho het han la da giai phong
        await DocGiuChoHetHanAsync(idCaLamViec, cancellationToken);

        var soGiuChoHieuLuc = await _db.GiuCho
            .CountAsync(g =>
                g.IdCaLamViec == idCaLamViec
                && !g.DaGiaiPhong
                && g.GioHetHan > now,
                cancellationToken);

        var tongDaDung = ca.SoSlotDaDat + soGiuChoHieuLuc;

        if (tongDaDung >= ca.SoSlotToiDa)
        {
            return new KetQuaKiemTraSlotDto(
                CoTheDat: false,
                SoSlotToiDa: ca.SoSlotToiDa,
                SoSlotDaDat: ca.SoSlotDaDat,
                SoGiuChoHieuLuc: soGiuChoHieuLuc,
                LyDoTuChoi: LyDoKhongDatDuoc.HetSlot);
        }

        return new KetQuaKiemTraSlotDto(
            CoTheDat: true,
            SoSlotToiDa: ca.SoSlotToiDa,
            SoSlotDaDat: ca.SoSlotDaDat,
            SoGiuChoHieuLuc: soGiuChoHieuLuc,
            LyDoTuChoi: null);
    }

    public async Task<bool> LaCaDuocDuyetAsync(
        int idCaLamViec, CancellationToken cancellationToken = default)
    {
        return await _db.CaLamViec
            .AnyAsync(c =>
                c.IdCaLamViec == idCaLamViec
                && c.TrangThaiDuyet == TrangThaiDuyetCa.DaDuyet,
                cancellationToken);
    }

    public async Task<int?> IncrementSoSlotDaDatAsync(
        int idCaLamViec, int delta, CancellationToken cancellationToken = default)
    {
        // Atomic UPDATE — khong Read-Modify-Write tu client.
        // SQL Server dam bao nguyen tu bang row-level lock tren UPDATE statement.
        // Ap constraint: SoSlotDaDat + delta khong duoc < 0 hoac > SoSlotToiDa.
        var rowsAffected = await _db.CaLamViec
            .Where(c => c.IdCaLamViec == idCaLamViec)
            .Where(c => c.SoSlotDaDat + delta >= 0 && c.SoSlotDaDat + delta <= c.SoSlotToiDa)
            .ExecuteUpdateAsync(
                s => s.SetProperty(c => c.SoSlotDaDat, c => c.SoSlotDaDat + delta),
                cancellationToken);

        // Neu khong co row nao duoc update, co nghia la:
        // - Khong tim thay CaLamViec tuong ung, hoac
        // - delta khien SoSlotDaDat vuot khoi [0, SoSlotToiDa]
        if (rowsAffected == 0)
        {
            return null;
        }

        // Doc lai gia tri moi (sau UPDATE da commit)
        return await _db.CaLamViec
            .Where(c => c.IdCaLamViec == idCaLamViec)
            .Select(c => (int?)c.SoSlotDaDat)
            .FirstAsync(cancellationToken);
    }

    /// <summary>
    /// Opportunistic cleanup: danh dau cac giu cho da het han la <c>DaGiaiPhong = true</c>.
    /// Thay the bang Hangfire job (Module 4) khi co.
    /// </summary>
    private async Task DocGiuChoHetHanAsync(int idCaLamViec, CancellationToken cancellationToken)
    {
        var now = _dateTimeProvider.UtcNow;

        await _db.GiuCho
            .Where(g =>
                g.IdCaLamViec == idCaLamViec
                && !g.DaGiaiPhong
                && g.GioHetHan <= now)
            .ExecuteUpdateAsync(
                s => s.SetProperty(g => g.DaGiaiPhong, true),
                cancellationToken);
    }
}
