using ClinicBooking.Application.Abstractions.Security;

namespace ClinicBooking.Infrastructure.Security;

public class PasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 11;

    public string HashPassword(string matKhauThuong)
    {
        return BCrypt.Net.BCrypt.HashPassword(matKhauThuong, WorkFactor);
    }

    public bool VerifyPassword(string matKhauThuong, string matKhauHash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(matKhauThuong, matKhauHash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            // Hash khong dung dinh dang BCrypt (vi du hash gia lap trong seed data).
            return false;
        }
    }
}
