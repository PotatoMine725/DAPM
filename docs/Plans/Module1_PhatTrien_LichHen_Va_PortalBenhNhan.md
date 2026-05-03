# Kế hoạch triển khai Module 1 — Luồng đặt/hủy/đổi lịch và portal bệnh nhân

Ngày lập: 2026-04-29  
Phạm vi: `ClinicBooking` Module 1  
Mục tiêu: hoàn thiện luồng đặt/hủy/đổi lịch, đồng thời áp dụng lại giao diện portal bệnh nhân theo mẫu HTML mới `docs/DAPM_UI/benhnhan_portal_new.html`.

## 1) Bối cảnh và nguồn tham chiếu

Tài liệu này được tổng hợp từ các file kế hoạch và tiến độ hiện có:

- `docs/Plans/Module_1_plan.md`
- `docs/Plans/ke-hoach-module1-giai-doan-tiep-theo.md`
- `docs/Plans/tien-do-module1.md`
- `docs/Specs/MO_TA_GIAO_DIEN.md`
- `docs/Others/design-system.md`
- `docs/DAPM_UI/benhnhan_portal_new.html`

Hiện tại Module 1 đã hoàn thành phần lớn nền tảng backend và đã có một phần UI bệnh nhân. Phần còn lại cần tập trung vào:

1. Hoàn thiện đúng các luồng nghiệp vụ đặt lịch.
2. Chuẩn hóa UX/UI portal bệnh nhân theo mẫu mới.
3. Tích hợp màn đăng nhập mới cho bệnh nhân ngay trong portal.

## 2) Mục tiêu triển khai

### Mục tiêu backend

- Hoàn thiện các command/query cốt lõi của lịch hẹn.
- Đảm bảo có validator, handler, mapping, controller và test cho từng luồng.
- Giữ đúng các quy tắc nghiệp vụ đã chốt trong plan Module 1.

### Mục tiêu UI

- Chuyển giao diện portal bệnh nhân sang layout mới theo HTML mẫu.
- Có màn đăng nhập bệnh nhân riêng, đồng bộ với portal.
- Giữ trải nghiệm thống nhất với design system trong `docs/Others/design-system.md`.

## 3) Phạm vi nghiệp vụ cần hoàn thiện

### 3.1 Luồng đặt lịch

Cần hoàn thiện/kiểm tra đầy đủ các bước:

- Chọn chuyên khoa / dịch vụ / bác sĩ / ca khám.
- Kiểm tra slot còn trống.
- Tạo lịch hẹn.
- Sinh mã lịch hẹn.
- Ghi lịch sử thao tác.
- Tạo thông báo liên quan.

Các thao tác liên quan:

- `TaoLichHen`
- `CheckInLichHen` nếu nằm trong flow vận hành sau khi đặt
- `XacNhanLichHen` nếu cần lễ tân duyệt

### 3.2 Luồng hủy lịch

- Hủy lịch theo đúng vai trò được phép.
- Phân biệt hủy bởi bệnh nhân, lễ tân hoặc admin.
- Kiểm tra điều kiện hủy muộn nếu có.
- Cập nhật trạng thái lịch hẹn và lịch sử.
- Gửi thông báo tương ứng.

### 3.3 Luồng đổi lịch

- Chọn lịch cũ cần đổi.
- Kiểm tra lịch cũ còn hợp lệ.
- Giữ nguyên audit trail của lịch cũ.
- Tạo lịch mới liên kết với lịch cũ.
- Tránh double-book và tránh đổi sang slot không hợp lệ.

### 3.4 Luồng giữ chỗ và hàng chờ liên quan

- `TaoGiuCho`
- `GiaiPhongGiuCho`
- `XemHangChoTheoCa`
- `HoanThanhLuotKham`
- `GoiBenhNhanKeTiep`

Các luồng này không phải trọng tâm của request hiện tại nhưng cần được giữ đồng bộ để portal bệnh nhân hiển thị đúng trạng thái lịch hẹn.

## 4) Kế hoạch backend còn lại

### Giai đoạn A — rà soát và khóa phạm vi

1. Rà soát lại toàn bộ luồng lịch hẹn hiện có trong Application.
2. Đối chiếu với plan và tiến độ để xác nhận phần nào đã xong, phần nào cần bổ sung.
3. Chốt các rule giao tiếp giữa backend và portal UI.

### Giai đoạn B — hoàn thiện luồng đặt/hủy/đổi lịch

#### B1. Đặt lịch

- Kiểm tra validator cho request tạo lịch.
- Kiểm tra mapping request/response.
- Đảm bảo handler bám đúng rule nghiệp vụ.
- Bổ sung test cho:
  - tạo lịch thành công
  - ca không tồn tại
  - slot hết chỗ
  - lịch không hợp lệ
  - lỗi conflict khi trùng slot

#### B2. Hủy lịch

- Bổ sung các nhánh lỗi cần thiết.
- Kiểm tra quyền theo vai trò.
- Đảm bảo trạng thái và lịch sử thay đổi đúng.
- Bổ sung test cho:
  - hủy thành công
  - lịch không tồn tại
  - không được hủy vì trạng thái không hợp lệ
  - hủy muộn nếu rule áp dụng

#### B3. Đổi lịch

- Giữ nguyên lịch sử lịch cũ.
- Tạo lịch mới với kiểm tra slot trước khi commit.
- Tránh làm mất dấu vết của lịch cũ.
- Bổ sung test cho:
  - đổi lịch thành công
  - lịch cũ không tồn tại
  - ca mới không hợp lệ
  - slot mới đã hết
  - conflict dữ liệu khi thao tác song song

### Giai đoạn C — đồng bộ API và contract

- Rà soát controller `ClinicBooking.Api/Controllers/LichHenController.cs`.
- Rà soát contract trong `ClinicBooking.Api/Contracts/LichHen/`.
- Bổ sung hoặc chỉnh sửa response DTO nếu UI cần trạng thái mới.
- Đảm bảo message lỗi tiếng Việt rõ ràng, thống nhất với exception handler hiện tại.

### Giai đoạn D — test và smoke check

- Unit test cho validator.
- Unit test cho handler.
- Smoke test qua Swagger hoặc API client.
- Nếu cần, bổ sung test để xác nhận không double-book.

## 5) Kế hoạch áp dụng giao diện portal bệnh nhân mới

### 5.1 Mục tiêu UI

Áp dụng lại portal bệnh nhân theo file mẫu:

- `docs/DAPM_UI/benhnhan_portal_new.html`

Mẫu này đã bao gồm:

- màn đăng nhập và đăng ký trong cùng shell
- dashboard bệnh nhân
- danh sách chuyên khoa
- màn đặt lịch
- danh sách lịch hẹn
- thông báo
- hồ sơ cá nhân
- menu điều hướng dạng sidebar
- toast / modal / card / table / status badge

### 5.2 Phạm vi cập nhật UI

#### A. Màn đăng nhập mới cho bệnh nhân

- Giữ màn đăng nhập trong portal, không tách sang giao diện cũ.
- Cho phép bệnh nhân đăng nhập rồi đi thẳng vào shell hệ thống.
- Có thể giữ tab đăng ký nếu luồng sản phẩm vẫn cần.
- Đảm bảo trạng thái mặc định và hành vi đăng nhập rõ ràng.

#### B. Portal bệnh nhân

Cần map lại các khu vực chính của mẫu HTML sang ứng dụng thật:

- Dashboard tổng quan
- Chuyên khoa / bác sĩ
- Đặt lịch
- Lịch hẹn
- Thông báo
- Hồ sơ cá nhân

#### C. Điều hướng và layout

- Sidebar trái cho portal.
- Topbar hiển thị tên bệnh nhân và thông tin đăng nhập.
- Content area đổi theo view.
- Responsive tốt trên màn hình nhỏ.

### 5.3 Chia nhỏ việc áp dụng UI

#### B1. Khung giao diện và layout chung

- Tách layout chính thành các vùng rõ ràng.
- Chuẩn hóa màu sắc, spacing, border radius, shadow theo design system.
- Đồng bộ style cho buttons, tabs, badges, cards, tables, modal, toast.

#### B2. Màn đăng nhập bệnh nhân

- Thiết kế lại theo mẫu mới.
- Giữ trải nghiệm đơn giản, ít bước.
- Sau khi đăng nhập thành công thì chuyển sang portal.
- Nếu cần đăng ký thì hiển thị như một tab phụ trong cùng shell.

#### B3. Dashboard bệnh nhân

- Hiển thị lịch hẹn sắp tới.
- Hiển thị số liệu tóm tắt.
- Có hành động nhanh như đặt lịch, xem lịch, xem thông báo.

#### B4. Màn lịch hẹn

- Hiển thị danh sách lịch hẹn theo trạng thái.
- Có phân trang nếu cần.
- Có action đổi lịch/hủy lịch theo rule backend.
- Có chi tiết lịch hẹn.

#### B5. Màn đặt lịch

- Cho phép chọn chuyên khoa, bác sĩ, ngày, giờ.
- Hiển thị ca trống theo dữ liệu backend.
- Có trạng thái disable cho slot đã hết.
- Đồng bộ với rule kiểm tra slot.

#### B6. Màn thông báo và hồ sơ

- Thông báo dạng danh sách, badge unread.
- Hồ sơ cá nhân cho phép cập nhật các trường cơ bản.
- Giữ giao diện nhất quán với mẫu mới.

### 5.4 Mapping từ HTML mẫu sang code thật

File HTML mẫu đang chứa đủ ý tưởng UI, nhưng trước khi áp dụng cần tách thành các phần thực thi:

- component/layout
- data binding
- event handlers
- state quản lý đăng nhập và portal
- call API thật thay cho dữ liệu giả

Nếu tiếp tục theo cách hiện tại của project, có thể triển khai theo từng trang Razor Page hoặc theo các component/partial dùng chung.

### 5.5 Yêu cầu đăng nhập mới cho bệnh nhân

Màn đăng nhập mới cần đáp ứng:

- cho bệnh nhân đăng nhập bằng thông tin hợp lệ theo hệ thống
- thông báo lỗi rõ ràng nếu sai tài khoản hoặc mật khẩu
- sau đăng nhập phải điều hướng đúng portal bệnh nhân
- nếu hết phiên thì quay lại màn đăng nhập

Nếu hệ thống còn đang dùng JWT / cookie / session theo tầng hiện tại thì cần chốt sớm cách render trạng thái login để tránh lệch UX.

## 6) Thứ tự ưu tiên triển khai

### Ưu tiên 1 — backend đặt/hủy/đổi lịch

- Hoàn thiện command/validator/handler cho 3 luồng chính.
- Bổ sung test lỗi và happy path.
- Đảm bảo không phá quy tắc nghiệp vụ hiện có.

### Ưu tiên 2 — portal bệnh nhân mới

- Áp dụng layout và navigation từ HTML mẫu.
- Tích hợp màn đăng nhập mới.
- Kết nối các view chính của portal.

### Ưu tiên 3 — đồng bộ API và polish

- Rà soát message lỗi, response DTO và trạng thái UI.
- Tinh chỉnh responsive.
- Dọn các điểm lệch giữa mock UI và backend thật.

## 7) Deliverables mong muốn

- Bộ luồng backend hoàn chỉnh cho đặt/hủy/đổi lịch.
- Portal bệnh nhân theo giao diện mới.
- Màn đăng nhập bệnh nhân tích hợp trong portal.
- Test cho các luồng chính.
- Tài liệu chốt phạm vi trong `docs/Plans` và nếu cần thì bổ sung `docs/Specs`.

## 8) Ghi chú triển khai

- Không phá cấu trúc module hiện có.
- Không làm lẫn giữa portal bệnh nhân và giao diện nội bộ lễ tân/bác sĩ.
- Ưu tiên dùng lại design system và style helper hiện có.
- Các thay đổi UI nên được tách theo bước nhỏ để dễ review và rollback.
