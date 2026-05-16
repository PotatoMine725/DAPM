# Kế hoạch triển khai OTP qua Gmail SMTP

**Ngày tạo:** 2026-05-13  
**Nhánh:** `feature/module1/portal-sat-demo`  
**Phạm vi:** Sửa bug + cấu hình để OTP gửi email thật qua Gmail

---

## 0. Phân tích hiện trạng

### Đã có (không cần viết lại)

| Thành phần | File | Trạng thái |
|---|---|---|
| `IEmailService` | `Application/Abstractions/Notifications/IEmailService.cs` | ✅ Hoàn chỉnh |
| `EmailService` | `Infrastructure/Services/Notifications/EmailService.cs` | ✅ Hoàn chỉnh (MailKit) |
| `IOtpService` | `Application/Abstractions/Security/IOtpService.cs` | ✅ Interface OK |
| `OtpService` | `Infrastructure/Services/Security/OtpService.cs` | ⚠️ Có 4 bugs |
| `OtpOptions` | `Application/Common/Options/OtpOptions.cs` | ✅ OK |
| `EmailSettings` | `Application/Common/Options/EmailSettings.cs` | ✅ OK |
| DI registration | `Infrastructure/DependencyInjection.cs` | ✅ `OtpService` đăng ký (không phải stub) |
| DB schema `OtpLog` | Migration `InitialCreate` | ✅ `MucDich`, `SoLanThu` đã có column |

### Vấn đề cần giải quyết

**A. Bugs trong `OtpService.cs`:**
1. Dùng `new Random()` — không an toàn về mặt crypto (stub dùng `RandomNumberGenerator`)
2. Không set `MucDich` trên `OtpLog` — index `IdTaiKhoan, MucDich, NgayTao` không được dùng
3. Không set `SoLanThu = 0` (defaults OK, nhưng không explicit)
4. Không invalidate OTP active cũ trước khi tạo mới (chỉ xóa expired)
5. Fallback email `benhNhan.TaiKhoan.Email` navigation không Include → NRE tiềm ẩn

**B. Walk-in flow — OTP gửi đi đâu?**  
Ghost account có email `walkin_{sdt}@local.invalid`. `OtpService` đọc `taiKhoan.Email` từ DB → gửi vào địa chỉ rác → OTP không đến được tay bệnh nhân.

**C. Gmail credentials chưa cấu hình:**  
`appsettings.json` có `SmtpHost`, `SmtpPort`, `StartTls`, `FromAddress` nhưng thiếu `Username` + `Password`.

**D. `EmailService.GuiEmailAsync` nuốt exception:**  
Hiện đang `catch (Exception)` và không throw → OTP tạo thành công trong DB nhưng email thất bại âm thầm.

---

## 1. Quyết định thiết kế

### 1.1 Walk-in OTP destination — Chọn Option A2

Thêm param `string? emailGui` vào `IOtpService.TaoVaGuiOtpDatLichAsync`:

```csharp
Task<string> TaoVaGuiOtpDatLichAsync(
    int idTaiKhoan,
    string soDienThoai,
    string? emailGui = null,           // mới — override email đích
    CancellationToken cancellationToken = default);
```

- Nếu `emailGui != null` → gửi đến địa chỉ này (walk-in flow)
- Nếu null → giữ behavior cũ (dùng `taiKhoan.Email`)
- Backward compatible: tất cả caller hiện tại không đổi (default null)

UI `LienKetHoSo` step 1 thêm field **"Email nhận mã OTP"** — bắt buộc.

### 1.2 MucDich enum — Thêm `KichHoatWalkIn`

```csharp
public enum MucDichOtp
{
    DatLich = 0,
    DangKy = 1,
    DangNhap = 2,
    KichHoatWalkIn = 3    // mới
}
```

Không cần migration mới — DB lưu enum dưới dạng string (nvarchar), chỉ cần thêm giá trị vào C# enum.

### 1.3 Dev fallback — Log OTP ra console

Khi `EmailService` không gửi được (credentials rỗng hoặc lỗi kết nối), `OtpService` log OTP ra `ILogger.LogWarning` với prefix `[OTP-DEV]` rõ ràng. TA chấm bài có thể mở console thấy OTP mà không cần setup Gmail.

Cơ chế: `OtpService` bắt exception từ `_emailService.GuiEmailAsync` và log fallback thay vì throw.

---

## 2. Setup Gmail App Password (Một lần, do dev tự làm)

```
1. Đăng nhập Gmail account dùng cho project (tạo mới nếu cần)
2. Vào myaccount.google.com → Security → 2-Step Verification → BẬT
3. Vào myaccount.google.com → Security → App passwords
4. Chọn app: "Mail" / device: "Other (Custom name)" → đặt tên "ClinicBooking"
5. Copy 16-char app password (dạng: xxxx xxxx xxxx xxxx)
```

**Lưu ý quan trọng:**
- `FromAddress` PHẢI là địa chỉ Gmail thật (ví dụ: `clinicbooking.demo@gmail.com`)
- Gmail SMTP sẽ rewrite From header nếu dùng địa chỉ khác
- **KHÔNG commit credentials vào git** — dùng `dotnet user-secrets`

---

## 3. Cấu hình credentials (dotnet user-secrets)

### Chạy từ thư mục gốc solution:

```bash
# Cho ClinicBooking.Web project
cd ClinicBooking.Web
dotnet user-secrets set "Email:Username" "clinicbooking.demo@gmail.com"
dotnet user-secrets set "Email:Password" "xxxx xxxx xxxx xxxx"
dotnet user-secrets set "Email:FromAddress" "clinicbooking.demo@gmail.com"
dotnet user-secrets set "Email:FromName" "Phong Kham Demo"
```

### Cập nhật `appsettings.json` (giá trị mặc định, không có credentials):

```json
"Email": {
  "SmtpHost": "smtp.gmail.com",
  "SmtpPort": 587,
  "StartTls": true,
  "FromAddress": "",
  "FromName": "Phong Kham Demo",
  "Username": "",
  "Password": ""
}
```

`Username` và `Password` để rỗng trong file — user-secrets override khi dev, env vars khi deploy.

---

## 4. Các thay đổi code

### 4.1 `MucDichOtp` enum — thêm giá trị

**File:** `ClinicBooking.Domain/Enums/MucDichOtp.cs`

Thêm `KichHoatWalkIn = 3`.

### 4.2 `IOtpService` interface — thêm param

**File:** `ClinicBooking.Application/Abstractions/Security/IOtpService.cs`

Thêm `string? emailGui = null` vào signature `TaoVaGuiOtpDatLichAsync`.

### 4.3 `OtpService.cs` — rewrite fix bugs

**File:** `ClinicBooking.Infrastructure/Services/Security/OtpService.cs`

Thay `new Random()` bằng `RandomNumberGenerator`. Set `MucDich`. Invalidate OTP active cũ. Dev fallback log. Email destination logic với `emailGui`.

Chi tiết:
```
TaoVaGuiOtpDatLichAsync(idTaiKhoan, soDienThoai, emailGui, ct):
  1. Tạo OTP bằng RandomNumberGenerator (6 chữ số)
  2. Invalidate tất cả OTP active cũ của account này (DaSuDung = true)
  3. Tạo OtpLog mới với MucDich, SoLanThu=0
  4. SaveChanges
  5. Xác định email đích: emailGui ?? taiKhoan.Email
  6. Nếu email đích rỗng hoặc @local.invalid → log [OTP-DEV] và return
  7. Gửi email qua IEmailService, bắt exception → log [OTP-DEV] fallback
  8. Return otp string
```

### 4.4 `OtpServiceStub.cs` — không xóa, giữ làm backup

Stub giữ nguyên nhưng **không đăng ký trong DI** (đã là OtpService rồi). Có thể xóa sau khi stable.

### 4.5 `GuiOtpKichHoatWalkInCommand` — thêm EmailNhan

**File:** `ClinicBooking.Application/Features/Auth/Commands/GuiOtpKichHoatWalkIn/GuiOtpKichHoatWalkInCommand.cs`

```csharp
public record GuiOtpKichHoatWalkInCommand(string SoDienThoai, string EmailNhan) : IRequest<int>;
```

### 4.6 `GuiOtpKichHoatWalkInHandler` — pass EmailNhan + set MucDich

**File:** `ClinicBooking.Application/Features/Auth/Commands/GuiOtpKichHoatWalkIn/GuiOtpKichHoatWalkInHandler.cs`

Gọi `_otpService.TaoVaGuiOtpDatLichAsync(taiKhoan.IdTaiKhoan, taiKhoan.SoDienThoai, request.EmailNhan)`.

### 4.7 `LienKetHoSo.cshtml` — thêm field Email bước 1

**File:** `ClinicBooking.Web/Pages/Auth/LienKetHoSo.cshtml`

Bước 1 form thêm input `EmailNhan` (required, type=email).

### 4.8 `LienKetHoSo.cshtml.cs` — bind EmailNhan

**File:** `ClinicBooking.Web/Pages/Auth/LienKetHoSo.cshtml.cs`

Thêm `[BindProperty] public string EmailNhan { get; set; }`. Pass vào command. Store trong TempData cho step 2 hiển thị lại.

### 4.9 `EmailService.cs` — throw thay vì nuốt exception (OTP flow)

Vì `OtpService` đã tự xử lý fallback, `EmailService` nên throw để caller biết lỗi thật. Hiện `GuiEmailAsync` đang catch-all và không throw. **Giải pháp:** không đổi `EmailService` (vì nó dùng chung cho cả notification — nuốt là đúng với notification), nhưng `OtpService` tự wrap try/catch và log fallback.

---

## 5. Checklist triển khai

```
[ ] 4.1 Thêm MucDichOtp.KichHoatWalkIn = 3
[ ] 4.2 IOtpService — thêm emailGui? param (default null)
[ ] 4.3 OtpService.cs — fix 5 bugs + dev fallback + emailGui logic
[ ] 4.5 GuiOtpKichHoatWalkInCommand — thêm EmailNhan
[ ] 4.6 GuiOtpKichHoatWalkInHandler — pass emailGui
[ ] 4.7 LienKetHoSo.cshtml — thêm Email field bước 1
[ ] 4.8 LienKetHoSo.cshtml.cs — bind + TempData EmailNhan
[ ] 3.  appsettings.json — thêm Username/Password keys (rỗng)
[ ] Setup Gmail App Password + dotnet user-secrets (dev tự làm, không commit)
[ ] Build: 0 errors
[ ] Test manual:
    [ ] Walk-in flow: bước 1 nhập SDT + email → nhận OTP qua email thật
    [ ] OTP sai: báo lỗi, không kích hoạt
    [ ] OTP đúng: kích hoạt → auto-login → redirect DanhSachLichHen
    [ ] Dev mode (không có credentials): OTP log ra console, flow vẫn chạy được
    [ ] OTP hết hạn (sau 5 phút): xác thực thất bại
    [ ] Gửi OTP lần 2 với cùng SDT: OTP cũ bị invalidate
```

---

## 6. Known limitations (đồ án)

- Gmail SMTP giới hạn ~500 email/ngày — đủ cho demo và chấm bài
- OTP chỉ gửi qua email, không gửi SMS (SMS cần provider có phí)
- `MucDich` của OTP trong walk-in flow sẽ dùng `KichHoatWalkIn` thay `DatLich` — semantic đúng hơn nhưng không ảnh hưởng DB schema
- Stub `OtpServiceStub.cs` giữ lại không xóa để tránh conflict nếu branch khác đang dùng

---

## 7. Không cần làm (đã có sẵn)

- Viết `IEmailService`, `EmailService`, `OtpOptions`, `EmailSettings` — đã có
- Migration DB mới — `MucDich` column đã trong `InitialCreate`
- Thay đổi DI registration — `OtpService` đã đăng ký đúng
