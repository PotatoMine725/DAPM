namespace ClinicBooking.Api.Contracts.DanhMuc;

public record CapNhatPhongRequest(
    string MaPhong,
    string TenPhong,
    int? SucChua,
    string? TrangBi,
    bool TrangThai);
