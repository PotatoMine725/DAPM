using ClinicBooking.Application.Features.LichHen.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.LichHen.Commands.TaoLichHen;

/// <summary>
/// Tao moi mot lich hen. benh_nhan tu dat cho minh (khong truyen IdBenhNhan),
/// le_tan dat ho (bat buoc truyen IdBenhNhan).
/// </summary>
public record TaoLichHenCommand(
    int IdCaLamViec,
    int IdDichVu,
    int? IdBenhNhan,
    int? IdBacSiMongMuon,
    string? BacSiMongMuonNote,
    string? TrieuChung) : IRequest<LichHenResponse>;
