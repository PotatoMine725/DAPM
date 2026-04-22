using ClinicBooking.Application.Features.DanhMuc.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoaCongKhai;

public sealed record DanhSachChuyenKhoaCongKhaiQuery(
    int SoTrang = 1,
    int KichThuocTrang = 20,
    string? TuKhoa = null) : IRequest<IReadOnlyList<ChuyenKhoaCongKhaiResponse>>;
