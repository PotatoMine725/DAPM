using DichVuEntity = ClinicBooking.Domain.Entities.DichVu;

namespace ClinicBooking.Application.Features.DanhMuc.Dtos;

public sealed record DichVuCongKhaiResponse(
    int IdDichVu,
    int IdChuyenKhoa,
    string TenDichVu,
    string? MoTa,
    int? ThoiGianUocTinh,
    string TenChuyenKhoa)
{
    public static DichVuCongKhaiResponse TuEntity(DichVuEntity entity) => new(
        entity.IdDichVu,
        entity.IdChuyenKhoa,
        entity.TenDichVu,
        entity.MoTa,
        entity.ThoiGianUocTinh,
        entity.ChuyenKhoa?.TenChuyenKhoa ?? string.Empty);
}
