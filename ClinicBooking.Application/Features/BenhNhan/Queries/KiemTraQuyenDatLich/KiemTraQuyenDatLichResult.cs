namespace ClinicBooking.Application.Features.BenhNhan.Queries.KiemTraQuyenDatLich;

public sealed record KiemTraQuyenDatLichResult(
    bool ChoPhep,
    string? LyDo,
    int SoLanHuyMuon,
    bool BiHanChe,
    DateTime? NgayHetHanChe,
    bool CanhBaoSom);
