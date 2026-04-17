using MediatR;

namespace ClinicBooking.Application.Features.LichHen.Commands.HuyLichHen;

/// <summary>
/// Huy lich hen. Role benh_nhan chi huy duoc lich cua minh; le_tan/admin huy bat ky.
/// </summary>
public record HuyLichHenCommand(int IdLichHen, string LyDo) : IRequest<Unit>;
