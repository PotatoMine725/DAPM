using ClinicBooking.Application.Features.Doctors.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.Doctors.Queries.DanhSachBacSiCongKhai;

public sealed record DanhSachBacSiCongKhaiQuery(
    int SoTrang = 1,
    int KichThuocTrang = 20,
    int? IdChuyenKhoa = null,
    string? TuKhoa = null,
    bool? DangLamViec = null) : IRequest<IReadOnlyList<BacSiPublicResponse>>;
