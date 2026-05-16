using MediatR;

namespace ClinicBooking.Application.Features.Scheduling.Queries.KiemTraDoPhuBacSi;

public sealed record KiemTraDoPhuBacSiQuery(
    int IdChuyenKhoa,
    DateOnly TuNgay,
    DateOnly DenNgay) : IRequest<KiemTraDoPhuBacSiResponse>;

public sealed record KiemTraDoPhuBacSiResponse(
    IReadOnlyList<NgayThieuBacSiDto> NgayThieu);

public sealed record NgayThieuBacSiDto(
    DateOnly Ngay,
    int SoCaChoDuyet,
    bool HoanToanTrong);