using ClinicBooking.Application.Features.LichHen.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.LichHen.Commands.TaoLichHen;

public record TaoLichHenCommand(
    DateOnly NgayLamViec,
    TimeOnly GioMongMuon,
    int IdDichVu,
    int? IdBenhNhan,
    int? IdBacSiMongMuon,
    string? BacSiMongMuonNote,
    string? TrieuChung) : IRequest<LichHenResponse>;
