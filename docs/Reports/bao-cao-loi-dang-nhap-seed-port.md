# Báo cáo lỗi đăng nhập Web: seed demo + xung đột port API

**Ngày ghi nhận:** 2026-04-24
**Phạm vi:** `ClinicBooking.Web`, `ClinicBooking.Api`, `DatabaseSeeder`
**Mục tiêu:** ghi lại toàn bộ quá trình điều tra lỗi đăng nhập demo, lỗi seed, và lỗi API không start do trùng port.

---

## 1) Triệu chứng ban đầu

Khi thử đăng nhập bằng các tài khoản demo:
- `patient001`
- `doctor001`
- `receptionist001`
- `admin001`

với mật khẩu demo đã ghi nhận, Web vẫn trả về:
- `Tai khoan hoac mat khau khong dung.`

Trong một số lần khác, API còn fail khi start.

---

## 2) Kết quả điều tra

### 2.1 Kiểm tra runtime log

Log của `ClinicBooking.Api` cho thấy trước đó từng có lỗi liên quan tới password verify / JWT config.
Sau khi rà lại code hiện tại thì lỗi JWT key rỗng của báo cáo cũ đã không còn là nguyên nhân chính nữa.

### 2.2 Kiểm tra DB local

Query trực tiếp vào DB local xác nhận các tài khoản demo đã tồn tại:
- `patient001`
- `doctor001`
- `receptionist001`
- `admin001`
- `admin`

Tuy nhiên, cột `MatKhau` của các tài khoản demo cũ không còn khớp với mật khẩu demo mới.

### 2.3 Kiểm tra seed runtime

Ban đầu `DatabaseSeeder` chỉ seed khi tài khoản chưa tồn tại.
Điều này dẫn tới tình huống:
- tài khoản đã có trong DB
- nhưng password hash cũ vẫn còn nguyên
- nên login luôn báo sai mật khẩu

Sau đó mình đổi seed theo hướng **upsert**:
- nếu account đã có thì update lại mật khẩu hash và thông tin cần thiết
- nếu profile `BenhNhan`, `BacSi`, `LeTan` đã có thì update luôn

### 2.4 Kiểm tra dữ liệu số điện thoại

Trong quá trình upsert, API từng fail do unique index `IX_TaiKhoan_SoDienThoai`.
Nguyên nhân là config seed và DB local dùng các số điện thoại khác nhau.

Sau khi đối chiếu bằng SSMS / `sqlcmd`, mình đồng bộ lại số điện thoại trong seed với DB local.

### 2.5 Kiểm tra lỗi start API

Sau khi seed đã chạy xong, API vẫn fail ở bước start do:
- `Failed to bind to address http://127.0.0.1:5212: address already in use`

Điều này có nghĩa là:
- process API khác đang giữ port 5212
- seeding đã chạy thành công, nhưng host không thể hoàn tất start vì đụng cổng

---

## 3) Root cause

Có 2 vấn đề chính:

### Vấn đề A — seed demo không upsert
Tài khoản demo đã có trong DB nhưng mật khẩu hash không được ghi đè lại, nên login không khớp.

### Vấn đề B — API trùng port khi restart
`ClinicBooking.Api` không start được vì port `5212` đang bị một process khác chiếm.

---

## 4) Cách xử lý đã áp dụng

### 4.1 Sửa seed để upsert tài khoản demo
Đã cập nhật `DatabaseSeeder` để:
- ghi đè mật khẩu hash đúng cho tài khoản demo
- tạo/cập nhật profile tương ứng
- giữ hoạt động idempotent hơn

### 4.2 Đồng bộ config seed với DB local
`ClinicBooking.Api/appsettings.Development.json` được chỉnh để khớp với dữ liệu đang có:
- `patient001` / `0912345678`
- `doctor001` / `0987654321`
- `receptionist001` / `0911111111`
- `admin001` / `0988888888`

Mật khẩu chung demo:
- `Demo@123456`

### 4.3 Làm password verify an toàn hơn
`PasswordHasher.VerifyPassword(...)` được bọc `try/catch` rộng hơn để hash lỗi không làm app văng 500.

### 4.4 Thêm debug vào màn đăng nhập Web
Trong `ClinicBooking.Web/Pages/Auth/DangNhap.cshtml.cs`, khi exception ngoài nhánh `UnauthorizedAccessException` xảy ra, UI sẽ hiển thị thêm:
- `[DEBUG] <ExceptionType>: <Message>`

Mục đích là lấy nguyên nhân chính xác ngay trên UI khi test.

### 4.5 Giải phóng port API
Khi API không start được, mình kiểm tra bằng `terminals/13.txt` và xác định port `5212` đang bị chiếm.
Sau khi dừng process giữ cổng, API start thành công.

---

## 5) Kết quả cuối cùng

Sau khi xử lý xong:
- seed demo chạy được
- tài khoản demo có mật khẩu đúng
- API start thành công sau khi giải phóng port
- Web login không còn lỗi phát sinh từ seed/port

Bộ login demo dùng chung:
- `patient001` / `Demo@123456`
- `doctor001` / `Demo@123456`
- `receptionist001` / `Demo@123456`
- `admin001` / `Demo@123456`

Tài khoản admin gốc:
- `admin` / `Admin@123456`

---

## 6) Bài học rút ra

- Nếu seed dữ liệu demo có thể đã tồn tại từ trước, cần **upsert** thay vì chỉ insert.
- Khi debug login, nên thêm log rõ ràng trên UI để phân biệt:
  - sai mật khẩu
  - sai JWT config
  - lỗi backend runtime
- Khi restart API, cần kiểm tra port trước để tránh nhầm tưởng lỗi do code.
- Với dữ liệu có unique index như số điện thoại, seed config phải khớp với DB local hoặc phải reset DB đúng cách.

---

## 7) Ghi chú cho team

Nếu thử login và gặp lỗi tương tự, hãy kiểm tra theo thứ tự:
1. API có start thật chưa
2. port có bị chiếm không
3. DB local có account seed chưa
4. password hash trong DB có khớp mật khẩu demo không
5. Web đang trỏ đúng backend chưa

