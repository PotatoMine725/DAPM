# Báo cáo rà soát logic runtime đặt / hủy / đổi lịch

Ngày lập: 2026-04-29  
Mục tiêu: rà lại runtime logic để đảm bảo đúng nghiệp vụ khi đặt, hủy và đổi lịch sau khi tích hợp scheduling thật từ Module 2.

## 1) Kết luận tổng quan

Sau khi rà soát các handler runtime chính, flow đã đi đúng hướng nghiệp vụ:

- đặt lịch chỉ thực hiện khi ca đã duyệt và còn slot
- hủy lịch chỉ cho đúng vai trò / đúng chủ sở hữu
- đổi lịch cần bảo toàn lịch cũ nếu ca mới không đặt được
- slot được cập nhật qua `ICaLamViecQueryService`, không write trực tiếp từ Module 1

## 2) Các điểm đã kiểm tra

### 2.1 `TaoLichHenHandler`

Đã xác nhận flow runtime hiện tại:

- xác định vai trò người dùng trước
- kiểm tra bệnh nhân có bị hạn chế đặt lịch hay không
- kiểm tra dịch vụ tồn tại
- kiểm tra ca làm việc tồn tại
- chỉ cho phép đặt khi ca ở trạng thái `DaDuyet`
- loại ca nếu đã qua thời điểm hiện tại
- kiểm tra slot trống có tính cả giữ chỗ còn hiệu lực
- tăng slot bằng `IncrementSoSlotDaDatAsync`
- nếu save fail thì rollback slot bằng `-1`

=> Flow này phù hợp với nghiệp vụ đặt lịch an toàn hơn trước.

### 2.2 `DoiLichHenHandler`

Đã kiểm tra logic đổi lịch:

- chỉ vai trò phù hợp mới được đổi lịch
- bệnh nhân chỉ đổi lịch của chính mình
- lịch cũ không được đổi nếu đã ở trạng thái đóng
- ca mới phải khác ca hiện tại
- ca mới phải tồn tại và đã duyệt
- ca mới phải còn slot
- slot ca mới được giữ trước khi hủy ca cũ
- nếu save fail thì rollback slot của ca mới
- sau save thành công mới trừ slot của ca cũ ở bước best-effort

=> Logic này ưu tiên an toàn cho bệnh nhân: nếu ca mới không hợp lệ thì lịch cũ không bị mất.

### 2.3 `TaoGiuChoHandler`

Đã xác nhận:

- chỉ `LeTan` / `Admin` được tạo giữ chỗ
- bệnh nhân bị hạn chế thì không cho tạo giữ chỗ
- chỉ cho giữ chỗ ở ca đã duyệt và còn slot
- slot được tăng thông qua service scheduling
- save fail thì rollback slot của ca

=> Phù hợp với rule giữ chỗ còn hiệu lực đã chốt.

### 2.4 `HuyLichHenHandler`

Đã xác nhận:

- bệnh nhân chỉ hủy lịch của chính mình
- lễ tân/admin có quyền hủy thay
- bác sĩ không có quyền hủy lịch
- lịch ở trạng thái đã hủy / đã hoàn thành / không đến thì không hủy lại
- khi hủy muộn thì tăng `SoLanHuyMuon` theo patient
- slot được trả về bằng `IncrementSoSlotDaDatAsync(-1)` theo kiểu best-effort

=> Phù hợp với luồng hủy lịch thực tế.

## 3) Điểm chú ý runtime

### 3.1 Best-effort trả slot

Hiện tại logic trả slot ở hủy / đổi lịch vẫn dùng cơ chế best-effort sau khi save, để tránh phá flow chính nếu service scheduling tạm thời lỗi.

Nếu cần chặt hơn nữa, có thể cân nhắc transaction liên kết nhiều thao tác, nhưng hiện tại phương án này giữ được tính ổn định của flow booking.

### 3.2 Business rule đang được thắt chặt ở service scheduling

`CaLamViecQueryService` hiện đã:

- coi chỉ ca `DaDuyet` là bookable
- slot trống phải tính cả giữ chỗ còn hiệu lực
- dùng atomic update để giảm race condition
- có recon job để đối soát khi lệch số liệu

## 4) Hướng đi tiếp theo đề xuất

- chạy `dotnet build` để xác nhận không còn lỗi compile sau khi tích hợp
- test từng case:
  - đặt lịch khi còn slot
  - đặt lịch khi hết slot
  - hủy lịch bệnh nhân
  - hủy lịch bởi lễ tân/admin
  - đổi lịch sang ca mới còn slot / hết slot
- nếu có issue nghiệp vụ phát sinh, chỉ tinh chỉnh handler liên quan thay vì mở rộng contract vội

## 5) Kết luận

Runtime logic hiện tại đã bám đúng nghiệp vụ hơn trước và phù hợp để tiếp tục test end-to-end với Module 2. Bước sau nên là kiểm tra build và chạy các test case booking thực tế để xác nhận các nhánh đặt / hủy / đổi lịch hoạt động đúng theo dữ liệu thật.
