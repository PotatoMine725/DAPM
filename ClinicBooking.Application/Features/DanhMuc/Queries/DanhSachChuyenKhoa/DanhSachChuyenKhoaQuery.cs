using ClinicBooking.Application.Features.DanhMuc.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoa;

public sealed record DanhSachChuyenKhoaQuery(
    int SoTrang = 1,
    int KichThuocTrang = 20,
    bool? HienThi = null,
    string? TuKhoa = null) : IRequest<IReadOnlyList<ChuyenKhoaResponse>>;
