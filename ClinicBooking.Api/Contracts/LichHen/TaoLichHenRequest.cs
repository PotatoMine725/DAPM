namespace ClinicBooking.Api.Contracts.LichHen;

public record TaoLichHenRequest(
    DateOnly NgayLamViec,
    TimeOnly GioMongMuon,
    int IdDichVu,
    int? IdBenhNhan = null,
    int? IdBacSiMongMuon = null,
    string? BacSiMongMuonNote = null,
    string? TrieuChung = null);
