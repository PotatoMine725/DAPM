namespace ClinicBooking.Api.Contracts.ToaThuoc;

public record ToaThuocDto(
    int IdToaThuoc,
    int IdHoSoKham,
    int IdThuoc,
    string TenThuoc,
    string? LieuLuong,
    string? CachDung,
    int? SoNgayDung,
    string? GhiChu,
    DateTime NgayKham,
    string MaLichHen);
