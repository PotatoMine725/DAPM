using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Application.Features.ThongBao.Dtos;

public sealed record ThongBaoResponse(
    int IdThongBao,
    string TieuDe,
    string NoiDung,
    KenhGui KenhGui,
    bool DaDoc,
    DateTime? NgayGui,
    int? IdThamChieu,
    LoaiThamChieu? LoaiThamChieu);
