using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Api.Contracts.BenhNhan;

public record BenhNhanDto(
    int IdBenhNhan,
    int IdTaiKhoan,
    string HoTen,
    string SoDienThoai,
    string Email,
    string? Cccd,
    DateOnly? NgaySinh,
    GioiTinh? GioiTinh,
    string? DiaChi,
    int SoLanHuyMuon,
    bool BiHanChe,
    DateTime? NgayHetHanChe,
    DateTime NgayTao);
