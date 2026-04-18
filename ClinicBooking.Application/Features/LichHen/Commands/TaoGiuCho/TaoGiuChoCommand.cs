using ClinicBooking.Application.Features.LichHen.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.LichHen.Commands.TaoGiuCho;

/// <summary>
/// Le tan tao mot luot giu cho tam thoi cho benh nhan. Het han sau
/// <c>LichHenOptions.GiuChoThoiHanPhut</c> phut neu khong duoc chuyen thanh lich hen.
/// </summary>
public record TaoGiuChoCommand(
    int IdCaLamViec,
    int IdBenhNhan) : IRequest<GiuChoResponse>;
