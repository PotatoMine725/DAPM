using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.BenhNhan.Queries.KiemTraQuyenDatLich;

public sealed class KiemTraQuyenDatLichHandler
    : IRequestHandler<KiemTraQuyenDatLichQuery, KiemTraQuyenDatLichResult>
{
    private readonly IAppDbContext _db;
    private readonly IDateTimeProvider _dateTimeProvider;

    public KiemTraQuyenDatLichHandler(IAppDbContext db, IDateTimeProvider dateTimeProvider)
    {
        _db = db;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<KiemTraQuyenDatLichResult> Handle(
        KiemTraQuyenDatLichQuery request,
        CancellationToken cancellationToken)
    {
        var benhNhan = await _db.BenhNhan
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.IdBenhNhan == request.IdBenhNhan, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay benh nhan.");

        var now = _dateTimeProvider.UtcNow;
        var dangBiHanChe = benhNhan.BiHanChe
                          && (benhNhan.NgayHetHanChe is null || benhNhan.NgayHetHanChe > now);

        if (dangBiHanChe)
        {
            var denNgay = benhNhan.NgayHetHanChe?.ToString("dd/MM/yyyy HH:mm") ?? "khong xac dinh";
            return new KiemTraQuyenDatLichResult(
                false,
                $"Benh nhan dang bi han che dat lich den {denNgay}.",
                benhNhan.SoLanHuyMuon,
                benhNhan.BiHanChe,
                benhNhan.NgayHetHanChe,
                benhNhan.SoLanHuyMuon >= BenhNhanConstants.NguongCanhBaoSom);
        }

        if (benhNhan.SoLanHuyMuon >= BenhNhanConstants.NgueongSoLanHuyMuonTrongThang)
        {
            return new KiemTraQuyenDatLichResult(
                false,
                $"Benh nhan da dat nguong {BenhNhanConstants.NgueongSoLanHuyMuonTrongThang} lan huy muon, tam thoi khong duoc dat lich moi.",
                benhNhan.SoLanHuyMuon,
                benhNhan.BiHanChe,
                benhNhan.NgayHetHanChe,
                true);
        }

        return new KiemTraQuyenDatLichResult(
            true,
            null,
            benhNhan.SoLanHuyMuon,
            benhNhan.BiHanChe,
            benhNhan.NgayHetHanChe,
            benhNhan.SoLanHuyMuon >= BenhNhanConstants.NguongCanhBaoSom);
    }
}
