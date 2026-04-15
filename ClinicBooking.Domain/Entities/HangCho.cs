using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Domain.Entities;

public class HangCho
{
    public int IdHangCho { get; set; }
    public int IdCaLamViec { get; set; }
    public int IdLichHen { get; set; }
    public int SoThuTu { get; set; }
    public TrangThaiHangCho TrangThai { get; set; }
    public DateTime NgayCheckIn { get; set; }

    // Navigation
    public CaLamViec CaLamViec { get; set; } = null!;
    public LichHen LichHen { get; set; } = null!;
}
