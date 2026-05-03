using MediatR;

namespace ClinicBooking.Application.Features.Scheduling.Commands.SinhCaLamViecTuLichNoiTru;

public sealed record SinhCaLamViecTuLichNoiTruCommand(int SoNgaySinhTruoc) : IRequest<int>;
