using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Application.Features.HangCho.Dtos;

public record HangChoResponse(
    int IdHangCho,
    int IdCaLamViec,
    int IdLichHen,
    string MaLichHen,
    int IdBenhNhan,
    string HoTenBenhNhan,
    int SoThuTu,
    TrangThaiHangCho TrangThai,
    DateTime NgayCheckIn);
