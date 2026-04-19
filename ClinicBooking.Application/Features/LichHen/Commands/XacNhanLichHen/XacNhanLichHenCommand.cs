using MediatR;

namespace ClinicBooking.Application.Features.LichHen.Commands.XacNhanLichHen;

/// <summary>
/// Le tan/admin xac nhan lich hen: ChoXacNhan -> DaXacNhan + gui thong bao.
/// </summary>
public record XacNhanLichHenCommand(int IdLichHen) : IRequest<Unit>;
