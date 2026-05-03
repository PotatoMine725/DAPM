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

        var benhNhan = await _db.BenhNhan
            .FirstOrDefaultAsync(bn => bn.IdBenhNhan == idBenhNhan, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay benh nhan.");

        var now = _dateTimeProvider.UtcNow;
        if (benhNhan.BiHanChe && (benhNhan.NgayHetHanChe is null || benhNhan.NgayHetHanChe > now))
        {
            throw new ConflictException("Tai khoan benh nhan dang bi han che dat lich.");
        }

        var dichVu = await _db.DichVu
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.IdDichVu == request.IdDichVu, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay dich vu.");

        var slots = await _db.CaLamViec
            .AsNoTracking()
            .Include(x => x.BacSi)
            .Include(x => x.Phong)
            .Include(x => x.ChuyenKhoa)
            .Where(x => x.NgayLamViec == request.NgayLamViec)
            .Where(x => x.TrangThaiDuyet == TrangThaiDuyetCa.DaDuyet)
            .Where(x => x.SoSlotDaDat < x.SoSlotToiDa)
            .OrderBy(x => x.GioBatDau)
            .ToListAsync(cancellationToken);

        var slotPhuHop = slots
            .Where(x => request.GioMongMuon >= x.GioBatDau && request.GioMongMuon < x.GioKetThuc)
            .FirstOrDefault()
            ?? throw new ConflictException("Khong tim thay slot phu hop voi gio mong muon.");

        var thongTinCa = new ThongTinCaLamViecDto(
            slotPhuHop.IdCaLamViec,
            slotPhuHop.IdBacSi,
            slotPhuHop.IdPhong,
            slotPhuHop.IdChuyenKhoa,
            slotPhuHop.IdDinhNghiaCa,
            slotPhuHop.NgayLamViec,
            slotPhuHop.GioBatDau,
            slotPhuHop.GioKetThuc,
            slotPhuHop.ThoiGianSlot,
            slotPhuHop.SoSlotToiDa,
            slotPhuHop.SoSlotDaDat,
            slotPhuHop.TrangThaiDuyet);

        var ketQuaKiemTra = await _caLamViecQueryService.KiemTraSlotTrongAsync(slotPhuHop.IdCaLamViec, cancellationToken);
        if (!ketQuaKiemTra.CoTheDat)
        {
            throw new ConflictException(LyDoTuChoiMessage(ketQuaKiemTra.LyDoTuChoi));
        }

        var soSlotMoi = await _caLamViecQueryService.IncrementSoSlotDaDatAsync(
            slotPhuHop.IdCaLamViec, 1, cancellationToken);

        if (soSlotMoi is null)
        {
            throw new ConflictException("Ca lam viec da het slot hoac bi xung dot cap nhat.");
        }

        var maLichHen = await _maLichHenGenerator.SinhMaLichHenAsync(thongTinCa.NgayLamViec, cancellationToken);

        var lichHen = new LichHenEntity
        {
            MaLichHen = maLichHen,
            IdBenhNhan = idBenhNhan,
            IdCaLamViec = slotPhuHop.IdCaLamViec,
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

        try
        {
            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            try
            {
                await _caLamViecQueryService.IncrementSoSlotDaDatAsync(
                    slotPhuHop.IdCaLamViec, -1, cancellationToken);
            }
            catch
            {
            }
            throw new ConflictException("Khong the dat lich do va cham slot. Vui long thu lai.");
        }

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

    private static string LyDoTuChoiMessage(LyDoKhongDatDuoc? lyDo) => lyDo switch
    {
        LyDoKhongDatDuoc.CaChuaDuyet => "Ca lam viec chua duoc duyet.",
        LyDoKhongDatDuoc.HetSlot => "Ca lam viec da het slot.",
        LyDoKhongDatDuoc.CaDaDiQua => "Ca lam viec da qua thoi diem hien tai.",
        LyDoKhongDatDuoc.KhongTonTai => "Khong tim thay ca lam viec.",
        LyDoKhongDatDuoc.DongThoiXungDot => "Khong the dat lich do xung dot cap nhat dong thoi.",
        LyDoKhongDatDuoc.CaKhongKhaDung => "Ca lam viec khong kha dung de dat lich.",
        _ => "Khong the dat lich hen vao ca nay."
    };
}
