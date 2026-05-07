using ClinicBooking.Application.Abstractions.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Admin.Queries.Dashboard;

public class AdminDashboardQuery
{
    public class Request : IRequest<AdminDashboardResponse>
    {
    }

    public class Handler : IRequestHandler<Request, AdminDashboardResponse>
    {
        private readonly IAppDbContext _db;

        public Handler(IAppDbContext db)
        {
            _db = db;
        }

        public async Task<AdminDashboardResponse> Handle(Request request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var today = DateOnly.FromDateTime(now);
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var weekEnd = weekStart.AddDays(6);

            // Statistics
            var tongLichHenHomNay = await _db.LichHen
                .CountAsync(l => DateOnly.FromDateTime(l.NgayTao) == today, cancellationToken);

            var benhNhanDaKham = await _db.LichHen
                .CountAsync(l => DateOnly.FromDateTime(l.NgayTao) == today && 
                               l.TrangThai == Domain.Enums.TrangThaiLichHen.HoanThanh, cancellationToken);

            var caChoDuyet = await _db.CaLamViec
                .CountAsync(c => c.TrangThaiDuyet == Domain.Enums.TrangThaiDuyetCa.ChoDuyet, cancellationToken);

            var lichBiHuyHomNay = await _db.LichHen
                .CountAsync(l => DateOnly.FromDateTime(l.NgayTao) == today &&
                               (l.TrangThai == Domain.Enums.TrangThaiLichHen.HuyBenhNhan ||
                                l.TrangThai == Domain.Enums.TrangThaiLichHen.HuyPhongKham), cancellationToken);

            // Recent appointments
            var recentAppointments = await _db.LichHen
                .Include(l => l.BenhNhan)
                .Include(l => l.CaLamViec)
                .ThenInclude(c => c.BacSi)
                .OrderByDescending(l => l.NgayTao)
                .Take(5)
                .Select(l => new RecentAppointmentDto
                {
                    MaLichHen = l.MaLichHen,
                    TenBenhNhan = l.BenhNhan.HoTen,
                    TenBacSi = l.CaLamViec.BacSi.HoTen,
                    GioBatDau = l.CaLamViec.GioBatDau,
                    TrangThai = l.TrangThai.ToString()
                })
                .ToListAsync(cancellationToken);

            // 7-day chart data
            var chartData = new List<int>();
            for (int i = 0; i < 7; i++)
            {
                var date = weekStart.AddDays(i);
                var count = await _db.LichHen
                    .CountAsync(l => DateOnly.FromDateTime(l.NgayTao) == date, cancellationToken);
                chartData.Add(count);
            }

            // Pending shifts
            var pendingShifts = await _db.CaLamViec
                .Include(c => c.BacSi)
                .Include(c => c.Phong)
                .Where(c => c.TrangThaiDuyet == Domain.Enums.TrangThaiDuyetCa.ChoDuyet)
                .Take(3)
                .Select(c => new PendingShiftDto
                {
                    IdCaLamViec = c.IdCaLamViec,
                    TenBacSi = c.BacSi.HoTen,
                    NgayLamViec = c.NgayLamViec,
                    GioBatDau = c.GioBatDau,
                    GioKetThuc = c.GioKetThuc,
                    TenPhong = c.Phong.TenPhong
                })
                .ToListAsync(cancellationToken);

            return new AdminDashboardResponse
            {
                Statistics = new StatisticsDto
                {
                    TongLichHenHomNay = tongLichHenHomNay,
                    BenhNhanDaKham = benhNhanDaKham,
                    CaChoDuyet = caChoDuyet,
                    LichBiHuyHomNay = lichBiHuyHomNay
                },
                RecentAppointments = recentAppointments,
                ChartData = chartData,
                PendingShifts = pendingShifts,
                LastUpdated = now
            };
        }
    }
}
