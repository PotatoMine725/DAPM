using ClinicBooking.Domain.Entities;

namespace ClinicBooking.Application.Features.DanhMuc.Dtos;

public sealed record DinhNghiaCaResponse(
    int IdDinhNghiaCa,
    string TenCa,
    TimeOnly GioBatDauMacDinh,
    TimeOnly GioKetThucMacDinh,
    string? MoTa,
    bool TrangThai)
{
    public static DinhNghiaCaResponse TuEntity(DinhNghiaCa entity) => new(
        entity.IdDinhNghiaCa,
        entity.TenCa,
        entity.GioBatDauMacDinh,
        entity.GioKetThucMacDinh,
        entity.MoTa,
        entity.TrangThai);
}
