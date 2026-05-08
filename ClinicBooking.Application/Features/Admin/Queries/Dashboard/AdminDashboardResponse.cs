namespace ClinicBooking.Application.Features.Admin.Queries.Dashboard;

public class AdminDashboardResponse
{
    public StatisticsDto Statistics { get; set; } = null!;
    public List<RecentAppointmentDto> RecentAppointments { get; set; } = new();
    public List<int> ChartData { get; set; } = new();
    public List<PendingShiftDto> PendingShifts { get; set; } = new();
    public DateTime LastUpdated { get; set; }
}

public class StatisticsDto
{
    public int TongLichHenHomNay { get; set; }
    public int BenhNhanDaKham { get; set; }
    public int CaChoDuyet { get; set; }
    public int LichBiHuyHomNay { get; set; }
}

public class RecentAppointmentDto
{
    public string MaLichHen { get; set; } = null!;
    public string TenBenhNhan { get; set; } = null!;
    public string TenBacSi { get; set; } = null!;
    public TimeOnly GioBatDau { get; set; }
    public string TrangThai { get; set; } = null!;
}

public class PendingShiftDto
{
    public int IdCaLamViec { get; set; }
    public string TenBacSi { get; set; } = null!;
    public DateOnly NgayLamViec { get; set; }
    public TimeOnly GioBatDau { get; set; }
    public TimeOnly GioKetThuc { get; set; }
    public string TenPhong { get; set; } = null!;
}
