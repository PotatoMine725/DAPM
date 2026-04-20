using ClinicBooking.Domain.Entities;

namespace ClinicBooking.Application.Features.DanhMuc.Dtos;

public sealed record ChuyenKhoaResponse(
    int IdChuyenKhoa,
    string TenChuyenKhoa,
    string? MoTa,
    int ThoiGianSlotMacDinh,
    TimeOnly? GioMoDatLich,
    TimeOnly? GioDongDatLich,
    bool HienThi)
{
    public static ChuyenKhoaResponse TuEntity(ChuyenKhoa entity) => new(
        entity.IdChuyenKhoa,
        entity.TenChuyenKhoa,
        entity.MoTa,
        entity.ThoiGianSlotMacDinh,
        entity.GioMoDatLich,
        entity.GioDongDatLich,
        entity.HienThi);
}
