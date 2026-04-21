namespace ClinicBooking.Api.Contracts.ToaThuoc;

public record TaoToaThuocRequest(
    int IdHoSoKham,
    IReadOnlyList<ToaThuocChiTietRequest> DanhSachThuoc);
