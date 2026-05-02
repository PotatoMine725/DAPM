# Yêu cầu hỗ trợ Module 1 từ Module 2

Ngày lập: 2026-04-29  
Người gửi: Module 1 owner  
Người nhận: Thành viên phụ trách Module 2

## Mục tiêu

Để Module 1 có thể hoàn thiện đầy đủ luồng đặt lịch, hủy lịch và đổi lịch, cần Module 2 cung cấp các phần sau trên nhánh của mình và sớm merge lên `develop`.

## Việc cần làm

### 1) Hoàn thiện implementation thật cho `ICaLamViecQueryService`

Hiện Module 1 đang dùng stub tạm. Cần module của bạn cung cấp implementation thật cho các API sau:

- `LayThongTinCaAsync(int idCaLamViec, CancellationToken ct)`
- `KiemTraSlotTrongAsync(int idCaLamViec, CancellationToken ct)`
- `LaCaDuocDuyetAsync(int idCaLamViec, CancellationToken ct)`
- `IncrementSoSlotDaDatAsync(int idCaLamViec, int delta, CancellationToken ct)`

### 2) Đảm bảo dữ liệu ca làm việc phục vụ booking là chính xác

Module 1 cần các thông tin sau từ ca làm việc:

- ca có tồn tại hay không
- ca có được duyệt không
- số slot tối đa và số slot đã đặt
- khả năng kiểm tra slot trống theo thời gian thực
- cập nhật số slot đã đặt theo cách an toàn khi có đồng thời

### 3) Giữ ổn định contract đã chốt

Xin giữ nguyên shape của DTO và semantic đã thống nhất trong docs Module 1 để không phải sửa lại handler:

- `ThongTinCaLamViecDto`
- `KetQuaKiemTraSlotDto`
- `LyDoKhongDatDuoc`

### 4) Tránh thay đổi breaking ở API scheduling hiện có

Nếu Module 2 có bổ sung endpoint hoặc đổi naming, vui lòng tránh phá các contract mà Module 1 sẽ gọi vào.

## Tại sao cần việc này

Các luồng sau của Module 1 đang phụ thuộc trực tiếp vào ca làm việc thật:

- `TaoLichHen`
- `DoiLichHen`
- `TaoGiuCho`
- kiểm tra slot trước khi đặt lịch
- cộng/trừ số slot đã đặt một cách an toàn

Nếu chưa có implementation thật từ Module 2, Module 1 chỉ có thể chạy bằng stub, chưa thể xác nhận end-to-end.

## Ưu tiên mong muốn

1. Chốt implementation của `ICaLamViecQueryService`
2. Merge sớm lên `develop`
3. Báo lại khi contract đã ổn để Module 1 đổi từ stub sang real service

## Ghi chú phối hợp

- Nếu cần chỉnh DTO hoặc logic tính slot, vui lòng báo trước để thống nhất.
- Nếu có xung đột với cách Module 1 đang tính giữ chỗ, cần chốt cùng nhau trước khi merge.
