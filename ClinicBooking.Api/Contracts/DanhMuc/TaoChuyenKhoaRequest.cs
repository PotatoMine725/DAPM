namespace ClinicBooking.Api.Contracts.DanhMuc;

public record TaoChuyenKhoaRequest(
    string TenChuyenKhoa,
    string? MoTa,
    int ThoiGianSlotMacDinh,
    TimeOnly? GioMoDatLich,
    TimeOnly? GioDongDatLich,
    bool HienThi);
