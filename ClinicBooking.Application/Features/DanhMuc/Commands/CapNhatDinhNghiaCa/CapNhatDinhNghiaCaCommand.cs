using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatDinhNghiaCa;

public sealed record CapNhatDinhNghiaCaCommand(
    int IdDinhNghiaCa,
    string TenCa,
    TimeOnly GioBatDauMacDinh,
    TimeOnly GioKetThucMacDinh,
    string? MoTa,
    bool TrangThai) : IRequest<Unit>;
