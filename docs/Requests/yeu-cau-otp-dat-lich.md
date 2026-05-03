# Yêu cầu chốt nghiệp vụ OTP cho đặt lịch

Ngày lập: 2026-04-29  
Người gửi: Module 1 owner  
Người nhận: các module phối hợp liên quan (đặc biệt Module 4 nếu có phần gửi SMS/OTP)

## Mục tiêu

Chốt rõ nghiệp vụ OTP cho flow đặt lịch trước khi code để tránh lệch contract giữa UI, backend và các module phối hợp.

## Nghiệp vụ OTP đã chốt

### 1) OTP áp dụng cho mọi lượt đặt lịch

Tất cả lượt đặt lịch đều phải xác thực OTP, không phân biệt:

- bệnh nhân đặt từ portal
- lễ tân/admin thao tác thay mặt bệnh nhân

### 2) Kênh gửi OTP

OTP sẽ được gửi qua:

- **SMS**

### 3) Thời hạn OTP

OTP có hiệu lực trong:

- **5 phút**

### 4) Số lần nhập sai tối đa

Người dùng được nhập sai OTP tối đa:

- **3 lần**

Sau 3 lần sai, OTP coi như không còn hợp lệ và cần gửi lại OTP mới.

### 5) Vai trò của OTP trong flow đặt lịch

OTP chỉ dùng để **xác nhận tạo lịch hẹn cuối cùng**.

Luồng nghiệp vụ:

1. Người dùng chọn ngày đến khám
2. Người dùng chọn giờ mong muốn
3. Người dùng chọn dịch vụ
4. Hệ thống tự khớp slot phù hợp
5. Hệ thống gửi OTP SMS
6. Người dùng nhập OTP
7. Chỉ khi OTP đúng thì mới tạo lịch hẹn

### 6) Nếu không nhận được OTP

Cho phép:

- **gửi lại OTP**

## Yêu cầu kỹ thuật mong muốn

### 1) Có contract rõ cho OTP đặt lịch

Cần có contract để phục vụ các bước:

- tạo OTP cho đặt lịch
- lưu log OTP
- xác thực OTP
- tăng số lần thử sai
- hết hạn sau 5 phút

### 2) Dữ liệu OTP cần bám vào `OtpLog`

Hiện domain đã có sẵn `OtpLog` và `MucDichOtp.DatLich`, nên flow mới nên tận dụng nền này thay vì tạo model riêng không cần thiết.

### 3) OTP phải gắn với ngữ cảnh đặt lịch

OTP cần biết:

- thuộc mục đích `DatLich`
- thuộc tài khoản / bệnh nhân nào
- thời điểm tạo
- thời điểm hết hạn
- số lần thử sai
- trạng thái đã dùng hay chưa

### 4) Không cho tạo lịch nếu OTP chưa xác thực

Lịch hẹn chỉ được tạo sau khi OTP được verify thành công.

## Kết quả mong muốn

Sau khi chốt và code xong, flow đặt lịch phải là:

- chọn ngày
- chọn giờ
- chọn dịch vụ
- nhận OTP SMS
- xác thực OTP
- tạo lịch hẹn

## Ghi chú phối hợp

Nếu có module phụ trách gửi SMS/notification, cần dùng flow này làm cơ sở để triển khai tiếp.

## Kết luận

OTP đặt lịch đã được chốt theo hướng:

- áp dụng cho mọi booking
- gửi qua SMS
- hết hạn 5 phút
- tối đa 3 lần nhập sai
- chỉ để xác nhận tạo lịch cuối cùng
- cho phép gửi lại OTP
