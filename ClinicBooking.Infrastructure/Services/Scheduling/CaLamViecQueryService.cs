using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Scheduling;
using ClinicBooking.Application.Abstractions.Scheduling.Dtos;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Infrastructure.Services.Scheduling;

public class CaLamViecQueryService : ICaLamViecQueryService
{
    private readonly IAppDbContext _db;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CaLamViecQueryService(IAppDbContext db, IDateTimeProvider dateTimeProvider)
    {
        _db = db;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ThongTinCaLamViecDto?> LayThongTinCaAsync(
        int idCaLamViec, CancellationToken cancellationToken = default)
    {
        return await _db.CaLamViec
            .AsNoTracking()
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
            return new KetQuaKiemTraSlotDto(false, 0, 0, 0, LyDoKhongDatDuoc.KhongTonTai);
        }

        if (ca.TrangThaiDuyet != TrangThaiDuyetCa.DaDuyet)
        {
            return new KetQuaKiemTraSlotDto(false, ca.SoSlotToiDa, ca.SoSlotDaDat, 0, LyDoKhongDatDuoc.CaChuaDuyet);
        }

        if (ca.NgayLamViec.ToDateTime(ca.GioBatDau) < _dateTimeProvider.UtcNow)
        {
            return new KetQuaKiemTraSlotDto(false, ca.SoSlotToiDa, ca.SoSlotDaDat, 0, LyDoKhongDatDuoc.CaDaDiQua);
        }

        var soGiuChoHieuLuc = await _db.GiuCho.CountAsync(g =>
            g.IdCaLamViec == idCaLamViec && !g.DaGiaiPhong && g.GioHetHan > _dateTimeProvider.UtcNow, cancellationToken);

        var tongDaDung = ca.SoSlotDaDat + soGiuChoHieuLuc;
        if (tongDaDung >= ca.SoSlotToiDa)
        {
            return new KetQuaKiemTraSlotDto(false, ca.SoSlotToiDa, ca.SoSlotDaDat, soGiuChoHieuLuc, LyDoKhongDatDuoc.HetSlot);
        }

        return new KetQuaKiemTraSlotDto(true, ca.SoSlotToiDa, ca.SoSlotDaDat, soGiuChoHieuLuc, null);
    }

    public async Task<bool> LaCaDuocDuyetAsync(
        int idCaLamViec, CancellationToken cancellationToken = default)
    {
        return await _db.CaLamViec
            .AnyAsync(c => c.IdCaLamViec == idCaLamViec && c.TrangThaiDuyet == TrangThaiDuyetCa.DaDuyet, cancellationToken);
    }

    public async Task<int?> IncrementSoSlotDaDatAsync(
        int idCaLamViec, int delta, CancellationToken cancellationToken = default)
    {
        var rowsAffected = await _db.CaLamViec
            .Where(c => c.IdCaLamViec == idCaLamViec)
            .Where(c => c.SoSlotDaDat + delta >= 0 && c.SoSlotDaDat + delta <= c.SoSlotToiDa)
            .ExecuteUpdateAsync(
                s => s.SetProperty(c => c.SoSlotDaDat, c => c.SoSlotDaDat + delta),
                cancellationToken);

        if (rowsAffected == 0)
        {
            return null;
        }

        return await _db.CaLamViec
            .Where(c => c.IdCaLamViec == idCaLamViec)
            .Select(c => (int?)c.SoSlotDaDat)
            .FirstAsync(cancellationToken);
    }

    public async Task<int> ChayReconSlotAsync(CancellationToken cancellationToken = default)
    {
        // Recon rule tam thoi:
        // - SoSlotDaDat = so LichHen dang ton tai va khong bi Huy/QuaHan/KhongDen/HoanThanh?
        // - cong them so GiuCho con hieu luc
        // - neu khac gia tri dang luu thi cap nhat ve gia tri doi soat
        //
        // Do concurrency + logic domain phuc tap, job nay chi sua cac truong hop lech ro rang,
        // con case mo/khac nghia se de lai log de xem tiep.

        var now = _dateTimeProvider.UtcNow;

        var caCanRecon = await _db.CaLamViec
            .AsNoTracking()
            .Select(c => new
            {
                c.IdCaLamViec,
                c.SoSlotDaDat,
                c.SoSlotToiDa,
                TongLichHen = _db.LichHen.Count(lh => lh.IdCaLamViec == c.IdCaLamViec &&
                                                     lh.TrangThai != TrangThaiLichHen.HuyBenhNhan &&
                                                     lh.TrangThai != TrangThaiLichHen.HuyPhongKham &&
                                                     lh.TrangThai != TrangThaiLichHen.DaQuaHan &&
                                                     lh.TrangThai != TrangThaiLichHen.KhongDen),
                TongGiuCho = _db.GiuCho.Count(gc => gc.IdCaLamViec == c.IdCaLamViec && !gc.DaGiaiPhong && gc.GioHetHan > now)
            })
            .ToListAsync(cancellationToken);

        var daCapNhat = 0;
        foreach (var item in caCanRecon)
        {
            var soSlotTinhLai = item.TongLichHen + item.TongGiuCho;
            if (soSlotTinhLai < 0 || soSlotTinhLai > item.SoSlotToiDa)
            {
                continue;
            }

            if (soSlotTinhLai == item.SoSlotDaDat)
            {
                continue;
            }

            daCapNhat += await _db.CaLamViec
                .Where(c => c.IdCaLamViec == item.IdCaLamViec)
                .ExecuteUpdateAsync(
                    s => s.SetProperty(c => c.SoSlotDaDat, soSlotTinhLai),
                    cancellationToken);
        }

        return daCapNhat;
    }
}
