namespace ClinicBooking.Api.Contracts.DanhMuc;

public record TaoPhongRequest(
    string MaPhong,
    string TenPhong,
    int? SucChua,
    string? TrangBi,
    bool TrangThai);
