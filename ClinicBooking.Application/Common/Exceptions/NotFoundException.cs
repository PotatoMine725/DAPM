namespace ClinicBooking.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string ten, object khoa)
        : base($"Khong tim thay {ten} voi khoa '{khoa}'.")
    {
    }
}
