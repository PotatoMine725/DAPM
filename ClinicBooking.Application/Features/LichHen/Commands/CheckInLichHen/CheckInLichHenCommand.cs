using ClinicBooking.Application.Features.HangCho.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.LichHen.Commands.CheckInLichHen;

/// <summary>
/// Le tan check-in benh nhan: tao HangCho voi SoThuTu = MAX+1 theo ca.
/// </summary>
public record CheckInLichHenCommand(int IdLichHen) : IRequest<HangChoResponse>;
