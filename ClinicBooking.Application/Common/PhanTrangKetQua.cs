namespace ClinicBooking.Application.Common;

public record PhanTrangKetQua<T>(
    IReadOnlyList<T> DanhSach,
    int TongSo,
    int Trang,
    int SoTrenMoiTrang)
{
    public int TongTrang => (int)Math.Ceiling((double)TongSo / SoTrenMoiTrang);
    public bool CoTrangTruoc => Trang > 1;
    public bool CoTrangSau => Trang < TongTrang;
}
