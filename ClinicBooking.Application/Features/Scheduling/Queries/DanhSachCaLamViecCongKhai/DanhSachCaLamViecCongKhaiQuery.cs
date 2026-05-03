using ClinicBooking.Application.Features.Scheduling.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.Scheduling.Queries.DanhSachCaLamViecCongKhai;

public sealed record DanhSachCaLamViecCongKhaiQuery(
    int SoTrang = 1,
    int KichThuocTrang = 20,
    int? IdBacSi = null,
    int? IdChuyenKhoa = null,
    int? IdPhong = null,
    DateOnly? TuNgay = null,
    DateOnly? DenNgay = null,
    bool? ConTrong = null) : IRequest<IReadOnlyList<CaLamViecPublicResponse>>;
