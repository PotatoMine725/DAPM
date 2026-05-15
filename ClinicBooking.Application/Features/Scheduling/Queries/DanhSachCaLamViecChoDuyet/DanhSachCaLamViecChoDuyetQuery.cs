using ClinicBooking.Application.Features.Scheduling.Dtos;
using ClinicBooking.Domain.Enums;
using MediatR;

namespace ClinicBooking.Application.Features.Scheduling.Queries.DanhSachCaLamViecChoDuyet;

public sealed record DanhSachCaLamViecChoDuyetQuery(
    int SoTrang = 1,
    int KichThuocTrang = 50,
    TrangThaiDuyetCa? TrangThaiDuyet = TrangThaiDuyetCa.ChoDuyet,
    NguonTaoCa? NguonTaoCa = null,
    int? IdChuyenKhoa = null,
    DateOnly? TuNgay = null,
    DateOnly? DenNgay = null) : IRequest<IReadOnlyList<CaLamViecAdminResponse>>;
