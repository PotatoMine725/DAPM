namespace ClinicBooking.Api.Contracts.DanhMuc;

public record PhongDto(
    int IdPhong,
    string MaPhong,
    string TenPhong,
    int? SucChua,
    string? TrangBi,
    bool TrangThai);
