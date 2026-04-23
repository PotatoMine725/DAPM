namespace ClinicBooking.Api.Contracts.HangCho;

public record ThuTuHangChoDto(
    int IdCaLamViec,
    int IdBenhNhan,
    int IdHangCho,
    int SoThuTu,
    bool DaCoTrongHangCho,
    DateTime? NgayCheckIn);
