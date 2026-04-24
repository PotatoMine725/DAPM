using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.ThongBao.Commands.DanhDauDaDocThongBao;

public sealed class DanhDauDaDocThongBaoHandler : IRequestHandler<DanhDauDaDocThongBaoCommand, Unit>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DanhDauDaDocThongBaoHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(DanhDauDaDocThongBaoCommand request, CancellationToken cancellationToken)
    {
        var idTaiKhoan = _currentUser.IdTaiKhoan
            ?? throw new ForbiddenException("Khong xac dinh duoc nguoi dung hien tai.");

        if (request.IdThongBao.HasValue)
        {
            // Danh dau 1 thong bao cu the
            var thongBao = await _db.ThongBao
                .FirstOrDefaultAsync(
                    x => x.IdThongBao == request.IdThongBao.Value && x.IdTaiKhoan == idTaiKhoan,
                    cancellationToken)
                ?? throw new NotFoundException("Khong tim thay thong bao.");

            thongBao.DaDoc = true;
        }
        else
        {
            // Danh dau tat ca chua doc
            await _db.ThongBao
                .Where(x => x.IdTaiKhoan == idTaiKhoan && !x.DaDoc)
                .ExecuteUpdateAsync(
                    s => s.SetProperty(x => x.DaDoc, true),
                    cancellationToken);
        }

        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
