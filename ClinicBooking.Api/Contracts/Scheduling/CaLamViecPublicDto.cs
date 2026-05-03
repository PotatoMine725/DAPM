namespace ClinicBooking.Api.Contracts.Scheduling;

public record CaLamViecPublicDto(
    int IdCaLamViec,
    int IdBacSi,
    int IdPhong,
    int IdChuyenKhoa,
    int IdDinhNghiaCa,
    DateOnly NgayLamViec,
    TimeOnly GioBatDau,
    TimeOnly GioKetThuc,
    int ThoiGianSlot,
    string TrangThaiDuyet,
    string NguonTaoCa,
    string HoTenBacSi,
    string MaPhong,
    string TenChuyenKhoa,
    bool ConTrong);
