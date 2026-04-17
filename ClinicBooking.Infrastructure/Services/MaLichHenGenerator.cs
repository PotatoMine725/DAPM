using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Application.Common.Options;
using ClinicBooking.Application.Common.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClinicBooking.Infrastructure.Services;

/// <summary>
/// Sinh <c>MaLichHen</c> theo format <c>{Prefix}-{yyyyMMdd}-{seq6}</c>.
/// Sequence tinh bang <c>MAX(MaLichHen)</c> parse 6 ky tu cuoi + 1.
/// Retry toi da <see cref="LichHenConstants.SoLanThuLaiToiDa"/> lan khi trung unique index.
/// </summary>
public class MaLichHenGenerator : IMaLichHenGenerator
{
    private readonly IAppDbContext _db;
    private readonly LichHenOptions _options;

    public MaLichHenGenerator(IAppDbContext db, IOptions<LichHenOptions> options)
    {
        _db = db;
        _options = options.Value;
    }

    public async Task<string> SinhMaLichHenAsync(DateOnly ngay, CancellationToken cancellationToken = default)
    {
        var prefix = _options.MaLichHenPrefix;
        var ngayStr = ngay.ToString("yyyyMMdd");
        var mauTimKiem = $"{prefix}-{ngayStr}-";

        // Lay MaLichHen lon nhat trong ngay de parse sequence
        var maLonNhat = await _db.LichHen
            .Where(lh => lh.MaLichHen.StartsWith(mauTimKiem))
            .OrderByDescending(lh => lh.MaLichHen)
            .Select(lh => lh.MaLichHen)
            .FirstOrDefaultAsync(cancellationToken);

        var sequenceTiepTheo = 1;

        if (maLonNhat is not null)
        {
            // Parse 6 ky tu cuoi cua MaLichHen: "LH-20260416-000042" -> "000042" -> 42
            var doDaiSeq = LichHenConstants.DoDaiSequenceMaLichHen;
            var phanSequence = maLonNhat[^doDaiSeq..];

            if (int.TryParse(phanSequence, out var sequenceHienTai))
            {
                sequenceTiepTheo = sequenceHienTai + 1;
            }
        }

        return $"{mauTimKiem}{sequenceTiepTheo.ToString().PadLeft(LichHenConstants.DoDaiSequenceMaLichHen, '0')}";
    }
}
