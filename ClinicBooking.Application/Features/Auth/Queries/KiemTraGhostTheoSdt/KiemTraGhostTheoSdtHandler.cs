using ClinicBooking.Application.Abstractions.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Auth.Queries.KiemTraGhostTheoSdt;

public class KiemTraGhostTheoSdtHandler : IRequestHandler<KiemTraGhostTheoSdtQuery, KiemTraGhostKetQua>
{
    private readonly IAppDbContext _db;

    public KiemTraGhostTheoSdtHandler(IAppDbContext db) => _db = db;

    public async Task<KiemTraGhostKetQua> Handle(KiemTraGhostTheoSdtQuery request, CancellationToken cancellationToken)
    {
        var taiKhoan = await _db.TaiKhoan
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SoDienThoai == request.SoDienThoai, cancellationToken);

        if (taiKhoan is null) return new KiemTraGhostKetQua(false, null);

        var isGhost = !taiKhoan.TrangThai && taiKhoan.TenDangNhap.StartsWith("walkin_");
        return new KiemTraGhostKetQua(isGhost, isGhost ? taiKhoan.IdTaiKhoan : null);
    }
}
