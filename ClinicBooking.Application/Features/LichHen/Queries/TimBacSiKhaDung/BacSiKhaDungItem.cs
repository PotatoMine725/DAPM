namespace ClinicBooking.Application.Features.LichHen.Queries.TimBacSiKhaDung;

public sealed record BacSiKhaDungItem(
    int IdBacSi,
    string HoTen,
    int SoSlotConLai,
    int IdCaLamViec
);
