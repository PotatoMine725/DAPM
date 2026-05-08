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
using Microsoft.EntityFrameworkCore.Storage;

namespace ClinicBooking.Application.Features.LichHen.Commands.DoiLichHen;

public class DoiLichHenHandler : IRequestHandler<DoiLichHenCommand, LichHenResponse>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICaLamViecQueryService _caLamViecQueryService;
    private readonly INotificationService _notificationService;
    private readonly IMaLichHenGenerator _maLichHenGenerator;

    public DoiLichHenHandler(
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

    public async Task<LichHenResponse> Handle(DoiLichHenCommand request, CancellationToken cancellationToken)
    {
        var vaiTro = _currentUser.VaiTro
            ?? throw new ForbiddenException("Khong xac dinh duoc vai tro nguoi dung.");

        if (vaiTro is not (VaiTro.BenhNhan or VaiTro.LeTan or VaiTro.Admin))
        {
            throw new ForbiddenException("Vai tro hien tai khong duoc phep doi lich hen.");
        }

        var lichHenCu = await _db.LichHen
            .Include(x => x.BenhNhan)
            .FirstOrDefaultAsync(x => x.IdLichHen == request.IdLichHenCu, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay lich hen can doi.");

        // benh_nhan chi doi lich cua minh.
        if (vaiTro == VaiTro.BenhNhan)
        {
            var idTaiKhoan = _currentUser.IdTaiKhoan
                ?? throw new ForbiddenException("Khong xac dinh duoc nguoi dung hien tai.");
            if (lichHenCu.BenhNhan.IdTaiKhoan != idTaiKhoan)
            {
                throw new ForbiddenException("Ban khong co quyen doi lich hen nay.");
            }
        }

        if (lichHenCu.TrangThai is TrangThaiLichHen.HuyBenhNhan
            or TrangThaiLichHen.HuyPhongKham
            or TrangThaiLichHen.HoanThanh
            or TrangThaiLichHen.KhongDen
            or TrangThaiLichHen.DangKham)
        {
            throw new ConflictException("Lich hen nay khong the doi o trang thai hien tai.");
        }

        if (request.IdCaLamViecMoi == lichHenCu.IdCaLamViec)
        {
            throw new ConflictException("Ca moi phai khac ca hien tai.");
        }

        var idDichVuMoi = request.IdDichVuMoi ?? lichHenCu.IdDichVu;
        var dichVu = await _db.DichVu
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.IdDichVu == idDichVuMoi, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay dich vu.");

        var now = _dateTimeProvider.UtcNow;

        // Kiem tra ca moi.
        var thongTinCaMoi = await _caLamViecQueryService.LayThongTinCaAsync(request.IdCaLamViecMoi, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ca lam viec moi.");

        if (thongTinCaMoi.TrangThaiDuyet != TrangThaiDuyetCa.DaDuyet)
        {
            throw new ConflictException("Ca lam viec moi chua duoc duyet.");
        }

        var thoiDiemBatDauMoi = thongTinCaMoi.NgayLamViec.ToDateTime(thongTinCaMoi.GioBatDau, DateTimeKind.Utc);
        if (thoiDiemBatDauMoi <= now)
        {
            throw new ConflictException("Ca lam viec moi da qua thoi diem hien tai.");
        }

        var ketQua = await _caLamViecQueryService.KiemTraSlotTrongAsync(request.IdCaLamViecMoi, cancellationToken);
        if (!ketQua.CoTheDat)
        {
            throw new ConflictException(LyDoTuChoiMessage(ketQua.LyDoTuChoi));
        }

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Chiem slot o ca moi truoc khi huy ca cu (de neu het slot thi khong mat lich cu).
            var soSlotMoi = await _caLamViecQueryService.IncrementSoSlotDaDatAsync(
                request.IdCaLamViecMoi, 1, cancellationToken);
            if (soSlotMoi is null)
            {
                throw new ConflictException("Ca lam viec moi da het slot.");
            }

            var maLichHen = await _maLichHenGenerator.SinhMaLichHenAsync(thongTinCaMoi.NgayLamViec, cancellationToken);

            var lichHenMoi = new LichHenEntity
            {
                MaLichHen = maLichHen,
                IdBenhNhan = lichHenCu.IdBenhNhan,
                IdCaLamViec = request.IdCaLamViecMoi,
                IdDichVu = idDichVuMoi,
                SoSlot = soSlotMoi.Value,
                HinhThucDat = lichHenCu.HinhThucDat,
                IdBacSiMongMuon = request.IdBacSiMongMuon ?? lichHenCu.IdBacSiMongMuon,
                BacSiMongMuonNote = request.BacSiMongMuonNote ?? lichHenCu.BacSiMongMuonNote,
                TrieuChung = request.TrieuChung ?? lichHenCu.TrieuChung,
                TrangThai = TrangThaiLichHen.ChoXacNhan,
                NgayTao = now
            };
            _db.LichHen.Add(lichHenMoi);

            // Huy lich hen cu.
            lichHenCu.TrangThai = vaiTro == VaiTro.BenhNhan
                ? TrangThaiLichHen.HuyBenhNhan
                : TrangThaiLichHen.HuyPhongKham;

            _db.LichSuLichHen.Add(new LichSuLichHenEntity
            {
                IdLichHen = lichHenCu.IdLichHen,
                HanhDong = HanhDongLichSu.DoiLich,
                IdNguoiThucHien = _currentUser.IdTaiKhoan,
                LyDo = request.LyDo,
                NgayTao = now
            });

            _db.LichSuLichHen.Add(new LichSuLichHenEntity
            {
                LichHen = lichHenMoi,
                HanhDong = HanhDongLichSu.DatMoi,
                IdNguoiThucHien = _currentUser.IdTaiKhoan,
                IdLichHenTruoc = lichHenCu.IdLichHen,
                NgayTao = now
            });

            // Tra slot ca cu trong cung transaction de ca cu/ca moi luon dong bo.
            var soSlotCaCu = await _caLamViecQueryService.IncrementSoSlotDaDatAsync(
                lichHenCu.IdCaLamViec, -1, cancellationToken);
            if (soSlotCaCu is null)
            {
                throw new ConflictException("Khong the giai phong slot cua ca cu.");
            }

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            await _notificationService.GuiThongBaoDoiLichHenAsync(
                lichHenCu.IdLichHen, lichHenMoi.IdLichHen, cancellationToken);

            return new LichHenResponse(
                lichHenMoi.IdLichHen,
                lichHenMoi.MaLichHen,
                lichHenMoi.IdBenhNhan,
                lichHenCu.BenhNhan.HoTen,
                lichHenMoi.IdCaLamViec,
                thongTinCaMoi.NgayLamViec,
                thongTinCaMoi.GioBatDau,
                thongTinCaMoi.GioKetThuc,
                lichHenMoi.IdDichVu,
                dichVu.TenDichVu,
                lichHenMoi.SoSlot,
                lichHenMoi.HinhThucDat,
                lichHenMoi.IdBacSiMongMuon,
                lichHenMoi.BacSiMongMuonNote,
                lichHenMoi.TrieuChung,
                lichHenMoi.TrangThai,
                lichHenMoi.NgayTao);
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
        _ => "Khong the doi lich hen vao ca nay."
    };
}
