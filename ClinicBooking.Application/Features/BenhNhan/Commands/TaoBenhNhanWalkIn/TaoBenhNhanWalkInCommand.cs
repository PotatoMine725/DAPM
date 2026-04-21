using ClinicBooking.Domain.Enums;
using MediatR;

namespace ClinicBooking.Application.Features.BenhNhan.Commands.TaoBenhNhanWalkIn;

public sealed record TaoBenhNhanWalkInCommand(
    string HoTen,
    string SoDienThoai,
    DateOnly? NgaySinh,
    GioiTinh? GioiTinh,
    string? Cccd,
    string? DiaChi) : IRequest<TaoBenhNhanWalkInResult>;
