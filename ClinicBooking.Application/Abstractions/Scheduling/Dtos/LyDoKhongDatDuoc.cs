namespace ClinicBooking.Application.Abstractions.Scheduling.Dtos;

/// <summary>
/// Ly do vi sao mot ca lam viec khong the nhan them lich hen hoac giu cho.
/// </summary>
public enum LyDoKhongDatDuoc
{
    /// <summary>Ca chua duoc admin duyet.</summary>
    CaChuaDuyet,

    /// <summary>Ca da het slot trong.</summary>
    HetSlot,

    /// <summary>Ca da qua thoi diem hien tai (khong the dat lich qua khu).</summary>
    CaDaDiQua,

    /// <summary>Ca khong ton tai trong he thong.</summary>
    KhongTonTai
}
