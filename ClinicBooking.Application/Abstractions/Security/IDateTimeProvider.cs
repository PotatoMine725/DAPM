namespace ClinicBooking.Application.Abstractions.Security;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
