namespace ClinicBooking.Application.Features.Scheduling.Dtos;

public sealed record SlotKhamPublicResponse(
    int IdCaLamViec,
    int IdBacSi,
    int IdPhong,
    int IdChuyenKhoa,
    DateOnly NgayLamViec,
    TimeOnly GioBatDau,
    TimeOnly GioKetThuc,
    int ThoiGianSlot,
    int SoSlotToiDa,
    int SoSlotDaDat,
    string HoTenBacSi,
    string MaPhong,
    string TenChuyenKhoa)
{
    public bool ConTrong => SoSlotDaDat < SoSlotToiDa;

    public bool KhopVoiGioMongMuon(TimeOnly gioMongMuon)
        => gioMongMuon >= GioBatDau && gioMongMuon < GioKetThuc && ConTrong;
}
