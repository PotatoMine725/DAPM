using ClinicBooking.Application.Features.Thuoc.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.Thuoc.Queries.DanhSachThuoc;

public sealed record DanhSachThuocQuery(
    int SoTrang = 1,
    int KichThuocTrang = 20,
    string? TuKhoa = null) : IRequest<IReadOnlyList<ThuocResponse>>;
