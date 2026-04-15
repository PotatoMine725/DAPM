namespace ClinicBooking.Application.Abstractions.Security;

public interface IPasswordHasher
{
    string HashPassword(string matKhauThuong);
    bool VerifyPassword(string matKhauThuong, string matKhauHash);
}
