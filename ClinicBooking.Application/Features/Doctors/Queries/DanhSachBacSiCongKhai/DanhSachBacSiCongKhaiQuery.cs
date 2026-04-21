using ClinicBooking.Application.Features.Doctors.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.Doctors.Queries.DanhSachBacSiCongKhai;

public sealed record DanhSachBacSiCongKhaiQuery(
    int SoTrang = 1,
    int KichThuocTrang = 20,
    int? IdChuyenKhoa = null,
    bool? DangLamViec = null,
    string? TuKhoa = null) : IRequest<IReadOnlyList<BacSiPublicResponse>>;
