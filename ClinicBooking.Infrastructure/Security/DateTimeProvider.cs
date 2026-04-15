using ClinicBooking.Application.Abstractions.Security;

namespace ClinicBooking.Infrastructure.Security;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
