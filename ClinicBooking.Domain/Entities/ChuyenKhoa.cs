namespace ClinicBooking.Domain.Entities;

public class ChuyenKhoa
{
    public int IdChuyenKhoa { get; set; }
    public string TenChuyenKhoa { get; set; } = string.Empty;
    public string? MoTa { get; set; }
    public int ThoiGianSlotMacDinh { get; set; }
    public TimeOnly? GioMoDatLich { get; set; }
    public TimeOnly? GioDongDatLich { get; set; }
    public bool HienThi { get; set; }

    // Navigation
    public ICollection<BacSi> DanhSachBacSi { get; set; } = new List<BacSi>();
    public ICollection<DichVu> DanhSachDichVu { get; set; } = new List<DichVu>();
    public ICollection<CaLamViec> DanhSachCaLamViec { get; set; } = new List<CaLamViec>();
}
