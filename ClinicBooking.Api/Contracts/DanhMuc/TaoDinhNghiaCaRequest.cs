namespace ClinicBooking.Api.Contracts.DanhMuc;

public record TaoDinhNghiaCaRequest(
    string TenCa,
    TimeOnly GioBatDauMacDinh,
    TimeOnly GioKetThucMacDinh,
    string? MoTa,
    bool TrangThai);
