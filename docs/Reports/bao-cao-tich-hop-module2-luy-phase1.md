# Báo cáo tích hợp Module 2 từ nhánh `Luy` — phase 1

Ngày lập: 2026-04-29  
Mục tiêu: bắt đầu tích hợp code mới của Module 2 từ nhánh remote `origin/Luy` để Module 1 dùng scheduling thật.

## 1) Tình trạng trước khi tích hợp

Trước khi tích hợp, Module 1 đang dùng stub cho scheduling service trong Infrastructure. Điều này khiến booking flow chỉ chạy tạm và không phản ánh dữ liệu thật từ Module 2.

## 2) Những gì đã xác nhận từ nhánh `origin/Luy`

Nhánh `origin/Luy` đã có implementation thật cho scheduling:

- `ICaLamViecQueryService`
- `CaLamViecQueryService`
- DTO scheduling
- enum lý do không đặt được
- test liên quan cho scheduling / booking flow

## 3) Những thay đổi đã áp dụng ở Module 1

### 3.1 Chuyển DI sang service thật

Trong `ClinicBooking.Infrastructure/DependencyInjection.cs`:

- đổi `ICaLamViecQueryService` từ stub sang implementation thật `CaLamViecQueryService`

### 3.2 Đồng bộ contract scheduling trong Application

Đã ghi lại các contract scheduling ở local theo đúng shape đang dùng:

- `ICaLamViecQueryService`
- `ThongTinCaLamViecDto`
- `KetQuaKiemTraSlotDto`
- `LyDoKhongDatDuoc`

### 3.3 Rà lại xử lý đặt lịch

Trong `TaoLichHenHandler`:

- giữ flow kiểm tra slot và tăng slot qua service scheduling
- cập nhật thông báo lỗi khi xung đột đồng thời hoặc hết slot
- giữ nguyên logic chính của booking flow để không phá portal bệnh nhân

## 4) Kế hoạch tiếp theo

### Phase 2: kiểm tra đồng bộ logic booking

Cần rà tiếp các handler sau:

- `TaoGiuCho`
- `DoiLichHen`
- `HuyLichHen`

Mục tiêu:

- đảm bảo đều dùng `ICaLamViecQueryService` đúng cách
- không còn viết trực tiếp vào slot của ca làm việc
- không bị race condition khi tăng / giảm slot

### Phase 3: kiểm tra build và test

Cần chạy:

- `dotnet build`
- test unit liên quan scheduling / booking

Nếu phát sinh lỗi contract hoặc lỗi compile thì sửa trước khi đi tiếp.

### Phase 4: tiếp tục đồng bộ portal bệnh nhân

Sau khi scheduling ổn định, tiếp tục rà:

- `DatLich`
- `DanhSachLichHen`
- `LichHen`
- `ChuyenKhoa`
- `ThongBao`

## 5) Ghi chú nghiệp vụ đã chốt

Theo phản hồi trước đó của team:

- nguồn slot là **hybrid**
- slot trống phải tính cả **giữ chỗ còn hiệu lực**
- chỉ ca ở trạng thái **Approved** được đặt
- chống overbook bằng **optimistic concurrency**
- enum `LyDoKhongDatDuoc` giữ nguyên và chỉ mở rộng nếu cần
- ưu tiên hoàn tất **booking** trước

## 6) Kết luận

Phase 1 đã bắt đầu tích hợp service scheduling thật từ nhánh `origin/Luy`. Bước tiếp theo là kiểm tra kỹ các luồng booking còn lại và chạy build/test để xác nhận Module 1 đã dùng được logic mới mà không vỡ portal.
