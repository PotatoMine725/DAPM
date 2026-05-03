# Báo cáo tinh chỉnh portal bệnh nhân — đồng bộ spacing và layout

Ngày lập: 2026-04-29  
Phạm vi: rà lại portal bệnh nhân ngoài trang đặt lịch, đồng bộ giao diện sát demo hơn và chuẩn hóa độ giãn nở UI.

## 1) Mục tiêu

Sau khi portal bệnh nhân đã có các trang chính, tiếp tục tinh chỉnh để:

- giao diện bám sát demo hơn
- các khối UI giãn nở đồng đều, không bị lệch kích thước gây khó chịu
- các trang ngoài `Đặt lịch` đồng nhất hơn về bố cục, card, khoảng cách và cách hiển thị dữ liệu

## 2) Vấn đề được nhận diện

### 2.1 UI giãn nở không đồng đều

Một số màn portal có các khối card / grid / item hiển thị khác nhau về chiều cao và độ co giãn, đặc biệt khi dữ liệu dài hoặc ngắn làm cho bố cục nhìn thiếu đều.

### 2.2 Danh sách lịch hẹn còn quá dạng bảng

Màn lịch hẹn nhìn chưa đủ giống portal demo và vẫn còn cảm giác giống màn quản trị bảng dữ liệu hơn là portal bệnh nhân.

### 2.3 Trang chuyên khoa còn thô

Trang chuyên khoa cần cảm giác portal hơn, có card bác sĩ rõ ràng hơn và spacing đều hơn.

## 3) Đã tinh chỉnh

### 3.1 Đồng bộ lại trang chuyên khoa

Đã cập nhật `ClinicBooking.Web/Pages/BenhNhan/ChuyenKhoa.cshtml` theo hướng:

- lọc chuyên khoa ở trên theo card
- danh sách bác sĩ hiển thị bằng card đều khối
- layout 2 cột bằng `panel-grid-2`
- card bác sĩ có avatar, tên, chuyên khoa, mô tả và thông tin phụ

Đồng thời bổ sung style chung trong `common.css` cho:

- `doctor-card`
- `doctor-avatar`
- `doctor-card__top`
- `doctor-card__body`

### 3.2 Đồng bộ lại danh sách lịch hẹn

Đã đổi `ClinicBooking.Web/Pages/BenhNhan/DanhSachLichHen.cshtml` từ dạng table sang dạng danh sách card để:

- nhìn giống portal hơn
- mỗi lịch hẹn có một card riêng
- nội dung co giãn đồng đều hơn
- tránh cảm giác dòng nào dài là kéo cả bảng méo layout

Đồng thời bổ sung style chung trong `common.css` cho:

- `appointment-list`
- `appointment-card`
- `appointment-card__top`
- `appointment-card__body`
- `appointment-card__actions`

### 3.3 Giảm sự lệch chiều cao giữa các khối

Trong `common.css` đã tiếp tục chuẩn hóa:

- `card`
- `panel-grid-2`
- `panel-grid-3`
- `summary-item`
- `detail-item`
- `empty-state`

Mục tiêu là giúp các block cùng khu vực có cảm giác đồng đều hơn khi nội dung nhiều ít khác nhau.

### 3.4 Giới hạn hiển thị lịch hẹn mỗi trang

`ClinicBooking.Web/Pages/BenhNhan/DanhSachLichHen.cshtml.cs` đã được đổi về:

- `KichThuocTrang = 5`

Việc này giúp:

- tránh UI tràn khi dữ liệu nhiều
- hợp với portal bệnh nhân hơn
- dễ đọc và đỡ nặng mắt trên từng trang

## 4) Các file đã cập nhật

- `ClinicBooking.Web/Pages/BenhNhan/ChuyenKhoa.cshtml`
- `ClinicBooking.Web/Pages/BenhNhan/DanhSachLichHen.cshtml`
- `ClinicBooking.Web/Pages/BenhNhan/DanhSachLichHen.cshtml.cs`
- `ClinicBooking.Web/wwwroot/css/common.css`

## 5) Kết quả

- Portal bệnh nhân nhìn sát demo hơn ở phần chuyên khoa và lịch hẹn.
- Các khối UI đã đồng đều hơn thay vì giãn nở lộn xộn.
- Danh sách lịch hẹn không còn thiên về bảng quản trị mà chuyển sang card-style portal.
- Pagination đã được siết xuống 5 item/trang để giữ giao diện gọn hơn.

## 6) Kiểm tra

- Đã kiểm tra lint cho các file vừa chỉnh.
- Không có lỗi lint mới.

## 7) Hướng tiếp theo

Nếu cần bám demo sát hơn nữa, nên tiếp tục xem lại:

- card chuyên khoa để thêm nhịp thông tin / icon / CTA
- trang chi tiết lịch hẹn để nhìn gần giống demo hơn
- spacing giữa các section trong portal
- các nút điều hướng trong sidebar nếu cần đổi tên hoặc thêm biểu tượng cụ thể hơn
