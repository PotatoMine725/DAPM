using MediatR;

namespace ClinicBooking.Application.Features.Auth.Queries.KiemTraGhostTheoSdt;

public record KiemTraGhostTheoSdtQuery(string SoDienThoai) : IRequest<KiemTraGhostKetQua>;

public record KiemTraGhostKetQua(bool IsGhost, int? IdTaiKhoan);
