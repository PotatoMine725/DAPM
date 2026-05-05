# Báo cáo xung đột tiềm tàng và kế hoạch đồng bộ Module 1 với `origin/develop`

Ngày lập: 2026-05-04  
Phạm vi: so sánh nhánh hiện tại `feature/module1/portal-sat-demo` với `origin/develop`, tập trung vào phần Module 1 và các file tài liệu liên quan.

## 1) Kết luận nhanh

Sau khi so sánh với `origin/develop`, nhánh hiện tại đang:

- **ahead 9 commit**
- **behind 4 commit**
- có **23 file khác nhau** giữa `origin/develop...HEAD`
- trong đó phần lớn là **test, seeder, integration tests và tài liệu**

Không thấy dấu hiệu xung đột trực tiếp ngay trên file-level từ kết quả diff hiện tại, nhưng có **một số vùng dễ phát sinh conflict logic** nếu sync/rebase lên `develop` mới nhất:

1. `ClinicBooking.Api/Program.cs`
2. `ClinicBooking.Infrastructure/Persistence/DatabaseSeeder.cs`
3. `ClinicBooking.Infrastructure/ClinicBooking.Infrastructure.csproj`
4. `ClinicBooking.Application/Features/Doctors/Queries/DanhSachBacSiCongKhai/DanhSachBacSiCongKhaiHandler.cs`
5. `DatLichPhongKham.slnx`
6. Các file test mới trong `ClinicBooking.Application.UnitTests` và `ClinicBooking.Integration.Tests`
7. Các file tài liệu Module 1 trong `docs/Plans` và `docs/Reports`

## 2) Context dự án đã đọc nhanh

### File Module 1 quan trọng đã đọc kỹ

- `docs/Plans/ke-hoach-hoan-thien-module1-dat-huy-doi-lich.md`
- `docs/Plans/tien-do-module1.md`
- `docs/Reports/bao-cao-context-module1-phan-co-the-lam-ngay.md`

### Nhận định từ context

- Module 1 hiện đã đi khá xa: có unit test, integration test, background job test, và smoke-flow planning.
- Có chủ đích giữ lại một phần code cũ/cấu hình cũ để hoàn thiện chức năng end-to-end.
- Nhiều thay đổi Module 1 là **phụ thuộc chéo** với Module 2/3/4, nhưng tài liệu ghi rõ phần nào đã xử lý và phần nào còn defer.

## 3) Những điểm có thể tạo xung đột khi đồng bộ với `develop`

### 3.1 `ClinicBooking.Api/Program.cs`

Đây là file dễ conflict nhất vì thường chứa:

- đăng ký DI
- middleware pipeline
- hosted services / background jobs
- cấu hình auth, seeding, feature flags

Nếu `develop` mới nhất đã sửa pipeline hoặc thêm services khác, phần thay đổi Module 1 có thể bị đè hoặc trùng thứ tự đăng ký.

**Rủi ro cần giữ lại:**
- đăng ký service cho background jobs
- đăng ký seeder/dev fixture
- các cấu hình phục vụ integration test hoặc smoke test

### 3.2 `DatabaseSeeder.cs`

File này chứa logic quan trọng để:

- seed account demo
- refresh `CaLamViec` dates
- giữ dữ liệu test/dev hợp lệ theo thời gian thực

Nếu `develop` cập nhật seed data hoặc thêm logic khởi động khác, cần merge cẩn thận vì đây là nơi dễ làm hỏng luồng test hiện tại.

**Phần cần giữ:**
- logic refresh ngày cho `CaLamViec`
- mật khẩu demo / upsert fixture accounts
- các guard chỉ chạy ở Development

### 3.3 `.csproj` và `slnx`

Các project file và solution file dễ conflict khi:

- thêm project test mới
- cập nhật reference package
- thay đổi target framework
- sửa order/format của solution

Nếu `develop` đã thêm project/module khác, phải tránh làm rơi entry mới khi resolve conflict.

### 3.4 Test projects

Các file test mới có thể conflict ở mức logic nếu `develop` đã:

- đổi namespace
- đổi helper/test fixture
- đổi seed model
- sửa endpoint contract

Đặc biệt cần xem lại:
- `ClinicBooking.Integration.Tests/ClinicBookingApiFactory.cs`
- `ClinicBooking.Application.UnitTests/Common/TestDbContextFactory.cs`
- các test cho `DoiLichHen`, `TaoLichHen`, `HuyLichHen`

### 3.5 Docs Module 1

Các file docs đang mô tả trạng thái rất cụ thể của Module 1. Nếu sync code mà không sync docs, dễ sinh mismatch giữa thực tế và tài liệu.

## 4) Kế hoạch đồng bộ an toàn

### Mục tiêu

- Đồng bộ nhánh với `origin/develop` mới nhất
- Giữ lại các thay đổi Module 1 đã chứng minh là cần thiết
- Không làm mất các fix cũ đang phục vụ flow hoàn thiện chức năng

### Phương án đề xuất

#### Bước 1 — Tạo checkpoint local

- giữ nguyên working tree hiện tại
- nếu cần, tạo nhánh backup hoặc stash riêng cho 2 file local đang modified:
  - `ClinicBooking.Web/appsettings.Development.json`
  - `docs/Plans/ke-hoach-hoan-thien-module1-dat-huy-doi-lich.md`

#### Bước 2 — Lấy state mới nhất của remote

- `git fetch origin --prune`
- xem lại diff của `origin/develop...HEAD`
- nếu cần, xem tiếp diff chi tiết theo file quan trọng

#### Bước 3 — Đồng bộ theo hướng merge/rebase có kiểm soát

Ưu tiên:

- **rebase** nếu muốn lịch sử sạch và nhánh riêng chưa share thêm commit mới
- **merge** nếu muốn an toàn hơn và giữ nguyên lịch sử hiện tại

Với nhánh này, vì đang có nhiều thay đổi test/docs và có phần phụ thuộc chéo, cách an toàn hơn là:

1. đồng bộ `develop` mới nhất vào nhánh
2. resolve theo từng file trọng yếu
3. chạy test lại ngay sau khi resolve

#### Bước 4 — Giữ lại phần code cũ cần thiết

Khi resolve conflict, ưu tiên giữ:

- logic seeder cho dev fixture
- refresh date cho `CaLamViec`
- test fixture helper cho integration tests
- các endpoint/handler hỗ trợ Module 1 core flow
- nội dung docs phản ánh đúng trạng thái đã hoàn thành

Không nên xóa vội:

- custom test setup
- integration test scaffolding
- background job tests
- docs tiến độ Module 1

#### Bước 5 — Xác minh sau đồng bộ

Nên chạy lại:

- `dotnet test ClinicBooking.Application.UnitTests`
- `dotnet test ClinicBooking.Integration.Tests`
- build solution

Nếu có smoke UI, chạy thêm luồng đặt/hủy/đổi lịch để chắc chắn flow không bị regress.

## 5) Gợi ý thứ tự xử lý conflict nếu phát sinh

1. `Program.cs`
2. `DatabaseSeeder.cs`
3. `.csproj` / `slnx`
4. test fixtures / integration tests
5. docs `Module 1`

Lý do: các file nền tảng phải chốt trước, rồi mới xử lý test và docs.

## 6) Kết luận hành động

Hiện tại chưa thấy conflict trực tiếp, nhưng có **rủi ro conflict logic cao** ở các file nền tảng và test fixtures. Kế hoạch an toàn là:

- fetch remote mới nhất
- đồng bộ `develop` vào nhánh hiện tại bằng merge/rebase có kiểm soát
- giữ nguyên các fix Module 1 đã chứng minh cần thiết
- chạy test lại ngay sau khi resolve
- cập nhật docs nếu code thay đổi làm lệch mô tả hiện tại

## 7) Ghi chú

Các file Module 1 đã đọc kỹ cho thấy repo đang ở trạng thái “code xong một phần lớn, docs đang theo sát”. Vì vậy khi sync, **đừng ưu tiên làm sạch lịch sử bằng mọi giá** nếu điều đó làm mất các fix test/dev-fixture đã chứng minh là cần thiết.
