namespace ClinicBooking.Api.Contracts.DanhMuc;

public record DichVuDto(
    int IdDichVu,
    int IdChuyenKhoa,
    string TenDichVu,
    string? MoTa,
    int? ThoiGianUocTinh,
    bool HienThi,
    DateTime NgayTao,
    string? TenChuyenKhoa);
