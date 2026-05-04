# Kế hoạch hoàn thiện Module 1 — Đặt lịch hẹn & Hàng chờ

> Ngày lập: 2026-05-03  
> Ngày cập nhật: 2026-05-04 (Revision 4 — Phase 2 xanh hoàn toàn)  
> Trạng thái: **Phase 2 hoàn thành — 11 pass, 1 skip. Tiếp theo: Phase 3 (background job tests)**  
> Branch: `feature/module1`

---

## Mục tiêu Revision 2

**Test full chức năng đặt / hủy / đổi lịch với thao tác và data thật qua Swagger.** Được phép implement code cho Module 2/3/4 nếu cần để đảm bảo flow hoạt động end-to-end. Tất cả các chức năng cốt lõi phải chạy được mà không cần mock hay stub nào gây block.

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
| **Phase B** — Smoke test thủ công | ⬜ **Chưa làm** | Unblock sau khi Phase 2 xanh hoàn toàn. ✅ Unblocked. |
| **Phase 3** — Background job unit tests | ✅ **Hoàn thành** | 241 unit tests pass (237 + 4 mới). `QuetGiuChoHetHanJobTests` (2) + `ChuyenLichHenDaQuaHanJobTests` (2). |
| **Phase C** — Module 4 ThongBao impl | ⬜ **Defer** | Chờ Module 4 owner. |

---

## Bug đang mở — `Huy_BenhNhanKhongSoHuu_Tra403` fail khi chạy full suite

### Triệu chứng

```
System.Net.Http.HttpRequestException: Response status code does not indicate success: 409 (Conflict).
  at HuyLichHenIntegrationTests.TaoLichHenAsync() (line 30)
  at HuyLichHenIntegrationTests.Huy_BenhNhanKhongSoHuu_Tra403 (line 52)
```

- **Chạy riêng lẻ**: `dotnet test --filter Huy_BenhNhanKhongSoHuu_Tra403` → **PASS** ✓  
- **Chạy full suite**: `dotnet test ClinicBooking.Integration.Tests` → **FAIL** ✗

### Phân tích hiện tại

Test helper `TaoLichHenAsync()` dùng cùng `(NgayLamViecHomSau, 13:15, idDichVu=2)` cho tất cả tests trong class `HuyLichHenIntegrationTests`. Với `IClassFixture`, các test trong class chạy **tuần tự** trên **cùng một DB**. Khi chạy full suite:

- Test thứ 1 (`Huy_BenhNhanChuSoHuu_Tra200`): tạo appointment trên CaLamViec 3002 → hủy → slot được release (best-effort qua `IncrementSoSlotDaDatAsync(-1)`)
- Test thứ 2 (`Huy_BenhNhanKhongSoHuu_Tra403`): gọi `TaoLichHenAsync()` → 409

`SoSlotToiDa = 12` cho CaLamViec 3002 (xác nhận từ migration designer). Không phải do hết slot.

### Nguyên nhân nghi ngờ chưa xác nhận

`TaoLichHenHandler` query CaLamViec qua `CaLamViecQueryService.KiemTraSlotTrongAsync` và `IncrementSoSlotDaDatAsync`. **File chưa đọc:** `ClinicBooking.Infrastructure/Services/Scheduling/CaLamViecQueryService.cs` — có thể có logic kiểm tra thêm (ví dụ: BenhNhan đang có appointment active trên cùng ca, hoặc time check khác với test isolation).

Khả năng khác: xUnit chạy test class **parallel** với class khác → có thể có shared state không phải DB (static field, singleton cache, v.v.).

### Hướng fix đề xuất

**Option A (nhanh, ít rủi ro):** Disable parallel execution cho integration test assembly bằng cách thêm vào `ClinicBookingApiFactory.cs` hoặc tạo file `xunit.runner.json`:
```json
{
  "parallelizeAssembly": false,
  "parallelizeTestCollections": false
}
```

**Option B (đúng hơn, cần điều tra trước):** Đọc `CaLamViecQueryService.cs` để hiểu tại sao trả 409. Nếu do rule "một BenhNhan không được có 2 appointment active cùng ca", thì helper `TaoLichHenAsync` phải được sửa để dùng **ca khác nhau** cho mỗi test hoặc **cleanup** sau mỗi test.

**Option C (robust nhất):** Mỗi test dùng CaLamViec riêng biệt — `Huy_BenhNhanChuSoHuu_Tra200` dùng 3001, `Huy_BenhNhanKhongSoHuu_Tra403` dùng 3002 (hoặc ngược lại). Tránh dùng cùng ca trong cùng class fixture.

---

## Thứ tự việc cần làm tiếp theo

```
1. Fix Huy_BenhNhanKhongSoHuu_Tra403 (xem hướng fix ở trên)
2. Verify: dotnet test ClinicBooking.Integration.Tests → 11 pass, 1 skip
3. Phase B: Smoke test thủ công qua Swagger
4. Phase 3: Background job unit tests
```

---

## Verify baseline trước khi bắt đầu

```bash
dotnet test ClinicBooking.Application.UnitTests
# Kỳ vọng: Passed 237, Failed 0  (231 gốc + 6 tests bổ sung từ Phase 1a/1b)
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

## Phase B — End-to-end smoke test flow

**Ước lượng:** ~1 giờ thực hiện thủ công  
**Prerequisite:** Phase A hoàn thành, app đang chạy, CaLamViec đã refresh  
**Tool:** Swagger UI (`http://localhost:<port>/swagger`) hoặc curl/Postman

### B1. Lấy tokens

```text
POST /api/auth/dang-nhap  →  { "tenDangNhap": "patient001", "matKhau": "Demo@123456" }
                              → PATIENT_TOKEN

POST /api/auth/dang-nhap  →  { "tenDangNhap": "receptionist001", "matKhau": "Demo@123456" }
                              → LETAM_TOKEN

POST /api/auth/dang-nhap  →  { "tenDangNhap": "doctor001", "matKhau": "Demo@123456" }
                              → BACSI_TOKEN
```

### B2. Kiểm tra CaLamViec tồn tại

```text
Auth: LETAM_TOKEN
GET /api/lich-hen/theo-ngay?ngay=2026-06-02
→ Kỳ vọng: list CaLamViec gồm IdCaLamViec=3001, IdDichVu có sẵn
```

### B3. [Core flow 1] Đặt lịch mới

> **Lưu ý API:** `TaoLichHenRequest` nhận `NgayLamViec + GioMongMuon + IdDichVu` (không phải `IdCaLamViec`).  
> Handler nội bộ tìm CaLamViec phù hợp theo ngày / giờ / chuyên khoa.

```text
Auth: PATIENT_TOKEN
POST /api/lich-hen/tao-lich-hen
Body: {
  "ngayLamViec": "2026-06-02",   ← today+30 (CaLamViec 3001/3002)
  "gioMongMuon": "08:00",        ← 07:00–12:00 => CaLamViec 3001
  "idDichVu": 1,
  "trieuChung": "Smoke test — dau nguc nhe"
}
→ 201 Created, IdLichHen = X
```

### B4. Xác nhận lịch

```text
Auth: LETAM_TOKEN
POST /api/lich-hen/{X}/xac-nhan
→ 200

Auth: PATIENT_TOKEN
GET /api/lich-hen/{X}
→ TrangThai = "DaXacNhan" ✓
```

### B5. Check-in và xử lý hàng chờ

```text
Auth: LETAM_TOKEN
POST /api/lich-hen/{X}/check-in
→ 200, IdHangCho = Y

GET /api/hang-cho/theo-ca/3001
→ Thấy bệnh nhân trong hàng chờ, TrangThai = ChoKham ✓

Auth: PATIENT_TOKEN
GET /api/hang-cho/thu-tu-cua-toi/3001
→ CoHangCho = true, SoThuTu = N ✓

Auth: BACSI_TOKEN
POST /api/hang-cho/goi-ke-tiep/3001
→ 200 ✓

POST /api/hang-cho/{Y}/hoan-thanh
→ 200 ✓
```

### B6. [Core flow 2] Đổi lịch

```text
Auth: PATIENT_TOKEN
POST /api/lich-hen/tao-lich-hen
Body: {
  "ngayLamViec": "2026-06-02",
  "gioMongMuon": "14:00",    ← 13:00–17:00 => CaLamViec 3002
  "idDichVu": 1,
  "trieuChung": "Lich can doi"
}
→ 201, IdLichHen = Z

Auth: LETAM_TOKEN
POST /api/lich-hen/{Z}/doi-lich
Body: { "idCaLamViecMoi": 3003, "lyDo": "Doi sang ca khac" }
→ 200, IdLichHenMoi = W ✓

GET /api/lich-hen/{Z}
→ TrangThai = "DaHuy" (lịch cũ bị hủy khi đổi) ✓

GET /api/lich-hen/{W}
→ TrangThai = "ChoXacNhan", IdCaLamViec = 3003 ✓
```

### B7. [Core flow 3] Hủy lịch (hủy sớm)

```text
Auth: PATIENT_TOKEN
POST /api/lich-hen/tao-lich-hen
Body: {
  "ngayLamViec": "2026-06-02",
  "gioMongMuon": "15:00",    ← 13:00–17:00 => CaLamViec 3002
  "idDichVu": 1,
  "trieuChung": "Lich se huy"
}
→ 201, IdLichHen = V

POST /api/lich-hen/{V}/huy
Body: { "lyDo": "Huy som de kiem tra" }
→ 200 ✓ (hủy >24h trước ca → không bị tính SoLanHuyMuon)
```

### Checklist smoke test

- [ ] `POST /api/auth/dang-nhap` patient001 → 200 + token ✓
- [ ] `GET /api/lich-hen/theo-ngay?ngay=2026-06-02` → có CaLamViec 3001/3002 ✓
- [ ] Đặt lịch → 201 ✓
- [ ] Xác nhận lịch → 200 ✓
- [ ] Check-in → 200, IdHangCho ✓
- [ ] Thu tự hàng chờ của tôi → CoHangCho=true ✓
- [ ] Gọi kế tiếp → 200 ✓
- [ ] Hoàn thành → 200 ✓
- [ ] Đổi lịch → 200, IdLichHenMoi ✓
- [ ] Hủy lịch sớm → 200, không tăng SoLanHuyMuon ✓

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

## Phase 2 — Integration test project 🟡 GẦN XONG (10/11 pass)

**Kết quả hiện tại:** `dotnet test ClinicBooking.Integration.Tests` → **10 pass, 1 fail, 1 skip**  
**Fail:** `HuyLichHenIntegrationTests.Huy_BenhNhanKhongSoHuu_Tra403` (xem mục **Bug đang mở**)  
**Skip:** `UnitTest1.Module1_Smoke_HuyVaDoiLichBangApiThat` (skip có chủ ý, khi fix bug trên có thể bỏ skip)

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
| `Huy_BenhNhanKhongSoHuu_Tra403` fail khi full suite | ✅ **Đã fix** | Root cause: migration seed LichHen trên 3001 (SoSlot=1,2) với SoSlotDaDat=0 → unique index collision khi test tái dùng slot. Fix: mỗi test dùng CaLamViec độc lập (3002/3003). |
| Module 4 `ThongBao` ghi cùng transaction | 🟡 Còn mở | Review khi Module 4 owner lên; hiện fire-and-forget |
| Timezone `NgayLamViec + GioBatDau` giờ địa phương | 🟡 Chưa có helper | Handlers giả định UTC-compatible; verify khi deploy production |
| `BenhNhan.SoLanHuyMuon` cross-module write | 🟡 Cần coordinate | Notify owner Module 3 trước khi merge |
| Background job tests chưa có | 🟡 Phase 3 | `QuetGiuChoHetHanJob`, `ChuyenLichHenDaQuaHanJob` — 4 tests ước lượng |
