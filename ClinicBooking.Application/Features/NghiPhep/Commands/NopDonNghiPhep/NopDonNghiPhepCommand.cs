using MediatR;

namespace ClinicBooking.Application.Features.NghiPhep.Commands.NopDonNghiPhep;

public sealed record NopDonNghiPhepCommand(
    int IdBacSi,
    int IdCaLamViec,
    string LoaiNghiPhep,
    string LyDo) : IRequest<int>;
