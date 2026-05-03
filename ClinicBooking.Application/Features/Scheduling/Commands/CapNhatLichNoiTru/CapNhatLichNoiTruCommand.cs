using MediatR;

namespace ClinicBooking.Application.Features.Scheduling.Commands.CapNhatLichNoiTru;

public sealed record CapNhatLichNoiTruCommand(
    int IdLichNoiTru,
    int IdPhong,
    int IdDinhNghiaCa,
    int NgayTrongTuan,
    DateOnly NgayApDung,
    DateOnly? NgayKetThuc) : IRequest;
