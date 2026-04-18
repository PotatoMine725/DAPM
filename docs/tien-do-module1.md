# Tiến độ Module 1 — Đặt lịch hẹn & Hàng chờ

> Cập nhật: 2026-04-17 (Wave 2 complete)  
> Owner: User + Claude (shared)  
> Branch: `develop`

---

## Wave 1 — Foundation ✅ HOÀN TẤT

**Mục tiêu**: Đặt nền tảng abstractions, stubs, DB constraints, test project — tất cả độc lập 100% so với Module 2/3/4.

### 1.1 Abstractions (Application layer)

| File | Mô tả |
|---|---|
| `Abstractions/Scheduling/ICaLamViecQueryService.cs` | Contract cho Module 2 implement — 4 method: `LayThongTinCaAsync`, `KiemTraSlotTrongAsync`, `LaCaDuocDuyetAsync`, `IncrementSoSlotDaDatAsync` |
| `Abstractions/Scheduling/Dtos/ThongTinCaLamViecDto.cs` | DTO read-only cho thông tin ca làm việc |
| `Abstractions/Scheduling/Dtos/KetQuaKiemTraSlotDto.cs` | DTO kết quả kiểm tra slot trống |
| `Abstractions/Scheduling/Dtos/LyDoKhongDatDuoc.cs` | Enum 4 lý do: `CaChuaDuyet`, `HetSlot`, `CaDaDiQua`, `KhongTonTai` |
| `Abstractions/Notifications/INotificationService.cs` | Contract cho Module 4 implement — 6 method fire-and-forget |
| `Common/Options/LichHenOptions.cs` | Config bind từ `appsettings.json` section `LichHen` (HuyMuonTruocGio, GiuChoThoiHanPhut, MaLichHenPrefix, TuDongHoanThanhLuotHienTai) |
| `Common/Constants/LichHenConstants.cs` | Hằng số: SoLanThuLaiToiDa=3, KichThuocTrangMacDinh=20, DoDaiSequenceMaLichHen=6 |
| `Common/Services/IMaLichHenGenerator.cs` | Interface sinh MaLichHen format `LH-yyyyMMdd-seq6` |

### 1.2 Infrastructure stubs + implementations

| File | Mô tả |
|---|---|
| `Services/Scheduling/CaLamViecQueryServiceStub.cs` | Stub đọc trực tiếp từ `IAppDbContext.CaLamViec`, atomic `ExecuteUpdateAsync` cho slot counter, opportunistic cleanup GiuCho hết hạn |
| `Services/Notifications/NotificationServiceStub.cs` | Stub chỉ log qua `ILogger`, không ghi DB — `TODO: thay thế khi Module 4 lên` |
| `Services/MaLichHenGenerator.cs` | Impl: `MAX(MaLichHen)` parse sequence + 1, retry khi trùng unique index |

### 1.3 Domain & DB changes

- **`LichHen.cs`**: thêm `public byte[] RowVersion { get; set; } = [];` (concurrency token)
- **`AppDbContext.cs`**: mở rộng `OnModelCreating` cho 4 entity Module 1:
  - `LichHen`: unique `(IdCaLamViec, SoSlot)` chống double-book, `RowVersion` IsRowVersion(), query indexes `(IdBenhNhan, TrangThai)` + `(IdCaLamViec, TrangThai)`, `MaLichHen` HasMaxLength(32)
  - `HangCho`: unique `(IdCaLamViec, SoThuTu)`, query index `(IdCaLamViec, TrangThai, SoThuTu)`
  - `GiuCho`: index `(IdCaLamViec, DaGiaiPhong, GioHetHan)`
  - `LichSuLichHen`: index `IdLichHen` (EF Core đã tự tạo qua FK — redundant, bị skip trong migration)

### 1.4 Migration

- **`20260417001523_Module1_ThemRangBuocLichHen`**: thêm RowVersion column, 5 indexes mới, alter MaLichHen maxLength. Down migration khôi phục đúng.

### 1.5 DI & Config

- `DependencyInjection.cs`: thêm `Configure<LichHenOptions>`, register `ICaLamViecQueryService → CaLamViecQueryServiceStub`, `INotificationService → NotificationServiceStub`, `IMaLichHenGenerator → MaLichHenGenerator`
- `appsettings.json`: thêm section `"LichHen": { "HuyMuonTruocGio": 24, "GiuChoThoiHanPhut": 15, "MaLichHenPrefix": "LH", "TuDongHoanThanhLuotHienTai": true }`

### 1.6 Test project

- `ClinicBooking.Application.UnitTests` — xUnit + FluentAssertions 8.3.0 + NSubstitute 5.3.0 + EF Core SQLite 9.0.10
- `TestDbContextFactory` — SQLite `:memory:` cho unique index + FK thật
- 2/2 smoke tests passed ✅

### Kết quả build

```
Build succeeded. 0 Warning(s), 0 Error(s)
Test Run Successful. Total tests: 2, Passed: 2
```

---

## Wave 2 — Handler độc lập ✅ HOÀN TẤT

### 2a. Pure handlers (không cần stub)

| # | Handler | Loại | Trạng thái |
|---|---|---|---|
| 1 | `GiaiPhongGiuCho` | Command | ✅ |
| 2 | `XemLichHen` | Query | ✅ |
| 3 | `DanhSachLichHenCuaToi` | Query (paging) | ✅ |
| 4 | `DanhSachLichHenTheoNgay` | Query | ✅ |
| 5 | `XemHangChoTheoCa` | Query | ✅ |
| 6 | `HoanThanhLuotKham` | Command | ✅ |

### 2b. Non-critical stub handlers

| # | Handler | Stub sử dụng | Trạng thái |
|---|---|---|---|
| 7 | `XacNhanLichHen` | INotificationService | ✅ |
| 8 | `HuyLichHen` | INotificationService + ICaLamViecQueryService | ✅ |
| 9 | `CheckInLichHen` | INotificationService | ✅ |
| 10 | `GoiBenhNhanKeTiep` | INotificationService | ✅ |

### 2c. API surface (LichHenController + HangChoController)

| Method | Route | Role | Response |
|---|---|---|---|
| POST | `api/lich-hen/{id}/xac-nhan` | le_tan, admin | 200 |
| POST | `api/lich-hen/{id}/huy` | benh_nhan, le_tan, admin | 200 |
| POST | `api/lich-hen/{id}/check-in` | le_tan | 200 + HangChoResponse |
| POST | `api/lich-hen/giu-cho/{id}/giai-phong` | le_tan | 204 |
| GET  | `api/lich-hen/{id}` | authenticated (owner/staff) | 200 + LichHenResponse |
| GET  | `api/lich-hen/cua-toi` | benh_nhan | 200 + DanhSachLichHenResponse |
| GET  | `api/lich-hen/theo-ngay?ngay=yyyy-MM-dd` | le_tan, admin | 200 + list |
| POST | `api/hang-cho/goi-ke-tiep/{idCa}` | bac_si, le_tan | 200 + HangChoResponse |
| POST | `api/hang-cho/{id}/hoan-thanh` | bac_si | 200 |
| GET  | `api/hang-cho/theo-ca/{idCa}` | le_tan, bac_si, admin | 200 + list |

### 2d. Ghi chú thiết kế

- **HuyLichHen**: tính "huỷ muộn" bằng `(NgayLamViec + GioBatDau) - UtcNow < LichHenOptions.HuyMuonTruocGio`. Nếu đúng: `DanhDauHuyMuon=true` + `BenhNhan.SoLanHuyMuon++`. Gọi `IncrementSoSlotDaDatAsync(-1)` best-effort (try/catch — reconciliation Wave 4 dò drift).
- **CheckInLichHen**: retry 3 lần (`LichHenConstants.SoLanThuLaiToiDa`) khi va chạm unique `(IdCaLamViec, SoThuTu)`. TODO Wave 4: bọc `IDbContextTransaction` rõ ràng để retry sạch change-tracker.
- **GoiBenhNhanKeTiep**: option `TuDongHoanThanhLuotHienTai` (default `true`) tự đóng lượt `DangKham` trước khi chọn `SoThuTu` nhỏ nhất `ChoKham`.
- **Auth**: dùng `VaiTroConstants` mọi chỗ, không hard-code string role.
- **Namespace alias**: `CheckInLichHenHandler` dùng `HangChoEntity` / `LichSuLichHenEntity` để tránh va với namespace `Features.HangCho`.

### Kết quả build & test

```
Build succeeded. 0 Warning(s), 0 Error(s)
Test Run Successful. Total tests: 2, Passed: 2
```

> Unit test chi tiết từng handler (happy path + nhánh lỗi) sẽ bổ sung trước khi merge `develop`. Scope Wave 2 hiện tại là ship code clean build + contract/endpoint đầy đủ để Module 2/4 bắt nhịp.

---

## Wave 3 — Critical-path stub handlers ⏳

| # | Handler | Phụ thuộc critical | Trạng thái |
|---|---|---|---|
| 11 | `TaoLichHen` | ICaLamViecQueryService (availability check + slot increment) | ⏳ |
| 12 | `DoiLichHen` | ICaLamViecQueryService | ⏳ |
| 13 | `TaoGiuCho` | ICaLamViecQueryService | ⏳ |

---

## Wave 4 — Integration (blocked trên team) 🔒

- Swap DI stubs → Module 2/4 real implementations
- Hangfire job thay lazy cleanup
- Integration tests (Testcontainers SQL Server)
- Reconciliation job slot counter

---

## Coordination points cần lưu ý

| Vấn đề | Module liên quan | Ghi chú |
|---|---|---|
| `BenhNhan.SoLanHuyMuon` | Module 3 | Module 1 ghi trực tiếp qua `IAppDbContext` — notify owner Module 3 |
| `GiuCho.SoSlot` semantics | PM | Chưa rõ ordinal hay quantity — chốt trước khi viết `TaoGiuCho` |
| `ICaLamViecQueryService` contract | Module 2 | Module 2 implement thật, replace stub |
| `INotificationService` contract | Module 4 | Module 4 implement thật, replace stub |
