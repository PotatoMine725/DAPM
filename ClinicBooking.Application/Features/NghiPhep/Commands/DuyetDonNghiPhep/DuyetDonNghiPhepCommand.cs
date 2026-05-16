using MediatR;

namespace ClinicBooking.Application.Features.NghiPhep.Commands.DuyetDonNghiPhep;

public sealed record DuyetDonNghiPhepCommand(
    int IdDonNghiPhep,
    bool ChapNhan,
    string? LyDoTuChoi,
    int IdNguoiDuyet) : IRequest<Unit>;
