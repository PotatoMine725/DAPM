# Báo cáo khảo sát `origin/Luy` và kế hoạch triển khai tiếp theo

Ngày lập: 2026-04-29  
Mục tiêu: kiểm tra thay đổi mới từ Module 2 trên nhánh `origin/Luy`, xác định mức độ sẵn sàng tích hợp cho Module 1, và chốt hướng triển khai tiếp theo.

## 1) Kết quả khảo sát

Đã fetch và kiểm tra nhánh `origin/Luy`. Nhánh này đã thêm implementation thật cho phần scheduling quan trọng, bao gồm:

- `ClinicBooking.Infrastructure/Services/Scheduling/CaLamViecQueryService.cs`
- `ClinicBooking.Application/Abstractions/Scheduling/ICaLamViecQueryService.cs`
- các DTO scheduling đi kèm
- test coverage cho scheduling service và luồng booking liên quan

Điều này xác nhận Module 2 đã tiến thêm đáng kể để hỗ trợ Module 1 booking end-to-end.

## 2) Điểm đáng chú ý từ `origin/Luy`

### 2.1 Contract scheduling đã có hình hài rõ ràng

Những phần sau đã được định nghĩa rõ:

- `LayThongTinCaAsync`
- `KiemTraSlotTrongAsync`
- `LaCaDuocDuyetAsync`
- `IncrementSoSlotDaDatAsync`
- `ChayReconSlotAsync`

DTO/enum:

- `ThongTinCaLamViecDto`
- `KetQuaKiemTraSlotDto`
- `LyDoKhongDatDuoc`

### 2.2 Nghiệp vụ slot đã được chốt theo hướng khá sát thực tế

Từ code remote có thể thấy rule đang đi theo hướng:

- `SoSlotDaDat` có thể đối soát lại từ lịch hẹn và giữ chỗ
- slot trống tính cả giữ chỗ còn hiệu lực
- chỉ ca `DaDuyet` mới được đặt
- chống race condition bằng update có điều kiện

### 2.3 Module 1 cần tích hợp lại theo contract mới

Vì Module 1 trước đó còn dùng stub trong infrastructure, sau khi Module 2 đã có service thật thì cần:

- kiểm tra lại wiring DI
- đảm bảo handler booking gọi đúng service thật
- kiểm tra lại các luồng đặt / giữ chỗ / đổi lịch / hủy lịch

## 3) Kế hoạch triển khai tiếp theo cho Module 1

### Bước 1. Đồng bộ contract với nhánh `origin/Luy`

- so sánh lại chữ ký interface và DTO scheduling đang có ở local
- xác định file nào cần merge hoặc thay thế
- giữ nguyên naming/shape nếu đã tương thích

### Bước 2. Chuyển Module 1 sang dùng service thật

- bỏ stub scheduling ở DI nếu local vẫn còn dùng stub
- đảm bảo `ICaLamViecQueryService` resolve vào implementation thật từ Module 2
- kiểm tra không còn code nào trong booking flow write trực tiếp vào slot ngoài interface

### Bước 3. Rà lại luồng booking trọng yếu

Ưu tiên theo yêu cầu hiện tại:

1. Đặt lịch
2. Giữ chỗ
3. Đổi lịch
4. Hủy lịch

### Bước 4. Kiểm tra portal bệnh nhân

- trang `Đặt lịch`
- trang `Chuyên khoa`
- trang `Lịch hẹn`
- sidebar portal bệnh nhân

### Bước 5. Ghi nhận kết quả kiểm tra vào docs

Sau mỗi bước quan trọng cần có báo cáo ngắn để:

- tránh mất context
- biết đã tích hợp tới đâu
- dễ bàn giao cho chủ module hoặc mở PR sau này

## 4) Rủi ro cần lưu ý

- Nếu local chưa merge các thay đổi từ `origin/Luy`, có thể đang lệch contract scheduling.
- Nếu interface/DTO ở local khác remote, cần chốt phiên bản nào là chuẩn trước khi tiếp tục sửa Module 1.
- Nếu booking handler vẫn còn assumption cũ về slot hoặc trạng thái ca, cần refactor để bám theo service mới.

## 5) Kết luận

Nhánh `origin/Luy` đã mang đến phần scheduling thật và khá sẵn sàng để Module 1 tích hợp tiếp. Bước tiếp theo nên là đồng bộ contract và kiểm tra lại luồng đặt lịch / giữ chỗ / đổi lịch dựa trên service thật.
