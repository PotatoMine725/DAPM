# Báo cáo lỗi: Đăng nhập Web thất bại với "Đã có lỗi xảy ra"

**Ngày phát sinh:** 2026-04-19
**Phạm vi:** `ClinicBooking.Web` (Razor Pages login)
**Ảnh hưởng:** Không ai đăng nhập được vào portal Web, kể cả tài khoản admin. Tài khoản API vẫn hoạt động bình thường.

---

## 1. Triệu chứng

- Form login `/Auth/DangNhap` trả về hai hành vi khác nhau:
  - **Sai mật khẩu** → hiển thị `"Tai khoan hoac mat khau khong dung."` (đúng kỳ vọng).
  - **Đúng mật khẩu** → hiển thị `"Đã có lỗi xảy ra. Vui lòng thử lại."` (bất thường).
- Console log chỉ thấy một dòng `SELECT TOP(1) [t].[IdTaiKhoan]...` từ bảng `TaiKhoan`, không có `UPDATE LanDangNhapCuoi` hay `INSERT INTO RefreshToken` → exception xảy ra sau khi `VerifyPassword` thành công, trước khi `SaveChanges`.
- Network tab DevTools: POST `/Auth/DangNhap` trả 200 (re-render page) thay vì 302 redirect.

---

## 2. Quy trình chẩn đoán (ghi lại để lần sau nhanh hơn)

### Hypothesis đã loại trừ

| Giả thiết | Cách kiểm | Kết quả |
|---|---|---|
| Hash BCrypt trong DB bị lỗi | Chạy `dotnet run --project tools/BamMatKhau -- verify "Admin@123456" "<hash>"` | Hash khớp → không phải |
| Sai config `Admin:MatKhauMacDinh` | So sánh `appsettings.json` Web vs Api | Đã đồng bộ `Admin@123456` |
| Password từ form bị whitespace / zero-width char | Thêm `ILogger` log `request.MatKhau` dưới dạng HEX | HEX `41646D696E40313233343536` = `Admin@123456` chính xác |
| `VerifyPassword` throw exception | Bọc `try/catch` quanh `_passwordHasher.VerifyPassword` | Không throw, `VerifyResult=True` |

### Phát hiện root cause

Log debug trong `DangNhap.cshtml.cs`:
```csharp
catch (Exception ex)
{
    ErrorMessage = "...[DEBUG] " + ex.GetType().Name + ": " + ex.Message;
}
```
Trình duyệt hiển thị:
> `[DEBUG] ArgumentException: IDX10703: Cannot create a 'Microsoft.IdentityModel.Tokens.SymmetricSecurityKey', key length is zero.`

Exception đến từ `TokenService.TaoAccessToken` dòng 40:
```csharp
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
```
`_jwtSettings.Key` rỗng → không ký được token HS256.

---

## 3. Root cause

**Binding `IOptions<JwtSettings>` ở Web project thất bại vì tên property không khớp giữa code và config.**

### Class `JwtSettings` (Infrastructure)
```csharp
public class JwtSettings
{
    public const string SectionName = "Jwt";
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;                      // ← "Key"
    public int AccessTokenExpirationMinutes { get; set; } = 15;          // ← "AccessTokenExpirationMinutes"
    public int RefreshTokenExpirationDays { get; set; } = 7;             // ← "RefreshTokenExpirationDays"
}
```

### `ClinicBooking.Web/appsettings.json` (trước khi sửa)
```json
"Jwt": {
  "SecretKey": "...",                   // ← sai: phải là "Key"
  "Issuer": "ClinicBookingApi",
  "Audience": "ClinicBookingClients",
  "AccessTokenExpiryMinutes": 60,       // ← sai: phải là "AccessTokenExpirationMinutes"
  "RefreshTokenExpiryDays": 7           // ← sai: phải là "RefreshTokenExpirationDays"
}
```

### `ClinicBooking.Api/appsettings.json` (đúng)
```json
"Jwt": {
  "Issuer": "ClinicBooking.Api",
  "Audience": "ClinicBooking.Client",
  "Key": "THAY_BANG_KHOA_BI_MAT_TOI_THIEU_32_KY_TU_CHO_HMAC_SHA256!!",
  "AccessTokenExpirationMinutes": 15,
  "RefreshTokenExpirationDays": 7
}
```

Vì tên property không khớp, `Configure<JwtSettings>` chỉ bind được `Issuer` + `Audience`, còn `Key` / `AccessTokenExpirationMinutes` / `RefreshTokenExpirationDays` rơi về giá trị default (`string.Empty` và `15` / `7`). Khi `TokenService` dùng `Encoding.UTF8.GetBytes("")` → mảng byte rỗng → `SymmetricSecurityKey` ném `IDX10703`.

---

## 4. Fix đã áp dụng

1. **`ClinicBooking.Web/appsettings.json`**: đồng bộ schema với Api, đổi `SecretKey`→`Key`, `AccessTokenExpiryMinutes`→`AccessTokenExpirationMinutes`, `RefreshTokenExpiryDays`→`RefreshTokenExpirationDays`.
2. Gỡ log debug trong `DangNhapHandler.Handle` và `DangNhap.cshtml.cs`, giữ lại `ILogger` cho production error logging (log stack trace thật khi exception, message user-facing vẫn là "Đã có lỗi xảy ra").

---

## 5. Bài học / Action items

### Đã xong

- [x] `catch (Exception ex)` trong Razor Page phải `_logger.LogError(ex, ...)` — trước đó nuốt stack trace, khó chẩn đoán.
- [x] Cảnh báo: trong `appsettings.json` **không được** sửa tên property section `Jwt` một cách tự phát — phải khớp với class `JwtSettings`.

### Cần làm tiếp (backlog)

- [ ] **Validate options on startup**: dùng `.ValidateDataAnnotations().ValidateOnStart()` cho `JwtSettings.Key` (`[Required, MinLength(32)]`). Startup sẽ throw rõ ràng nếu thiếu key, thay vì fail khi request đầu vào.
- [ ] **Tạo test smoke cho config**: một test đọc `appsettings.json` ở cả Api lẫn Web, deserialize `JwtSettings`, assert `Key.Length >= 32`. Ngăn drift giữa 2 project.
- [ ] **Chuyển `Jwt:Key` sang User Secrets / env var** ở môi trường dev. Hiện tại còn commit vào repo — không phải best practice nhưng chấp nhận được cho team học tập; cần dọn trước khi deploy prod.
- [ ] **Suy nghĩ lại vai trò JWT trong Web portal**: Razor Pages dùng Cookie Auth, không thật sự cần JWT access/refresh token. `XacThucResponse.AccessToken` chỉ được tạo rồi bỏ. Đang sửa tạm bằng cách vẫn giữ `TokenService` nhưng xem xét tách nhánh handler Web không sinh JWT ở bước sau.

---

## 6. Quy trình chẩn đoán lỗi login tổng quát (template cho lần sau)

1. **So sánh nhánh error**: sai password có message khác đúng password không? → khoanh vùng exception trước/sau `VerifyPassword`.
2. **Bật `_logger.LogError(ex, ...)` trong tất cả `catch (Exception)`** ở Razor Pages/Controllers. Không bao giờ nuốt stack trace.
3. **Tạm gắn debug text vào `ErrorMessage`** (`ex.GetType().Name + ": " + ex.Message`) để nhìn thấy trên UI mà không cần đọc log. GỠ NGAY sau khi tìm ra.
4. **Kiểm binding `IOptions<T>`** bằng cách log `options.Value` lúc startup — phát hiện ngay key mismatch giữa class và appsettings.
5. Dùng `tools/BamMatKhau verify` để tách bạch "hash sai" vs "flow sai".

---

*Người lập báo cáo: Claude — 2026-04-19*
