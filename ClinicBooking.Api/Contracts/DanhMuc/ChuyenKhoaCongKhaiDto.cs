namespace ClinicBooking.Api.Contracts.DanhMuc;

public record ChuyenKhoaCongKhaiDto(
    int IdChuyenKhoa,
    string TenChuyenKhoa,
    string? MoTa,
    int ThoiGianSlotMacDinh,
    TimeOnly? GioMoDatLich,
    TimeOnly? GioDongDatLich);
