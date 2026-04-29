using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Web.Pages.BenhNhan;

[Authorize(Roles = "benh_nhan")]
public class ChuyenKhoaModel : PageModel
{
    public IReadOnlyList<string> DanhSachChuyenKhoa { get; private set; } = ["Nội tổng quát", "Nhi khoa", "Tai mũi họng", "Da liễu"];
    public IReadOnlyList<BacSiCongKhaiViewModel> DanhSachBacSi { get; private set; } = [];
    public string? ChuyenKhoa { get; private set; }

    public void OnGet(string? chuyenKhoa = null)
    {
        ChuyenKhoa = chuyenKhoa;
        DanhSachBacSi = [
            new("BS. Nguyễn Văn An", "Nội tổng quát", "12 năm", "Phòng 201", "Khám nội tổng quát, tư vấn sức khỏe định kỳ."),
            new("BS. Trần Thị Bích", "Nhi khoa", "8 năm", "Phòng 105", "Khám trẻ em, theo dõi phát triển và tư vấn dinh dưỡng."),
            new("BS. Lê Minh Khôi", "Tai mũi họng", "10 năm", "Phòng 307", "Khám và điều trị các bệnh lý tai mũi họng."),
            new("BS. Phạm Hồng Nhung", "Da liễu", "9 năm", "Phòng 212", "Tư vấn và điều trị các vấn đề da liễu thường gặp.")
        ];

        if (!string.IsNullOrWhiteSpace(chuyenKhoa))
        {
            DanhSachBacSi = DanhSachBacSi.Where(x => string.Equals(x.TenChuyenKhoa, chuyenKhoa, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}

public sealed record BacSiCongKhaiViewModel(string HoTen, string TenChuyenKhoa, string KinhNghiem, string PhongLamViec, string MoTa);
