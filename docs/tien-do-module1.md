# Tiến độ Module 1 — Đặt lịch hẹn & Hàng chờ

> Cập nhật: 2026-04-19 (Web Portal partial, chờ Module 2)
> Owner: User + Claude (shared)
> Branch: `feature/module1/wave2`

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

## Wave 3 — Critical-path stub handlers ✅ HOÀN TẤT

### 3a. Handlers

| # | Handler | Phụ thuộc critical | Trạng thái |
|---|---|---|---|
| 11 | `TaoLichHen` | `ICaLamViecQueryService` (`LayThongTinCaAsync` + `KiemTraSlotTrongAsync` + `IncrementSoSlotDaDatAsync`), `IMaLichHenGenerator`, `INotificationService` | ✅ |
| 12 | `DoiLichHen` | Như `TaoLichHen` + decrement slot cũ best-effort | ✅ |
| 13 | `TaoGiuCho` | `ICaLamViecQueryService` | ✅ |

### 3b. API surface (bổ sung vào LichHenController)

| Method | Route | Role | Response |
|---|---|---|---|
| POST | `api/lich-hen/tao-lich-hen` | benh_nhan, le_tan, admin | 201 + LichHenResponse |
| POST | `api/lich-hen/{id}/doi-lich` | benh_nhan, le_tan, admin | 200 + LichHenResponse |
| POST | `api/lich-hen/tao-giu-cho` | le_tan, admin | 201 + GiuChoResponse |

### 3c. Ghi chú thiết kế

- **TaoLichHen**: `HinhThucDat` suy từ vai trò (`benh_nhan → TrucTuyen`, `le_tan/admin → TaiQuay`). `benh_nhan` luôn dùng `IdBenhNhan` suy từ `IdTaiKhoan`; `le_tan/admin` phải truyền `IdBenhNhan` trong body. Flow: validate `BenhNhan.BiHanChe`, `TrangThaiDuyetCa.DaDuyet`, `thoiDiemBatDau > now`, `KiemTraSlotTrongAsync`, atomic `IncrementSoSlotDaDatAsync(+1)` → `SoSlot` ordinal → insert `LichHen` + `LichSuLichHen(DatMoi)`. Khi `DbUpdateException` (va chạm unique `(IdCaLamViec, SoSlot)`) hoặc `soSlot > SoSlotToiDa`: decrement counter + `ConflictException`.
- **DoiLichHen**: chiếm slot ca mới TRƯỚC khi huỷ ca cũ để tránh mất cả 2 slot khi race. Tạo `LichSuLichHen(DoiLich)` trên lịch cũ + `LichSuLichHen(DatMoi, IdLichHenTruoc=cũ)` trên lịch mới — chain không reset. Decrement slot ca cũ best-effort sau `SaveChanges` thành công.
- **TaoGiuCho**: tạm coi `GiuCho.SoSlot` là ordinal giống `LichHen.SoSlot`. **Coordination point: cần PM xác nhận semantic** trước khi swap stub Module 2 (Wave 4). `GioHetHan = UtcNow + LichHenOptions.GiuChoThoiHanPhut`.
- **Rollback counter**: tất cả 3 handler đều decrement counter qua `IncrementSoSlotDaDatAsync(-1)` trong `catch` khi save fail. Nếu decrement cũng fail → reconciliation job Wave 4 dò drift.

### Kết quả build & test

```
Build succeeded. 0 Warning(s), 0 Error(s)
Test Run Successful. Total tests: 2, Passed: 2
```

> Unit test chi tiết từng handler Wave 3 (happy path + nhánh lỗi: het slot / ca chua duyet / race) sẽ bổ sung trước khi merge `develop`.

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

---

## Wave 5 — Web Portal (Razor Pages) 🔶 ĐANG LÀM

**Bối cảnh**: Trong lúc chờ team Module 2/3/4, bổ sung UI Razor Pages (`ClinicBooking.Web`) để demo luồng end-to-end với stub. Portal dùng **Cookie Authentication** (không JWT), reuse MediatR handler đã có từ Wave 2/3.

### 5.1 Hạ tầng Web

| Hạng mục | Trạng thái | Ghi chú |
|---|---|---|
| `Program.cs` — AddRazorPages, Cookie auth, middleware pipeline | ✅ | `UseHttpsRedirection` chỉ bật ở production |
| `Pages/Shared/_Layout.cshtml` — sidebar điều hướng theo vai trò | ✅ | Render menu item dựa vào `User.FindFirstValue(ClaimTypes.Role)` |
| `Pages/Shared/_LoginLayout.cshtml` — layout riêng cho Auth | ✅ | |
| `Helpers/BadgeHelper.cs` — render HTML badge trạng thái | ✅ | Dùng cho bảng lịch hẹn / hàng chờ |
| `_ViewImports.cshtml` — `@using ClinicBooking.Web.Helpers` | ✅ | Fix build error "BadgeHelper does not exist" |
| CSS system: `common.css`, `components.css`, `layout.css` + Phosphor Icons CDN | ✅ | Xem `docs/huong-dan-phat-trien-ui.md` |
| `Pages/Index.cshtml(.cs)` — redirect theo vai trò | ✅ | admin → `/Admin/Dashboard`, letan → `/LeTan/Dashboard`, bacsi → `/BacSi/HangCho`, benhnhan → `/BenhNhan/DanhSachLichHen` |

### 5.2 Trang theo vai trò

| Vai trò | Trang | Trạng thái | Ghi chú |
|---|---|---|---|
| Auth | `Pages/Auth/DangNhap.cshtml(.cs)` | ✅ | Cookie sign-in, redirect theo `VaiTroConstants` |
| Auth | `Pages/Auth/DangKy.cshtml(.cs)` | ✅ | Bệnh nhân tự đăng ký qua `DangKyCommand` |
| Auth | `Pages/Auth/DangXuat.cshtml(.cs)` | ✅ | `SignOutAsync` → redirect `/Auth/DangNhap`. Form dùng `asp-page` để auto-inject antiforgery token |
| Auth | `Pages/Auth/TuChoi.cshtml` | ✅ | Access denied page |
| Admin | `Pages/Admin/Dashboard.cshtml(.cs)` | 🔶 placeholder | Landing page riêng cho admin (không dùng chung `/LeTan/Dashboard`). Chờ Module 4 làm UI quản trị thật |
| Lễ tân | `Pages/LeTan/Dashboard.cshtml(.cs)` | ✅ | Thống kê lịch hẹn hôm nay + action Xác nhận / Check-in |
| Lễ tân | `Pages/LeTan/QuanLyLichHen.cshtml(.cs)` | ✅ | Danh sách lịch hẹn theo ngày, filter trạng thái |
| Bác sĩ | `Pages/BacSi/HangCho.cshtml(.cs)` | ✅ | Hàng chờ trong ca, gọi bệnh nhân kế tiếp, hoàn thành lượt khám |
| Bệnh nhân | `Pages/BenhNhan/DanhSachLichHen.cshtml(.cs)` | ✅ | Danh sách lịch khám của tôi + modal huỷ lịch |
| Bệnh nhân | `Pages/BenhNhan/DatLich.cshtml(.cs)` | ⏸️ **BLOCKED** | Chờ Module 2 — xem mục 5.4 bên dưới |

### 5.3 Dev fixture seed

Mở rộng `DatabaseSeeder` để seed tài khoản dev idempotent (chỉ chạy khi `Admin:DevFixture:Enabled=true`):

| File | Mô tả |
|---|---|
| `Infrastructure/Persistence/AdminSeederSettings.cs` | Thêm `DevFixtureSettings` + `FixtureTaiKhoan` nested classes |
| `Infrastructure/Persistence/DatabaseSeeder.cs` | Thêm `SeedDevFixtureAsync` — insert `TaiKhoan` + `LeTan` nếu chưa có |
| `ClinicBooking.Web/appsettings.json` | Bật `Admin.DevFixture.LeTan` — seed `letan / Letan@123456` |

Tài khoản seed hiện có:
- `admin / Admin@123456` (qua migration + `FixMatKhauAdminAsync`)
- `letan / Letan@123456` (qua `SeedDevFixtureAsync`)

Chưa có fixture: `bac_si`, `benh_nhan` — có thể mở rộng `DevFixture` khi cần.

### 5.4 Vì sao `DatLich` (đặt lịch mới) chưa làm được

Trang đặt lịch phía bệnh nhân cần flow 3 cấp dropdown + xác nhận:

```
Chuyên khoa → Bác sĩ theo CK → Ca làm việc khả dụng → TaoLichHenCommand
 (đã seed 6)    (chưa seed)       (chưa seed)          (handler xong)
```

Trong đó:
- **Chuyên khoa**: đã seed 6 chuyên khoa (Tim mạch, Nhi, Nội, Ngoại, TMH, Da liễu).
- **Danh sách bác sĩ theo chuyên khoa**: **Module 2 chủ sở hữu** — cần handler `DanhSachBacSiTheoChuyenKhoa`. Module 1 không nên tự viết handler đọc vì đây là domain của Module 2.
- **Danh sách ca làm việc khả dụng**: **Module 2 chủ sở hữu** — cần handler `DanhSachCaLamViecKhaDungCuaBacSi(idBacSi, tuNgay, denNgay)`. `ICaLamViecQueryService.LayThongTinCaAsync` hiện chỉ lấy 1 ca theo id, không list.
- **Tạo lịch hẹn**: `TaoLichHenCommand` + handler **đã xong** (Wave 3) — backend sẵn sàng.

**Quyết định**: không viết `DatLich` theo hướng "bypass Module 2 đọc thẳng `IAppDbContext`" — vì dropdown bác sĩ/ca là domain Module 2, viết tạm rồi vứt đi sẽ lãng phí. Chờ Module 2 ship:
1. Handler query danh sách bác sĩ theo chuyên khoa
2. Handler query ca làm việc khả dụng
3. Seed fixture CaLamViec + BacSi (hoặc coordinate Module 1 seed)

Khi Module 2 xong → viết `DatLich.cshtml(.cs)` với `IMediator.Send(query)` + gọi `TaoLichHenCommand` ở submit.

**Tạm thời**: sidebar `/BenhNhan/DatLich` vẫn link (404 khi bấm). Cân nhắc ẩn tab hoặc disable cho đến khi có Module 2 — để đó reminder cũng được.

### 5.5 Bug đã phát hiện & fix trong Wave 5

| Bug | File báo cáo | Root cause | Trạng thái |
|---|---|---|---|
| Login Web fail `IDX10703: key length is zero` | `docs/bao-cao-loi-dang-nhap-jwt-key.md` | `appsettings.json` Web có section `Jwt` với tên property sai (`SecretKey` thay vì `Key`, `AccessTokenExpiryMinutes` thay vì `AccessTokenExpirationMinutes`) → binding `IOptions<JwtSettings>` fail → `Key` rỗng | ✅ Fixed |
| 404 tại `http://localhost:5181/` | — | Orphan `Index.cshtml.cs` không có view | ✅ Fixed (tạo Index.cshtml) |
| Build fail: "BadgeHelper does not exist" × 7 file | — | Thiếu `@using ClinicBooking.Web.Helpers` trong `_ViewImports.cshtml` | ✅ Fixed |
| HTTP 400 khi POST `/Auth/DangXuat` | — | Form trong `_Layout.cshtml` dùng `action="/Auth/DangXuat"` (raw HTML) → không auto-inject antiforgery token | ✅ Fixed (dùng `asp-page`) |
| `/Auth/DangXuat` trả 404 | — | Có `.cs` nhưng chưa có `.cshtml` → Razor Pages không đăng ký route | ✅ Fixed |
| Warning `Failed to determine the https port` | — | `UseHttpsRedirection` bật ở dev nhưng launchSettings không có HTTPS port | ✅ Fixed (chỉ bật ở prod) |
| Admin bị redirect về `/LeTan/Dashboard` | — | Logic redirect cũ | ✅ Fixed (tạo `/Admin/Dashboard` placeholder riêng) |

### 5.6 Tài khoản test

| Vai trò | Tên đăng nhập | Mật khẩu | Landing page |
|---|---|---|---|
| Admin | `admin` | `Admin@123456` | `/Admin/Dashboard` (placeholder) |
| Lễ tân | `letan` | `Letan@123456` | `/LeTan/Dashboard` |
| Bệnh nhân | tự đăng ký qua `/Auth/DangKy` | tuỳ user | `/BenhNhan/DanhSachLichHen` |
| Bác sĩ | **chưa có seed** | — | — |

---

## Việc cần làm tiếp (theo thứ tự ưu tiên)

1. **Chờ Module 2** ship:
   - Handler `DanhSachBacSiTheoChuyenKhoa` (Query)
   - Handler `DanhSachCaLamViecKhaDungCuaBacSi` (Query)
   - Seed fixture: 2-3 bác sĩ + vài ca làm việc `DaDuyet` cho dev
2. Khi có Module 2 → viết `Pages/BenhNhan/DatLich.cshtml(.cs)` (wizard 3 bước).
3. Bổ sung **unit tests** chi tiết từng handler Wave 2/3 trước khi merge `develop` (xem note cuối Wave 2, Wave 3).
4. Cân nhắc ẩn tab "Đặt lịch mới" trong sidebar cho đến khi `DatLich` có thật — hoặc để đó làm reminder.
5. Dọn `Jwt:Key` trong `appsettings.json` (cả Api + Web) → chuyển sang User Secrets / env var trước khi deploy production. Xem `docs/bao-cao-loi-dang-nhap-jwt-key.md` mục 5 (action items).
6. **Wave 4** vẫn blocked trên team: Hangfire job, reconciliation job, integration test Testcontainers — không thể làm trước vì cần Module 2/4 real implementation.
