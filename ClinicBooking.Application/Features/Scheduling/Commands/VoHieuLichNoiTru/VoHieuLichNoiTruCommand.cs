using MediatR;

namespace ClinicBooking.Application.Features.Scheduling.Commands.VoHieuLichNoiTru;

public sealed record VoHieuLichNoiTruCommand(int IdLichNoiTru) : IRequest;
