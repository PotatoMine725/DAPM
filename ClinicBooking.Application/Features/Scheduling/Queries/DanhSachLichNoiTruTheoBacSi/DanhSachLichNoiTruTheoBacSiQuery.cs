using ClinicBooking.Application.Features.Scheduling.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.Scheduling.Queries.DanhSachLichNoiTruTheoBacSi;

public sealed record DanhSachLichNoiTruTheoBacSiQuery(
    int? IdBacSi = null,
    bool? ChiHienHieuLuc = null) : IRequest<IReadOnlyList<LichNoiTruResponse>>;
