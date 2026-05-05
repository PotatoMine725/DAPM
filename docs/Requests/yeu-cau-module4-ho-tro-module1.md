# Yêu cầu hỗ trợ Module 1 từ Module 4

Ngày lập: 2026-04-29  
Người gửi: Module 1 owner  
Người nhận: Thành viên phụ trách Module 4

## Mục tiêu

Để Module 1 hoàn thiện luồng đặt lịch, hủy lịch, đổi lịch và portal bệnh nhân, cần Module 4 cung cấp phần thông báo, job nền và các hỗ trợ vận hành liên quan.

## Việc cần làm

### 1) Hoàn thiện implementation thật cho `INotificationService`

Hiện Module 1 đang dùng notification stub. Cần có implementation thật cho các action sau:

- gửi thông báo tạo lịch hẹn
- gửi thông báo xác nhận lịch hẹn
- gửi thông báo hủy lịch hẹn
- gửi thông báo đổi lịch hẹn
- gửi thông báo check-in
- gửi thông báo gọi bệnh nhân tiếp theo

### 2) Đảm bảo thông báo có thể dùng được trong flow Module 1

Module 1 cần thông báo xuất hiện đúng thời điểm sau khi:

- tạo lịch hẹn
- xác nhận lịch
- hủy lịch
- đổi lịch
- check-in
- gọi lượt khám tiếp theo

### 3) Cung cấp cơ chế job nền nếu cần

Nếu Module 4 đã có hoặc sẽ có scheduler/job nền, vui lòng ưu tiên các job sau:

- quét giữ chỗ hết hạn
- đồng bộ trạng thái lịch hẹn quá hạn
- xử lý tác vụ lặp liên quan đến nhắc lịch hoặc dọn dẹp dữ liệu vận hành

### 4) Giữ ổn định semantic thông báo

Nếu có template, bảng thông báo hoặc rule hiển thị, cần giữ tương thích để Module 1 chỉ cần gọi service mà không phải đổi luồng nghiệp vụ.

## Tại sao cần việc này

Module 1 hiện có thể vận hành bằng stub, nhưng để đưa portal và booking flow vào trạng thái hoàn chỉnh thì cần thông báo thật và job nền thật. Đây là phần làm trải nghiệm người dùng và vận hành nhất quán.

## Ưu tiên mong muốn

1. Chốt implementation `INotificationService`
2. Nếu có job nền, chốt các recurring job cần thiết
3. Báo lại khi contract đã ổn để Module 1 tích hợp vào flow chính

## Ghi chú phối hợp

- Nếu template thông báo cần dữ liệu bổ sung, hãy báo trước để Module 1 cung cấp thêm field trong command hoặc query.
- Nếu job nền có thể ảnh hưởng tới trạng thái lịch hẹn, cần chốt rule với Module 1 trước khi merge.
