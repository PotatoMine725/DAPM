# Báo cáo chốt nghiệp vụ đặt lịch theo giờ mong muốn

Ngày lập: 2026-04-29  
Mục tiêu: đổi nghiệp vụ đặt lịch từ chọn ca làm việc sang chọn ngày + giờ mong muốn, để hệ thống tự khớp slot phù hợp.

## 1) Yêu cầu nghiệp vụ mới

Người dùng:

- vẫn **chọn ngày đến khám**
- không còn chọn ca làm việc thủ công
- sẽ chọn **giờ mong muốn đến khám**
- hệ thống tự tìm slot phù hợp nhất trong ngày đã chọn

## 2) Ý nghĩa của thay đổi

Trước đây portal yêu cầu người dùng tự chọn ca làm việc. Điều này làm UI không sát demo mới và khiến nghiệp vụ dễ bị rối khi người dùng không biết slot nào là đúng.

Sau thay đổi:

- user chỉ cần chọn ngày + giờ mong muốn + dịch vụ
- hệ thống tự ghép slot dựa trên giờ đó
- không cho user chọn bác sĩ trực tiếp

## 3) Đã thay đổi

### 3.1 Command đặt lịch

`TaoLichHenCommand` đã được đổi sang nhận:

- `IdDichVu`
- `NgayLamViec`
- `GioMongMuon`
- `IdBenhNhan`
- `IdBacSiMongMuon`
- `BacSiMongMuonNote`
- `TrieuChung`

### 3.2 UI đặt lịch

`ClinicBooking.Web/Pages/BenhNhan/DatLich.cshtml` và `DatLich.cshtml.cs` đang được đổi sang:

- chọn ngày đến khám
- chọn giờ mong muốn
- hiển thị slot phù hợp trong ngày
- bỏ chọn ca làm việc thủ công

### 3.3 Slot resolve

Backend sẽ:

- lấy ca đã duyệt trong ngày đã chọn
- lọc slot còn trống
- chọn slot khớp với giờ mong muốn
- tạo lịch hẹn vào slot đó

## 4) Hướng xử lý runtime

- giữ nguyên rule slot còn trống và giữ chỗ còn hiệu lực
- vẫn update slot qua `ICaLamViecQueryService`
- nếu không có slot phù hợp thì trả lỗi rõ ràng

## 5) Trạng thái hiện tại

- phần UI và command đã bắt đầu đổi sang model mới
- phần handler đặt lịch đang được sửa để resolve slot tự động
- cần build/test lại sau khi hoàn tất handler và UI

## 6) Kết luận

Nghiệp vụ đặt lịch đã được chốt theo hướng mới: **chọn ngày + chọn giờ mong muốn**, hệ thống tự khớp slot phù hợp thay vì user chọn ca làm việc.
