namespace ClinicBooking.Domain.Enums;

public enum HanhDongLichSu
{
    DatMoi,
    XacNhan,
    DoiLich,
    HuyBenhNhan,
    HuyPhongKham,
    CheckIn,
    /// <summary>
    /// Ca lam viec da ket thuc, lich hen tu dong chuyen sang DaQuaHan.
    /// Ghi boi background job ChuyenLichHenDaQuaHanJob.
    /// </summary>
    QuaHan
}
