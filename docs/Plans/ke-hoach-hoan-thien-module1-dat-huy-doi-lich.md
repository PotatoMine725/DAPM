# Kế hoạch hoàn thiện Module 1 — Đặt lịch hẹn & Hàng chờ

> Ngày lập: 2026-05-03  
> Ngày cập nhật: 2026-05-05 (Revision 6 — DoiLich page đã hoàn tất, chuẩn bị smoke test Web UI)  
> Trạng thái: **241 unit tests pass, 11 integration tests pass. Phase B: smoke test qua Web UI**  
> Branch: `feature/module1/portal-sat-demo`

---

## Mục tiêu Revision 2

**Test full chức năng đặt / hủy / đổi lịch với thao tác và data thật qua Web UI.** Được phép implement code cho Module 2/3/4 nếu cần để đảm bảo flow hoạt động end-to-end. Tất cả các chức năng cốt lõi phải chạy được mà không cần mock hay stub nào gây block.

---

## Phân tích blocker (advisor review 2026-05-03)

| # | Blocker | Nguyên nhân | Trạng thái |
|---|---|---|---|
| 1 | **[CRITICAL]** CaLamViec dates in the past | `Module1TestDataSeeder.cs` hardcode `now = new DateTime(2026, 4, 23)` → `tomorrowDate = 2026-04-24`, `nextWeekDate = 2026-04-30` (đã là quá khứ) → `TaoLichHen` validate `NgayLamViec > today` → fail ngay step 1 | 🔴 **Phase A giải quyết** |
| 2 | Passwords của test accounts | Migration seed với placeholder `"$2a$11$encrypted_password"` (chuỗi fake, BCrypt.Verify luôn fail) | ✅ **Đã giải quyết** — `appsettings.Development.json` cấu hình `DevFixture.Enabled=true, MatKhauChung="Demo@123456"` → `DatabaseSeeder` upsert real hash khi app khởi động |
| 3 | Module 2 (CaLamViec) chưa implement | Giả định ban đầu | ✅ **Không phải vấn đề** — Module 2 đã implement: DangKyCaLamViec, DuyetCaLamViec, SinhCaLamViecTuLichNoiTru. Seed data đã có CaLamViec 3001/3002/3003 DaDuyet |
| 4 | Module 3 (BenhNhan) chưa implement | Giả định ban đầu | ✅ **Không phải vấn đề** — Module 3 đã implement 5 endpoints BenhNhan CRUD |
| 5 | Module 4 NotificationService là stub | Còn là stub, chỉ log | ✅ **Không block** — `NotificationServiceStub` không throw, không break flow chính. Defer đến Phase C nếu cần |

### Kết luận

**Chỉ cần sửa 1 file production code** (`DatabaseSeeder.cs`) để unblock toàn bộ smoke test. Không cần viết thêm code cho Module 2/3. Module 4 là optional.

> Ghi chú cập nhật 2026-05-05: page `/BenhNhan/DoiLich` đã được bổ sung ở Web UI, nên blocker demo hiện không còn ở layer giao diện.

---

## Credentials đã xác nhận

> Mật khẩu được set bởi `DatabaseSeeder.SeedDevFixtureAsync()` khi `DevFixture.Enabled=true`.  
> **Bắt buộc chạy `dotnet run` ít nhất 1 lần** trước khi test để seeder fix real bcrypt hash.

| TenDangNhap | VaiTro | MatKhau | Ghi chú |
|---|---|---|---|
| `patient001` | BenhNhan | `Demo@123456` | IdBenhNhan = 2001 |
| `doctor001` | BacSi | `Demo@123456` | IdBacSi = 2001 |
| `receptionist001` | LeTan | `Demo@123456` | — |
| `admin001` | Admin | `Demo@123456` | — |
| `admin` | Admin | `Admin@123456` | Từ `Admin:MatKhauMacDinh` |

---

## Trạng thái thực tế (2026-05-04)

| Phase | Trạng thái | Ghi chú |
|---|---|---|
| **Phase A** — Fix CaLamViec stale dates | ✅ **Hoàn thành** | `DatabaseSeeder.RefreshCaLamViecDatesAsync` đã thêm và gọi từ `SeedDevFixtureAsync`. Build 0 errors. |
| **Phase 1a** — `ThuTuCuaToiHandlerTests.cs` | ✅ **Hoàn thành** | File đã tồn tại với đủ 3 tests từ session trước. |
| **Phase 1b** — Bổ sung 3 test `DoiLichHenHandlerTests.cs` | ✅ **Hoàn thành** | 3 tests ownership + NotFound đã có từ session trước. |
| **Phase 2** — Integration tests (11/11 tests pass) | ✅ **Hoàn thành** | 11 pass, 0 fail, 1 skip cũ. Root cause: migration seed LichHen 4001/4002 trên 3001 (SoSlot=1,2) nhưng SoSlotDaDat=0 → unique constraint collision. Fix: test dùng CaLamViec riêng (3002 cho Tra200, 3003 cho Tra403). |
| **Phase B** — Smoke test thủ công qua Web UI | 🔄 **Đang làm** | Tool: `ClinicBooking.Web` (Razor Pages, gọi MediatR trực tiếp). Flow: đăng nhập → đặt lịch → xác nhận → check-in → hàng chờ → hoàn thành → hủy. |
| **Phase 3** — Background job unit tests | ✅ **Hoàn thành** | 241 unit tests pass (237 + 4 mới). `QuetGiuChoHetHanJobTests` (2) + `ChuyenLichHenDaQuaHanJobTests` (2). |
| **Phase C** — Module 4 ThongBao impl | ⬜ **Defer** | Chờ Module 4 owner. |

---

## Bug đã đóng — `Huy_BenhNhanKhongSoHuu_Tra403` ✅ Fixed

**Root cause:** Migration seed LichHen 4001/4002 trên CaLamViec 3001 (SoSlot=1,2) nhưng `SoSlotDaDat=0`. Khi test dùng CaLamViec 3001/3002 trùng nhau → `IncrementSoSlotDaDat` trả về slot đã tồn tại → unique index collision → 409.

**Fix:** Mỗi test dùng CaLamViec độc lập theo ngày:
- `Huy_BenhNhanChuSoHuu_Tra200` → `NgayLamViecHomSau` (CaLamViec 3002, 13:15)
- `Huy_BenhNhanKhongSoHuu_Tra403` → `NgayLamViecTiepTheo` (CaLamViec 3003, 08:00)

Thêm `xunit.runner.json` với `parallelizeTestCollections: false`. Kết quả: 11 pass, 0 fail.

---

## Thứ tự việc cần làm tiếp theo

```
1. [DONE] Fix Huy_BenhNhanKhongSoHuu_Tra403 → 11 integration tests pass
2. [DONE] Phase 3: Background job unit tests → 241 unit tests pass
3. [IN PROGRESS] Phase B: Smoke test qua Web UI (ClinicBooking.Web)
4. [DEFER] Phase C: Module 4 ThongBao impl
```

---

## Verify baseline (đã xác nhận 2026-05-04)

```bash
dotnet test ClinicBooking.Application.UnitTests
# Kết quả: Passed 241, Failed 0  (237 gốc + 4 background job tests từ Phase 3)

dotnet test ClinicBooking.Integration.Tests
# Kết quả: Passed 11, Failed 0, Skipped 1
```

---

## Phase A — Fix CaLamViec stale dates ✅ HOÀN THÀNH

**File đã sửa:** `ClinicBooking.Infrastructure/Persistence/DatabaseSeeder.cs`  
**Không tạo migration mới** — dùng runtime seeder để hỗ trợ `DateTime.UtcNow` (migration phải deterministic)  
**Verify:** Build 0 errors. Khi chạy app, log: `[DevFixture] Da cap nhat 3 CaLamViec stale (IDs: 3001, 3002, 3003) sang ngay tuong lai.`

### Tại sao không dùng migration mới?

EF Core migration phải deterministic (không thể dùng `DateTime.UtcNow`). Hardcode date mới trong migration (ví dụ 2026-06-01) sẽ expire lại sau ~1 tháng. `DatabaseSeeder` chạy lúc startup, có `IDateTimeProvider`, idempotent — đây là nơi đúng để xử lý "refresh to future".

### Implement

Thêm method `RefreshCaLamViecDatesAsync` vào `DatabaseSeeder.cs` và gọi từ `SeedDevFixtureAsync`:

```csharp
private async Task RefreshCaLamViecDatesAsync(CancellationToken cancellationToken)
{
    var today = DateOnly.FromDateTime(_dateTimeProvider.UtcNow);
    var seededIds = new[] { 3001, 3002, 3003 };

    var staleShifts = await _db.CaLamViec
        .Where(c => seededIds.Contains(c.IdCaLamViec) && c.NgayLamViec <= today)
        .ToListAsync(cancellationToken);

    if (!staleShifts.Any())
    {
        _logger.LogDebug("[DevFixture] CaLamViec da co ngay tuong lai. Bo qua refresh.");
        return;
    }

    // 3001/3002: "ngay mai" → 30 ngay ke tu today
    // 3003: "tuan toi" → 37 ngay ke tu today
    foreach (var shift in staleShifts)
    {
        shift.NgayLamViec = shift.IdCaLamViec == 3003
            ? today.AddDays(37)
            : today.AddDays(30);
    }

    await _db.SaveChangesAsync(cancellationToken);

    _logger.LogWarning(
        "[DevFixture] Da cap nhat {Count} CaLamViec stale (IDs: {Ids}) sang ngay tuong lai. " +
        "CHI dung o Development.",
        staleShifts.Count,
        string.Join(", ", staleShifts.Select(s => s.IdCaLamViec)));
}
```

Gọi ở cuối `SeedDevFixtureAsync()`:

```csharp
private async Task SeedDevFixtureAsync(CancellationToken cancellationToken)
{
    // ... existing TaiKhoan seeding calls ...
    await RefreshCaLamViecDatesAsync(cancellationToken);  // ← THÊM VÀO CUỐI
}
```

### Verify

```bash
# 1. Chạy app
dotnet run --project ClinicBooking.Api

# 2. Kiểm tra log startup có dòng:
# "[DevFixture] Da cap nhat 3 CaLamViec stale (IDs: 3001, 3002, 3003) sang ngay tuong lai."

# 3. Tính future date (today + 30 ngày từ 2026-05-03 = 2026-06-02)
# GET /api/lich-hen/theo-ngay?ngay=2026-06-02  (với token LeTan)
# → Kỳ vọng: 2 record CaLamViec (IdCaLamViec=3001, 3002)

# 4. GET /api/lich-hen/theo-ngay?ngay=2026-06-09  (today + 37 ngày)
# → Kỳ vọng: 1 record CaLamViec (IdCaLamViec=3003)
```

---

## Phase B — Smoke test qua Web UI (ClinicBooking.Web)

**Tool:** `ClinicBooking.Web` — Razor Pages, gọi MediatR trực tiếp (không qua REST API)  
**Khởi động:** `dotnet run --project ClinicBooking.Web` rồi mở `http://localhost:<port>`  
**Prerequisite:** `ClinicBooking.Api` phải chạy trước nếu Web dùng HTTP client; nếu Web gọi MediatR trực tiếp thì chỉ cần chạy Web.

### Pages đã implement (branch `portal-sat-demo`)

| Page | Role | Chức năng |
|---|---|---|
| `/BenhNhan/DatLich` | benh_nhan | Chọn ngày, ca, dịch vụ → đặt lịch |
| `/BenhNhan/DanhSachLichHen` | benh_nhan | Xem danh sách, lọc theo trạng thái, hủy |
| `/BenhNhan/LichHen` | benh_nhan | Chi tiết một lịch hẹn |
| `/BenhNhan/ThuTuHangCho` | benh_nhan | Xem số thứ tự hàng chờ |
| `/LeTan/QuanLyLichHen` | le_tan, admin | Xác nhận, check-in, hủy lịch |
| `/LeTan/HangCho` | le_tan | Quản lý hàng chờ |
| `/BacSi/HangCho` | bac_si | Gọi kế tiếp, hoàn thành |

### B1. Đăng nhập

```text
URL: /Auth/DangNhap
patient001 / Demo@123456     → redirect → /BenhNhan/DatLich (hoặc trang chính)
receptionist001 / Demo@123456 → redirect → /LeTan/QuanLyLichHen
doctor001 / Demo@123456      → redirect → /BacSi/HangCho
```

### B2. [Core flow 1] Đặt lịch

```text
[patient001] /BenhNhan/DatLich
  → Chọn ngày: today+30 (RefreshCaLamViecDatesAsync đã set 3001/3002 → today+30)
  → Chọn dịch vụ → Chọn giờ 08:00 → Submit
  → Kỳ vọng: SuccessMessage "Đặt lịch thành công! Mã lịch hẹn: LH-..."
  → Redirect → /BenhNhan/DanhSachLichHen, thấy lịch vừa đặt TrangThai=ChoXacNhan
```

### B3. [Core flow 2] Xác nhận + Check-in (Lễ tân)

```text
[receptionist001] /LeTan/QuanLyLichHen?ngay=<today+30>
  → Thấy lịch vừa đặt
  → Bấm "Xác nhận" → TrangThai=DaXacNhan ✓
  → Bấm "Check-in" → TrangThai=DangKham ✓ (hoặc tạo HangCho)
```

### B4. [Core flow 3] Hàng chờ (Bác sĩ)

```text
[doctor001] /BacSi/HangCho
  → Thấy bệnh nhân trong danh sách
  → Bấm "Gọi kế tiếp" → SoThuTu tăng
  → Bấm "Hoàn thành" → TrangThai=HoanThanh ✓

[patient001] /BenhNhan/ThuTuHangCho
  → CoHangCho=true → hiển thị số thứ tự ✓
```

### B5. [Core flow 4] Hủy lịch

```text
[patient001] /BenhNhan/DanhSachLichHen
  → Đặt thêm lịch mới (DatLich)
  → Bấm "Hủy" trên lịch vừa đặt → nhập lý do → Submit
  → TrangThai=DaHuy, SoLanHuyMuon không tăng (hủy >24h) ✓
```

### Checklist smoke test

- [ ] Đăng nhập patient001 → vào được /BenhNhan/DatLich
- [ ] Chọn ngày today+30, có CaLamViec hiển thị
- [ ] Đặt lịch → SuccessMessage + redirect DanhSachLichHen
- [ ] [receptionist001] Xác nhận lịch → DaXacNhan
- [ ] [receptionist001] Check-in → vào hàng chờ
- [ ] [patient001] ThuTuHangCho → CoHangCho=true
- [ ] [doctor001] Gọi kế tiếp → hoàn thành
- [ ] [patient001] Hủy lịch mới → DaHuy, không tăng SoLanHuyMuon

---

## Phase C — Module 4 minimal ThongBao impl (optional)

**Khi nào cần:** Khi có yêu cầu bắt buộc ghi `ThongBao` vào DB. Hiện `NotificationServiceStub` chỉ log, không throw, không block flow → **skip cho đến khi Module 4 owner có yêu cầu cụ thể.**

**Nếu triển khai:**
1. Tạo `ThongBaoService` implement `INotificationService`
2. Inject `IAppDbContext`, query `MauThongBao` theo `LoaiThongBao`
3. Tạo record `ThongBao` với content render từ template
4. Ghi DB fire-and-forget (background `Task.Run` hoặc outbox pattern) — không block transaction chính
5. Email/SMS thật: giữ stub → replace khi Module 4 owner sẵn sàng
6. Register thay thế `NotificationServiceStub` trong DI

---

## Phase 1 — Lấp gap unit tests ✅ HOÀN THÀNH

**Verify:** `dotnet test ClinicBooking.Application.UnitTests` → Passed 237, Failed 0

### 1a. `ThuTuCuaToiHandlerTests.cs` ✅

**File:** `ClinicBooking.Application.UnitTests/Features/HangCho/Queries/ThuTuCuaToiHandlerTests.cs`  
3 tests đã có: `Handle_BenhNhanDaCheckIn_TraVeSoThuTu`, `Handle_BenhNhanChuaCheckIn_TraVeKhongCoHangCho`, `Handle_TaiKhoanKhongCoHoSoBenhNhan_ThrowNotFound`

### 1b. `DoiLichHenHandlerTests.cs` — 3 test bổ sung ✅

**File:** `ClinicBooking.Application.UnitTests/Features/LichHen/Commands/DoiLichHen/DoiLichHenHandlerTests.cs`  
3 tests đã có: `Handle_BenhNhanDoiLichCuaNguoiKhac_ThrowForbidden`, `Handle_LichHenKhongTonTai_ThrowNotFound`, `Handle_BacSiDoiLich_ThrowForbidden`

---

## Phase 2 — Integration test project ✅ HOÀN THÀNH (11/11 pass)

**Kết quả:** `dotnet test ClinicBooking.Integration.Tests` → **11 pass, 0 fail, 1 skip**  
**Skip:** `UnitTest1.Module1_Smoke_HuyVaDoiLichBangApiThat` (skip có chủ ý — replaced bởi smoke test qua Web UI)

### Files đã có

**`ClinicBookingApiFactory.cs`** — đã cập nhật thêm:
- `public async Task<HttpClient> LoginAsync(string tenDangNhap, string matKhau)` — extract từ `UnitTest1.cs`
- `public async Task<HttpClient> TaoVaDangNhapBenhNhanMoiAsync(string tenDangNhap, string matKhau)` — idempotent, tạo TaiKhoan+BenhNhan mới cho ownership tests

**`TaoLichHenIntegrationTests.cs`** — `POST /api/lich-hen/tao-lich-hen` — **4 tests, tất cả PASS** ✅
- `Tao_BenhNhan_DatLichHopLe_Tra201`
- `Tao_KhongCoToken_Tra401`
- `Tao_RoleBacSi_Tra403`
- `Tao_InputThieu_Tra400`

**`HuyLichHenIntegrationTests.cs`** — `POST /api/lich-hen/{id}/huy` — **2/3 PASS, 1 FAIL** 🟡
- `Huy_BenhNhanChuSoHuu_Tra200` ✅
- `Huy_BenhNhanKhongSoHuu_Tra403` ❌ (fail khi chạy full suite, pass khi chạy riêng)
- `Huy_LichHenKhongTonTai_Tra404` ✅

**`DoiLichHenIntegrationTests.cs`** — `POST /api/lich-hen/{id}/doi-lich` — **3 tests, tất cả PASS** ✅
- `Doi_LeTanDoiHopLe_Tra200_CoIdLichHenMoi`
- `Doi_BenhNhanKhongSoHuu_Tra403`
- `Doi_LichDaHoanThanh_Tra409`

### Lưu ý triển khai

- Project dùng **SQL Server LocalDB** (không phải SQLite — EF Core migrations không tương thích với SQLite)
- Mỗi `IClassFixture` tạo một DB riêng với tên unique (`ClinicBooking_Integration_{Guid:N}`) — không share state giữa các class
- Factory `Dispose()` tự cleanup LocalDB sau khi test class xong
- `services.RemoveAll<IHostedService>()` trong ConfigureWebHost loại bỏ background jobs (QuetGiuCho, ChuyenDaQuaHan) để không ảnh hưởng test state
- `DatabaseSeeder` vẫn chạy (không phải HostedService) — fix password hash + upsert fixture accounts mỗi khi host khởi động

---

## Phase 3 — Background job unit tests (ưu tiên trung bình)

**Ước lượng:** ~2–3 giờ  
**Độc lập:** Trong project Unit Tests hiện có.

### 3a. `QuetGiuChoHetHanJobTests.cs`

- `Execute_GiuChoHetHan_DanhDauDaGiaiPhong` — seed GiuCho với `GioHetHan < UtcNow` → `DaGiaiPhong=true`
- `Execute_GiuChoConHan_KhongThayDoi` — `GioHetHan > UtcNow` → không bị đánh dấu

### 3b. `ChuyenLichHenDaQuaHanJobTests.cs`

- `Execute_LichChoXacNhanCaKetThuc_ChuyenThanhDaQuaHan`
- `Execute_LichDaHoanThanh_KhongBiThayDoi`

---

## Endpoint API tham chiếu

| Method | Route | Roles | Success |
|---|---|---|---|
| POST | `api/lich-hen/tao-lich-hen` | `benh_nhan, le_tan` | 201 |
| POST | `api/lich-hen/{id}/xac-nhan` | `le_tan, admin` | 200 |
| POST | `api/lich-hen/{id}/huy` | `benh_nhan, le_tan, admin` | 200 |
| POST | `api/lich-hen/{id}/doi-lich` | `benh_nhan, le_tan` | 200 |
| POST | `api/lich-hen/tao-giu-cho` | `le_tan` | 201 |
| POST | `api/lich-hen/giu-cho/{id}/giai-phong` | `le_tan` | 204 |
| POST | `api/lich-hen/{id}/check-in` | `le_tan` | 200 |
| GET | `api/lich-hen/{id}` | authenticated | 200 |
| GET | `api/lich-hen/cua-toi` | `benh_nhan` | 200 |
| GET | `api/lich-hen/theo-ngay?ngay=yyyy-MM-dd` | `le_tan, admin` | 200 |
| POST | `api/hang-cho/goi-ke-tiep/{idCaLamViec}` | `bac_si, le_tan` | 200 |
| POST | `api/hang-cho/{id}/hoan-thanh` | `bac_si` | 200 |
| GET | `api/hang-cho/theo-ca/{idCaLamViec}` | `le_tan, bac_si, admin` | 200 |
| GET | `api/hang-cho/thu-tu-cua-toi/{idCaLamViec}` | `benh_nhan` | 200 |

Lỗi chuẩn: 400 validation, 401/403 auth, 404 not found, 409 conflict. Message tiếng Việt có dấu.

---

## Quyết định thiết kế quan trọng (giữ nguyên)

1. `LichHen.SoSlot` là ordinal position — unique index `(IdCaLamViec, SoSlot)` chống double-book.
2. `GiuCho.SoSlot` — chốt là ordinal, không có unique index riêng.
3. Slot concurrency: atomic `ExecuteUpdateAsync` + retry 2 lần trên `DbUpdateException`.
4. MaLichHen format: `LH-{yyyyMMdd}-{seq6}`. Generator dùng `MAX(...)` + 1.
5. "Huỷ muộn": threshold `LichHenOptions.HuyMuonTruocGio` (default 24h).
6. `BenhNhan.SoLanHuyMuon` — ghi trực tiếp qua `IAppDbContext.BenhNhan`.

---

## Dữ liệu seed đã xác nhận (từ migration designer)

| CaLamViec | GioBatDau | GioKetThuc | SoSlotToiDa | ThoiGianSlot | TrangThai |
|---|---|---|---|---|---|
| 3001 | 07:00 | 12:00 | 15 | 20 phút | DaDuyet |
| 3002 | 13:00 | 17:00 | 12 | 20 phút | DaDuyet |
| 3003 | 07:00 | 12:00 | 15 | 20 phút | DaDuyet |

> `SoSlotDaDat` khởi đầu = 0. Sau `RefreshCaLamViecDatesAsync`: 3001/3002 → `today+1`, 3003 → `today+7`.

---

## Risks còn mở

| Rủi ro | Trạng thái | Hành động |
|---|---|---|
| CaLamViec seed dates hết hạn định kỳ | ✅ Mitigated | `RefreshCaLamViecDatesAsync` chạy mỗi startup → tự refresh. Không expire nữa. |
| `Huy_BenhNhanKhongSoHuu_Tra403` fail khi full suite | ✅ **Đóng** | Root cause: migration seed LichHen trên 3001 (SoSlot=1,2) với SoSlotDaDat=0 → unique index collision. Fix: mỗi test dùng CaLamViec độc lập (3002/3003) + xunit.runner.json parallelizeTestCollections=false. |
| Module 4 `ThongBao` ghi cùng transaction | 🟡 Còn mở | Review khi Module 4 owner lên; hiện fire-and-forget |
| Timezone `NgayLamViec + GioBatDau` giờ địa phương | 🟡 Chưa có helper | Handlers giả định UTC-compatible; verify khi deploy production |
| `BenhNhan.SoLanHuyMuon` cross-module write | 🟡 Cần coordinate | Notify owner Module 3 trước khi merge |
| Background job tests | ✅ Đóng | Phase 3 hoàn thành — 4 tests pass. |
