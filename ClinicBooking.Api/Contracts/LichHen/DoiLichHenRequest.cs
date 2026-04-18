namespace ClinicBooking.Api.Contracts.LichHen;

public record DoiLichHenRequest(
    int IdCaLamViecMoi,
    int? IdDichVuMoi = null,
    int? IdBacSiMongMuon = null,
    string? BacSiMongMuonNote = null,
    string? TrieuChung = null,
    string? LyDo = null);
