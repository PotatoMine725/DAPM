using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Commands.TaoChuyenKhoa;

/// <summary>
/// Command to create a new ChuyenKhoa (specialty) record.
/// </summary>
public sealed record TaoChuyenKhoaCommand(
    string TenChuyenKhoa,
    string? MoTa,
    int ThoiGianSlotMacDinh,
    TimeOnly? GioMoDatLich,
    TimeOnly? GioDongDatLich,
    bool HienThi) : IRequest<int>;