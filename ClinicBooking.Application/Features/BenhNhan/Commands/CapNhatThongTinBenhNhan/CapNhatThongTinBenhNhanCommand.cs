using ClinicBooking.Domain.Enums;
using MediatR;

namespace ClinicBooking.Application.Features.BenhNhan.Commands.CapNhatThongTinBenhNhan;

public sealed record CapNhatThongTinBenhNhanCommand(
    int IdBenhNhan,
    string HoTen,
    DateOnly? NgaySinh,
    GioiTinh? GioiTinh,
    string? Cccd,
    string? DiaChi) : IRequest<Unit>;
