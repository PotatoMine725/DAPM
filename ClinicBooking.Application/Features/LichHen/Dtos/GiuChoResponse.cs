namespace ClinicBooking.Application.Features.LichHen.Dtos;

public record GiuChoResponse(
    int IdGiuCho,
    int IdCaLamViec,
    int IdBenhNhan,
    string HoTenBenhNhan,
    int SoSlot,
    DateTime GioHetHan,
    DateTime NgayTao);
