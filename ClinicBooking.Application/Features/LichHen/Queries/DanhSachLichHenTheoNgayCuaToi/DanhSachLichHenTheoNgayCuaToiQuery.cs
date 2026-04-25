using ClinicBooking.Application.Features.LichHen.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.LichHen.Queries.DanhSachLichHenTheoNgayCuaToi;

public sealed record DanhSachLichHenTheoNgayCuaToiQuery(DateOnly Ngay) : IRequest<IReadOnlyList<LichHenTomTatResponse>>;
