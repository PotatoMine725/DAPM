using MediatR;

namespace ClinicBooking.Application.Features.ThongBao.Commands.DanhDauDaDocThongBao;

/// <summary>
/// Danh dau mot thong bao la da doc.
/// <paramref name="IdThongBao"/> = null de danh dau TAT CA thong bao chua doc cua nguoi dung.
/// </summary>
public sealed record DanhDauDaDocThongBaoCommand(int? IdThongBao) : IRequest<Unit>;
