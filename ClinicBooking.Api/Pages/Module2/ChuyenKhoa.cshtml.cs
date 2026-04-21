using System.ComponentModel.DataAnnotations;
using ClinicBooking.Application.Features.DanhMuc.Commands.CapNhatChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Commands.TaoChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Commands.XoaChuyenKhoa;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachChuyenKhoa;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ClinicBooking.Api.Pages.Module2;

public class ChuyenKhoaModel : PageModel
{
    private readonly IMediator _mediator;

    public ChuyenKhoaModel(IMediator mediator)
    {
        _mediator = mediator;
    }

    public IReadOnlyList<ChuyenKhoaResponse> DanhSach { get; private set; } = [];

    [BindProperty(SupportsGet = true)]
    public int SoTrang { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int KichThuocTrang { get; set; } = 20;

    [BindProperty(SupportsGet = true)]
    public bool? HienThi { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? TuKhoa { get; set; }

    [BindProperty]
    public TaoChuyenKhoaInput TaoMoi { get; set; } = new();

    [TempData]
    public string? SuccessMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync(CancellationToken cancellationToken)
    {
        await TaiDuLieuAsync(cancellationToken);
    }

    public async Task<IActionResult> OnPostCreateAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await TaiDuLieuAsync(cancellationToken);
            return Page();
        }

        try
        {
            await _mediator.Send(
                new TaoChuyenKhoaCommand(
                    TaoMoi.TenChuyenKhoa,
                    TaoMoi.MoTa,
                    TaoMoi.ThoiGianSlotMacDinh,
                    TaoMoi.GioMoDatLich,
                    TaoMoi.GioDongDatLich,
                    TaoMoi.HienThi),
                cancellationToken);

            SuccessMessage = "Tao chuyen khoa thanh cong.";
            return RedirectToPage(new { SoTrang, KichThuocTrang, HienThi, TuKhoa });
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            await TaiDuLieuAsync(cancellationToken);
            return Page();
        }
    }

    public async Task<IActionResult> OnPostUpdateAsync(CapNhatChuyenKhoaInput capNhat, CancellationToken cancellationToken)
    {
        if (!TryValidateModel(capNhat, nameof(CapNhatChuyenKhoaInput)))
        {
            await TaiDuLieuAsync(cancellationToken);
            return Page();
        }

        try
        {
            await _mediator.Send(
                new CapNhatChuyenKhoaCommand(
                    capNhat.IdChuyenKhoa,
                    capNhat.TenChuyenKhoa,
                    capNhat.MoTa,
                    capNhat.ThoiGianSlotMacDinh,
                    capNhat.GioMoDatLich,
                    capNhat.GioDongDatLich,
                    capNhat.HienThi),
                cancellationToken);

            SuccessMessage = "Cap nhat chuyen khoa thanh cong.";
            return RedirectToPage(new { SoTrang, KichThuocTrang, HienThi, TuKhoa });
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return RedirectToPage(new { SoTrang, KichThuocTrang, HienThi, TuKhoa });
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int idChuyenKhoa, CancellationToken cancellationToken)
    {
        try
        {
            await _mediator.Send(new XoaChuyenKhoaCommand(idChuyenKhoa), cancellationToken);
            SuccessMessage = "Xoa chuyen khoa thanh cong.";
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }

        return RedirectToPage(new { SoTrang, KichThuocTrang, HienThi, TuKhoa });
    }

    private async Task TaiDuLieuAsync(CancellationToken cancellationToken)
    {
        DanhSach = await _mediator.Send(
            new DanhSachChuyenKhoaQuery(SoTrang, KichThuocTrang, HienThi, TuKhoa),
            cancellationToken);
    }

    public sealed class TaoChuyenKhoaInput
    {
        [Required]
        [MaxLength(450)]
        public string TenChuyenKhoa { get; set; } = string.Empty;

        [MaxLength(4000)]
        public string? MoTa { get; set; }

        [Range(1, 1440)]
        public int ThoiGianSlotMacDinh { get; set; } = 20;

        public TimeOnly? GioMoDatLich { get; set; } = new(7, 0);

        public TimeOnly? GioDongDatLich { get; set; } = new(17, 0);

        public bool HienThi { get; set; } = true;
    }

    public sealed class CapNhatChuyenKhoaInput
    {
        [Range(1, int.MaxValue)]
        public int IdChuyenKhoa { get; set; }

        [Required]
        [MaxLength(450)]
        public string TenChuyenKhoa { get; set; } = string.Empty;

        [MaxLength(4000)]
        public string? MoTa { get; set; }

        [Range(1, 1440)]
        public int ThoiGianSlotMacDinh { get; set; }

        public TimeOnly? GioMoDatLich { get; set; }

        public TimeOnly? GioDongDatLich { get; set; }

        public bool HienThi { get; set; }
    }
}
