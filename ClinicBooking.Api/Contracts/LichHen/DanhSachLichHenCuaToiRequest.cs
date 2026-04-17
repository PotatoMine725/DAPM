using ClinicBooking.Application.Common.Constants;
using ClinicBooking.Domain.Enums;

namespace ClinicBooking.Api.Contracts.LichHen;

public record DanhSachLichHenCuaToiRequest(
    TrangThaiLichHen? TrangThai = null,
    int SoTrang = 1,
    int KichThuocTrang = LichHenConstants.KichThuocTrangMacDinh);
