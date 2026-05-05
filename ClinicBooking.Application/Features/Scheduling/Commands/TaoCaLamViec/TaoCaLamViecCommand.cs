using MediatR;

namespace ClinicBooking.Application.Features.Scheduling.Commands.TaoCaLamViec;

public sealed record TaoCaLamViecCommand(
    int IdBacSi,
    int IdPhong,
    int IdChuyenKhoa,
    int IdDinhNghiaCa,
    DateOnly NgayLamViec,
    TimeOnly GioBatDau,
    TimeOnly GioKetThuc,
    int ThoiGianSlot,
    int SoSlotToiDa,
    int? IdBacSiYeuCau = null) : IRequest<int>;
