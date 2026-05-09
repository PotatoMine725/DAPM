using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Application.Features.BenhNhan.Queries.TimBenhNhanTheoSdt;

public sealed record TimBenhNhanKetQua(
    int IdBenhNhan,
    string HoTen,
    DateOnly? NgaySinh,
    GioiTinh? GioiTinh
);
