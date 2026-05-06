using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.ToaThuoc.Commands.HuyToaThuoc;

public class HuyToaThuocHandler : IRequestHandler<HuyToaThuocCommand>
{
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public HuyToaThuocHandler(IAppDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task Handle(HuyToaThuocCommand request, CancellationToken cancellationToken)
    {
        // Lấy thông tin toa thuốc
        var toaThuoc = await _context.ToaThuoc.FirstOrDefaultAsync(
            x => x.IdToaThuoc == request.IdToaThuoc && x.IdHoSoKham == request.IdHoSoKham,
            cancellationToken);

        if (toaThuoc is null)
            throw new NotFoundException("Toa thuoc khong ton tai.");

        // Lấy thông tin hồ sơ khám để kiểm tra quyền
        var hoSoKham = await _context.HoSoKham.Include(x => x.BacSi)
            .FirstOrDefaultAsync(x => x.IdHoSoKham == request.IdHoSoKham, cancellationToken);

        if (hoSoKham is null)
            throw new NotFoundException("Ho so kham khong ton tai.");

        // Kiểm tra quyền: chỉ bác sĩ tạo toa này mới được xóa (hoặc admin)
        var currentUserId = _currentUserService.IdTaiKhoan;
        var currentUserRole = _currentUserService.VaiTro;
        
        if (currentUserRole != VaiTro.Admin && hoSoKham.IdBacSi != currentUserId)
            throw new ForbiddenException("Ban khong co quyen xoa toa thuoc nay.");

        // Hard-delete (vì entity không có BiXoa, NgayXoa)
        _context.ToaThuoc.Remove(toaThuoc);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
