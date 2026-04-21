namespace ClinicBooking.Application.Features.Thuoc.Dtos;

public sealed record ThuocResponse(
    int IdThuoc,
    string TenThuoc,
    string? HoatChat,
    string? DonVi,
    string? GhiChu)
{
    public static ThuocResponse TuEntity(ClinicBooking.Domain.Entities.Thuoc entity) => new(
        entity.IdThuoc,
        entity.TenThuoc,
        entity.HoatChat,
        entity.DonVi,
        entity.GhiChu);
}
