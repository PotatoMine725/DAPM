using ClinicBooking.Application.Abstractions.Notifications;
using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Scheduling;
using ClinicBooking.Application.Abstractions.Scheduling.Dtos;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Common.Services;
using ClinicBooking.Application.Features.LichHen.Dtos;
using ClinicBooking.Domain.Enums;
using LichHenEntity = ClinicBooking.Domain.Entities.LichHen;
using LichSuLichHenEntity = ClinicBooking.Domain.Entities.LichSuLichHen;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.LichHen.Commands.TaoLichHen;

public class TaoLichHenHandler : IRequestHandler<TaoLichHenCommand, LichHenResponse>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICaLamViecQueryService _caLamViecQueryService;
    private readonly INotificationService _notificationService;
    private readonly IMaLichHenGenerator _maLichHenGenerator;

    public TaoLichHenHandler(
        IAppDbContext db,
        ICurrentUserService currentUser,
        IDateTimeProvider dateTimeProvider,
        ICaLamViecQueryService caLamViecQueryService,
        INotificationService notificationService,
        IMaLichHenGenerator maLichHenGenerator)
    {
        _db = db;
        _currentUser = currentUser;
        _dateTimeProvider = dateTimeProvider;
        _caLamViecQueryService = caLamViecQueryService;
        _notificationService = notificationService;
        _maLichHenGenerator = maLichHenGenerator;
    }

    public async Task<LichHenResponse> Handle(TaoLichHenCommand request, CancellationToken cancellationToken)
    {
        var vaiTro = _currentUser.VaiTro
            ?? throw new ForbiddenException("Khong xac dinh duoc vai tro nguoi dung.");

        // 1. Xac dinh IdBenhNhan va HinhThucDat dua tren vai tro.
        int idBenhNhan;
        HinhThucDat hinhThucDat;
        if (vaiTro == VaiTro.BenhNhan)
        {
            var idTaiKhoan = _currentUser.IdTaiKhoan
                ?? throw new ForbiddenException("Khong xac dinh duoc nguoi dung hien tai.");

            var benhNhanHienTai = await _db.BenhNhan
                .AsNoTracking()
                .FirstOrDefaultAsync(bn => bn.IdTaiKhoan == idTaiKhoan, cancellationToken)
                ?? throw new NotFoundException("Khong tim thay ho so benh nhan cua tai khoan hien tai.");

            idBenhNhan = benhNhanHienTai.IdBenhNhan;
            hinhThucDat = HinhThucDat.TrucTuyen;
        }
        else if (vaiTro is VaiTro.LeTan or VaiTro.Admin)
        {
            idBenhNhan = request.IdBenhNhan
                ?? throw new ConflictException("Le tan phai chon benh nhan de dat lich.");
            hinhThucDat = HinhThucDat.TaiQuay;
        }
        else
        {
            throw new ForbiddenException("Vai tro hien tai khong duoc phep dat lich hen.");
        }

        // 2. Kiem tra benh nhan co bi han che khong.
        var benhNhan = await _db.BenhNhan
            .FirstOrDefaultAsync(bn => bn.IdBenhNhan == idBenhNhan, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay benh nhan.");

        var now = _dateTimeProvider.UtcNow;
        if (benhNhan.BiHanChe && (benhNhan.NgayHetHanChe is null || benhNhan.NgayHetHanChe > now))
        {
            throw new ConflictException("Tai khoan benh nhan dang bi han che dat lich.");
        }

        // 3. Kiem tra dich vu ton tai.
        var dichVu = await _db.DichVu
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.IdDichVu == request.IdDichVu, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay dich vu.");

        // 4. Kiem tra ca lam viec + da duyet + con slot.
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

        var ketQuaKiemTra = await _caLamViecQueryService.KiemTraSlotTrongAsync(request.IdCaLamViec, cancellationToken);
        if (!ketQuaKiemTra.CoTheDat)
        {
            throw new ConflictException(LyDoTuChoiMessage(ketQuaKiemTra.LyDoTuChoi));
        }

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // 5. Chiem slot bang atomic increment; SoSlot ordinal = gia tri tra ve.
            var soSlotMoi = await _caLamViecQueryService.IncrementSoSlotDaDatAsync(
                request.IdCaLamViec, 1, cancellationToken);

            if (soSlotMoi is null)
            {
                throw new ConflictException("Ca lam viec da het slot.");
            }

            // 6. Sinh MaLichHen + insert LichHen + LichSuLichHen.
            var maLichHen = await _maLichHenGenerator.SinhMaLichHenAsync(thongTinCa.NgayLamViec, cancellationToken);

            var lichHen = new LichHenEntity
            {
                MaLichHen = maLichHen,
                IdBenhNhan = idBenhNhan,
                IdCaLamViec = request.IdCaLamViec,
                IdDichVu = request.IdDichVu,
                SoSlot = soSlotMoi.Value,
                HinhThucDat = hinhThucDat,
                IdBacSiMongMuon = request.IdBacSiMongMuon,
                BacSiMongMuonNote = request.BacSiMongMuonNote,
                TrieuChung = request.TrieuChung,
                TrangThai = TrangThaiLichHen.ChoXacNhan,
                NgayTao = now
            };
            _db.LichHen.Add(lichHen);

            _db.LichSuLichHen.Add(new LichSuLichHenEntity
            {
                LichHen = lichHen,
                HanhDong = HanhDongLichSu.DatMoi,
                IdNguoiThucHien = _currentUser.IdTaiKhoan,
                NgayTao = now
            });

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            // 7. Notification fire-and-forget.
            await _notificationService.GuiThongBaoTaoLichHenAsync(lichHen.IdLichHen, cancellationToken);

            return new LichHenResponse(
                lichHen.IdLichHen,
                lichHen.MaLichHen,
                lichHen.IdBenhNhan,
                benhNhan.HoTen,
                lichHen.IdCaLamViec,
                thongTinCa.NgayLamViec,
                thongTinCa.GioBatDau,
                thongTinCa.GioKetThuc,
                lichHen.IdDichVu,
                dichVu.TenDichVu,
                lichHen.SoSlot,
                lichHen.HinhThucDat,
                lichHen.IdBacSiMongMuon,
                lichHen.BacSiMongMuonNote,
                lichHen.TrieuChung,
                lichHen.TrangThai,
                lichHen.NgayTao);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private static string LyDoTuChoiMessage(LyDoKhongDatDuoc? lyDo) => lyDo switch
    {
        LyDoKhongDatDuoc.CaChuaDuyet => "Ca lam viec chua duoc duyet.",
        LyDoKhongDatDuoc.HetSlot => "Ca lam viec da het slot.",
        LyDoKhongDatDuoc.CaDaDiQua => "Ca lam viec da qua thoi diem hien tai.",
        LyDoKhongDatDuoc.KhongTonTai => "Khong tim thay ca lam viec.",
        _ => "Khong the dat lich hen vao ca nay."
    };
}
