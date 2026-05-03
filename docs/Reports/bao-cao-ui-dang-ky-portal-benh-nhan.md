# Báo cáo chi tiết — chỉnh sửa phần đăng ký portal bệnh nhân

Ngày lập: 2026-04-29  
Phạm vi: giao diện đăng nhập / đăng ký của portal bệnh nhân  
Mục tiêu: đồng bộ phần đăng ký với demo portal mới và tránh lỗi UI chồng lớp.

## 1) Bối cảnh

Sau khi áp dụng portal bệnh nhân mới theo `docs/DAPM_UI/benhnhan_portal_new.html`, phần đăng ký ban đầu còn bị hiển thị chồng lên màn đăng nhập do thiếu class ẩn trạng thái và form đăng ký chưa được bố trí lại cho sát demo.

## 2) Vấn đề đã gặp

### 2.1 Form đăng ký bị hiện cùng form đăng nhập

Nguyên nhân chính:
- class `.hidden` chưa có trong CSS dùng chung
- form `register-form` không được ẩn đúng cách
- dẫn tới giao diện đăng nhập và đăng ký nhìn như đang chồng lên nhau

### 2.2 Form đăng ký chưa đồng bộ style với portal mới

Nguyên nhân phụ:
- control form còn mang cảm giác “HTML thô”
- spacing và layout chưa theo hướng card / grid của demo
- chưa đồng bộ với hệ thống input/select hiện có của app

## 3) Đã chỉnh sửa những gì

### 3.1 Giao diện đăng ký trong `DangNhap.cshtml`

- Giữ tab `Đăng nhập` / `Đăng ký` theo đúng luồng trong demo.
- Chỉ hiển thị một form tại một thời điểm.
- Làm lại form đăng ký theo grid rõ ràng hơn.
- Dùng lại style input/select của hệ thống để đồng bộ với toàn bộ portal.
- Giữ form đăng ký dành riêng cho bệnh nhân.

### 3.2 CSS dùng chung

- Thêm class `.hidden { display: none !important; }` vào `ClinicBooking.Web/wwwroot/css/common.css`.
- Bổ sung helper layout chung cho portal bệnh nhân để các trang trong portal nhìn thống nhất hơn.
- Thêm style hỗ trợ cho layout đăng nhập / đăng ký và nhóm form đăng ký.

### 3.3 Script chuyển tab

- Tab `Đăng nhập` và `Đăng ký` đã được gắn script chuyển view rõ ràng.
- Khi bấm tab, chỉ form tương ứng được hiển thị.
- Tránh tình trạng form đăng ký lẫn vào khu đăng nhập.

## 4) File đã cập nhật

- `ClinicBooking.Web/Pages/Auth/DangNhap.cshtml`
- `ClinicBooking.Web/wwwroot/css/common.css`
- `ClinicBooking.Web/Pages/Shared/_LoginLayout.cshtml` để render section `Scripts`

## 5) Kết quả đạt được

- Giao diện đăng nhập / đăng ký đã khớp hơn với portal mới.
- Form đăng ký không còn hiển thị chồng lên form đăng nhập.
- Style của form đăng ký thống nhất hơn với design system hiện có.
- Lỗi Razor do thiếu `@RenderSection("Scripts")` đã được xử lý ở layout.

## 6) Ghi chú kỹ thuật

- Phần đăng ký vẫn chỉ là UI portal bệnh nhân, chưa chạm vào luồng backend nếu chưa cần thiết.
- Tất cả thay đổi này nằm trong phần **không bị block** của Module 1.
- Các phần booking backend vẫn giữ nguyên chờ các module phụ thuộc nếu cần.

## 7) Hướng tiếp theo đề xuất

- Kiểm tra lại toàn bộ portal bệnh nhân sau đăng nhập trên trình duyệt.
- Nếu cần, tiếp tục tinh chỉnh các trang:
  - `BenhNhan/DatLich`
  - `BenhNhan/DanhSachLichHen`
  - `BenhNhan/LichHen`
  - `BenhNhan/ThongBao`
  - `BenhNhan/HoSoCaNhan`
- Chỉ triển khai tiếp các phần không bị block hoặc đã chốt contract rõ ràng.
