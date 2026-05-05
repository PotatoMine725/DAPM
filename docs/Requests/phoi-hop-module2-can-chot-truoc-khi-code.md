# Phản hồi làm rõ các điểm cần chốt trước khi code

Chào team Module 1,

Mình đã đọc yêu cầu hỗ trợ và đồng ý rằng Module 2 cần cung cấp implementation thật cho `ICaLamViecQueryService` để luồng đặt lịch của Module 1 có thể chạy end-to-end. Trước khi code, mình muốn chốt lại một vài điểm để tránh lệch contract hoặc sai logic nghiệp vụ.

## Các điểm cần chốt

### 1) Nguồn dữ liệu chuẩn của `SoSlotDaDat`
Cần xác định rõ `SoSlotDaDat` được:
- lưu trực tiếp trên bảng `CaLamViec`, hoặc
- tính từ bảng booking/giữ chỗ, hoặc
- vừa lưu vừa đối soát lại từ dữ liệu phát sinh.

Điểm này ảnh hưởng trực tiếp đến cách `LayThongTinCaAsync`, `KiemTraSlotTrongAsync` và `IncrementSoSlotDaDatAsync` được triển khai.

### 2) Quy ước “slot trống”
Cần chốt rõ slot trống được hiểu là:
- chỉ kiểm tra `SoSlotDaDat < SoSlotToiDa`, hay
- phải tính cả các giữ chỗ còn hiệu lực, hay
- phải loại trừ cả booking pending / cancelled / expired.

Nếu có `TaoGiuCho`, rule này cần thống nhất để tránh overbook.

### 3) Trạng thái nào được phép đặt lịch
Cần xác định chính xác trạng thái ca nào được coi là bookable:
- chỉ `Approved`?
- hay `Active` cũng được?
- `Pending` có cho xem thông tin nhưng không cho đặt không?
- `Rejected` / `Inactive` xử lý thế nào?

### 4) Semantics của `LyDoKhongDatDuoc`
Cần chốt danh sách lý do trả về để Module 1 map lỗi đúng, ví dụ:
- không tồn tại
- chưa được duyệt
- hết slot
- cập nhật đồng thời thất bại
- ca không khả dụng
- dữ liệu không hợp lệ

Nếu enum hiện tại chưa đủ, nên mở rộng theo hướng tương thích ngược.

### 5) Cách cập nhật slot an toàn khi đồng thời
`IncrementSoSlotDaDatAsync` cần có cơ chế chống race condition. Cần chốt hướng triển khai:
- transaction + row lock,
- optimistic concurrency,
- hay atomic update có điều kiện trong DB.

Mục tiêu là không cho phép slot vượt quá giới hạn hoặc bị âm khi hủy/đổi lịch.

### 6) Contract DTO cần giữ nguyên
Mình sẽ giữ nguyên shape và semantics của các DTO đã thống nhất:
- `ThongTinCaLamViecDto`
- `KetQuaKiemTraSlotDto`
- `LyDoKhongDatDuoc`

Nếu cần bổ sung field mới, nên chốt trước để tránh phải sửa handler bên Module 1.

### 7) Scope của thay đổi API
Nếu Module 2 có bổ sung endpoint hoặc đổi naming, cần đảm bảo không làm break các contract mà Module 1 đang gọi.

## Đề xuất cách làm tiếp

1. Chốt các rule ở trên trước khi code.
2. Module 2 implement thật `ICaLamViecQueryService`.
3. Chạy test với các luồng:
   - `TaoLichHen`
   - `DoiLichHen`
   - `TaoGiuCho`
4. Nếu có điều chỉnh DTO hoặc logic slot, báo lại sớm để Module 1 cập nhật đồng bộ.

## Kết luận

Mình sẵn sàng triển khai phần Module 2 ngay sau khi chốt các điểm trên, để hai bên có thể merge sớm lên `develop` và kiểm tra end-to-end với dữ liệu thật.
