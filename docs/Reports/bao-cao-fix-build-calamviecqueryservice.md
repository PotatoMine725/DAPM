# Báo cáo fix build `ICaLamViecQueryService`

Ngày lập: 2026-04-29  
Nguyên nhân: build fail sau khi tích hợp scheduling thật từ Module 2.

## 1) Lỗi build

Terminal báo lỗi:

- `CaLamViecQueryServiceStub` không implement đầy đủ interface `ICaLamViecQueryService`
- thiếu method:
  - `ChayReconSlotAsync(CancellationToken)`

## 2) Nguyên nhân

Trong quá trình chốt contract scheduling từ Module 2, interface `ICaLamViecQueryService` đã có thêm method:

- `ChayReconSlotAsync`

Nhưng stub cũ của Infrastructure vẫn chưa được cập nhật, nên build fail ở project `ClinicBooking.Infrastructure`.

## 3) Đã sửa

### 3.1 Bổ sung implementation tạm cho stub

Trong `ClinicBooking.Infrastructure/Services/Scheduling/CaLamViecQueryServiceStub.cs`:

- thêm method `ChayReconSlotAsync(CancellationToken)` trả về `Task.FromResult(0)`
- mục tiêu chỉ để build pass trong trường hợp stub vẫn còn được dùng ở một số nhánh / trạng thái tạm thời

### 3.2 Đồng bộ contract runtime

Đã giữ nguyên các file contract scheduling đã chốt:

- `ICaLamViecQueryService`
- `ThongTinCaLamViecDto`
- `KetQuaKiemTraSlotDto`
- `LyDoKhongDatDuoc`

### 3.3 Rà lại service thật

`CaLamViecQueryService` trong Infrastructure vẫn là implementation thật đang dùng cho runtime.

## 4) Kết quả

- build lỗi do thiếu method interface đã được xử lý
- contract scheduling không còn mismatch ở phần stub
- có thể tiếp tục kiểm tra build/test các handler booking

## 5) Ghi chú

Dù stub đã được bổ sung method để không làm gãy build, hệ thống hiện tại đang ưu tiên dùng `CaLamViecQueryService` thật trong DI. Stub chỉ còn mang tính dự phòng / tương thích.
