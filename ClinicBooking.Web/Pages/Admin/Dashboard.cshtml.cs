using ClinicBooking.Application.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ClinicBooking.Web.Pages.Admin;

[Authorize(Roles = VaiTroConstants.Admin)]
public class DashboardModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public DashboardModel(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    // Mock data classes for demonstration
    public class MockDashboardStatistics
    {
        public int TongLichHenHomNay { get; set; }
        public int BenhNhanDaKham { get; set; }
        public int CaChoDuyet { get; set; }
        public int LichHenBiHuy { get; set; }
    }

    public class MockRecentAppointment
    {
        public string MaLichHen { get; set; } = "";
        public string TenBenhNhan { get; set; } = "";
        public string TenBacSi { get; set; } = "";
        public string ThoiGian { get; set; } = "";
        public string TrangThai { get; set; } = "";
    }

    public class MockPendingShift
    {
        public int IdCaLamViec { get; set; }
        public string TenBacSi { get; set; } = "";
        public string Ngay { get; set; } = "";
        public string Ca { get; set; } = "";
        public int SoSlot { get; set; }
    }

    public class MockChartDataPoint : IComparable<MockChartDataPoint>
    {
        public string Day { get; set; } = "";
        public double Value { get; set; }

        public int CompareTo(MockChartDataPoint? other)
        {
            if (other == null) return 1;
            return Value.CompareTo(other.Value);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class MockDashboardResponse
    {
        public MockDashboardStatistics Statistics { get; set; } = new();
        public List<MockRecentAppointment> RecentAppointments { get; set; } = new();
        public List<MockPendingShift> PendingShifts { get; set; } = new();
        public List<MockChartDataPoint> ChartData { get; set; } = new();
    }

    public MockDashboardResponse? DashboardData { get; private set; }
    public string? ErrorMessage { get; private set; }

    public void OnGet()
    {
        // Mock dashboard data for demonstration
        DashboardData = new MockDashboardResponse
        {
            Statistics = new MockDashboardStatistics
            {
                TongLichHenHomNay = 24,
                BenhNhanDaKham = 18,
                CaChoDuyet = 6,
                LichHenBiHuy = 2
            },
            RecentAppointments = new List<MockRecentAppointment>
            {
                new MockRecentAppointment { MaLichHen = "LH001", TenBenhNhan = "Nguyễn Văn A", TenBacSi = "BS. Trần Minh Khoa", ThoiGian = "09:00", TrangThai = "Đã đến" },
                new MockRecentAppointment { MaLichHen = "LH002", TenBenhNhan = "Trần Thị B", TenBacSi = "BS. Phạm Thu Hà", ThoiGian = "09:30", TrangThai = "Đang chờ" },
                new MockRecentAppointment { MaLichHen = "LH003", TenBenhNhan = "Lê Văn C", TenBacSi = "BS. Vũ Đức Thắng", ThoiGian = "10:00", TrangThai = "Đã hủy" }
            },
            PendingShifts = new List<MockPendingShift>
            {
                new MockPendingShift { IdCaLamViec = 3001, TenBacSi = "BS. Nguyễn Văn D", Ngay = "2026-05-08", Ca = "Sáng", SoSlot = 12 },
                new MockPendingShift { IdCaLamViec = 3002, TenBacSi = "BS. Trần Thị E", Ngay = "2026-05-08", Ca = "Chiều", SoSlot = 8 }
            },
            ChartData = new List<MockChartDataPoint>
            {
                new MockChartDataPoint { Day = "T2", Value = 142 },
                new MockChartDataPoint { Day = "T3", Value = 158 },
                new MockChartDataPoint { Day = "T4", Value = 195 },
                new MockChartDataPoint { Day = "T5", Value = 178 },
                new MockChartDataPoint { Day = "T6", Value = 203 },
                new MockChartDataPoint { Day = "T7", Value = 165 },
                new MockChartDataPoint { Day = "CN", Value = 89 }
            }
        };
    }
}
