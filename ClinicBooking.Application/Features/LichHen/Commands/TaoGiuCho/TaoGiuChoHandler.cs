using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Scheduling;
using ClinicBooking.Application.Abstractions.Scheduling.Dtos;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Common.Options;
using ClinicBooking.Application.Features.LichHen.Dtos;
using ClinicBooking.Domain.Enums;
using GiuChoEntity = ClinicBooking.Domain.Entities.GiuCho;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClinicBooking.Application.Features.LichHen.Commands.TaoGiuCho;

public class TaoGiuChoHandler : IRequestHandler<TaoGiuChoCommand, GiuChoResponse>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICaLamViecQueryService _caLamViecQueryService;
    private readonly LichHenOptions _options;

    public TaoGiuChoHandler(
        IAppDbContext db,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider,
        ICaLamViecQueryService caLamViecQueryService,
        IOptions<LichHenOptions> options)
    {
        _db = db;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
        _caLamViecQueryService = caLamViecQueryService;
        _options = options.Value;
    }

    public async Task<GiuChoResponse> Handle(TaoGiuChoCommand request, CancellationToken cancellationToken)
    {
        var vaiTro = _currentUser.VaiTro
            ?? throw new ForbiddenException("Khong xac dinh duoc vai tro nguoi dung.");

        if (vaiTro is not (VaiTro.LeTan or VaiTro.Admin))
        {
            throw new ForbiddenException("Chi le tan/admin duoc phep tao giu cho.");
        }

        var benhNhan = await _db.BenhNhan
            .AsNoTracking()
            .FirstOrDefaultAsync(bn => bn.IdBenhNhan == request.IdBenhNhan, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay benh nhan.");

        var now = _dateTimeProvider.UtcNow;
        if (benhNhan.BiHanChe && (benhNhan.NgayHetHanChe is null || benhNhan.NgayHetHanChe > now))
        {
            throw new ConflictException("Tai khoan benh nhan dang bi han che.");
        }

        var thongTinCa = await _caLamViecQueryService.LayThongTinCaAsync(request.IdCaLamViec, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ca lam viec.");

        if (thongTinCa.TrangThaiDuyet != TrangThaiDuyetCa.DaDuyet)
        {
            throw new ConflictException("Ca lam viec chua duoc duyet.");
        }

        var thoiDiemBatDau = thongTinCa.NgayLamViec.ToDateTime(thongTinCa.GioBatDau, DateTimeKind.Utc);
        if (thoiDiemBatDau <= now)
        {
            throw new ConflictException("Ca lam viec da qua thoi diem hien tai.");
        }

        var ketQua = await _caLamViecQueryService.KiemTraSlotTrongAsync(request.IdCaLamViec, cancellationToken);
        if (!ketQua.CoTheDat)
        {
            throw new ConflictException(LyDoTuChoiMessage(ketQua.LyDoTuChoi));
        }

        // NOTE: `GiuCho.SoSlot` dang tam hieu la ordinal (giong LichHen.SoSlot).
        // Khi PM chot semantic chinh xac, co the can sua lai migration + logic.
        var soSlotMoi = await _caLamViecQueryService.IncrementSoSlotDaDatAsync(
            request.IdCaLamViec, 1, cancellationToken);

        if (soSlotMoi is null)
        {
            // Stub da tu block (qua SoSlotToiDa hoac < 0) — khong can roll back.
            throw new ConflictException("Ca lam viec da het slot.");
        }

        var giuCho = new GiuChoEntity
        {
            IdCaLamViec = request.IdCaLamViec,
            IdBenhNhan = request.IdBenhNhan,
            SoSlot = soSlotMoi.Value,
            GioHetHan = now.AddMinutes(_options.GiuChoThoiHanPhut),
            DaGiaiPhong = false,
            NgayTao = now
        };
        _db.GiuCho.Add(giuCho);

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            try { await _caLamViecQueryService.IncrementSoSlotDaDatAsync(request.IdCaLamViec, -1, cancellationToken); }
            catch { /* reconciliation Wave 4 */ }
            throw new ConflictException("Khong the tao giu cho. Vui long thu lai.");
        }

        return new GiuChoResponse(
            giuCho.IdGiuCho,
            giuCho.IdCaLamViec,
            giuCho.IdBenhNhan,
            benhNhan.HoTen,
            giuCho.SoSlot,
            giuCho.GioHetHan,
            giuCho.NgayTao);
    }

    private static string LyDoTuChoiMessage(LyDoKhongDatDuoc? lyDo) => lyDo switch
    {
        LyDoKhongDatDuoc.CaChuaDuyet => "Ca lam viec chua duoc duyet.",
        LyDoKhongDatDuoc.HetSlot => "Ca lam viec da het slot.",
        LyDoKhongDatDuoc.CaDaDiQua => "Ca lam viec da qua thoi diem hien tai.",
        LyDoKhongDatDuoc.KhongTonTai => "Khong tim thay ca lam viec.",
        _ => "Khong the tao giu cho cho ca nay."
    };
}
