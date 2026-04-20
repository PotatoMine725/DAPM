namespace ClinicBooking.Api.Contracts.DanhMuc;

public record ChuyenKhoaDto(
    int IdChuyenKhoa,
    string TenChuyenKhoa,
    string? MoTa,
    int ThoiGianSlotMacDinh,
    TimeOnly? GioMoDatLich,
    TimeOnly? GioDongDatLich,
    bool HienThi);
