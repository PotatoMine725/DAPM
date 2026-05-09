using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.LichHen.Commands.GanBacSiChoLichHen;

public class GanBacSiChoLichHenHandler : IRequestHandler<GanBacSiChoLichHenCommand, GanBacSiKetQua>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GanBacSiChoLichHenHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<GanBacSiKetQua> Handle(GanBacSiChoLichHenCommand request, CancellationToken cancellationToken)
    {
        // 1. Load LichHen (include CaLamViec, HangCho)
        var lichHen = await _db.LichHen
            .Include(x => x.CaLamViec)
            .Include(x => x.HangCho)
            .FirstOrDefaultAsync(x => x.IdLichHen == request.IdLichHen, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay lich hen.");

        // 2. State guards
        if (lichHen.TrangThai != TrangThaiLichHen.ChoXacNhan &&
            lichHen.TrangThai != TrangThaiLichHen.DaXacNhan)
        {
            throw new ConflictException("Khong the gan bac si o trang thai nay.");
        }

        if (lichHen.HangCho != null)
        {
            throw new ConflictException("Benh nhan da check-in, khong the doi ca.");
        }

        // 3. Load CaLamViecMoi
        var caLamViecMoi = await _db.CaLamViec
            .FirstOrDefaultAsync(x => x.IdCaLamViec == request.IdCaLamViecMoi, cancellationToken);

        if (caLamViecMoi is null)
            return new GanBacSiKetQua(false, "Khong tim thay ca lam viec.");

        if (caLamViecMoi.TrangThaiDuyet != TrangThaiDuyetCa.DaDuyet)
            return new GanBacSiKetQua(false, "Ca lam viec chua duoc duyet.");

        if (caLamViecMoi.SoSlotDaDat >= caLamViecMoi.SoSlotToiDa)
            return new GanBacSiKetQua(false, "Bac si khong con slot phu hop trong ca nay.");

        // 4. Transaction
        try
        {
            var caLamViecCu = lichHen.CaLamViec;
            caLamViecCu.SoSlotDaDat--;
            caLamViecMoi.SoSlotDaDat++;

            lichHen.IdCaLamViec = caLamViecMoi.IdCaLamViec;
            lichHen.IdBacSiMongMuon = caLamViecMoi.IdBacSi;

            _db.LichSuLichHen.Add(new LichSuLichHen
            {
                IdLichHen = lichHen.IdLichHen,
                HanhDong = HanhDongLichSu.GanBacSi,
                IdNguoiThucHien = _currentUser.IdTaiKhoan,
                NgayTao = DateTime.UtcNow
            });

            await _db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            return new GanBacSiKetQua(false, "Du lieu vua thay doi, vui long thu lai.");
        }

        return new GanBacSiKetQua(true, null);
    }
}
