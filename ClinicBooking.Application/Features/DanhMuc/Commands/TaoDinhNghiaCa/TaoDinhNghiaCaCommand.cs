using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.TaoDinhNghiaCa;

public sealed record TaoDinhNghiaCaCommand(
    string TenCa,
    TimeOnly GioBatDauMacDinh,
    TimeOnly GioKetThucMacDinh,
    string? MoTa,
    bool TrangThai) : IRequest<int>;
