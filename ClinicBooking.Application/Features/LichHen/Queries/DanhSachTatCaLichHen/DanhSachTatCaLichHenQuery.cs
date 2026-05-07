using ClinicBooking.Application.Common;
using ClinicBooking.Application.Features.LichHen.Dtos;
using ClinicBooking.Domain.Enums;
using MediatR;

namespace ClinicBooking.Application.Features.LichHen.Queries.DanhSachTatCaLichHen;

public record DanhSachTatCaLichHenQuery(
    DateOnly? Ngay,
    TrangThaiLichHen? TrangThai,
    int Trang = 1,
    int SoTrenMoiTrang = 10
) : IRequest<PhanTrangKetQua<LichHenTomTatResponse>>;
