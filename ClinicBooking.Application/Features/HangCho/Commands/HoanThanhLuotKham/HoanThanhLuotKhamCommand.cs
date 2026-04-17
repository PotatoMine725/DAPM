using MediatR;

namespace ClinicBooking.Application.Features.HangCho.Commands.HoanThanhLuotKham;

/// <summary>
/// Danh dau mot luot kham trong hang cho da hoan thanh.
/// Cap nhat HangCho.TrangThai = HoanThanh va LichHen.TrangThai = HoanThanh.
/// </summary>
public record HoanThanhLuotKhamCommand(int IdHangCho) : IRequest<Unit>;
