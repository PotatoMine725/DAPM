namespace ClinicBooking.Api.Contracts.DanhMuc;

public record TaoDichVuRequest(
    int IdChuyenKhoa,
    string TenDichVu,
    string? MoTa,
    int? ThoiGianUocTinh,
    bool HienThi);
