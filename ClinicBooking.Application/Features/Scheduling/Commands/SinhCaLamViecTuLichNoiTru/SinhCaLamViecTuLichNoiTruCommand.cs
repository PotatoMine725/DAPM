using MediatR;

namespace ClinicBooking.Application.Features.Scheduling.Commands.SinhCaLamViecTuLichNoiTru;

public sealed record SinhCaLamViecTuLichNoiTruCommand(int SoNgaySinhTruoc) : IRequest<KetQuaSinhCaLamViecTuLichNoiTruResponse>;

public sealed record KetQuaSinhCaLamViecTuLichNoiTruResponse(
    int SoCaSinh,
    int SoCaBoQua,
    IReadOnlyList<XungDotSinhCaDto> DanhSachXungDot);

public sealed record XungDotSinhCaDto(
    DateOnly NgayLamViec,
    int IdBacSi,
    int IdPhong,
    LoaiXungDotSinhCa Loai,
    string LyDo);

public enum LoaiXungDotSinhCa
{
    CaDaTonTai,
    TrungLichBacSi,
    TrungLichPhong,
    DonNghiPhepDaDuyet,
    Khac
}