using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Application.Abstractions.Scheduling.Dtos;

public sealed record ThongTinCaLamViecDto(
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
