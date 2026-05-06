using MediatR;

namespace ClinicBooking.Application.Features.ToaThuoc.Commands.HuyToaThuoc;

public record HuyToaThuocCommand(int IdHoSoKham, int IdToaThuoc) : IRequest;
