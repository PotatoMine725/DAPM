using MediatR;

namespace ClinicBooking.Application.Features.Auth.Commands.DangXuat;

public record DangXuatCommand(string RefreshToken) : IRequest;
