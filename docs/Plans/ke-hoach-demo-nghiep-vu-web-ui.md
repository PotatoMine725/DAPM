# Kế hoạch Demo Nghiệp vụ trên Web UI

> Ngày lập: 2026-05-05  
> Branch: `feature/module1/portal-sat-demo`  
> Trạng thái: **Phase B đang thực hiện — page DoiLich đã có, chuẩn bị smoke test**

---

## 1. Tổng kết những gì đã làm

### 1.1 Backend — Application Layer

| Chức năng | Command/Handler | Trạng thái |
|---|---|---|
| Đặt lịch hẹn | `TaoLichHenCommand` + Handler | ✅ Done |
| Hủy lịch hẹn | `HuyLichHenCommand` + Handler | ✅ Done |
| Đổi lịch hẹn | `DoiLichHenCommand` + Handler | ✅ Done |
| Xác nhận lịch (lễ tân) | `XacNhanLichHenCommand` + Handler | ✅ Done |
| Check-in (lễ tân) | `CheckInCommand` + Handler | ✅ Done |
| Gọi bệnh nhân kế tiếp (bác sĩ) | `GoiKeTiepCommand` + Handler | ✅ Done |
| Hoàn thành khám (bác sĩ) | `HoanThanhKhamCommand` + Handler | ✅ Done |
| Lấy số thứ tự hàng chờ | `ThuTuCuaToiQuery` + Handler | ✅ Done |
| Background: hết hạn giữ chỗ | `QuetGiuChoHetHanJob` | ✅ Done |
| Background: chuyển lịch quá hạn | `ChuyenLichHenDaQuaHanJob` | ✅ Done |

### 1.2 Infrastructure

| Hạng mục | Trạng thái | Ghi chú |
|---|---|---|
| `DatabaseSeeder` — seed tài khoản demo | ✅ Done | patient001, doctor001, receptionist001, admin001 |
| `DatabaseSeeder.RefreshCaLamViecDatesAsync` | ✅ Done | Tự refresh CaLamViec mỗi startup, không bao giờ hết hạn |
| Seed CaLamViec ID 3001–3003 | ✅ Done | Ngày future = today+30 (3001/3002), today+37 (3003) |
| OTP Service stub | ✅ Done | `NotificationServiceStub` — log only, không block flow |

### 1.3 Tests

| Phase | Files | Kết quả |
|---|---|---|
| Unit tests — ThuTuCuaToiHandler | `ThuTuCuaToiHandlerTests.cs` | ✅ 3/3 pass |
| Unit tests — DoiLichHenHandler | `DoiLichHenHandlerTests.cs` | ✅ 6/6 pass |
| Unit tests — TaoLichHenHandler | `TaoLichHenHandlerTests.cs` | ✅ pass |
| Unit tests — BackgroundJobs | `QuetGiuChoHetHanJobTests.cs`, `ChuyenLichHenDaQuaHanJobTests.cs` | ✅ 4/4 pass |
| Integration tests | `TaoLichHenIntegrationTests`, `HuyLichHenIntegrationTests`, `DoiLichHenIntegrationTests` | ✅ 11/11 pass |
| **Tổng** | | **241 unit + 11 integration = 252 tests pass** |

### 1.4 Web UI — Razor Pages (`ClinicBooking.Web`)

| Page | Role | Chức năng | Trạng thái |
|---|---|---|---|
| `/Auth/DangNhap` | all | Đăng nhập cookie | ✅ Done |
| `/BenhNhan/DatLich` | benh_nhan | Chọn ngày, ca, dịch vụ → đặt lịch | ✅ Done |
| `/BenhNhan/DanhSachLichHen` | benh_nhan | Danh sách lịch, lọc trạng thái, hủy | ✅ Done |
| `/BenhNhan/LichHen` | benh_nhan | Chi tiết lịch hẹn, nút hủy | ✅ Done |
| `/BenhNhan/ThuTuHangCho` | benh_nhan | Xem số thứ tự hàng chờ | ✅ Done |
| `/LeTan/QuanLyLichHen` | le_tan, admin | Xác nhận, check-in, hủy lịch | ✅ Done |
| `/LeTan/HangCho` | le_tan | Quản lý hàng chờ | ✅ Done |
| `/BacSi/HangCho` | bac_si | Gọi kế tiếp, hoàn thành khám | ✅ Done |
| `/BenhNhan/DoiLich` | benh_nhan | Đổi sang ca/ngày khác | ✅ Done |

### 1.5 Merge & Branch state (2026-05-05)

- Đã merge `origin/develop` → không conflict
- Branch `feature/module1/portal-sat-demo` ahead origin 11 commits
- Chưa push lên remote

---

## 2. Kế hoạch tiếp theo — Demo nghiệp vụ trên Web UI

### 2.1 Việc cần làm trước khi demo

#### Task B1 — Hoàn tất page `/BenhNhan/DoiLich`

Page này đã được thêm vào Web UI và dùng để đổi sang ca/ngày khác.

**Hành vi hiện tại:**

```
GET  /BenhNhan/DoiLich?id={lichHenId}
  → Load LichHen hiện tại (kiểm tra ownership + trạng thái)
  → Load danh sách CaLamViec còn slot (ngày tương lai)
  → Render form chọn ca mới

POST /BenhNhan/DoiLich (handler: DoiLich)
  → Gọi DoiLichHenCommand { IdLichHen, IdCaLamViecMoi, LyDo }
  → Success → redirect /BenhNhan/LichHen?id={lichHenId}
  → Error → hiển thị lỗi inline
```

#### Task B2 — Chạy full smoke test thủ công

Xem chi tiết tại mục 3 (Kịch bản demo).

### 2.2 Thứ tự thực hiện

```
[B1] Tạo DoiLich page
  ↓
[B2] Build + chạy ClinicBooking.Web
  ↓
[B3] Smoke test 3 core flow theo kịch bản
  ↓
[B4] Commit + push + tạo PR → develop
```

---

## 3. Kịch bản demo nghiệp vụ

### Điều kiện tiên quyết

- App đang chạy: `dotnet run --project ClinicBooking.Web`
- Database đã seed (tự động khi start với `ASPNETCORE_ENVIRONMENT=Development`)
- CaLamViec ID 3001–3003 đã được refresh sang ngày tương lai

### Tài khoản demo

| Role | TenDangNhap | MatKhau |
|---|---|---|
| Bệnh nhân | `patient001` | `Demo@123456` |
| Lễ tân | `receptionist001` | `Demo@123456` |
| Bác sĩ | `doctor001` | `Demo@123456` |
| Admin | `admin` | `Admin@123456` |

---

### Flow 1 — Đặt lịch hẹn

```
1. Đăng nhập patient001
2. Vào /BenhNhan/DatLich
3. Chọn ngày (today+30), chọn CaLamViec (ID 3001 hoặc 3002)
4. Chọn chuyên khoa / dịch vụ → Submit
5. Kết quả mong đợi: redirect /BenhNhan/DanhSachLichHen, thấy lịch mới TrangThai=ChoDuyet
```

---

### Flow 2 — Xác nhận → Check-in → Hàng chờ → Hoàn thành

```
1. Đăng nhập receptionist001
2. Vào /LeTan/QuanLyLichHen
3. Tìm lịch vừa đặt → bấm "Xác nhận" → TrangThai=DaXacNhan
4. Bấm "Check-in" → TrangThai=DangCho, xuất hiện trong hàng chờ

5. [Tab mới] Đăng nhập patient001
6. Vào /BenhNhan/ThuTuHangCho → thấy số thứ tự

7. Đăng nhập doctor001
8. Vào /BacSi/HangCho → bấm "Gọi kế tiếp" → TrangThai=DangKham
9. Bấm "Hoàn thành" → TrangThai=HoanThanh
```

---

### Flow 3 — Đổi lịch hẹn

```
1. Đăng nhập patient001
2. Đặt lịch mới (Flow 1) — cần lịch ở trạng thái ChoDuyet
3. Vào /BenhNhan/DanhSachLichHen → bấm vào lịch vừa đặt
4. Trên trang chi tiết /BenhNhan/LichHen → bấm "Đổi lịch"
5. Vào /BenhNhan/DoiLich?id={id} → chọn ca mới (ID 3003, ngày today+37)
6. Submit
7. Kết quả mong đợi: redirect về LichHen, thấy CaLamViec đã đổi sang ca mới
```

---

### Flow 4 — Hủy lịch hẹn

```
1. Đăng nhập patient001
2. Đặt lịch mới (Flow 1)
3. Vào /BenhNhan/DanhSachLichHen → bấm "Hủy" trên lịch vừa đặt
4. Nhập lý do (tùy chọn) → Confirm
5. Kết quả mong đợi: TrangThai=DaHuy, SoLanHuyMuon không tăng (hủy >24h so với ngày khám)
```

---

### Checklist smoke test tổng hợp

```
[ ] Flow 1 — Đặt lịch: patient001 đặt được, TrangThai=ChoDuyet
[ ] Flow 2a — Xác nhận: receptionist001 xác nhận → DaXacNhan
[ ] Flow 2b — Check-in: receptionist001 check-in → DangCho
[ ] Flow 2c — Hàng chờ: patient001 thấy số thứ tự
[ ] Flow 2d — Bác sĩ gọi + hoàn thành → HoanThanh
[ ] Flow 3 — Đổi lịch: patient001 đổi sang ca khác → CaLamViec thay đổi
[ ] Flow 4 — Hủy lịch: patient001 hủy → DaHuy, SoLanHuyMuon không đổi
```

---

## 4. Hướng dẫn triển khai local

### 4.1 Yêu cầu

| Công cụ | Phiên bản tối thiểu |
|---|---|
| .NET SDK | 8.0 |
| SQL Server | 2019+ (hoặc LocalDB) |
| dotnet-ef CLI | `dotnet tool install -g dotnet-ef` |

### 4.2 Các bước khởi động lần đầu

```bash
# 1. Clone và build
git clone <repo>
cd DatLichPhongKham
dotnet build DatLichPhongKham.slnx

# 2. Cấu hình connection string
# Sửa file ClinicBooking.Web/appsettings.Development.json
# Thêm hoặc kiểm tra mục ConnectionStrings:DefaultConnection
# Ví dụ LocalDB:
#   "ConnectionStrings": {
#     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ClinicBooking;Trusted_Connection=True;"
#   }

# 3. Tạo database + apply migrations
dotnet ef database update \
  --project ClinicBooking.Infrastructure \
  --startup-project ClinicBooking.Web

# 4. Chạy Web app (tự động seed dữ liệu demo)
dotnet run --project ClinicBooking.Web

# 5. Mở trình duyệt
# http://localhost:5000 hoặc https://localhost:5001
```

### 4.3 Seed dữ liệu demo

Seed chạy **tự động** khi start nếu:
- `ASPNETCORE_ENVIRONMENT=Development`
- `Admin:DevFixture:Enabled=true` (đã có trong `appsettings.Development.json`)

Seed tạo:
- Tài khoản demo (patient001, doctor001, receptionist001, admin001)
- CaLamViec ID 3001–3003 với ngày tự động refresh sang tương lai
- Các dữ liệu master (BacSi, ChuyenKhoa, DichVu)

> Seed idempotent: chạy lại nhiều lần không reset dữ liệu đã có.

### 4.4 Environment variables thường dùng

```bash
# Windows PowerShell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ASPNETCORE_URLS = "http://localhost:5050"

# Hoặc qua launchSettings.json đã có sẵn
dotnet run --project ClinicBooking.Web --launch-profile Development
```

### 4.5 Chạy tests trước khi demo

```bash
# Unit tests
dotnet test ClinicBooking.Application.UnitTests

# Integration tests (cần SQL Server thật, không dùng LocalDB)
dotnet test ClinicBooking.Integration.Tests

# Tất cả (trừ integration)
dotnet test --filter "FullyQualifiedName!~Integration"
```

### 4.6 Lưu ý khi chạy integration tests

Integration tests dùng `ClinicBookingApiFactory` (WebApplicationFactory) với DB thật.
Cần có connection string trong `appsettings.Development.json` trỏ đến SQL Server (không phải LocalDB).
Tests chạy tuần tự (`parallelizeTestCollections=false` trong `xunit.runner.json`).

---

## 5. Risks còn mở

| Rủi ro | Mức độ | Hành động |
|---|---|---|
| `DoiLich` page chưa có → flow 3 không demo được | 🔴 Chặn | Task B1: tạo page trước khi demo |
| Module 4 `ThongBao` chưa ghi DB | 🟡 Low | `NotificationServiceStub` log only, không block flow. Defer đến Module 4 owner. |
| Timezone `NgayLamViec + GioBatDau` giả định UTC | 🟡 Low | Verify khi deploy production |
| `BenhNhan.SoLanHuyMuon` cross-module write | 🟡 Low | Notify Module 3 owner trước khi merge lên main |
| Chưa có Docker Compose | 🟡 Low | Thêm sau khi smoke test xong |

---

## 6. Tiếp theo sau demo

1. Push branch + tạo PR → `develop`
2. Bổ sung Docker Compose cho `ClinicBooking.Web` + SQL Server
3. Phối hợp với Module 4 owner để thay `NotificationServiceStub` bằng implementation thật
4. Verify `SoLanHuyMuon` logic với Module 3 owner trước khi merge main
