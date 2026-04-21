using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Web.Helpers;

public static class BadgeHelper
{
    public static string TrangThaiLichHen(TrangThaiLichHen trangThai) => trangThai switch
    {
        Domain.Enums.TrangThaiLichHen.ChoXacNhan   => Badge("badge-warning", "Chờ xác nhận"),
        Domain.Enums.TrangThaiLichHen.DaXacNhan    => Badge("badge-success", "Đã xác nhận"),
        Domain.Enums.TrangThaiLichHen.DangKham      => Badge("badge-active",  "Đang khám"),
        Domain.Enums.TrangThaiLichHen.HoanThanh     => Badge("badge-info",    "Hoàn thành"),
        Domain.Enums.TrangThaiLichHen.HuyBenhNhan   => Badge("badge-danger",  "Bệnh nhân huỷ"),
        Domain.Enums.TrangThaiLichHen.HuyPhongKham  => Badge("badge-danger",  "Phòng khám huỷ"),
        Domain.Enums.TrangThaiLichHen.KhongDen      => Badge("badge-neutral", "Không đến"),
        _ => Badge("badge-neutral", trangThai.ToString())
    };

    public static string TrangThaiHangCho(TrangThaiHangCho trangThai) => trangThai switch
    {
        Domain.Enums.TrangThaiHangCho.ChoKham   => Badge("badge-neutral", "Chờ khám"),
        Domain.Enums.TrangThaiHangCho.DangKham  => Badge("badge-active",  "Đang khám"),
        Domain.Enums.TrangThaiHangCho.HoanThanh => Badge("badge-info",    "Hoàn thành"),
        _ => Badge("badge-neutral", trangThai.ToString())
    };

    public static string TrangThaiText(TrangThaiLichHen trangThai) => trangThai switch
    {
        Domain.Enums.TrangThaiLichHen.ChoXacNhan   => "Chờ xác nhận",
        Domain.Enums.TrangThaiLichHen.DaXacNhan    => "Đã xác nhận",
        Domain.Enums.TrangThaiLichHen.DangKham     => "Đang khám",
        Domain.Enums.TrangThaiLichHen.HoanThanh    => "Hoàn thành",
        Domain.Enums.TrangThaiLichHen.HuyBenhNhan  => "Bệnh nhân huỷ",
        Domain.Enums.TrangThaiLichHen.HuyPhongKham => "Phòng khám huỷ",
        Domain.Enums.TrangThaiLichHen.KhongDen     => "Không đến",
        _ => trangThai.ToString()
    };

    private static string Badge(string cssClass, string text) =>
        $"<span class=\"badge {cssClass}\">{text}</span>";
}
