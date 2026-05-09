using MediatR;

namespace ClinicBooking.Application.Features.DichVu.Queries.DanhSachDichVu;

public sealed record DanhSachDichVuQuery() : IRequest<List<DichVuResponse>>;
