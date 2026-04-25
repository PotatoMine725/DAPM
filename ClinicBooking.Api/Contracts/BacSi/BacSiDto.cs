namespace ClinicBooking.Api.Contracts.BacSi;

public record BacSiDto(
    int IdBacSi,
    int IdTaiKhoan,
    int IdChuyenKhoa,
    string HoTen,
    string? AnhDaiDien,
    string? BangCap,
    int? NamKinhNghiem,
    string? TieuSu,
    string LoaiHopDong,
    string TrangThai,
    string TenChuyenKhoa);
