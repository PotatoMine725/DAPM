using MediatR;

namespace ClinicBooking.Application.Features.BenhNhan.Queries.KiemTraQuyenDatLich;

public sealed record KiemTraQuyenDatLichQuery(int IdBenhNhan)
    : IRequest<KiemTraQuyenDatLichResult>;
