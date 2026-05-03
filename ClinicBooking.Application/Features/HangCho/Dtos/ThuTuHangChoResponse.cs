namespace ClinicBooking.Application.Features.HangCho.Dtos;

public record ThuTuHangChoResponse(
    int IdCaLamViec,
    int IdBenhNhan,
    int IdHangCho,
    int SoThuTu,
    bool DaCoTrongHangCho,
    DateTime? NgayCheckIn);
