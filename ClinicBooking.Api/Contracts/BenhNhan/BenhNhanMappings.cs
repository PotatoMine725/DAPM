using ClinicBooking.Application.Features.BenhNhan.Dtos;

namespace ClinicBooking.Api.Contracts.BenhNhan;

public static class BenhNhanMappings
{
    public static BenhNhanDto TuDto(this BenhNhanResponse response) => new(
        response.IdBenhNhan,
        response.IdTaiKhoan,
        response.HoTen,
        response.SoDienThoai,
        response.Email,
        response.Cccd,
        response.NgaySinh,
        response.GioiTinh,
        response.DiaChi,
        response.SoLanHuyMuon,
        response.BiHanChe,
        response.NgayHetHanChe,
        response.NgayTao);
}
