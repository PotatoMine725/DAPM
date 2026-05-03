# Yêu cầu hỗ trợ Module 1 từ Module 3

Ngày lập: 2026-04-29  
Người gửi: Module 1 owner  
Người nhận: Thành viên phụ trách Module 3

## Mục tiêu

Để hoàn thiện luồng đặt lịch, hủy lịch và đổi lịch của Module 1, cần Module 3 thống nhất và cung cấp các phần dữ liệu liên quan đến bệnh nhân và trạng thái hồ sơ.

## Việc cần làm

### 1) Ổn định các trường dữ liệu bệnh nhân dùng trong booking flow

Module 1 cần các thông tin ổn định để kiểm tra người đặt lịch có đủ điều kiện hay không, ví dụ:

- thông tin định danh bệnh nhân
- trạng thái hạn chế nếu có
- thông tin liên hệ phục vụ thông báo
- dữ liệu hồ sơ cá nhân tối thiểu để hiển thị trên portal

### 2) Giữ ổn định contract của hồ sơ bệnh nhân

Nếu Module 3 đang làm việc với:

- `BenhNhan`
- `HoSoKham`
- `CapNhatHoSoCuaToi`
- hồ sơ cá nhân bệnh nhân

vui lòng giữ nguyên các trường cần thiết mà Module 1 đang hiển thị hoặc kiểm tra.

### 3) Không đổi semantics gây ảnh hưởng đến lịch hẹn

Một số luồng của Module 1 sẽ cần đọc hồ sơ bệnh nhân để:

- hiển thị trong portal bệnh nhân
- xác thực người đặt lịch
- cho phép xem lịch hẹn của chính mình
- phản ánh trạng thái hồ sơ trong UI

Nếu có chỉnh sửa logic hoặc schema, cần thông báo sớm để tránh làm lệch luồng Module 1.

## Tại sao cần việc này

Portal bệnh nhân của Module 1 sẽ hiển thị dữ liệu từ hồ sơ người dùng. Nếu Module 3 đổi contract hoặc thiếu field, UI/handler của Module 1 sẽ bị lệch so với dữ liệu thật.

## Ưu tiên mong muốn

1. Chốt các field bệnh nhân/hồ sơ tối thiểu mà Module 1 sẽ dùng
2. Đảm bảo các query/update liên quan không phá contract hiện tại
3. Báo lại nếu có ảnh hưởng tới `DangKy`, `DangNhap`, `LayHoSoCuaToi`, `CapNhatHoSoCuaToi`

## Ghi chú phối hợp

- Nếu cần thêm field hiển thị cho portal bệnh nhân, ưu tiên mở rộng không phá cấu trúc cũ.
- Nếu có rule mới về trạng thái bệnh nhân, cần báo để Module 1 điều chỉnh validation và UI.
