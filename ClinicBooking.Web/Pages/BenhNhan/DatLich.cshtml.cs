using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Options;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDichVu;
using ClinicBooking.Application.Features.LichHen.Commands.TaoLichHen;
using ClinicBooking.Application.Features.Scheduling.Dtos;
using ClinicBooking.Application.Features.Scheduling.Queries.DanhSachCaLamViecCongKhai;
using CaLamViecCongKhaiDto = ClinicBooking.Application.Features.Scheduling.Dtos.CaLamViecPublicResponse;
using ClinicBooking.Application.Features.Scheduling.Queries.KiemTraDoPhuBacSi;
using ClinicBooking.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace ClinicBooking.Web.Pages.BenhNhan;

[Authorize(Roles = "benh_nhan")]
public class DatLichModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IAppDbContext _db;
    private readonly IOtpService _otpService;
    private readonly ICurrentUserService _currentUser;
    private readonly OtpOptions _otpOptions;

    public DatLichModel(IMediator mediator, IAppDbContext db, IOtpService otpService, ICurrentUserService currentUser, IOptions<OtpOptions> otpOptions)
    {
        _mediator = mediator;
        _db = db;
        _otpService = otpService;
        _currentUser = currentUser;
        _otpOptions = otpOptions.Value;
    }

    public IReadOnlyList<DichVuResponse> DanhSachDichVu { get; private set; } = [];
    public IReadOnlyList<NgayThieuBacSiDto> CanhBaoThieuBacSi { get; private set; } = [];
    public IReadOnlyList<CaLamViecPublicResponse> DanhSachCa { get; private set; } = [];
    [BindProperty(SupportsGet = true)] public DateOnly NgayChon { get; set; }
    public string ChuyenKhoaHienTai => string.IsNullOrWhiteSpace(TenChuyenKhoaDaChon) ? "Hệ thống sẽ tự sắp xếp theo dịch vụ và giờ mong muốn" : TenChuyenKhoaDaChon;
    public string TenDichVuDaChon => string.IsNullOrWhiteSpace(TenDichVu) ? "—" : TenDichVu;
    public bool CanSubmit => IdDichVu > 0;
    public bool BatBuocOtp => _otpOptions.BatBuocChoDatLich;

    [BindProperty] public int IdDichVu { get; set; }
    [BindProperty] public TimeOnly GioMongMuon { get; set; } = new TimeOnly(8, 0);
    [BindProperty] public string? TrieuChung { get; set; }
    [BindProperty] public string? GhiChu { get; set; }
    [BindProperty] public string? Otp { get; set; }
    [BindProperty] public int IdChuyenKhoa { get; set; } = 1;

    public string? TenDichVu { get; private set; }
    public string? TenChuyenKhoaDaChon { get; private set; }

    public async Task OnGetAsync(DateOnly? ngay)
    {
        NgayChon = ngay ?? DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        await TaiDuLieuAsync(NgayChon);
    }

    public async Task<IActionResult> OnPostGuiOtpAsync()
    {
        if (!BatBuocOtp)
        {
            TempData["SuccessMessage"] = "Bỏ qua OTP theo cấu hình Development.";
            return RedirectToPage(new { ngay = NgayChon });
        }

        await TaiDuLieuAsync(NgayChon);

        var taiKhoan = await LayTaiKhoanHienTaiAsync();
        if (taiKhoan is null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy tài khoản bệnh nhân hiện tại.";
            return Page();
        }

        var otp = await _otpService.TaoVaGuiOtpDatLichAsync(taiKhoan.IdTaiKhoan, taiKhoan.SoDienThoai);
        TempData["OtpCode"] = otp;
        TempData["OtpPhone"] = CheNoiDienThoai(taiKhoan.SoDienThoai);
        TempData.Keep("OtpCode");
        TempData.Keep("OtpPhone");
        TempData["SuccessMessage"] = $"OTP đã được tạo cho số {CheNoiDienThoai(taiKhoan.SoDienThoai)}. Mã OTP: {otp}.";
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        await TaiDuLieuAsync(NgayChon);

        if (!ModelState.IsValid || IdDichVu <= 0)
        {
            TempData["ErrorMessage"] = "Vui lòng chọn dịch vụ hợp lệ.";
            return Page();
        }

        var taiKhoan = await LayTaiKhoanHienTaiAsync();
        if (taiKhoan is null)
        {
            TempData["ErrorMessage"] = "Không tìm thấy tài khoản bệnh nhân hiện tại.";
            return Page();
        }

        if (BatBuocOtp)
        {
            if (string.IsNullOrWhiteSpace(Otp))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập OTP đã nhận để hoàn tất đặt lịch.";
                return Page();
            }

            var daXacThuc = await _otpService.XacThucOtpDatLichAsync(taiKhoan.IdTaiKhoan, Otp.Trim());
            if (!daXacThuc)
            {
                TempData["ErrorMessage"] = "OTP không hợp lệ hoặc đã hết hạn. Vui lòng gửi OTP mới.";
                return Page();
            }
        }

        try
        {
            var command = new TaoLichHenCommand(NgayChon, GioMongMuon, IdDichVu, null, null, TrieuChung, GhiChu);
            var result = await _mediator.Send(command);
            TempData["SuccessMessage"] = $"Đặt lịch thành công! Mã lịch hẹn: {result.MaLichHen}.";
            return RedirectToPage("/BenhNhan/DanhSachLichHen");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return Page();
        }
    }

    private async Task TaiDuLieuAsync(DateOnly ngay)
    {
<<<<<<< HEAD
        DanhSachDichVu = await _mediator.Send(new DanhSachDichVuQuery(SoTrang: 1, KichThuocTrang: 100, IdChuyenKhoa: null, HienThi: true, TuKhoa: null));
        DanhSachCa = await _mediator.Send(new DanhSachCaLamViecCongKhaiQuery(SoTrang: 1, KichThuocTrang: 50, IdBacSi: null, IdChuyenKhoa: IdChuyenKhoa, IdPhong: null, TuNgay: ngay, DenNgay: ngay, ConTrong: true));
=======
        DanhSachDichVu = await _mediator.Send(new DanhSachDichVuQuery(1, 100, null, true));
        DanhSachCa = await _mediator.Send(new DanhSachCaLamViecCongKhaiQuery(1, 50, null, IdChuyenKhoa, null, ngay, ngay, true));
>>>>>>> 7e0dfb3 (feat_module2_finish_admin_scheduling_polish)
        CanhBaoThieuBacSi = (await _mediator.Send(new KiemTraDoPhuBacSiQuery(IdChuyenKhoa, ngay, ngay))).NgayThieu;
        TenDichVu = DanhSachDichVu.FirstOrDefault(x => x.IdDichVu == IdDichVu)?.TenDichVu;
        TenChuyenKhoaDaChon = "Nội tổng quát";
    }

    private async Task<TaiKhoan?> LayTaiKhoanHienTaiAsync()
    {
        var idTaiKhoan = _currentUser.IdTaiKhoan;
        if (!idTaiKhoan.HasValue)
        {
            return null;
        }

        return await _db.TaiKhoan.FirstOrDefaultAsync(x => x.IdTaiKhoan == idTaiKhoan.Value);
    }

    private static string CheNoiDienThoai(string so)
        => so.Length <= 4 ? so : new string('*', Math.Max(0, so.Length - 4)) + so[^4..];
}
