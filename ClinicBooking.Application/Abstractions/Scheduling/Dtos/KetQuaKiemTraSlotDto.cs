namespace ClinicBooking.Application.Abstractions.Scheduling.Dtos;

/// <summary>
/// Ket qua cua viec kiem tra xem mot ca lam viec con cho de dat lich/giu cho hay khong.
/// </summary>
/// <param name="CoTheDat">True neu con slot trong va ca dang o trang thai hop le.</param>
/// <param name="SoSlotToiDa">Tong so slot cua ca.</param>
/// <param name="SoSlotDaDat">So slot da duoc dat (tinh ca lich hen dang hoat dong).</param>
/// <param name="SoGiuChoHieuLuc">So luot giu cho tam thoi con hieu luc (chua hetHan/chua giai phong).</param>
/// <param name="LyDoTuChoi">Null khi <see cref="CoTheDat"/>=true, nguoc lai la ly do bi tu choi.</param>
public record KetQuaKiemTraSlotDto(
    bool CoTheDat,
    int SoSlotToiDa,
    int SoSlotDaDat,
    int SoGiuChoHieuLuc,
    LyDoKhongDatDuoc? LyDoTuChoi);
