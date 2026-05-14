using MediatR;

namespace ClinicBooking.Application.Features.Auth.Commands.GuiOtpKichHoatWalkIn;

// Returns IdTaiKhoan of the ghost account (needed by Web layer to track session)
public record GuiOtpKichHoatWalkInCommand(string SoDienThoai, string EmailNhan) : IRequest<int>;
