using ClinicBooking.Application.Features.Doctors.Dtos;
using ClinicBooking.Domain.Enums;
using MediatR;

namespace ClinicBooking.Application.Features.Doctors.Queries.DanhSachBacSiCongKhai;

public sealed record DanhSachBacSiCongKhaiQuery(
    int SoTrang = 1,
    int KichThuocTrang = 20,
    int? IdChuyenKhoa = null,
    string? TuKhoa = null,
    bool? DangLamViec = null,
    LoaiHopDong? LoaiHopDong = null) : IRequest<IReadOnlyList<BacSiPublicResponse>>;
