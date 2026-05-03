using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Application.Abstractions.Scheduling.Dtos;

public sealed record KetQuaKiemTraSlotDto(
    bool CoTheDat,
    int SoSlotToiDa,
    int SoSlotDaDat,
    int SoGiuChoHieuLuc,
    LyDoKhongDatDuoc? LyDoTuChoi);
