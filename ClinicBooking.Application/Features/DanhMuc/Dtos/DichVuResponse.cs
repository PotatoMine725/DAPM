using DichVuEntity = ClinicBooking.Domain.Entities.DichVu;

namespace ClinicBooking.Application.Features.DanhMuc.Dtos;

public sealed record DichVuResponse(
    int IdDichVu,
    int IdChuyenKhoa,
    string TenDichVu,
    string? MoTa,
    int? ThoiGianUocTinh,
    bool HienThi,
    DateTime NgayTao,
    string? TenChuyenKhoa)
{
    public static DichVuResponse TuEntity(DichVuEntity entity) => new(
        entity.IdDichVu,
        entity.IdChuyenKhoa,
        entity.TenDichVu,
        entity.MoTa,
        entity.ThoiGianUocTinh,
        entity.HienThi,
        entity.NgayTao,
        entity.ChuyenKhoa?.TenChuyenKhoa);
}
