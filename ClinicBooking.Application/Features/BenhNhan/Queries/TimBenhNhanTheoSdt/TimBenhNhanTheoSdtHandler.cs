using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.BenhNhan.Queries.TimBenhNhanTheoSdt;

public sealed class TimBenhNhanTheoSdtHandler : IRequestHandler<TimBenhNhanTheoSdtQuery, TimBenhNhanKetQua?>
{
    private readonly IAppDbContext _db;

    public TimBenhNhanTheoSdtHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<TimBenhNhanKetQua?> Handle(TimBenhNhanTheoSdtQuery request, CancellationToken cancellationToken)
    {
        var taiKhoan = await _db.TaiKhoan
            .Include(x => x.BenhNhan)
            .FirstOrDefaultAsync(x => x.SoDienThoai == request.SoDienThoai, cancellationToken);

        if (taiKhoan is null)
            return null;

        if (taiKhoan.BenhNhan is null)
            throw new ConflictException("Số điện thoại này không thuộc hồ sơ bệnh nhân");

        return new TimBenhNhanKetQua(
            taiKhoan.BenhNhan.IdBenhNhan,
            taiKhoan.BenhNhan.HoTen,
            taiKhoan.BenhNhan.NgaySinh,
            taiKhoan.BenhNhan.GioiTinh
        );
    }
}
