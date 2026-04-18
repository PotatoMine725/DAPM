using ClinicBooking.Application.Features.LichHen.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.LichHen.Commands.DoiLichHen;

/// <summary>
/// Doi lich hen sang ca moi. Giu nguyen IdBenhNhan, IdDichVu (neu khong truyen thi giu gia tri cu).
/// Tao lich hen moi va huy lich hen cu trong mot transaction — IdLichHenTruoc trong LichSuLichHen
/// tro ve lich goc cho audit (chain khong reset).
/// </summary>
public record DoiLichHenCommand(
    int IdLichHenCu,
    int IdCaLamViecMoi,
    int? IdDichVuMoi,
    int? IdBacSiMongMuon,
    string? BacSiMongMuonNote,
    string? TrieuChung,
    string? LyDo) : IRequest<LichHenResponse>;
