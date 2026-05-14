# Hướng dẫn setup Gmail SMTP cho OTP

## Tổng quan

OTP gửi qua Gmail SMTP. Cần 2 bước: (1) tạo App Password trên web Google, (2) lưu credentials vào dotnet user-secrets trên máy local.

**Không cần viết code.** Không commit credentials vào git.

---

## Bước 1 — Tạo Gmail App Password (làm trên web)

> Bắt buộc dùng Gmail riêng cho project, không dùng Gmail cá nhân chính.

1. Mở trình duyệt, đăng nhập tài khoản Gmail muốn dùng.

2. Vào: **myaccount.google.com → Security**

3. Tìm mục **"2-Step Verification"** → Bật nếu chưa bật (bắt buộc phải bật trước).

4. Sau khi bật xong, quay lại **Security**, tìm mục **"App passwords"**
   - Nếu không thấy mục này → 2-Step Verification chưa được bật đúng cách.

5. Tại "App passwords":
   - Ô **App name**: nhập `ClinicBooking` (tên tùy ý)
   - Nhấn **Create**

6. Google hiện ra **16 ký tự** dạng `xxxx xxxx xxxx xxxx` → **copy ngay**, chỉ hiện 1 lần.

---

## Bước 2 — Lưu credentials vào dotnet user-secrets

Mở terminal tại thư mục gốc solution (`D:\Code\C#\DatLichPhongKham`), chạy lần lượt:

```powershell
cd ClinicBooking.Web

dotnet user-secrets set "Email:Username" "ten-gmail-cua-ban@gmail.com"
dotnet user-secrets set "Email:Password" "xxxx xxxx xxxx xxxx"
dotnet user-secrets set "Email:FromAddress" "ten-gmail-cua-ban@gmail.com"
```

> `Email:Password` là App Password 16 ký tự ở bước 1, **không phải mật khẩu Gmail thông thường**.
> `Email:FromAddress` phải là đúng địa chỉ Gmail đó (Gmail SMTP tự rewrite nếu sai).

### Kiểm tra đã lưu chưa

```powershell
dotnet user-secrets list
```

Kết quả mong đợi:
```
Email:Username = ten-gmail-cua-ban@gmail.com
Email:Password = xxxx xxxx xxxx xxxx
Email:FromAddress = ten-gmail-cua-ban@gmail.com
```

---

## Bước 3 — Chạy và test

Khởi động project bình thường:

```powershell
cd ..
dotnet run --project ClinicBooking.Web
```

Vào trang **Liên kết hồ sơ** → nhập SĐT walk-in + email thật → nhấn "Gửi mã OTP" → kiểm tra hòm thư.

### Nếu email không đến

Mở console log, tìm dòng `[OTP-DEV]`:

```
[OTP-DEV] Da gui OTP den ... Ma OTP (dev only): 123456
```

Dev fallback tự kích hoạt khi SMTP lỗi — dùng mã này để test tiếp, flow vẫn chạy được.

---

## Lưu ý

| Vấn đề | Nguyên nhân |
|--------|-------------|
| "Less secure app access" không thấy | Gmail mới không có tùy chọn này — dùng App Password thay thế |
| App passwords không hiện | Chưa bật 2-Step Verification |
| Email đến folder Spam | Bình thường với SMTP test — đánh dấu "Not spam" |
| Lỗi `535 Authentication failed` | App Password sai hoặc copy thiếu ký tự |
| Lỗi `Connection refused` | Firewall chặn port 587 — thử mạng khác (tắt VPN nếu đang bật) |

## Xóa secrets khi cần

```powershell
cd ClinicBooking.Web
dotnet user-secrets remove "Email:Password"
dotnet user-secrets clear   # xóa tất cả
```
