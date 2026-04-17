using MediatR;

namespace ClinicBooking.Application.Features.LichHen.Commands.GiaiPhongGiuCho;

/// <summary>
/// Giai phong mot ban ghi GiuCho (le tan huy tam giu truoc khi tao lich that).
/// </summary>
public record GiaiPhongGiuChoCommand(int IdGiuCho) : IRequest<Unit>;
