using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Application.Features.BenhNhan.Dtos;

public sealed record BenhNhanResponse(
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
    DateTime NgayTao)
{
    public static BenhNhanResponse TuEntity(ClinicBooking.Domain.Entities.BenhNhan entity) => new(
        entity.IdBenhNhan,
        entity.IdTaiKhoan,
        entity.HoTen,
        entity.TaiKhoan.SoDienThoai,
        entity.TaiKhoan.Email,
        entity.Cccd,
        entity.NgaySinh,
        entity.GioiTinh,
        entity.DiaChi,
        entity.SoLanHuyMuon,
        entity.BiHanChe,
        entity.NgayHetHanChe,
        entity.NgayTao);
}
