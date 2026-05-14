using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Auth.Commands.GuiOtpKichHoatWalkIn;

public class GuiOtpKichHoatWalkInHandler : IRequestHandler<GuiOtpKichHoatWalkInCommand, int>
{
    private readonly IAppDbContext _db;
    private readonly IOtpService _otpService;

    public GuiOtpKichHoatWalkInHandler(IAppDbContext db, IOtpService otpService)
    {
        _db = db;
        _otpService = otpService;
    }

    public async Task<int> Handle(GuiOtpKichHoatWalkInCommand request, CancellationToken cancellationToken)
    {
        var taiKhoan = await _db.TaiKhoan
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.SoDienThoai == request.SoDienThoai, cancellationToken);

        if (taiKhoan is null || taiKhoan.TrangThai || !taiKhoan.TenDangNhap.StartsWith("walkin_"))
            throw new NotFoundException("Khong tim thay ho so vang lai voi so dien thoai nay.");

        await _otpService.TaoVaGuiOtpDatLichAsync(taiKhoan.IdTaiKhoan, taiKhoan.SoDienThoai, request.EmailNhan, cancellationToken);
        return taiKhoan.IdTaiKhoan;
    }
}
