using ClinicBooking.Application.Features.LichHen.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.LichHen.Queries.DanhSachLichHenTheoNgay;

/// <summary>
/// Danh sach lich hen trong mot ngay cu the — dung cho le tan/admin xem tong quan.
/// </summary>
public record DanhSachLichHenTheoNgayQuery(DateOnly Ngay) : IRequest<IReadOnlyList<LichHenTomTatResponse>>;
