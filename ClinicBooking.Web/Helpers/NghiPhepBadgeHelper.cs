using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Web.Helpers;

public static class NghiPhepBadgeHelper
{
    public static string TrangThai(TrangThaiDuyetDon trangThai) => trangThai switch
    {
        TrangThaiDuyetDon.ChoDuyet => Badge("badge-warning", "Chờ duyệt"),
        TrangThaiDuyetDon.DaDuyet => Badge("badge-success", "Đã duyệt"),
        TrangThaiDuyetDon.TuChoi => Badge("badge-danger", "Từ chối"),
        _ => Badge("badge-neutral", trangThai.ToString())
    };

    public static string Loai(LoaiNghiPhep loai) => loai switch
    {
        LoaiNghiPhep.CoKeHoach => Badge("badge-info", "Có kế hoạch"),
        LoaiNghiPhep.DotXuat => Badge("badge-warning", "Đột xuất"),
        _ => Badge("badge-neutral", loai.ToString())
    };

    private static string Badge(string cssClass, string text) =>
        $"<span class=\"badge {cssClass}\">{text}</span>";
}
