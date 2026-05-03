# Báo cáo UI đặt lịch sát demo và rà build/runtime

Ngày lập: 2026-04-29  
Mục tiêu: làm lại UI đặt lịch bám sát demo hơn và kiểm tra build/runtime sau khi đổi nghiệp vụ đặt lịch theo giờ mong muốn.

## 1) UI đặt lịch đã được chỉnh

### 1.1 Giữ chọn ngày đến khám

Người dùng vẫn chọn ngày đến khám như yêu cầu nghiệp vụ đã chốt.

### 1.2 Bỏ chọn ca làm việc thủ công

UI cũ cho phép user chọn radio ca làm việc. Đã bỏ phần này để đúng nghiệp vụ mới.

### 1.3 Thay bằng chọn giờ mong muốn

Đã thêm input giờ mong muốn:

- chọn theo mốc 30 phút
- hệ thống tự khớp slot gần nhất còn trống trong ngày đã chọn

### 1.4 Làm lại layout sát demo hơn

Trang `DatLich` đã được làm lại theo hướng:

- hero card đầu trang
- cột trái là khung giờ trong ngày
- cột phải là giờ mong muốn, dịch vụ và thông tin bổ sung
- spacing và card style đồng bộ hơn với portal mới

## 2) Runtime đã được chỉnh

### 2.1 Command đặt lịch

`TaoLichHenCommand` đã đổi sang nhận:

- `NgayLamViec`
- `GioMongMuon`
- `IdDichVu`
- thông tin phụ trợ như trước

### 2.2 Handler đặt lịch

`TaoLichHenHandler` đã được cập nhật để:

- lấy danh sách ca trong ngày đã chọn
- lọc các slot đã duyệt và còn chỗ
- chọn slot khớp giờ mong muốn
- tiếp tục dùng `ICaLamViecQueryService` để kiểm tra và tăng slot

## 3) Rà build/runtime

Sau khi chỉnh UI và runtime:

- đã kiểm tra lint cho các file liên quan
- không có lỗi lint mới

## 4) Kết luận

Trang đặt lịch đã tiến gần hơn tới demo:

- giữ chọn ngày
- chọn giờ mong muốn thay vì chọn ca
- hệ thống tự khớp slot
- UI card/slot gọn hơn và rõ hơn

## 5) Ghi chú

Bước tiếp theo nên là chạy lại `dotnet build`/mở web để xác nhận flow đặt lịch thực sự chạy end-to-end với dữ liệu thật.
