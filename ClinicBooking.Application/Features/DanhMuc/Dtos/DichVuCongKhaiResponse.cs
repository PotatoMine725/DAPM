using ClinicBooking.Domain.Entities;

namespace ClinicBooking.Application.Features.DanhMuc.Dtos;

public sealed record DichVuCongKhaiResponse(
    int IdDichVu,
    int IdChuyenKhoa,
    string TenDichVu,
    string? MoTa,
    int? ThoiGianUocTinh,
    string TenChuyenKhoa)
{
    public static DichVuCongKhaiResponse TuEntity(DichVu entity) => new(
        entity.IdDichVu,
        entity.IdChuyenKhoa,
        entity.TenDichVu,
        entity.MoTa,
        entity.ThoiGianUocTinh,
        entity.ChuyenKhoa?.TenChuyenKhoa ?? string.Empty);
}
