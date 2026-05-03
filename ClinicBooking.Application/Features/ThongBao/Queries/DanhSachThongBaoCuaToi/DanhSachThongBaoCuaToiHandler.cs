using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Features.ThongBao.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.ThongBao.Queries.DanhSachThongBaoCuaToi;

public sealed class DanhSachThongBaoCuaToiHandler
    : IRequestHandler<DanhSachThongBaoCuaToiQuery, DanhSachThongBaoResponse>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DanhSachThongBaoCuaToiHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<DanhSachThongBaoResponse> Handle(
        DanhSachThongBaoCuaToiQuery request,
        CancellationToken cancellationToken)
    {
        var idTaiKhoan = _currentUser.IdTaiKhoan
            ?? throw new ForbiddenException("Khong xac dinh duoc nguoi dung hien tai.");

        var query = _db.ThongBao
            .AsNoTracking()
            .Where(x => x.IdTaiKhoan == idTaiKhoan);

        if (request.ChiChuaDoc == true)
        {
            query = query.Where(x => !x.DaDoc);
        }

        var soChuaDoc = await _db.ThongBao
            .AsNoTracking()
            .CountAsync(x => x.IdTaiKhoan == idTaiKhoan && !x.DaDoc, cancellationToken);

        var tongSo = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.NgayGui)
            .Skip((request.SoTrang - 1) * request.KichThuocTrang)
            .Take(request.KichThuocTrang)
            .Select(x => new ThongBaoResponse(
                x.IdThongBao,
                x.TieuDe,
                x.NoiDung,
                x.KenhGui,
                x.DaDoc,
                x.NgayGui,
                x.IdThamChieu,
                x.LoaiThamChieu))
            .ToListAsync(cancellationToken);

        return new DanhSachThongBaoResponse(items, tongSo, request.SoTrang, request.KichThuocTrang, soChuaDoc);
    }
}
