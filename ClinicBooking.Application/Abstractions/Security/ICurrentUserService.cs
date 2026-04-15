using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Application.Abstractions.Security;

public interface ICurrentUserService
{
    int? IdTaiKhoan { get; }
    VaiTro? VaiTro { get; }
    string? TenDangNhap { get; }
    bool DaXacThuc { get; }
}
