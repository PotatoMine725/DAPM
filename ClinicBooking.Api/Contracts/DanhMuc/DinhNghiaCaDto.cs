namespace ClinicBooking.Api.Contracts.DanhMuc;

public record DinhNghiaCaDto(
    int IdDinhNghiaCa,
    string TenCa,
    TimeOnly GioBatDauMacDinh,
    TimeOnly GioKetThucMacDinh,
    string? MoTa,
    bool TrangThai);
