using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Features.DanhMuc.Dtos;
using ClinicBooking.Application.Features.DanhMuc.Queries.DanhSachDichVu;
using ClinicBooking.Application.Features.LichHen.Commands.TaoLichHen;
using ClinicBooking.Application.Features.Scheduling.Dtos;
using ClinicBooking.Application.Features.Scheduling.Queries.DanhSachCaLamViecCongKhai;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Web.Pages.BenhNhan;

[Authorize(Roles = "benh_nhan")]
public class DatLichModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly IAppDbContext _db;
    private readonly IOtpService _otpService;
    private readonly ICurrentUserService _currentUser;

    public DatLichModel(IMediator mediator, IAppDbContext db, IOtpService otpService, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _db = db;
        _otpService = otpService;
        _currentUser = currentUser;
    }

    public IReadOnlyList<DichVuResponse> DanhSachDichVu { get; private set; } = [];
    [BindProperty(SupportsGet = true)] public DateOnly NgayChon { get; set; }
    public string ChuyenKhoaHienTai => "Hệ thống sẽ tự sắp xếp theo dịch vụ và giờ mong muốn";
    public string TenDichVuDaChon => string.IsNullOrWhiteSpace(TenDichVu) ? "—" : TenDichVu;
    public bool CanSubmit => IdDichVu > 0;

    [BindProperty] public int IdDichVu { get; set; }
    [BindProperty] public TimeOnly GioMongMuon { get; set; } = new TimeOnly(8, 0);
    [BindProperty] public string? TrieuChung { get; set; }
    [BindProperty] public string? GhiChu { get; set; }
    [BindProperty] public string? Otp { get; set; }

    public string? TenDichVu { get; private set; }
    public bool DaGuiOtp { get; private set; }
    public bool DaXacThucOtp { get; private set; }
    public string? SoDienThoaiNhanOtp { get; private set; }

    public async Task OnGetAsync(DateOnly? ngay)
    {
        NgayChon = ngay ?? DateOnly.FromDateTime(DateTime.Now.AddDays(1));
        await TaiDuLieuAsync();
    }

    public async Task<IActionResult> OnPostGuiOtpAsync()
    {
        await TaiDuLieuAsync();

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
        await TaiDuLieuAsync();

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

        try
        {
            var command = new TaoLichHenCommand(
                NgayChon,
                GioMongMuon,
                IdDichVu,
                IdBenhNhan: null,
                IdBacSiMongMuon: null,
                BacSiMongMuonNote: GhiChu,
                TrieuChung);

            var result = await _mediator.Send(command);
            TempData["SuccessMessage"] = $"Đặt lịch thành công! Mã lịch hẹn: {result.MaLichHen}. Hệ thống sẽ tự sắp xếp ca khám phù hợp và gửi thông báo sau khi hoàn tất.";
            return RedirectToPage("/BenhNhan/DanhSachLichHen");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return Page();
        }
    }

    private async Task TaiDuLieuAsync()
    {
        DanhSachDichVu = await _mediator.Send(new DanhSachDichVuQuery(
            SoTrang: 1,
            KichThuocTrang: 100,
            HienThi: true));
    }

    private async Task CapNhatTrangThaiOtpAsync()
    {
        var otp = HttpContext.Session.GetString(TaoSessionOtpKey());
        var otpPhone = HttpContext.Session.GetString(TaoSessionOtpPhoneKey());
        var taiKhoan = await LayTaiKhoanHienTaiAsync();

        DaGuiOtp = !string.IsNullOrWhiteSpace(otp) && taiKhoan is not null && otpPhone == taiKhoan.SoDienThoai;
        SoDienThoaiNhanOtp = otpPhone;
        DaXacThucOtp = HttpContext.Session.GetString(TaoSessionOtpVerifiedKey()) == "true";

        if (!DaGuiOtp)
        {
            DaXacThucOtp = false;
            HttpContext.Session.Remove(TaoSessionOtpVerifiedKey());
            return;
        }

        if (!DaXacThucOtp && !string.IsNullOrWhiteSpace(Otp) && taiKhoan is not null)
        {
            DaXacThucOtp = await _otpService.XacThucOtpDatLichAsync(taiKhoan.IdTaiKhoan, Otp);
            if (DaXacThucOtp)
            {
                HttpContext.Session.SetString(TaoSessionOtpVerifiedKey(), "true");
            }
        }
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

    private string TaoSessionOtpKey() => $"booking-otp:{_currentUser.IdTaiKhoan}";
    private string TaoSessionOtpPhoneKey() => $"booking-otp-phone:{_currentUser.IdTaiKhoan}";
    private string TaoSessionOtpStampKey() => $"booking-otp-stamp:{_currentUser.IdTaiKhoan}";
    private string TaoSessionOtpVerifiedKey() => $"booking-otp-verified:{_currentUser.IdTaiKhoan}";
    private static string CheNoiDienThoai(string so)
        => so.Length <= 4 ? so : new string('*', Math.Max(0, so.Length - 4)) + so[^4..];
}
