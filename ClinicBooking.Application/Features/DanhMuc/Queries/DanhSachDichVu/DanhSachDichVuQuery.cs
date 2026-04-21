using ClinicBooking.Application.Features.DanhMuc.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDichVu;

public sealed record DanhSachDichVuQuery(
    int SoTrang = 1,
    int KichThuocTrang = 20,
    int? IdChuyenKhoa = null,
    bool? HienThi = null,
    string? TuKhoa = null) : IRequest<IReadOnlyList<DichVuResponse>>;
