using MediatR;

namespace ClinicBooking.Application.Features.BenhNhan.Queries.TimBenhNhanTheoSdt;

public sealed record TimBenhNhanTheoSdtQuery(string SoDienThoai)
    : IRequest<TimBenhNhanKetQua?>;
