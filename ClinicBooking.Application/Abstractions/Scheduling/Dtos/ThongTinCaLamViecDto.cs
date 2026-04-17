using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Application.Abstractions.Scheduling.Dtos;

/// <summary>
/// DTO read-only mieu ta mot ca lam viec, do <see cref="ICaLamViecQueryService"/> tra ve.
/// Module 1 chi doc, khong dung DTO nay de write truc tiep len CaLamViec.
/// </summary>
public record ThongTinCaLamViecDto(
    int IdCaLamViec,
    int IdBacSi,
    int IdPhong,
    int IdChuyenKhoa,
    int IdDinhNghiaCa,
    DateOnly NgayLamViec,
    TimeOnly GioBatDau,
    TimeOnly GioKetThuc,
    int ThoiGianSlot,
    int SoSlotToiDa,
    int SoSlotDaDat,
    TrangThaiDuyetCa TrangThaiDuyet);
