# Yêu cầu triển khai `ICaLamViecQueryService` cho Module 2

Ngày lập: 2026-04-29  
Người gửi: Module 1 owner  
Người nhận: Chủ module phụ trách Module 2

## Mục tiêu

Module 1 đang cần implementation thật của `ICaLamViecQueryService` để hoàn thiện luồng đặt lịch end-to-end với dữ liệu thật. Hiện tại Module 1 vẫn dùng stub cho service này, nên booking flow chỉ chạy được ở mức tạm thời.

Sau khi Module 2 hoàn tất, Module 1 sẽ có thể tích hợp lại để kiểm tra các luồng:

- đặt lịch
- giữ chỗ
- đổi lịch
- hủy lịch

## Phạm vi cần làm

### 1) Implement thật `ICaLamViecQueryService`

Cần thay stub hiện tại bằng implementation thật cho các method liên quan, tối thiểu gồm:

- `LayThongTinCaAsync`
- `KiemTraSlotTrongAsync`
- `IncrementSoSlotDaDatAsync`

Nếu Module 2 có thêm method hỗ trợ booking flow thì cần giữ contract tương thích với Module 1.

### 2) Chốt nguồn dữ liệu của `SoSlotDaDat`

Cần thống nhất theo rule đã phản hồi:

- `SoSlotDaDat` nên theo mô hình **hybrid**
- tức là có thể lưu trên `CaLamViec`, nhưng vẫn cần đối soát với dữ liệu booking/giữ chỗ
- không được chỉ dựa hoàn toàn vào cache nội bộ nếu điều đó làm lệch dữ liệu thật

### 3) Chốt rule slot trống

Một ca được xem là còn trống khi:

- số slot đã đặt + slot đang giữ chỗ còn hiệu lực < `SoSlotToiDa`

Tức là rule slot trống cần **bao gồm giữ chỗ còn hiệu lực**, không chỉ đơn giản so sánh `SoSlotDaDat < SoSlotToiDa`.

### 4) Chốt trạng thái ca được phép đặt

Theo thống nhất hiện tại:

- chỉ các ca ở trạng thái `Approved` mới được đặt lịch

Các trạng thái khác:

- `Pending` chỉ có thể xem nếu cần, nhưng không được đặt
- `Rejected` / `Inactive` không được đặt

### 5) Chống overbook khi tăng slot

`IncrementSoSlotDaDatAsync` cần cơ chế chống race condition theo hướng đã chốt:

- **optimistic concurrency**

Mục tiêu là:

- không vượt quá giới hạn slot
- không làm âm slot khi hủy/đổi lịch
- tránh ghi đè sai khi nhiều request đồng thời

### 6) Giữ nguyên contract DTO

Ưu tiên giữ nguyên các DTO/enum đã thống nhất để Module 1 không phải sửa nhiều:

- `ThongTinCaLamViecDto`
- `KetQuaKiemTraSlotDto`
- `LyDoKhongDatDuoc`

Nếu thực sự thiếu case, chỉ mở rộng theo hướng tương thích ngược.

### 7) Quy ước `LyDoKhongDatDuoc`

Giữ nguyên enum hiện tại và chỉ mở rộng khi cần. Các lý do cần map tối thiểu phải bao phủ:

- không tồn tại
- chưa được duyệt
- hết slot
- ca không khả dụng
- cập nhật đồng thời thất bại
- dữ liệu không hợp lệ

### 8) Không phá contract Module 1 đang gọi

Nếu Module 2 cần thêm endpoint, đổi naming hoặc đổi shape dữ liệu, vui lòng chốt trước với Module 1 để không làm vỡ flow đang có.

## Luồng cần test sau khi implement

Sau khi có implementation thật, cần test lại các luồng sau:

1. `TaoLichHen`
2. `DoiLichHen`
3. `TaoGiuCho`

Kiểm tra các tình huống:

- đặt khi còn slot
- đặt khi hết slot
- đặt khi ca chưa `Approved`
- giữ chỗ còn hiệu lực
- đồng thời nhiều request tăng slot
- hủy / đổi lịch không làm sai số lượng

## Ưu tiên giao việc

Module 1 ưu tiên hoàn tất luồng **đặt lịch** trước, sau đó mới quay lại các luồng hủy/đổi lịch nếu còn cần tinh chỉnh.

## Kết quả mong muốn

Sau khi Module 2 hoàn tất:

- Module 1 sẽ bỏ stub `ICaLamViecQueryService`
- booking flow có thể chạy end-to-end với dữ liệu thật
- portal bệnh nhân có thể kiểm tra slot trống và trạng thái ca chính xác hơn
- hai bên có thể merge lên `develop` để kiểm thử thực tế

## Ghi chú phối hợp

Nếu trong quá trình implement thấy rule nào chưa rõ, vui lòng phản hồi sớm trước khi code để tránh phải sửa lại contract sau này.
