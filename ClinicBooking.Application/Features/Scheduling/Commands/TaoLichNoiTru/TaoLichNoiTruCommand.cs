using MediatR;

namespace ClinicBooking.Application.Features.Scheduling.Commands.TaoLichNoiTru;

public sealed record TaoLichNoiTruCommand(
    int IdBacSi,
    int IdPhong,
    int IdDinhNghiaCa,
    int NgayTrongTuan,
    DateOnly NgayApDung,
    DateOnly? NgayKetThuc) : IRequest<int>;
