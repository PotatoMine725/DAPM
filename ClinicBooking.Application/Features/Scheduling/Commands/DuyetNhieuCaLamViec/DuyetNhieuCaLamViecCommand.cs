using MediatR;

namespace ClinicBooking.Application.Features.Scheduling.Commands.DuyetNhieuCaLamViec;

public sealed record DuyetNhieuCaLamViecCommand(
    List<int> DanhSachIdCaLamViec,
    int IdAdminDuyet) : IRequest<DuyetNhieuCaLamViecResponse>;

public sealed record DuyetNhieuCaLamViecResponse(
    int SoThanhCong,
    int SoThatBai,
    List<CaLamViecLoi> DanhSachLoi);

public sealed record CaLamViecLoi(
    int IdCaLamViec,
    string LyDo);
