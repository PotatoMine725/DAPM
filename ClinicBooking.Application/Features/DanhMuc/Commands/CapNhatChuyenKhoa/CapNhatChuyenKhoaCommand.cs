using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatChuyenKhoa;

public sealed record CapNhatChuyenKhoaCommand(
    int IdChuyenKhoa,
    string TenChuyenKhoa,
    string? MoTa,
    int ThoiGianSlotMacDinh,
    TimeOnly? GioMoDatLich,
    TimeOnly? GioDongDatLich,
    bool HienThi) : IRequest<Unit>;
