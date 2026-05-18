using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Features.BenhNhan.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.BenhNhan.Queries.TimKiemBenhNhan;

public sealed class TimKiemBenhNhanHandler : IRequestHandler<TimKiemBenhNhanQuery, IReadOnlyList<BenhNhanResponse>>
{
    private readonly IAppDbContext _db;

    public TimKiemBenhNhanHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<BenhNhanResponse>> Handle(
        TimKiemBenhNhanQuery request,
        CancellationToken cancellationToken)
    {
        var tuKhoa = request.TuKhoa.Trim();

        return await _db.BenhNhan
            .AsNoTracking()
            .Include(x => x.TaiKhoan)
            .Where(x =>
                x.HoTen.Contains(tuKhoa)
                || x.TaiKhoan.SoDienThoai.Contains(tuKhoa)
                || (x.Cccd != null && x.Cccd.Contains(tuKhoa))
                || ($"BN-{x.IdBenhNhan}").Contains(tuKhoa)
                || x.IdBenhNhan.ToString().Contains(tuKhoa))
            .OrderBy(x => x.HoTen)
            .ThenBy(x => x.IdBenhNhan)
            .Take(Math.Clamp(request.Limit, 1, 100))
            .Select(x => new BenhNhanResponse(
                x.IdBenhNhan,
                x.IdTaiKhoan,
                x.HoTen,
                x.TaiKhoan.SoDienThoai,
                x.TaiKhoan.Email,
                x.Cccd,
                x.NgaySinh,
                x.GioiTinh,
                x.DiaChi,
                x.SoLanHuyMuon,
                x.BiHanChe,
                x.NgayHetHanChe,
                x.NgayTao))
            .ToListAsync(cancellationToken);
    }
}
