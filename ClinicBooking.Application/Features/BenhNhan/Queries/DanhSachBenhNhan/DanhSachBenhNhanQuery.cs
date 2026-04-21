using ClinicBooking.Application.Features.BenhNhan.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.BenhNhan.Queries.DanhSachBenhNhan;

public sealed record DanhSachBenhNhanQuery(
    int SoTrang = 1,
    int KichThuocTrang = 20,
    bool? BiHanChe = null,
    string? TuKhoa = null) : IRequest<IReadOnlyList<BenhNhanResponse>>;
