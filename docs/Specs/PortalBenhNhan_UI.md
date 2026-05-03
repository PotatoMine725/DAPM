# Spec giao diện Portal Bệnh nhân

Ngày lập: 2026-04-29  
Mục đích: mô tả phạm vi và cách áp dụng giao diện portal bệnh nhân theo file mẫu `docs/DAPM_UI/benhnhan_portal_new.html`.

## 1) Mục tiêu

- Cung cấp một portal bệnh nhân hiện đại, rõ ràng, dễ dùng.
- Tích hợp luôn màn đăng nhập mới cho bệnh nhân trong cùng trải nghiệm.
- Đồng bộ với backend Module 1 cho đặt lịch, hủy lịch, đổi lịch, xem lịch hẹn và hồ sơ cá nhân.

## 2) Nguồn thiết kế

Nguồn tham chiếu chính:

- `docs/DAPM_UI/benhnhan_portal_new.html`
- `docs/Others/design-system.md`
- `docs/Specs/MO_TA_GIAO_DIEN.md`

## 3) Cấu trúc màn hình chính

### 3.1 Màn đăng nhập / đăng ký

- Bố cục 2 cột:
  - cột trái: hero / giới thiệu / hotline / địa chỉ
  - cột phải: form đăng nhập và đăng ký
- Có tab chuyển giữa đăng nhập và đăng ký.
- Đăng nhập bệnh nhân là luồng chính.
- Nếu hệ thống giữ đăng ký trong portal thì hiển thị dưới dạng tab phụ.

### 3.2 Portal sau đăng nhập

Portal gồm các vùng sau:

- Sidebar điều hướng
- Topbar hiển thị người dùng hiện tại
- Content area render theo từng view

Các view tối thiểu:

- Tổng quan
- Chuyên khoa
- Đặt lịch
- Lịch hẹn
- Thông báo
- Hồ sơ cá nhân

## 4) Nguyên tắc UI/UX

- Màu sắc chủ đạo là xanh y tế, sạch và tin cậy.
- Card, badge, button, modal phải thống nhất theo design system.
- Responsive tốt trên mobile, tablet, desktop.
- Trạng thái rõ ràng: success, warning, danger, neutral.
- Các action quan trọng như đặt lịch, đổi lịch, hủy lịch phải nổi bật nhưng không gây rối.

## 5) Yêu cầu hành vi

### 5.1 Đăng nhập

- Sai tài khoản / mật khẩu phải có thông báo rõ.
- Đăng nhập thành công thì chuyển sang portal bệnh nhân.
- Hết phiên thì quay lại màn đăng nhập.

### 5.2 Đặt lịch

- Hiển thị slot còn trống.
- Slot đã đầy phải disable hoặc gắn nhãn không khả dụng.
- Người dùng phải thấy rõ thông tin chuyên khoa, bác sĩ, giờ, phòng.

### 5.3 Lịch hẹn

- Cho phép lọc theo trạng thái.
- Hiển thị được thao tác đổi/hủy nếu hợp lệ.
- Danh sách phải phân trang nếu dài.

### 5.4 Thông báo

- Có badge số lượng chưa đọc.
- Có màn chi tiết thông báo.
- Đã đọc thì cập nhật state hiển thị.

### 5.5 Hồ sơ cá nhân

- Cho phép chỉnh thông tin cơ bản.
- Cần phản hồi rõ sau khi lưu.
- Không hiển thị trường nội bộ không thuộc phạm vi bệnh nhân.

## 6) Mapping sang implementation

Khi triển khai thật, có thể tách thành các nhóm sau:

- layout shell
- auth view
- dashboard view
- booking view
- appointment view
- notification view
- profile view

Nếu dùng Razor Pages, nên chia layout chung và các partial/component cho từng khu vực. Nếu dùng JS SPA-like shell, cần tách state, render, event handler và API service rõ ràng.

## 7) Dữ liệu cần từ backend

Portal bệnh nhân cần tối thiểu:

- danh sách chuyên khoa
- danh sách bác sĩ theo chuyên khoa
- slot/ca khám còn trống
- lịch hẹn của bệnh nhân
- chi tiết lịch hẹn
- thông báo của bệnh nhân
- hồ sơ cá nhân

## 8) Không thuộc phạm vi

- Giao diện nội bộ lễ tân / admin / bác sĩ.
- Dashboard quản trị hệ thống.
- Các luồng nghiệp vụ của module khác không liên quan trực tiếp tới portal bệnh nhân.
