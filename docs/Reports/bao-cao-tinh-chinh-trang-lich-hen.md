# Báo cáo tinh chỉnh trang Lịch hẹn

Ngày lập: 2026-04-29  
Phạm vi: làm lại trang chi tiết lịch hẹn cho sát demo và đồng bộ với portal bệnh nhân mới.

## 1) Mục tiêu

Trang `Lịch hẹn` cần trở thành trang detail chính của portal bệnh nhân, với bố cục rõ ràng hơn và ít cảm giác kiểu bảng quản trị.

## 2) Đã chỉnh

### 2.1 Layout tổng thể

- giữ `page-header` với CTA quay lại danh sách và huỷ lịch
- dùng `panel-grid-2` để chia hai cột
- các khối chính được đặt trong card rõ ràng, đồng đều hơn

### 2.2 Khối thông tin lịch hẹn

Đã làm lại khối tổng quan để hiển thị:

- mã lịch hẹn
- ngày khám
- khung giờ
- số thứ tự
- dịch vụ
- ngày đặt
- trạng thái ở header card

### 2.3 Khối thông tin người khám

Hiển thị gọn hơn:

- người khám
- hình thức đặt
- bác sĩ mong muốn
- ghi chú
- triệu chứng / lý do khám

### 2.4 Khối tóm tắt nhanh

Giữ khối tóm tắt bên phải để hỗ trợ user xem nhanh:

- trạng thái
- ca làm việc
- dịch vụ
- mã bệnh nhân

### 2.5 Lịch sử trạng thái

- giữ timeline lịch sử trạng thái
- style đồng bộ với `common.css`
- hiển thị theo card portal thay vì dạng bảng

## 3) Kiểm tra

Đã kiểm tra lint cho file trang Lịch hẹn.

- không có lỗi lint mới

## 4) Kết luận

Trang `Lịch hẹn` đã sát demo hơn và đồng bộ hơn với portal mới, đồng thời vẫn giữ đầy đủ thông tin chi tiết và chức năng huỷ lịch.
