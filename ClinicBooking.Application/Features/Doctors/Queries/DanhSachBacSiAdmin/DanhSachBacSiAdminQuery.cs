using ClinicBooking.Application.Features.Doctors.Dtos;
using ClinicBooking.Domain.Enums;
using MediatR;

namespace ClinicBooking.Application.Features.Doctors.Queries.DanhSachBacSiAdmin;

public sealed record DanhSachBacSiAdminQuery(
    int SoTrang = 1,
    int KichThuocTrang = 100,
    int? IdChuyenKhoa = null,
    LoaiHopDong? LoaiHopDong = null,
    TrangThaiBacSi? TrangThai = null,
    string? TuKhoa = null) : IRequest<IReadOnlyList<BacSiAdminResponse>>;
