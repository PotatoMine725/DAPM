using ClinicBooking.Application.Features.Auth.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.Auth.Commands.DangNhap;

public record DangNhapCommand(
    string TenDangNhapHoacEmail,
    string MatKhau) : IRequest<XacThucResponse>;
