namespace ClinicBooking.Domain.Entities;

public class GiuCho
{
    public int IdGiuCho { get; set; }
    public int IdCaLamViec { get; set; }
    public int SoSlot { get; set; }
    public int IdBenhNhan { get; set; }
    public DateTime GioHetHan { get; set; }
    public bool DaGiaiPhong { get; set; }
    public DateTime NgayTao { get; set; }

    // Navigation
    public CaLamViec CaLamViec { get; set; } = null!;
    public BenhNhan BenhNhan { get; set; } = null!;
}
