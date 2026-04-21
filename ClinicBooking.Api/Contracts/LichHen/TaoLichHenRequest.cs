namespace ClinicBooking.Api.Contracts.LichHen;

public record TaoLichHenRequest(
    int IdCaLamViec,
    int IdDichVu,
    int? IdBenhNhan = null,
    int? IdBacSiMongMuon = null,
    string? BacSiMongMuonNote = null,
    string? TrieuChung = null);
