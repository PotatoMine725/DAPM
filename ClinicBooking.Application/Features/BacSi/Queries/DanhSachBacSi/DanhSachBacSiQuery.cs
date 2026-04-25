using ClinicBooking.Application.Features.BacSi.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.BacSi.Queries.DanhSachBacSi;

public sealed record DanhSachBacSiQuery(
    int SoTrang = 1,
    int KichThuocTrang = 20,
    int? IdChuyenKhoa = null,
    string? TuKhoa = null) : IRequest<IReadOnlyList<BacSiResponse>>;
