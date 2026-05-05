# Báo cáo sửa luồng đặt lịch OTP và kiểm tra UI

Ngày lập: 2026-05-05  
Phạm vi: luồng đặt lịch của bệnh nhân, xác thực OTP, và một số điểm kiểm tra giao diện/hồ sơ cá nhân.

## 1. Những gì đã làm

### 1.1 Luồng đặt lịch

- Chuyển form đặt lịch sang hướng nhập **ngày + giờ mong muốn + dịch vụ**, để hệ thống tự khớp ca phù hợp.
- Tách rõ hai bước:
  1. Gửi OTP
  2. Xác thực OTP rồi mới tạo lịch hẹn
- Khi bấm **Gửi mã OTP**, hệ thống tạo OTP thật và lưu vào DB thông qua `OtpLog`.
- Khi bấm **Hoàn tất đặt lịch**, hệ thống xác thực OTP từ DB rồi mới gọi `TaoLichHenCommand`.
- Giữ nguyên hành vi hệ thống tự sắp xếp ca và gửi thông báo sau khi tạo lịch.

### 1.2 Hồ sơ cá nhân

- Cho phép bệnh nhân cập nhật thêm:
  - Email
  - Số điện thoại
- Giữ các trường chỉnh sửa sẵn có như họ tên, ngày sinh, giới tính, CCCD, địa chỉ.

### 1.3 UI/UX

- Bỏ cách diễn đạt “demo OTP” trong giao diện.
- Làm lại câu chữ theo hướng người dùng có thể hiểu luồng: gửi OTP → nhập OTP → hoàn tất đặt lịch.
- Giữ thông điệp rõ rằng hệ thống sẽ tự khớp ca khám.

## 2. Lỗi đã gặp trong quá trình sửa

### 2.1 Lỗi compile do thiếu hàm key session

**Triệu chứng**
- Build fail với lỗi:
  - `CS0103: The name 'TaoSessionOtpStampKey' does not exist in the current context`

**Nguyên nhân**
- Khi chỉnh lại luồng OTP, có đoạn code vẫn gọi key session timestamp nhưng hàm helper tương ứng chưa được khai báo lại.

**Cách xử lý**
- Thêm lại `TaoSessionOtpStampKey()` vào `DatLich.cshtml.cs`.
- Build lại toàn solution để xác nhận hết lỗi.

### 2.2 Lỗi `init-only` khi chỉnh `BenhNhanResponse`

**Triệu chứng**
- Build fail với lỗi:
  - `CS8852: Init-only property or indexer 'BenhNhanResponse.Email' can only be assigned in an object initializer...`
  - `CS8852: Init-only property or indexer 'BenhNhanResponse.SoDienThoai' can only be assigned in an object initializer...`

**Nguyên nhân**
- `BenhNhanResponse` là `record` có các property `init` only, nên không thể gán lại sau khi load dữ liệu.

**Cách xử lý**
- Bỏ hướng sửa trực tiếp vào `HoSo` response.
- Giữ bản edit riêng trong form binding của `HoSoCaNhanModel`.

### 2.3 Lỗi luồng OTP chưa khớp với số điện thoại ảo

**Triệu chứng**
- Số điện thoại trong fixture là dữ liệu giả, nên nếu hiểu OTP theo kiểu gửi SMS thật sẽ không biết mã để nhập.

**Nguyên nhân**
- OTP ban đầu chưa có contract rõ giữa UI và backend.
- Người dùng nhìn thấy nút gửi OTP nhưng không có cơ chế xác thực rõ ràng.

**Cách xử lý**
- Chuẩn hóa theo luồng local/test:
  - OTP được tạo thật ở backend
  - lưu vào `OtpLog`
  - trả mã OTP trong thông báo để người dùng nhập lại và xác thực

## 3. Kết quả cuối cùng

- `dotnet build DatLichPhongKham.slnx` đã pass.
- Luồng đặt lịch đã chạy theo hướng:
  - chọn thời gian mong muốn
  - gửi OTP
  - xác thực OTP
  - tạo lịch hẹn
- Hồ sơ cá nhân đã có thể chỉnh thêm email và số điện thoại.

## 4. Ghi chú

- Bản hiện tại phù hợp để test local và demo nghiệp vụ trên máy của bạn.
- Nếu về sau cần production thật, nên thay phần OTP trả trong thông báo bằng kênh gửi thật (SMS/email) và ẩn OTP khỏi UI.
