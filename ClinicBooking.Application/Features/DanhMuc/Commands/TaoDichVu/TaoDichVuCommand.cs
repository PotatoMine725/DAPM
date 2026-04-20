using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.TaoDichVu;

public sealed record TaoDichVuCommand(
    int IdChuyenKhoa,
    string TenDichVu,
    string? MoTa,
    int? ThoiGianUocTinh,
    bool HienThi) : IRequest<int>;
