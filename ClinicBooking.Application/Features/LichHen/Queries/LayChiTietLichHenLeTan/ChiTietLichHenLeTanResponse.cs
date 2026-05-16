using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Application.Features.LichHen.Queries.LayChiTietLichHenLeTan;

public sealed record ChiTietLichHenLeTanResponse(
    int IdLichHen,
    string MaLichHen,
    string TenBenhNhan,
    string SoDienThoai,
    string TenDichVu,
    string TenChuyenKhoa,
    int IdChuyenKhoa,
    DateOnly NgayLamViec,
    TimeOnly GioBatDau,
    TimeOnly GioKetThuc,
    TrangThaiLichHen TrangThai,
    HinhThucDat HinhThucDat,
    string? TrieuChung,
    string? BacSiMongMuonNote,
    int? IdBacSiMongMuon,
    string? TenBacSiDuocGan,
    bool DaCheckIn
);
