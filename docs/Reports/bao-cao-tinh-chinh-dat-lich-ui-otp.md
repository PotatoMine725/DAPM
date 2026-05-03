# Báo cáo tinh chỉnh UI đặt lịch theo demo và bổ sung OTP

Ngày lập: 2026-04-29  
Phạm vi: làm lại bố cục đặt lịch sát demo hơn và rà logic OTP/mapping liên quan.

## 1) Mục tiêu

Khắc phục điểm chưa khớp với UI demo ở màn `Đặt lịch`:

- bố cục còn khác demo
- thiếu khối OTP
- cần kiểm tra database / code liên quan đến OTP để ghép logic đúng hướng

## 2) Đã chỉnh UI

### 2.1 Layout tổng thể

- đổi tiêu đề sang `Đặt lịch online`
- thêm stepbar mô phỏng luồng demo:
  - chọn thông tin
  - chọn giờ khám
  - xác thực OTP
- giữ chọn ngày đến khám, nhưng bố cục rõ hơn và sát demo hơn

### 2.2 Khối chọn ngày và khung giờ

- vẫn giữ chọn ngày
- hiển thị danh sách khung giờ trống theo card/chip
- giao diện giờ khám chuyển từ radio list sang slot chip
- user chọn một khung giờ nhìn giống demo hơn

### 2.3 Khối thông tin khám

- giữ dịch vụ khám
- giữ lý do khám / triệu chứng
- thêm ghi chú nếu có
- tạo vùng hiển thị thông tin đặt khám ở cột phải

### 2.4 Khối OTP

- bổ sung input OTP
- bổ sung nút gửi OTP
- bổ sung nút hoàn tất đặt lịch
- tạo vị trí khối OTP ở cột phải, gần với demo hơn

## 3) Rà logic OTP và database

### 3.1 Kiểm tra hiện trạng

Đã kiểm tra các phần liên quan:

- entity `OtpLog`
- enum `MucDichOtp`
- repository/dbset liên quan

Kết quả:

- DB/domain đã có `OtpLog`
- mục đích OTP đã có `DatLich`, `DangKy`, `DangNhap`
- nhưng chưa có flow application hoàn chỉnh để gửi / verify OTP cho đặt lịch

### 3.2 Tình trạng tích hợp

Hiện tại phần OTP trong đặt lịch đang được đặt đúng vị trí UI, nhưng logic gửi và verify OTP vẫn cần được chốt contract riêng để tránh đoán sai quy trình nghiệp vụ.

Tạm thời đã để khối OTP và action gửi OTP ở mức khung giao diện + placeholder runtime, chờ chốt flow rõ hơn để nối vào service thật.

## 4) Rà build/runtime

- đã kiểm tra lint cho các file đặt lịch vừa chỉnh
- không có lỗi lint mới

## 5) Kết luận

Màn `Đặt lịch` đã được đẩy sát demo hơn đáng kể về bố cục, step flow và vị trí OTP. Phần data model OTP đã có trong database/domain, nhưng business flow gửi/verify OTP vẫn cần chốt contract riêng trước khi nối thật vào runtime.

## 6) Hướng tiếp theo

- chốt nghiệp vụ OTP cho đặt lịch
- ghép logic send/verify OTP vào runtime
- nếu cần, chỉnh tiếp spacing/màu sắc của từng card để giống ảnh demo hơn nữa
