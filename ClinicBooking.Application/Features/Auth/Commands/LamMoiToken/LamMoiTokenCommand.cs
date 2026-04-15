using ClinicBooking.Application.Features.Auth.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.Auth.Commands.LamMoiToken;

public record LamMoiTokenCommand(string RefreshToken) : IRequest<XacThucResponse>;
