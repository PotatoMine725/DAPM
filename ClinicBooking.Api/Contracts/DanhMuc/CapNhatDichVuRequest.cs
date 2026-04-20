namespace ClinicBooking.Api.Contracts.DanhMuc;

public record CapNhatDichVuRequest(
    int IdChuyenKhoa,
    string TenDichVu,
    string? MoTa,
    int? ThoiGianUocTinh,
    bool HienThi);
