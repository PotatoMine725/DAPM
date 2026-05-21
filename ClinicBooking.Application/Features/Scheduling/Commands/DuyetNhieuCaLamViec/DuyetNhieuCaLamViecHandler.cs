using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClinicBooking.Application.Features.Scheduling.Commands.DuyetNhieuCaLamViec;

public sealed class DuyetNhieuCaLamViecHandler : IRequestHandler<DuyetNhieuCaLamViecCommand, DuyetNhieuCaLamViecResponse>
{
    private readonly IAppDbContext _db;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<DuyetNhieuCaLamViecHandler> _logger;

    public DuyetNhieuCaLamViecHandler(
        IAppDbContext db,
        IDateTimeProvider dateTimeProvider,
        ILogger<DuyetNhieuCaLamViecHandler> logger)
    {
        _db = db;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<DuyetNhieuCaLamViecResponse> Handle(DuyetNhieuCaLamViecCommand request, CancellationToken cancellationToken)
    {
        var soThanhCong = 0;
        var soThatBai = 0;
        var danhSachLoi = new List<CaLamViecLoi>();

        foreach (var idCa in request.DanhSachIdCaLamViec)
        {
            try
            {
                // Each approval in its own transaction for safety
                var ca = await _db.CaLamViec
                    .FirstOrDefaultAsync(x => x.IdCaLamViec == idCa, cancellationToken);

                if (ca is null)
                {
                    danhSachLoi.Add(new CaLamViecLoi(idCa, "Không tìm thấy ca làm việc"));
                    soThatBai++;
                    _logger.LogWarning("Bulk approve: Ca {IdCa} không tồn tại", idCa);
                    continue;
                }

                if (ca.TrangThaiDuyet != TrangThaiDuyetCa.ChoDuyet)
                {
                    danhSachLoi.Add(new CaLamViecLoi(idCa, $"Ca không ở trạng thái chờ duyệt (hiện tại: {ca.TrangThaiDuyet})"));
                    soThatBai++;
                    _logger.LogWarning("Bulk approve: Ca {IdCa} không ở trạng thái chờ duyệt", idCa);
                    continue;
                }

                ca.TrangThaiDuyet = TrangThaiDuyetCa.DaDuyet;
                ca.IdAdminDuyet = request.IdAdminDuyet;
                ca.NgayDuyet = _dateTimeProvider.UtcNow;
                ca.LyDoTuChoi = null;

                await _db.SaveChangesAsync(cancellationToken);
                soThanhCong++;
                _logger.LogInformation("Bulk approve: Ca {IdCa} đã được duyệt bởi admin {IdAdmin}", idCa, request.IdAdminDuyet);
            }
            catch (Exception ex)
            {
                danhSachLoi.Add(new CaLamViecLoi(idCa, $"Lỗi hệ thống: {ex.Message}"));
                soThatBai++;
                _logger.LogError(ex, "Bulk approve: Lỗi khi duyệt ca {IdCa}", idCa);
            }
        }

        return new DuyetNhieuCaLamViecResponse(soThanhCong, soThatBai, danhSachLoi);
    }
}
