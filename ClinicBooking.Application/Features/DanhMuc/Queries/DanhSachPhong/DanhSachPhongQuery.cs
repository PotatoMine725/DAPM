using ClinicBooking.Application.Features.DanhMuc.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachPhong;

public sealed record DanhSachPhongQuery(
    int SoTrang = 1,
    int KichThuocTrang = 20,
    bool? TrangThai = null,
    string? TuKhoa = null) : IRequest<IReadOnlyList<PhongResponse>>;
