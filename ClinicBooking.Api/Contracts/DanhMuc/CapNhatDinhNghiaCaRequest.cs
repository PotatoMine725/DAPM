namespace ClinicBooking.Api.Contracts.DanhMuc;

public record CapNhatDinhNghiaCaRequest(
    string TenCa,
    TimeOnly GioBatDauMacDinh,
    TimeOnly GioKetThucMacDinh,
    string? MoTa,
    bool TrangThai);
