# Kế hoạch Hoàn thiện Chức năng Đặt / Hủy / Đổi Lịch trên Localhost

> Ngày lập: 2026-05-05 | Cập nhật: 2026-05-07  
> Branch: `feature/module1/portal-sat-demo`  
> Mục tiêu: **4 flow đặt / hủy / đổi lịch + hàng chờ chạy thật trên localhost — không mock, không stub chặn flow.**

---

## 1. Tổng kết đã làm

### 1.1 Backend — Application Layer

| Chức năng | Handler | Trạng thái |
|---|---|---|
| Đặt lịch hẹn | `TaoLichHenHandler` | ✅ |
| Hủy lịch hẹn | `HuyLichHenHandler` | ✅ |
| Đổi lịch hẹn | `DoiLichHenHandler` | ✅ |
| Xác nhận lịch (lễ tân) | `XacNhanLichHenHandler` | ✅ |
| Check-in (lễ tân) | `CheckInHandler` | ✅ |
| Gọi kế tiếp / Hoàn thành (bác sĩ) | `GoiKeTiepHandler`, `HoanThanhKhamHandler` | ✅ |
| Lấy số thứ tự | `ThuTuCuaToiHandler` | ✅ |
| Background jobs | `QuetGiuChoHetHanJob`, `ChuyenLichHenDaQuaHanJob` | ✅ |

### 1.2 Infrastructure — trạng thái thật/stub

| Service | Impl | Ghi chú |
|---|---|---|
| `ICaLamViecQueryService` | ✅ **Real** (`CaLamViecQueryService`) | Query DB thật, kiểm tra slot, increment — không stub |
| `IMaLichHenGenerator` | ✅ **Real** (`MaLichHenGenerator`) | Sinh mã lịch hẹn thật |
| `INotificationService` | 🟡 Stub (log-only) | Không throw, không block flow — defer Module 4 |
| `IOtpService` | 🟡 Stub-name, logic thật | Ghi DB thật (`OtpLog`), có expiry + rate limit. **Điểm stub duy nhất: không gửi email, hiện OTP ra màn hình** |
| `DatabaseSeeder` | ✅ **Real** | `SyncCaLamViecVaLichHenDemoAsync`: xóa LichHen demo cũ → refresh CaLamViec dates → re-seed 4 LichHen demo mỗi startup |

### 1.3 Tests

**252 tests pass** — 241 unit + 11 integration.

### 1.4 Web UI

| Page | Trạng thái |
|---|---|
| `/Auth/DangNhap` | ✅ |
| `/BenhNhan/DatLich` | ✅ (OTP bypass đang bật — không cần nhập OTP) |
| `/BenhNhan/DanhSachLichHen` | ✅ |
| `/BenhNhan/LichHen` | ✅ |
| `/BenhNhan/DoiLich` | ✅ |
| `/BenhNhan/ThuTuHangCho` | ✅ |
| `/LeTan/QuanLyLichHen` | ✅ |
| `/LeTan/HangCho` | ✅ Dropdown chọn ca (hôm nay + ngày mai) — auto-submit khi đổi |
| `/BacSi/HangCho` | ✅ Dropdown chọn ca của bác sĩ — auto-submit khi đổi |

---

## 2. Seed data và Ca làm việc

### 2.1 CaLamViec hiện có

| Id | GioBatDau | GioKetThuc | NgayLamViec | Ghi chú |
|---|---|---|---|---|
| 3001 | 07:00 | 12:00 | **ngày mai** | refresh mỗi startup |
| 3002 | 13:00 | 17:00 | **ngày mai** | refresh mỗi startup |
| 3003 | 07:00 | 12:00 | **hôm nay +7 ngày** | refresh mỗi startup |
| 3004 | 07:00 | 12:00 | **hôm nay** | refresh mỗi startup (thêm 2026-05-07) |
| 3005 | 13:00 | 17:00 | **hôm nay** | refresh mỗi startup (thêm 2026-05-07) |

> Tất cả có `TrangThaiDuyet=DaDuyet`, `SoSlotToiDa=15` (3004) / 12 (3005).

### 2.2 LichHen demo — tự seed mỗi startup

`DatabaseSeeder.SyncCaLamViecVaLichHenDemoAsync` xóa LichHen cũ (prefix `DEMO-` hoặc id 4001/4002) rồi tạo lại:

| MaLichHen | CaLamViec | TrangThai | Mô tả |
|---|---|---|---|
| `DEMO-{today}-01` | 3004 (sáng hôm nay) | `ChoXacNhan` | Chờ lễ tân xác nhận |
| `DEMO-{today}-02` | 3004 (sáng hôm nay) | `DaXacNhan` | Sẵn sàng check-in |
| `DEMO-{today}-03` | 3004 (sáng hôm nay) | `DaXacNhan` + HangCho.ChoKham | Đã check-in, đang chờ trong hàng |
| `DEMO-{tomorrow}-01` | 3001 (sáng ngày mai) | `ChoXacNhan` | Demo flow ngày mai |

> `SoSlotDaDat`: Ca 3004 = 3, Ca 3001 = 1; tất cả ca còn lại = 0.

---

## 3. Phân tích blocker và phụ thuộc Module 2/3/4

### 3.1 Kết luận phân tích

| Nguồn | Phụ thuộc | Trạng thái | Block flow? |
|---|---|---|---|
| Module 2 | `ICaLamViecQueryService` | ✅ Real impl, query DB thật | Không |
| Module 3 | `BenhNhan.SoLanHuyMuon` | ✅ Đã write trong `HuyLichHenHandler` | Không |
| Module 4 | `INotificationService` | 🟡 Stub, fire-and-forget, không throw | Không |
| Module 1 | OTP trong `DatLich` page | ✅ **Đã bypass** — `BatBuocChoDatLich=false` | **Không** |

**Không còn blocker. App sẵn sàng chạy đủ 4 flow.**

### 3.2 Bypass OTP — đã áp dụng

`appsettings.Development.json` đã có:

```json
"Otp": {
  "BatBuocChoDatLich": false
}
```

Khi `false` → `DatLich.cshtml.cs` bỏ qua toàn bộ bước OTP, submit thẳng.

---

## 4. Kế hoạch tiếp theo

### Giai đoạn hiện tại — Verify 4 flow (ưu tiên cao)

```
[✅] OTP bypass đang bật
[✅] Migration 20260507000000_SeedCaLamViecHomNay tự apply khi startup
[✅] 4 LichHen demo tự seed mỗi startup
[✅] HangCho pages có dropdown ca (không cần nhớ ID)
[ ] Chạy app, verify log [DevFixture] seed đúng
[ ] Test 4 flow theo kịch bản mục 5, tick checklist
[ ] Push branch + tạo PR vào develop
```

### Giai đoạn 2 — Email OTP thật (sau khi flow xanh)

```
[2a] Thêm MailKit package vào Infrastructure
[2b] Tạo IEmailService + SmtpEmailService
[2c] Tạo OtpService thật (thay OtpServiceStub) — gọi email thay vì trả về màn hình
[2d] Cấu hình SMTP qua user-secrets
[2e] Bật lại BatBuocChoDatLich=true
[2f] Test lại Flow 1 với OTP về email
[2g] Commit
```

### Giai đoạn 3 — Wrap up

```
[3a] Push branch + tạo PR → develop
[3b] Thêm Docker Compose (Web + SQL Server + Mailhog)
```

---

## 5. Kịch bản kiểm tra 4 flow

### Điều kiện tiên quyết

- `dotnet run --project ClinicBooking.Web` (port xem trong launchSettings)
- `ASPNETCORE_ENVIRONMENT=Development` → seed tự động khi start
- OTP bypass đang bật (`BatBuocChoDatLich=false` trong `appsettings.Development.json`)
- Confirm log `[DevFixture]` in ra — nghĩa là 4 LichHen demo đã được seed đúng

### Tài khoản

| Role | TenDangNhap | MatKhau |
|---|---|---|
| Bệnh nhân | `patient001` | `Demo@123456` |
| Lễ tân | `receptionist001` | `Demo@123456` |
| Bác sĩ | `doctor001` | `Demo@123456` |
| Admin | `admin` | `Admin@123456` |

---

### Flow 1 — Đặt lịch hẹn mới

```
1. Đăng nhập patient001
2. /BenhNhan/DatLich → chọn ngày (ngày mai trở đi), giờ nằm trong ca (07:00–12:00 hoặc 13:00–17:00)
3. Submit (không cần OTP khi bypass)
4. ✅ Mong đợi: redirect DanhSachLichHen, lịch mới TrangThai=ChoXacNhan
5. Verify DB: bảng LichHen có record mới
```

> ⚠️ Ngày tối thiểu = **ngày mai**. Hôm nay đã dùng hết cho LichHen demo (SoSlotDaDat=3/15).  
> Giờ mong muốn phải nằm trong khung ca: 07:00–12:00 hoặc 13:00–17:00.

### Flow 2 — Xác nhận → Check-in → Hàng chờ → Hoàn thành

Dùng LichHen demo sẵn có để demo nhanh không cần đặt trước.

```
Bước 2a — Xác nhận:
  [receptionist001] /LeTan/QuanLyLichHen → tìm DEMO-{today}-01 → Xác nhận
  ✅ TrangThai = DaXacNhan

Bước 2b — Check-in:
  [receptionist001] /LeTan/QuanLyLichHen → tìm DEMO-{today}-02 → Check-in
  ✅ Tạo HangCho với TrangThaiHangCho=ChoKham
  ✅ LichHen.TrangThai vẫn là DaXacNhan (không đổi — đây là đúng)

Bước 2c — Xem số thứ tự (bệnh nhân):
  [patient001] /BenhNhan/ThuTuHangCho → thấy số thứ tự của DEMO-{today}-03

Bước 2d — Gọi kế tiếp (bác sĩ):
  [doctor001] /BacSi/HangCho → chọn Ca #{3004} → Gọi bệnh nhân tiếp theo
  ✅ HangCho đầu tiên TrangThaiHangCho=DangKham, row highlight

Bước 2e — Hoàn thành (bác sĩ):
  [doctor001] Bấm "Hoàn thành" trên hàng DangKham
  ✅ TrangThaiHangCho=HoanThanh
```

### Flow 3 — Đổi lịch hẹn

```
1. [patient001] Đặt lịch mới (Flow 1) → ChoXacNhan
2. /BenhNhan/LichHen?id={id} → bấm "Đổi lịch"
3. /BenhNhan/DoiLich?id={id} → chọn ngày/giờ khác (Ca 3001 ngày mai hoặc Ca 3003)
4. Submit
5. ✅ Mong đợi: CaLamViec đổi, lịch cũ TrangThai=DaHuyDoDoiLich, LichSuLichHen có chain
```

### Flow 4 — Hủy lịch hẹn

```
1. [patient001] Đặt lịch mới (Flow 1) → ChoXacNhan
2. /BenhNhan/DanhSachLichHen → bấm "Hủy" → nhập lý do → Confirm
3. ✅ Mong đợi: TrangThai=HuyBenhNhan, SoLanHuyMuon không tăng (hủy >24h trước ngày khám)
```

### Checklist

```
[ ] Flow 1 — Đặt lịch thành công, TrangThai=ChoXacNhan
[ ] Flow 2a — Xác nhận → DaXacNhan
[ ] Flow 2b — Check-in → HangCho.ChoKham (LichHen.TrangThai vẫn DaXacNhan)
[ ] Flow 2c — Bệnh nhân thấy số thứ tự trên ThuTuHangCho
[ ] Flow 2d — Bác sĩ gọi kế tiếp → DangKham
[ ] Flow 2e — Bác sĩ hoàn thành → HoanThanh
[ ] Flow 3 — Đổi lịch, CaLamViec mới, chain LichSuLichHen đúng
[ ] Flow 4 — Hủy, HuyBenhNhan, SoLanHuyMuon không đổi
```

---

## 6. Lỗi đã gặp và cách sửa

### 6.1 "Không tìm thấy slot phù hợp" khi đặt lịch

**Triệu chứng:** `TaoLichHenHandler` ném exception "Khong tim thay slot phu hop voi gio mong muon" dù CaLamViec còn slot trống.

**Nguyên nhân gốc:**

1. Migration cũ seed trực tiếp LichHen 4001 (SoSlot=1) và 4002 (SoSlot=2) cho CaLamViec 3001.
2. `DatabaseSeeder` reset `SoSlotDaDat=0` nhưng **không xóa** các LichHen đó.
3. Khi bệnh nhân đặt lịch mới: `IncrementSoSlotDaDatAsync` trả về `SoSlot=1`.
4. INSERT LichHen với `(IdCaLamViec=3001, SoSlot=1)` → **vi phạm UNIQUE CONSTRAINT** → SQL throw exception → handler bắt nhầm là "không tìm thấy slot".

**Chuỗi lỗi chi tiết:**
```
TaoLichHenHandler
  → CaLamViecQueryService.IncrementSoSlotDaDatAsync(3001) → trả về SoSlot=1
  → INSERT LichHen (IdCaLamViec=3001, SoSlot=1) → UNIQUE CONSTRAINT violation
  → Exception bị bắt và re-throw là "Khong tim thay slot"
```

**Cách sửa (đã áp dụng trong commit `a977fe8` và mở rộng sau đó):**

`DatabaseSeeder.SyncCaLamViecVaLichHenDemoAsync` mỗi startup:
1. Xóa tất cả HangCho liên quan đến LichHen demo
2. Xóa tất cả LichSuLichHen liên quan
3. Xóa LichHen có `MaLichHen` bắt đầu bằng `"DEMO-"` hoặc `IdLichHen IN (4001, 4002)`
4. Refresh `NgayLamViec` của 5 CaLamViec (3004/3005→hôm nay, 3003→+7 ngày, 3001/3002→ngày mai)
5. Re-seed 4 LichHen demo với `SoSlot` tường minh
6. Set `SoSlotDaDat` chính xác: ca 3004=3, ca 3001=1; các ca còn lại=0

Kết quả: không còn LichHen "ma" chiếm slot, `IncrementSoSlotDaDatAsync` trả về SoSlot chưa bị dùng.

**File liên quan:**
- `ClinicBooking.Infrastructure/Persistence/DatabaseSeeder.cs` — `SyncCaLamViecVaLichHenDemoAsync`
- `ClinicBooking.Infrastructure/Persistence/Migrations/20260507000000_SeedCaLamViecHomNay.cs`
- `ClinicBooking.Application/Features/LichHen/Commands/TaoLichHen/TaoLichHenHandler.cs` (lines 88-102: slot matching logic)

### 6.2 Ràng buộc đặt lịch cần nhớ

| Ràng buộc | Giá trị |
|---|---|
| Ngày sớm nhất | Ngày mai (hôm nay không đặt được — slot demo đang chiếm) |
| Giờ mong muốn | Phải nằm trong: 07:00–12:00 hoặc 13:00–17:00 |
| Slot condition | `SoSlotDaDat < SoSlotToiDa` |
| Trạng thái ca | `TrangThaiDuyet = DaDuyet` |

### 6.3 Lỗi enum `TrangThaiLichHen.DangCho` không tồn tại

**Triệu chứng:** `CS0117: 'TrangThaiLichHen' does not contain a definition for 'DangCho'`

**Nguyên nhân:** Hiểu nhầm flow check-in — tưởng check-in đổi `LichHen.TrangThai` sang `DangCho`.

**Thực tế:** Check-in **không thay đổi** `LichHen.TrangThai`. Sau check-in:
- `LichHen.TrangThai` vẫn là `DaXacNhan`
- Một record `HangCho` mới được tạo với `TrangThaiHangCho=ChoKham`

`TrangThaiLichHen` enum hiện có: `ChoXacNhan`, `DaXacNhan`, `DangKham`, `HoanThanh`, `HuyBenhNhan`, `HuyPhongKham`, `KhongDen`, `DaQuaHan`.

---

## 7. Hướng dẫn triển khai local

### 7.1 Yêu cầu

| Công cụ | Ghi chú |
|---|---|
| .NET SDK 8.0 | |
| SQL Server 2019+ | SQL Server Express đủ; LocalDB chỉ dùng cho unit test |
| `dotnet-ef` | `dotnet tool install -g dotnet-ef` |

### 7.2 Khởi động lần đầu

```bash
# Build
dotnet build DatLichPhongKham.slnx

# Kiểm tra / cấu hình connection string
# ClinicBooking.Web/appsettings.json đã có: Server=POTATO;Database=ClinicBooking;...
# Đổi Server= theo tên máy hoặc dùng (localdb)\mssqllocaldb nếu chỉ test unit

# Apply migrations (tự động khi startup, hoặc chạy tay)
dotnet ef database update \
  --project ClinicBooking.Infrastructure \
  --startup-project ClinicBooking.Web

# Chạy (seed tự động, log [DevFixture] khi seed xong)
dotnet run --project ClinicBooking.Web
```

### 7.3 Bypass OTP — đã áp dụng

`ClinicBooking.Web/appsettings.Development.json` đã có:

```json
"Otp": {
  "BatBuocChoDatLich": false
}
```

### 7.4 Cấu hình SMTP (Giai đoạn 2 — sau khi flow xanh)

**Cách dễ nhất — Mailhog (không cần email thật):**

```bash
docker run -d -p 1025:1025 -p 8025:8025 --name mailhog mailhog/mailhog
# Xem mail nhận được tại: http://localhost:8025
```

```bash
dotnet user-secrets init --project ClinicBooking.Web
dotnet user-secrets set "Email:SmtpHost" "localhost" --project ClinicBooking.Web
dotnet user-secrets set "Email:SmtpPort" "1025" --project ClinicBooking.Web
dotnet user-secrets set "Email:StartTls" "false" --project ClinicBooking.Web
dotnet user-secrets set "Email:FromAddress" "noreply@clinicbooking.local" --project ClinicBooking.Web
dotnet user-secrets set "Email:FromName" "ClinicBooking" --project ClinicBooking.Web
```

**Cách dùng Gmail App Password:**

```bash
dotnet user-secrets set "Email:SmtpHost" "smtp.gmail.com" --project ClinicBooking.Web
dotnet user-secrets set "Email:SmtpPort" "587" --project ClinicBooking.Web
dotnet user-secrets set "Email:StartTls" "true" --project ClinicBooking.Web
dotnet user-secrets set "Email:FromAddress" "your@gmail.com" --project ClinicBooking.Web
dotnet user-secrets set "Email:FromName" "ClinicBooking" --project ClinicBooking.Web
dotnet user-secrets set "Email:Username" "your@gmail.com" --project ClinicBooking.Web
dotnet user-secrets set "Email:Password" "xxxx xxxx xxxx xxxx" --project ClinicBooking.Web
```

> **Bắt buộc dùng user-secrets.** Không commit SMTP password vào appsettings.

### 7.5 Chạy tests

```bash
# Unit tests (không cần SQL Server)
dotnet test ClinicBooking.Application.UnitTests

# Integration tests (cần SQL Server thật, không dùng LocalDB)
dotnet test ClinicBooking.Integration.Tests

# Chỉ unit tests
dotnet test --filter "FullyQualifiedName!~Integration"
```

> Integration tests chạy tuần tự (`parallelizeTestCollections=false`).

---

## 8. Risks

| Rủi ro | Mức | Hành động |
|---|---|---|
| OTP bypass quên tắt trước khi merge production | 🔴 | `BatBuocChoDatLich=false` chỉ có trong `appsettings.Development.json`, không vào `appsettings.json` — không ảnh hưởng production |
| SMTP credentials leak vào git | 🔴 | Bắt buộc dùng `user-secrets` cho mọi SMTP config |
| `NotificationServiceStub` không ghi DB | 🟡 | Không block flow. Defer Module 4 owner |
| `BenhNhan.SoLanHuyMuon` cross-module write | 🟡 | Đã implement trong `HuyLichHenHandler`. Notify Module 3 owner trước merge main |
| Timezone giả định UTC | 🟡 | Verify khi deploy production |
| LichHen demo cũ không bị xóa nếu seeder crash | 🟡 | Seeder dùng transaction; nếu crash → restart app, seeder chạy lại |
