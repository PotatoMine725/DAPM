# Báo cáo context Module 1 — phần có thể làm ngay

Ngày lập: 2026-04-29  
Mục tiêu: ghi lại phần Module 1 có thể triển khai ngay, tránh chạm vào các phần đang bị block bởi Module 2/3/4.

## 1) Kết luận ngắn

Các phần có thể làm ngay của Module 1 là phần UI portal bệnh nhân và các trang read-only / shell giao diện. Phần backend booking lõi vẫn có các phụ thuộc sang Module 2 và một phần Module 4, nên chưa triển khai thêm ở thời điểm này.

## 2) Những gì đã làm ngay

### 2.1 Màn đăng nhập bệnh nhân

- Làm mới giao diện `ClinicBooking.Web/Pages/Auth/DangNhap.cshtml` theo hướng portal bệnh nhân.
- Tích hợp hero panel, tab đăng nhập / đăng ký và các thông tin giới thiệu dịch vụ.
- Giữ luồng đăng nhập hiện tại qua `DangNhapModel`.

### 2.2 Tối ưu trải nghiệm portal bệnh nhân

- Bổ sung style cho layout portal bệnh nhân trong `ClinicBooking.Web/wwwroot/css/common.css`.
- Thêm các helper cho:
  - `page-header`
  - `card`
  - `section-stack`
  - `filter-bar`
  - `action-bar`
  - `panel-grid-2`
  - `panel-grid-3`
  - `detail-grid`
  - `timeline`
  - `empty-state`
- Mục tiêu là làm các trang bệnh nhân hiện có khớp hơn với style mới.

## 3) Những gì đã kiểm tra nhưng không làm

Các phần sau được xác định là nên chờ hoặc đang bị block:

- luồng đặt lịch thật cần ca làm việc / slot thật từ Module 2
- luồng hủy / đổi lịch có thể chạm rule scheduling và dữ liệu từ Module 2
- notification thật và job nền vận hành có thể phụ thuộc Module 4
- các luồng thay đổi dữ liệu bệnh nhân liên quan Module 3 thì chưa đụng tới

## 4) Trạng thái git/local/remote đã ghi nhận

- Local branch hiện tại: `develop`
- Remote: `origin`
- Nhánh remote liên quan Module 1/2/4 đã tồn tại, nhưng phần cần cho booking lõi vẫn có stub ở `ICaLamViecQueryService` và `INotificationService`.

## 5) File đã chạm trong đợt làm ngay

- `ClinicBooking.Web/Pages/Auth/DangNhap.cshtml`
- `ClinicBooking.Web/Pages/Auth/DangNhap.cshtml.cs`
- `ClinicBooking.Web/wwwroot/css/common.css`

## 6) Ghi chú tiếp theo

- Tiếp tục mở rộng portal bệnh nhân ở các trang không bị block.
- Không triển khai thêm booking backend cho tới khi Module 2 chốt scheduling contract.
- Không triển khai notification thật cho tới khi Module 4 chốt implementation.
