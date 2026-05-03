# Kế hoạch hoàn thiện Module 1 — Đặt lịch hẹn & Hàng chờ

> Ngày lập: 2026-05-03  
> Trạng thái: **Đang triển khai**  
> Branch: `feature/module1/portal-sat-demo`

## Context

Dự án `ClinicBooking` (`D:\Code\C#\DatLichPhongKham`). Module 1 đã được triển khai xong phần lớn (Wave 1–3 theo kế hoạch gốc). Kế hoạch này ghi lại **những gì còn lại** để đưa Module 1 đến trạng thái ship-ready, với **ưu tiên hàng đầu là 3 chức năng cốt lõi: đặt / hủy / đổi lịch khám**.

Mỗi phase dưới đây độc lập, có thể `dotnet build` + `dotnet test` xanh ngay sau khi hoàn thành.

---

## Hiện trạng đã hoàn thành (tính đến 2026-05-03)

> **231/231 unit tests pass** (xác nhận theo session 2026-05-03).  
> Executor phải chạy `dotnet test ClinicBooking.Application.UnitTests` đầu tiên để xác minh baseline trước khi bắt đầu bất kỳ phase nào.

### Handlers đã implement

| Nhóm | Handler | Tests | Ghi chú |
|---|---|---|---|
| LichHen Command | TaoLichHen | 6 tests ✓ | Stub CaLamViecQueryService → real |
| LichHen Command | HuyLichHen | 6 tests ✓ | Late cancel, ownership, state |
| LichHen Command | DoiLichHen | 4 tests ✓ | **Thiếu: ownership + NotFound** |
| LichHen Command | XacNhanLichHen | ✓ | — |
| LichHen Command | CheckInLichHen | ✓ | Retry unique constraint |
| LichHen Command | TaoGiuCho | 3 tests ✓ | — |
| LichHen Command | GiaiPhongGiuCho | ✓ | — |
| LichHen Query | XemLichHen | ✓ | — |
| LichHen Query | DanhSachLichHenCuaToi | ✓ | — |
| LichHen Query | DanhSachLichHenTheoNgay | ✓ | — |
| LichHen Query | DanhSachLichHenTheoNgayCuaToi | ✓ | — |
| HangCho Command | GoiBenhNhanKeTiep | ✓ | — |
| HangCho Command | HoanThanhLuotKham | ✓ | — |
| HangCho Query | XemHangChoTheoCa | ✓ | — |
| HangCho Query | ThuTuCuaToi | handler ✓ | **Test file MISSING** |

### Infrastructure đã implement

- `CaLamViecQueryService` — **real** (không còn stub)
- `NotificationServiceStub` — stub log-only (chờ Module 4)
- `MaLichHenGenerator` — real
- `QuetGiuChoHetHanJob` — HostedService, chưa có unit test
- `ChuyenLichHenDaQuaHanJob` — HostedService, chưa có unit test
- `LichHenController` — 10 endpoints
- `HangChoController` — 4 endpoints
- DI registrations đầy đủ

### Quyết định thiết kế quan trọng (giữ nguyên)

1. `LichHen.SoSlot` là ordinal position — unique index `(IdCaLamViec, SoSlot)` là backstop double-book.
2. `GiuCho.SoSlot` — đã chốt là ordinal (giống LichHen), không có unique index riêng.
3. Slot concurrency: atomic `ExecuteUpdateAsync` + retry 2 lần trên `DbUpdateException`.
4. MaLichHen format: `LH-{yyyyMMdd}-{seq6}`. Generator dùng `MAX(...)` + 1.
5. "Huỷ muộn": threshold `LichHenOptions.HuyMuonTruocGio` (default 24h).
6. `BenhNhan.SoLanHuyMuon` — Module 1 ghi trực tiếp qua `IAppDbContext.BenhNhan`.

---

## Các phase còn lại

### Verify baseline trước khi bắt đầu

```bash
dotnet test ClinicBooking.Application.UnitTests
# Kỳ vọng: Passed 231, Failed 0
```

---

### Phase 1 — Lấp gap test cho đặt / hủy / đổi lịch ⭐ (ưu tiên cao nhất)

**Ước lượng:** ~2–3 giờ  
**Độc lập:** Chỉ thêm file test, không sửa production code.  
**Verify:** `dotnet test` tăng số test, tất cả xanh.

#### 1a. `ThuTuCuaToiHandlerTests.cs` (file còn thiếu)

File: `ClinicBooking.Application.UnitTests/Features/HangCho/Queries/ThuTuCuaToiHandlerTests.cs`

Viết ≥ 3 test:
- `Handle_BenhNhanDaCheckIn_TraVeSoThuTu` — happy path: patient đã check-in vào HangCho của ca → trả về `DaCoTrongHangCho=true, SoThuTu=N`.
- `Handle_BenhNhanChuaCheckIn_TraVeKhongCoHangCho` — patient chưa check-in → `DaCoTrongHangCho=false`.
- `Handle_TaiKhoanKhongCoHoSoBenhNhan_ThrowNotFound` — `BenhNhan` không tồn tại → `NotFoundException`.

Pattern: dùng `TestDbContextFactory` + `TestDataSeeder.SeedCaLamViec / SeedBenhNhan / SeedTaiKhoan`.

#### 1b. `DoiLichHenHandlerTests.cs` — bổ sung 3 test còn thiếu

File: `ClinicBooking.Application.UnitTests/Features/LichHen/Commands/DoiLichHen/DoiLichHenHandlerTests.cs`

Thêm vào class hiện có:

| Test method | Scenario | Expected |
|---|---|---|
| `Handle_BenhNhanDoiLichCuaNguoiKhac_ThrowForbidden` | BenhNhan A cố đổi lịch của BenhNhan B | `ForbiddenException` |
| `Handle_LichHenKhongTonTai_ThrowNotFound` | `idLichHen` không tồn tại trong DB | `NotFoundException` |
| `Handle_BacSiDoiLich_ThrowForbidden` | Role `BacSi` không được phép đổi lịch | `ForbiddenException` |

Cách seed: dùng `TestDataSeeder.SeedTaiKhoan(db, VaiTro.BenhNhan)` × 2 account khác nhau để test ownership.

---

### Phase 2 — Integration test project ⭐ (ưu tiên cao)

**Ước lượng:** ~0.5–1 ngày  
**Mục tiêu:** Verify toàn bộ HTTP flow (routing, auth, validation, serialization) cho 3 chức năng ưu tiên.  
**Verify:** `dotnet test ClinicBooking.Integration.Tests` xanh.

#### 2a. Tạo project

```bash
dotnet new xunit -n ClinicBooking.Integration.Tests -o ClinicBooking.Integration.Tests
dotnet sln DatLichPhongKham.slnx add ClinicBooking.Integration.Tests
```

Dependencies: `Microsoft.AspNetCore.Mvc.Testing`, `Microsoft.EntityFrameworkCore.Sqlite`, `FluentAssertions`.

File gốc: `ClinicBooking.Integration.Tests/Common/ClinicWebApplicationFactory.cs`
- Kế thừa `WebApplicationFactory<Program>`
- Override `ConfigureWebHost`: thay `AppDbContext` bằng SQLite `:memory:`; gọi `EnsureCreated()` + cleanup seed (copy logic từ `TestDbContextFactory.cs`)
- Helper `LoginAsync(role)` → trả `Authorization: Bearer <token>` header

#### 2b. Test files ưu tiên

**`TaoLichHenIntegrationTests.cs`** — `POST /api/lich-hen/tao-lich-hen`
- `Tao_BenhNhan_DatLichHopLe_Tra201` (happy path)
- `Tao_KhongCoToken_Tra401`
- `Tao_RoleBacSi_Tra403`
- `Tao_InputThieu_Tra400` (FluentValidation pipeline)

**`HuyLichHenIntegrationTests.cs`** — `POST /api/lich-hen/{id}/huy`
- `Huy_BenhNhanChuSoHuu_Tra200`
- `Huy_BenhNhanKhongSoHuu_Tra403`
- `Huy_LichHenKhongTonTai_Tra404`

**`DoiLichHenIntegrationTests.cs`** — `POST /api/lich-hen/{id}/doi-lich`
- `Doi_LeTanDoiHopLe_Tra200_CoIdLichHenMoi`
- `Doi_BenhNhanKhongSoHuu_Tra403`
- `Doi_LichDaHoanThanh_Tra409`

---

### Phase 3 — Background job unit tests (ưu tiên trung bình)

**Ước lượng:** ~2–3 giờ  
**Độc lập:** Test trong project Unit Tests hiện có.  
**Verify:** `dotnet test` tăng thêm tests.

#### 3a. `QuetGiuChoHetHanJobTests.cs`

File: `ClinicBooking.Application.UnitTests/BackgroundJobs/QuetGiuChoHetHanJobTests.cs`

- `Execute_GiuChoHetHan_DanhDauDaGiaiPhong` — seed GiuCho với `GioHetHan < UtcNow`, chạy job → `DaGiaiPhong=true`
- `Execute_GiuChoConHan_KhongThayDoi` — `GioHetHan > UtcNow` → không bị đánh dấu

#### 3b. `ChuyenLichHenDaQuaHanJobTests.cs`

File: `ClinicBooking.Application.UnitTests/BackgroundJobs/ChuyenLichHenDaQuaHanJobTests.cs`

- `Execute_LichChoXacNhanCaKetThuc_ChuyenThanhDaQuaHan` — seed CaLamViec kết thúc từ lâu + LichHen `ChoXacNhan` → job chuyển sang `DaQuaHan`, thêm `LichSuLichHen`
- `Execute_LichDaHoanThanh_KhongBiThayDoi` — LichHen `HoanThanh` không bị đụng đến

---

### Phase 4 — Smoke test + polish (optional, sau khi Phase 1–3 xanh)

**Ước lượng:** ~0.5 ngày  
**Mục tiêu:** Xác minh toàn bộ flow end-to-end qua Swagger với dữ liệu seed thật.

#### Flow smoke test

```text
1. dotnet run --project ClinicBooking.Api
2. Swagger /api/auth/dang-nhap (benh_nhan từ Module1_TestDataSeed) → lấy token
3. POST /api/lich-hen/tao-lich-hen → IdLichHen = X (201)
4. GET  /api/lich-hen/{X} → TrangThai = ChoXacNhan (200)
5. Login le_tan → POST /api/lich-hen/{X}/xac-nhan (200)
6. GET  /api/lich-hen/{X} → TrangThai = DaXacNhan
7. POST /api/lich-hen/{X}/check-in (200) → IdHangCho = Y
8. POST /api/hang-cho/goi-ke-tiep/{IdCaLamViec} (200)
9. POST /api/hang-cho/{Y}/hoan-thanh (200)
10. Tạo lịch mới → POST /api/lich-hen/{Z}/doi-lich (200) → IdLichHenMoi
11. POST /api/lich-hen/{Z}/huy với LyDo (hủy sớm, >24h) (200)
```

#### Polish checklist

- [ ] `HangChoController.GoiKeTiep` route hiện là `goi-ke-tiep/{idCaLamViec}` — khớp với plan gốc `POST /api/hang-cho/goi-ke-tiep`; xác nhận không cần sửa.
- [ ] Verify tất cả `[ProducesResponseType]` đầy đủ (XacNhan, Huy trả `IActionResult` không có typed response — đã đúng per design).
- [ ] Commit note: update `CLAUDE.md` nếu có endpoint nào thay đổi signature.

---

## Endpoint API tham chiếu (đã deploy)

> Tất cả endpoints dưới đây đã implement và hoạt động. Dùng để tham chiếu khi viết integration tests (Phase 2).

`LichHenController` route `api/lich-hen`, `HangChoController` route `api/hang-cho`.

| Method | Route | `[Authorize(Roles = ...)]` | Success |
|---|---|---|---|
| POST | `api/lich-hen/tao-lich-hen` | `benh_nhan, le_tan` | 201 |
| POST | `api/lich-hen/{idLichHen:int}/xac-nhan` | `le_tan, admin` | 200 |
| POST | `api/lich-hen/{idLichHen:int}/huy` | `benh_nhan, le_tan, admin` | 200 |
| POST | `api/lich-hen/{idLichHen:int}/doi-lich` | `benh_nhan, le_tan` | 200 |
| POST | `api/lich-hen/tao-giu-cho` | `le_tan` | 201 |
| POST | `api/lich-hen/giu-cho/{idGiuCho:int}/giai-phong` | `le_tan` | 204 |
| POST | `api/lich-hen/{idLichHen:int}/check-in` | `le_tan` | 200 |
| GET | `api/lich-hen/{idLichHen:int}` | authenticated (handler kiểm ownership) | 200 |
| GET | `api/lich-hen/cua-toi` | `benh_nhan` | 200 |
| GET | `api/lich-hen/theo-ngay?ngay=yyyy-MM-dd` | `le_tan, admin` | 200 |
| GET | `api/lich-hen/theo-ngay/cua-toi?ngay=yyyy-MM-dd` | `bac_si, admin` | 200 |
| POST | `api/hang-cho/goi-ke-tiep/{idCaLamViec:int}` | `bac_si, le_tan` | 200 |
| POST | `api/hang-cho/{idHangCho:int}/hoan-thanh` | `bac_si` | 200 |
| GET | `api/hang-cho/theo-ca/{idCaLamViec:int}` | `le_tan, bac_si, admin` | 200 |
| GET | `api/hang-cho/thu-tu-cua-toi/{idCaLamViec:int}` | `benh_nhan` | 200 |

Lỗi chuẩn: 400 validation, 401/403 auth, 404 not found, 409 conflict. Message user-facing tiếng Việt có dấu.

---

## Quyết định thiết kế chính

1. **`LichHen.SoSlot` là ordinal position trong ca** — xác nhận từ `database/clinic.dbml`. Unique index `(IdCaLamViec, SoSlot)` hợp lệ, là xương sống chống double-book.
2. **`GiuCho.SoSlot` — đã chốt là ordinal** (giống LichHen), không có unique index riêng vì không có yêu cầu chống trùng.
3. **Slot concurrency**: atomic `ExecuteUpdateAsync` tăng `SoSlotDaDat` + unique index `(IdCaLamViec, SoSlot)` + retry tối đa 2 lần trên `DbUpdateException`. Không pessimistic lock.
4. **`SoThuTu` check-in**: `MAX+1` per `IdCaLamViec` + unique index + retry 3 lần. Tần suất thấp (thao tác thủ công lễ tân) → retry acceptable.
5. **"Huỷ muộn"** threshold từ `LichHenOptions.HuyMuonTruocGio` (default 24h). Handler so `(CaLamViec.NgayLamViec + GioBatDau) - UtcNow`.
6. **MaLichHen**: format `LH-{yyyyMMdd}-{seq6}`. Generator dùng `MAX(...)` parse + 1 (không COUNT — tránh drift khi có xoá).
7. **Cross-module write `BenhNhan.SoLanHuyMuon`** — cột thuộc Module 3. Ghi qua `IAppDbContext.BenhNhan`. Coordination point: notify owner Module 3 trước khi merge.
8. **GiuCho expiration**: `QuetGiuChoHetHanJob` (HostedService) quét mỗi 1 phút, đánh dấu `DaGiaiPhong=true`.
9. **Gọi tiếp auto-complete luồng hiện tại**: `LichHenOptions.TuDongHoanThanhLuotHienTai`, default `true`.

---

## Risks còn mở (tính đến 2026-05-03)

| Rủi ro | Trạng thái | Hành động |
|---|---|---|
| `GiuCho.SoSlot` ordinal vs quantity | ✅ Chốt là ordinal | Đã implement TaoGiuCho theo ordinal |
| Module 2 mô hình hoá `SoSlotDaDat` khác | 🟡 Còn mở | `ICaLamViecQueryService` là buffer — swap DI khi Module 2 lên |
| Module 4 yêu cầu `ThongBao` ghi cùng transaction | 🟡 Còn mở | Review lại khi Module 4 lên; hiện fire-and-forget |
| Timezone `NgayLamViec + GioBatDau` giờ địa phương | 🟡 Chưa có helper | Handlers giả định UTC-compatible; cần verify khi deploy production |
| `BenhNhan.SoLanHuyMuon` cross-module write | 🟡 Cần coordinate | Notify owner Module 3 trước khi merge |
| Integration tests chưa có | 🔴 Phase 2 của plan này | Xem Phase 2 ở trên |
