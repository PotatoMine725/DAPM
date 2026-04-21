using ClinicBooking.Domain.Entities;

namespace ClinicBooking.Application.Features.DanhMuc.Dtos;

public sealed record PhongResponse(
    int IdPhong,
    string MaPhong,
    string TenPhong,
    int? SucChua,
    string? TrangBi,
    bool TrangThai)
{
    public static PhongResponse TuEntity(Phong entity) => new(
        entity.IdPhong,
        entity.MaPhong,
        entity.TenPhong,
        entity.SucChua,
        entity.TrangBi,
        entity.TrangThai);
}
