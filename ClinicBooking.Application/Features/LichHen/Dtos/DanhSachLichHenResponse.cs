namespace ClinicBooking.Application.Features.LichHen.Dtos;

/// <summary>
/// Ket qua phan trang cho danh sach lich hen.
/// </summary>
public record DanhSachLichHenResponse(
    IReadOnlyList<LichHenTomTatResponse> KetQua,
    int TongSo,
    int SoTrang,
    int KichThuocTrang);
