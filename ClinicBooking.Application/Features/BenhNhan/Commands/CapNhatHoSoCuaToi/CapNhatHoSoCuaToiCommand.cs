using ClinicBooking.Domain.Enums;
using MediatR;

namespace ClinicBooking.Application.Features.BenhNhan.Commands.CapNhatHoSoCuaToi;

public sealed record CapNhatHoSoCuaToiCommand(
    string HoTen,
    DateOnly? NgaySinh,
    GioiTinh? GioiTinh,
    string? Cccd,
    string? DiaChi) : IRequest<Unit>;
