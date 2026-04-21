namespace ClinicBooking.Api.Contracts.Doctors;

public record BacSiPublicDto(
    int IdBacSi,
    int IdChuyenKhoa,
    string HoTen,
    string? AnhDaiDien,
    string? BangCap,
    int? NamKinhNghiem,
    string? TieuSu,
    string LoaiHopDong,
    string TrangThai,
    string TenChuyenKhoa);
