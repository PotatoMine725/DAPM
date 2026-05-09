# Bàn giao cập nhật UI/UX — Bệnh nhân và Bác sĩ

Ngày lập: 2026-05-09  
Phạm vi: rà soát giao diện bệnh nhân, chuẩn hóa tiếng Việt có dấu, cải thiện UX bằng Razor/CSS, và bắt đầu phần bác sĩ theo cùng hướng.

## 1. Mục tiêu công việc

Mục tiêu của đợt làm việc này là:

- đọc lại `AGENTS.md`, `CLAUDE.md` và các tài liệu liên quan trong `docs/Notes`
- rà soát giao diện phía bệnh nhân để xử lý các điểm UX được ghi trong note
- sau khi phần bệnh nhân ổn, chuyển sang phần bác sĩ theo cùng cách tiếp cận
- ưu tiên nội dung chính nằm trong Razor, chỉ dùng JavaScript nếu thật sự cần cho UX nhẹ

## 2. Những gì đã làm

### 2.1 Rà soát tài liệu và cấu trúc

- Đọc `AGENTS.md` để nắm quy tắc làm việc của dự án.
- Đọc `docs/Notes/ux-cai-thien-benh-nhan.md` để xác định các hạng mục cần triển khai.
- Đọc thêm tài liệu kế hoạch đặt lịch khách vãng lai tại quầy lễ tân để lấy thêm bối cảnh khi cần.
- Kiểm tra các trang Razor liên quan đến bệnh nhân và bác sĩ để xác định điểm cần chỉnh.

### 2.2 Cập nhật UI phía bệnh nhân

- Thay bộ lọc trạng thái lịch hẹn bằng **tab ngang** thay cho dropdown + nút lọc.
- Giữ nguyên logic lọc backend, chỉ đổi giao diện gửi tham số.
- Thêm style cho tab active để nhìn rõ trạng thái đang chọn.
- Bổ sung phần hiển thị **giờ dự kiến khám** trong trang chi tiết lịch hẹn.
- Rà soát và chuẩn hóa một số nhãn / mô tả tiếng Việt có dấu ở trang chi tiết lịch hẹn.
- Chuyển thêm một số text trên trang đổi lịch và hồ sơ cá nhân sang dạng tiếng Việt chuẩn.
- Sửa lỗi build phát sinh do dùng nhầm `GioKetThuc` ở trang danh sách lịch hẹn, chuyển sang tính toán hiển thị dự kiến ngay trong Razor.

### 2.3 Rà soát trang bệnh nhân

- Quét lại toàn bộ nhóm trang bệnh nhân để tìm chỗ còn sót text không dấu hoặc hiển thị dính.
- Xác nhận các trang chính đã ổn theo scope note hiện tại.
- Không thêm JavaScript mới cho phần bệnh nhân vì các cải tiến đã đủ dùng bằng Razor + CSS.

### 2.4 Bắt đầu phần bác sĩ

- Chuyển sang phần bác sĩ sau khi kiểm tra xong phần bệnh nhân.
- Chuẩn hóa các text chính trong `QuanLyKham` và `HangCho` sang tiếng Việt có dấu.
- Tinh chỉnh giao diện theo hướng đồng nhất với phần bệnh nhân.
- Giữ logic nghiệp vụ và luồng submit ở Razor, không thay đổi cấu trúc xử lý chính.
- Không thêm JS mới cho phần bác sĩ ở giai đoạn này vì chưa có điểm cần thiết rõ ràng.

## 3. Những file đã chỉnh

### Bệnh nhân

- `ClinicBooking.Web/Pages/BenhNhan/DanhSachLichHen.cshtml`
- `ClinicBooking.Web/Pages/BenhNhan/LichHen.cshtml`
- `ClinicBooking.Web/Pages/BenhNhan/DoiLich.cshtml`
- `ClinicBooking.Web/Pages/BenhNhan/HoSoCaNhan.cshtml`
- `ClinicBooking.Web/Pages/BenhNhan/DatLich.cshtml`
- `ClinicBooking.Web/wwwroot/css/common.css`

### Bác sĩ

- `ClinicBooking.Web/Pages/BacSi/QuanLyKham.cshtml`
- `ClinicBooking.Web/Pages/BacSi/HangCho.cshtml`

## 4. Ghi chú kỹ thuật

### 4.1 JS và UX

- Trong đợt này **không viết JS mới** cho luồng chính.
- Hướng thực hiện là giữ nội dung chính trong Razor, còn JS chỉ nên dùng nếu có hiệu ứng UX nhẹ thật sự cần thiết.
- Các cải thiện hiện tại chủ yếu đến từ:
  - markup Razor rõ ràng hơn
  - CSS cho tab / layout / trạng thái active
  - text chuẩn hóa

### 4.2 Build và kiểm tra

- Đã rebuild `ClinicBooking.Web` sau khi sửa lỗi `GioKetThuc` ở `DanhSachLichHen`.
- Build hiện tại pass.
- Còn một số warning từ dependency và một warning null reference ở `Pages/Admin/Dashboard.cshtml`, nhưng không chặn build.

## 5. Kết quả hiện tại

Đến thời điểm ghi nhận:

- Phần bệnh nhân đã được rà soát và chỉnh các điểm UX chính theo note.
- Phần bác sĩ đã được bắt đầu, tập trung vào chuẩn hóa text và đồng nhất UI.
- Chưa dùng JavaScript để thay đổi luồng chính của trang.
- Build web project thành công sau khi sửa lỗi phát sinh.

## 6. Kết luận bàn giao

Công việc hiện tại đã đi qua 2 bước:

1. hoàn tất rà soát và cải thiện phần bệnh nhân
2. mở đầu phần bác sĩ theo cùng phong cách, ưu tiên Razor và CSS

Nếu làm tiếp, hướng hợp lý tiếp theo là:
- rà soát thêm các trang bác sĩ còn lại để chuẩn hóa tiếng Việt có dấu
- chỉ thêm JS ở những chỗ thực sự cần cho UX nhẹ như animation, trạng thái active, hoặc hiệu ứng hiển thị ẩn/hiện
