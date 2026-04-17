using ClinicBooking.Application.Features.LichHen.Dtos;
using ClinicBooking.Domain.Enums;
using MediatR;

namespace ClinicBooking.Application.Features.LichHen.Queries.DanhSachLichHenCuaToi;

public record DanhSachLichHenCuaToiQuery(
    TrangThaiLichHen? TrangThai,
    int SoTrang,
    int KichThuocTrang) : IRequest<DanhSachLichHenResponse>;
