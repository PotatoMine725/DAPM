using ClinicBooking.Application.Features.HangCho.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.HangCho.Commands.GoiBenhNhanKeTiep;

/// <summary>
/// Goi benh nhan tiep theo trong hang cho cua ca.
/// Neu <see cref="ClinicBooking.Application.Common.Options.LichHenOptions.TuDongHoanThanhLuotHienTai"/> = true,
/// luot dang kham truoc do se duoc dong dau HoanThanh.
/// </summary>
public record GoiBenhNhanKeTiepCommand(int IdCaLamViec) : IRequest<HangChoResponse>;
