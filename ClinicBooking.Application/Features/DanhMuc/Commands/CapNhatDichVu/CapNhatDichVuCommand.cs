using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatDichVu;

public sealed record CapNhatDichVuCommand(
    int IdDichVu,
    int IdChuyenKhoa,
    string TenDichVu,
    string? MoTa,
    int? ThoiGianUocTinh,
    bool HienThi) : IRequest<Unit>;
