# Bàn giao công việc — luồng đặt lịch OTP và seed dữ liệu test

Ngày lập: 2026-05-06  
Phạm vi: luồng đặt lịch bệnh nhân, xác thực OTP, seed dữ liệu test, và những vấn đề đã điều tra trong quá trình verify flow trên localhost.

## 1. Mục tiêu công việc

Mục tiêu ban đầu là bám theo kế hoạch demo nghiệp vụ web UI và giúp luồng đặt lịch / hủy / đổi có thể test thủ công trên localhost. Sau đó, yêu cầu được điều chỉnh sang:

- quay về plan gốc bằng cách bypass OTP trong Development để verify flow nhanh
- sau đó xử lý tiếp các lỗi phát sinh trong UI, OTP, và seed dữ liệu test
- cuối cùng là ghi nhận lại toàn bộ triệu chứng, điều tra và thay đổi đã thực hiện để bàn giao

## 2. Những gì đã làm

### 2.1 Cập nhật tài liệu kế hoạch / báo cáo

- Rà lại và chuẩn hóa các tài liệu kế hoạch liên quan Module 1.
- Viết báo cáo tổng hợp thay đổi và lỗi đã gặp trong quá trình sửa.
- Ghi nhận lại các triệu chứng và kết quả điều tra trong quá trình test.

### 2.2 Điều chỉnh luồng đặt lịch

- Chuyển form đặt lịch sang hướng nhập:
  - ngày đến khám
  - giờ mong muốn
  - dịch vụ
  - triệu chứng / ghi chú
- Tách phần gửi OTP và xác thực OTP thành luồng riêng khi cần.
- Sau đó quay về plan gốc bằng cách bật bypass OTP trong `Development` để verify 4 flow.
- Cập nhật UI hiển thị ngày theo định dạng `dd/MM/yyyy`.

### 2.3 Seed dữ liệu test

- Bổ sung / chỉnh seed để có dữ liệu cơ bản phục vụ test đặt lịch:
  - tài khoản demo
  - chuyên khoa
  - dịch vụ
  - bác sĩ
  - ca làm việc test
- Cố gắng đồng bộ seed để có thể đặt lịch vào thời điểm test (ngày 6/5, 8:00 AM).
- Điều tra và chỉnh các mốc ngày trong seed / seeder để giảm chênh lệch giữa dữ liệu test và ngày người dùng nhập.

### 2.4 Hồ sơ cá nhân

- Cho phép chỉnh thêm:
  - email
  - số điện thoại
- Giữ các trường hồ sơ cá nhân hiện có vẫn cập nhật được bình thường.

## 3. Triệu chứng đã gặp

### 3.1 Đặt lịch báo không có slot phù hợp

Triệu chứng lặp lại nhiều lần:

- đăng nhập `patient001`
- chọn ngày khám là ngày mai (`6/5`)
- nhập giờ `8:00 AM`
- chọn dịch vụ
- bấm đặt lịch
- hệ thống trả lỗi kiểu:
  - `Khong tim thay slot phu hop voi gio mong muon`

Ngay cả sau khi:
- build lại
- run lại app nhiều lần
- mở tab ẩn danh
- đổi sang luồng bypass OTP

… lỗi vẫn xuất hiện.

### 3.2 UI ngày tháng hiển thị theo định dạng không mong muốn

- Trên UI có chỗ hiển thị theo kiểu `mm/dd/yyyy`.
- Người dùng mong muốn hiển thị `dd/MM/yyyy`.
- Đã sửa phần summary/date render trên giao diện.

### 3.3 OTP và số điện thoại ảo

- Số điện thoại trong fixture là dữ liệu ảo.
- Luồng OTP ban đầu gây hiểu nhầm vì người dùng không biết lấy mã ở đâu.
- Đã thử nhiều hướng xử lý:
  - lưu OTP vào DB (`OtpLog`)
  - hiển thị OTP trong thông báo cho local/test
  - sau đó quay về bypass OTP theo plan để verify flow

## 4. Những gì đã điều tra

### 4.1 Kiểm tra luồng đặt lịch

Đã đọc và kiểm tra các phần:

- `ClinicBooking.Web/Pages/BenhNhan/DatLich.cshtml`
- `ClinicBooking.Web/Pages/BenhNhan/DatLich.cshtml.cs`
- `ClinicBooking.Application/Features/LichHen/Commands/TaoLichHen/TaoLichHenHandler.cs`
- `ClinicBooking.Infrastructure/Persistence/DatabaseSeeder.cs`
- `ClinicBooking.Infrastructure/Persistence/Migrations/Module1TestDataSeeder.cs`
- `ClinicBooking.Web/Program.cs`
- `ClinicBooking.Web/appsettings.Development.json`

### 4.2 Kết luận điều tra kỹ thuật

- `TaoLichHenHandler` đang khớp slot rất chặt theo điều kiện:
  - đúng ngày
  - đúng khung giờ
  - còn slot trống
  - ca đã duyệt
- Vì vậy chỉ cần seed / ngày test / giờ test lệch một chút là hệ thống báo không tìm thấy slot phù hợp.
- `DatabaseSeeder` và `Module1TestDataSeeder` đã được chỉnh nhiều lần để cố tạo dữ liệu test bám với ngày người dùng nhập, nhưng thực tế vẫn còn lệch giữa dữ liệu seed và thao tác test.
- Kết quả thực tế cho tới hiện tại: lỗi không còn nằm ở cache trình duyệt hay restart app.
- Vấn đề gốc vẫn là **dữ liệu seed và điều kiện khớp slot trong handler chưa đủ ổn định cho scenario test cụ thể này**.

### 4.3 Build / run / test

- `dotnet build DatLichPhongKham.slnx` đã chạy nhiều lần và nhìn chung pass sau các sửa đổi.
- Một warning lặp lại trong `DatabaseSeeder.cs`:
  - `CS8602: Dereference of a possibly null reference`
- `dotnet test ClinicBooking.Application.UnitTests` đã pass.
- `dotnet ef database update` theo phản ánh của người dùng không chạy được trong môi trường hiện tại.

## 5. Những lỗi đã gặp trong quá trình làm

### 5.1 Lỗi compile do thiếu helper

- Thiếu `TaoSessionOtpStampKey()` trong `DatLich.cshtml.cs`.
- Đã thêm lại và build pass.

### 5.2 Lỗi `init-only` khi sửa hồ sơ

- Gặp lỗi gán lại property `init` trong `BenhNhanResponse`.
- Đã bỏ hướng sửa trực tiếp vào response và tách state chỉnh sửa trong form binding.

### 5.3 Lỗi PowerShell khi commit

- Khi ghép lệnh bằng `&&`, PowerShell báo không hỗ trợ separator này.
- Đã chuyển sang dùng `;` để commit.

### 5.4 Lỗi slot test không khớp

- Dù đã sửa seed và chạy lại app nhiều lần, vẫn có trường hợp đặt lịch báo không tìm thấy slot.
- Điều tra cho thấy đây là vấn đề về dữ liệu test và điều kiện khớp slot của handler, không phải cache hay do người dùng chưa restart.

## 6. Kết quả hiện tại

Đến thời điểm bàn giao:

- Luồng UI và backend đặt lịch đã được chỉnh nhiều vòng.
- Bypass OTP trong `Development` đã được bật theo plan.
- UI ngày tháng đã sửa sang `dd/MM/yyyy`.
- Hồ sơ cá nhân có thể chỉnh thêm email và số điện thoại.
- Build solution đã pass.
- Tuy nhiên, scenario đặt lịch với ngày `6/5` lúc `8:00 AM` vẫn còn lỗi không tìm thấy slot phù hợp.

## 7. Kết luận bàn giao

Công việc hiện tại chưa được chốt hoàn toàn ở mức “verify flow đặt lịch chạy mượt cho đúng ngày 6/5 8:00 AM”.

Điểm dừng hiện tại là:
- code đã được chỉnh đáng kể
- báo cáo và kế hoạch đã được cập nhật
- build pass
- nhưng vẫn còn mismatch giữa seed dữ liệu và điều kiện khớp slot trong luồng đặt lịch

Người tiếp nhận nên ưu tiên kiểm tra tiếp:
1. dữ liệu seed thật trong DB sau khi app khởi động
2. ngày `NgayLamViec` của các `CaLamViec` test
3. điều kiện lọc slot trong `TaoLichHenHandler`
4. khả năng cần thêm slot test cứng hoặc nới điều kiện khớp slot cho môi trường Development
