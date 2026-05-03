namespace ClinicBooking.Api.Contracts.DanhMuc;

public record DichVuCongKhaiDto(
    int IdDichVu,
    int IdChuyenKhoa,
    string TenDichVu,
    string? MoTa,
    int? ThoiGianUocTinh,
    string TenChuyenKhoa);
