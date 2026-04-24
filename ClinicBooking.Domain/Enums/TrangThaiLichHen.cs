namespace ClinicBooking.Domain.Enums;

public enum TrangThaiLichHen
{
    ChoXacNhan,
    DaXacNhan,
    DangKham,
    HoanThanh,
    HuyBenhNhan,
    HuyPhongKham,
    KhongDen,
    /// <summary>
    /// Ca lam viec da ket thuc nhung lich hen chua duoc check-in / hoan thanh / huy.
    /// Duoc dat boi background job ChuyenLichHenDaQuaHanJob.
    /// </summary>
    DaQuaHan
}
