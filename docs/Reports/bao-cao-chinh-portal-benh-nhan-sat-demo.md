# Báo cáo chỉnh portal bệnh nhân sát demo

Ngày lập: 2026-04-29  
Phạm vi: sidebar portal bệnh nhân, trang đặt lịch, trang chuyên khoa công khai, và điều hướng portal bệnh nhân.

## 1) Mục tiêu

Điều chỉnh portal bệnh nhân để sát demo nhất có thể về cả giao diện và logic, gồm:

- sửa lại sidebar và tên tab
- thay trang `Thứ tự hàng chờ` bằng trang `Chuyên khoa`
- làm lại layout trang `Đặt lịch`
- bỏ thao tác chọn bác sĩ trong nghiệp vụ đặt lịch
- giữ và hoàn thiện trang `Lịch hẹn` để xem chi tiết lịch hẹn

## 2) Những thay đổi đã thực hiện

### 2.1 Sidebar portal bệnh nhân

Trong `ClinicBooking.Web/Pages/Shared/_Layout.cshtml`:

- bỏ mục `Thứ tự hàng chờ`
- đổi mục điều hướng thành `Chuyên khoa`
- đổi nhãn `Lịch khám của tôi` thành `Lịch hẹn`
- giữ các mục còn lại gồm:
  - `Đặt lịch mới`
  - `Chuyên khoa`
  - `Hồ sơ cá nhân`
  - `Thông báo`

Mục tiêu là đưa sidebar gần sát flow trong demo portal.

### 2.2 Thêm trang `Chuyên khoa`

Đã thêm:

- `ClinicBooking.Web/Pages/BenhNhan/ChuyenKhoa.cshtml`
- `ClinicBooking.Web/Pages/BenhNhan/ChuyenKhoa.cshtml.cs`

Chức năng:

- cho phép bệnh nhân xem chuyên khoa công khai
- lọc theo chuyên khoa
- hiển thị danh sách bác sĩ công khai theo chuyên khoa tương ứng

Dữ liệu hiện tại là view model công khai, chưa chạm vào logic block của Module 2/3/4.

### 2.3 Làm lại trang `Đặt lịch`

Đã chỉnh `ClinicBooking.Web/Pages/BenhNhan/DatLich.cshtml` theo hướng sát demo hơn:

- layout tách hai cột rõ ràng
- cột trái chọn ngày + danh sách ca trống
- cột phải chọn dịch vụ và nhập triệu chứng
- bỏ hoàn toàn thao tác chọn bác sĩ theo nghiệp vụ
- chỉ giữ lựa chọn ca, dịch vụ và mô tả triệu chứng
- dùng card/slot style thay cho UI cũ

Đồng thời đã sửa `ClinicBooking.Web/Pages/BenhNhan/DatLich.cshtml.cs` để load dữ liệu an toàn hơn, tránh lỗi xử lý song song trên cùng context.

### 2.4 Trang `Lịch hẹn`

Giữ và tiếp tục hoàn thiện trang chi tiết lịch hẹn:

- trang `ClinicBooking.Web/Pages/BenhNhan/LichHen.cshtml`
- trang `ClinicBooking.Web/Pages/BenhNhan/LichHen.cshtml.cs`

Trang này đóng vai trò xem chi tiết từng lịch hẹn, đúng mục tiêu demo portal.

## 3) Giao diện và logic đã được chuẩn hóa

- portal bệnh nhân có sidebar rõ ràng hơn
- luồng điều hướng gọn hơn
- trang chuyên khoa thay thế phần thứ tự hàng chờ không còn phù hợp với demo
- đặt lịch bám sát trải nghiệm người dùng hơn
- không cho chọn bác sĩ thủ công trong booking, đúng nghiệp vụ đã chốt

## 4) File đã chạm

### Layout và điều hướng
- `ClinicBooking.Web/Pages/Shared/_Layout.cshtml`
- `ClinicBooking.Web/wwwroot/css/common.css`

### Portal bệnh nhân
- `ClinicBooking.Web/Pages/BenhNhan/DatLich.cshtml`
- `ClinicBooking.Web/Pages/BenhNhan/DatLich.cshtml.cs`
- `ClinicBooking.Web/Pages/BenhNhan/ChuyenKhoa.cshtml`
- `ClinicBooking.Web/Pages/BenhNhan/ChuyenKhoa.cshtml.cs`
- `ClinicBooking.Web/Pages/BenhNhan/DanhSachLichHen.cshtml`
- `ClinicBooking.Web/Pages/BenhNhan/LichHen.cshtml`
- `ClinicBooking.Web/Pages/BenhNhan/ThongBao.cshtml`
- `ClinicBooking.Web/Pages/BenhNhan/HoSoCaNhan.cshtml`

### Đăng nhập / đăng ký liên quan portal
- `ClinicBooking.Web/Pages/Auth/DangNhap.cshtml`
- `ClinicBooking.Web/Pages/Shared/_LoginLayout.cshtml`

## 5) Lưu ý kỹ thuật

- Các thay đổi này tập trung vào phần **không bị block** và UI/logic portal bệnh nhân.
- Các luồng booking backend vẫn phải giữ đúng phụ thuộc Module 2 nếu cần mở rộng sâu hơn.
- Trang `Chuyên khoa` hiện đang dùng dữ liệu công khai mẫu, để đảm bảo tiến độ trước khi tích hợp nguồn dữ liệu thật nếu cần.

## 6) Kiểm tra sau chỉnh sửa

- Đã kiểm tra lint cho các file chính vừa sửa.
- Không phát sinh lỗi lint mới.

## 7) Hướng tiếp theo đề xuất

- Chạy `dotnet build` và kiểm tra portal bệnh nhân trên trình duyệt.
- Nếu demo yêu cầu thêm chi tiết hơn, tiếp tục tinh chỉnh:
  - card chuyên khoa
  - card slot đặt lịch
  - màn chi tiết lịch hẹn
  - màu sắc và spacing theo demo HTML

