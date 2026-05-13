using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Application.Features.Doctors.Dtos;

public sealed record HoSoBacSiCuaToiResponse(
    int IdBacSi,
    string HoTen,
    string TenChuyenKhoa,
    LoaiHopDong LoaiHopDong);
