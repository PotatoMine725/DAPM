using ClinicBooking.Application.Features.DanhMuc.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDichVuCongKhai;

public sealed record DanhSachDichVuCongKhaiQuery(
    int SoTrang = 1,
    int KichThuocTrang = 20,
    int? IdChuyenKhoa = null,
    string? TuKhoa = null) : IRequest<IReadOnlyList<DichVuCongKhaiResponse>>;
