using ClinicBooking.Application.Features.ThongBao.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.ThongBao.Queries.DanhSachThongBaoCuaToi;

/// <summary>
/// Lay danh sach thong bao cua nguoi dung dang dang nhap.
/// <paramref name="ChiChuaDoc"/> = true chi lay thong bao chua doc.
/// </summary>
public sealed record DanhSachThongBaoCuaToiQuery(
    bool? ChiChuaDoc = null,
    int SoTrang = 1,
    int KichThuocTrang = 20) : IRequest<DanhSachThongBaoResponse>;

public sealed record DanhSachThongBaoResponse(
    IReadOnlyList<ThongBaoResponse> Items,
    int TongSo,
    int SoTrang,
    int KichThuocTrang,
    int SoChuaDoc);
