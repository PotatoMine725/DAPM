# Ke hoach Module 1 - giai doan tiep theo

Ngay lap: 2026-04-24
Nguoi lap: Claude (dong so huu Module 1)
Nhanh lam viec hien tai: `docs/module1-notes-wip`
Base: `origin/develop`

## 1) Tom tat tien do

Theo `docs/Module_1_plan.md` (ban goc Wave 1-4) va tinh trang tren `origin/develop`:

| Hang muc | Trang thai | Ghi chu |
|---|---|---|
| Wave 1 - Abstractions + stubs + migration + generator | DONE | `ICaLamViecQueryService`, `INotificationService`, `IMaLichHenGenerator`, `LichHenOptions`, migration `Module1_ThemRangBuocLichHen`, test project. |
| Wave 2 - Pure handler + non-critical stub (6+4 handler) | DONE | `GiaiPhongGiuCho`, `XemLichHen`, `DanhSachLichHenCuaToi`, `DanhSachLichHenTheoNgay`, `XemHangChoTheoCa`, `HoanThanhLuotKham`, `XacNhanLichHen`, `HuyLichHen`, `CheckInLichHen`, `GoiBenhNhanKeTiep`. |
| Wave 3 - Critical-path stub (3 handler) | DONE | `TaoLichHen`, `DoiLichHen`, `TaoGiuCho`. |
| Phan E - theo doi hang cho + lich hen theo ngay cua toi | DONE | `ThuTuCuaToi`, `DanhSachLichHenTheoNgayCuaToi`; PR #21 da merge. |
| Wave 4 - Integration / swap DI / Hangfire / concurrency tests | CHUA LAM | Phu thuoc Module 2 (swap DI) va Module 4 (Integration test project, Hangfire). |
| Background job cua Module 1 (theo `phan-chia-module-va-huong-dan-du-an.md` §3) | CHUA LAM | Quet `GiuCho` het han + tu dong chuyen `LichHen` sang `DaQuaHan`. Khong phu thuoc module khac. |
| UI Razor Pages `ClinicBooking.Web` | BONUS | Khong nam trong plan goc Module 1. Da co 3 trang benh nhan (DanhSachLichHen, LichHen, ThuTuHangCho). |
| Test data dev (seed) | DONE | Migration `Module1_TestDataSeed` + `Module1TestDataSeeder` da commit. |

**Ket luan**: phan code API cua Module 1 theo plan goc da xong toan bo Wave 1-3 + Phan E. Hai hang muc con lai nam trong scope ban dau ma khong phu thuoc team khac la:

1. Background job quet giu cho het han + chuyen lich hen sang `DaQuaHan`.
2. Smoke test end-to-end tren Swagger voi seed data.

Cac hang muc phu thuoc team khac (Wave 4) cho Module 2 / Module 4 merge.

## 2) Cac commit da thuc hien o giai doan nay (Phan A - phan loai local work)

Tren nhanh `docs/module1-notes-wip`:

| Commit | Mo ta |
|---|---|
| `2dc2d0b` | `fix(module1)`: giai quyet xung dot ten `LichHen` (namespace vs entity) trong `DanhSachLichHenTheoNgayCuaToiHandler`. |
| `9d52259` | `chore(module1)`: them seed test data Module 1 (dev only), migration `Module1_TestDataSeed`. |
| `c27fb5d` | `feat(web)`: them trang chi tiet lich hen + thu tu hang cho cho benh nhan + chuan hoa CSS helper. |
| `e981e8e` | `docs(module1)`: cap nhat plan, tien do va bao cao fix hang cho + seed. |

Cac file khong commit (co y): `CLAUDE.md` (GitNexus auto stats), `.mcp.json`, `.opencode.json`, `AGENTS.md`.

## 3) Ke hoach trien khai tiep

### Giai doan B - Background job Module 1 (khong phu thuoc ai, in-scope)

Theo plan phan chia module §3: *"Background job: quet giu cho het han, dong bo trang thai lich hen sang da_qua_han neu ca ket thuc."*

#### B1 - Job `QuetGiuChoHetHan`

**Muc tieu**: moi phut, danh dau cac ban ghi `GiuCho` co `GioHetHan < UtcNow` va `DaGiaiPhong == false` sang `DaGiaiPhong = true`, `LyDoGiaiPhong = "HetHan"`.

**Triangle hoat**:
- Tao `ClinicBooking.Infrastructure/BackgroundJobs/QuetGiuChoHetHanJob.cs`.
- Implement `IHostedService` voi `PeriodicTimer(TimeSpan.FromMinutes(1))` (khong dung Hangfire o giai doan nay - giu doc lap voi Module 4).
- Logic: `_db.GiuCho.Where(x => !x.DaGiaiPhong && x.GioHetHan < now).ExecuteUpdateAsync(s => s.SetProperty(x => x.DaGiaiPhong, true).SetProperty(x => x.LyDoGiaiPhong, LyDoGiaiPhong.HetHan))`.
- Log so ban ghi da quet qua `ILogger<QuetGiuChoHetHanJob>`.
- Dang ky qua `services.AddHostedService<QuetGiuChoHetHanJob>()` trong `Infrastructure/DependencyInjection.cs`.
- Co option `LichHenOptions.BackgroundJob.QuetGiuChoPhut` (default 1) de tuy chinh.

**Luu y**: khi Module 4 swap sang Hangfire, chi can remove `AddHostedService` va dang ky job Hangfire tuong duong. Logic khong thay doi.

**Test**:
- Unit test logic query tren SQLite in-memory.
- Smoke: tao 1 `GiuCho` voi `GioHetHan = UtcNow - 1 phut`, doi 60s, kiem tra `DaGiaiPhong = true`.

#### B2 - Job `ChuyenLichHenDaQuaHan`

**Muc tieu**: moi 30 phut, chuyen cac `LichHen` co ca lam viec da ket thuc > 1h ve truoc, trang thai van la `ChoXacNhan` / `DaXacNhan` sang `DaQuaHan` (enum da co).

**Triangle hoat**:
- Tao `ClinicBooking.Infrastructure/BackgroundJobs/ChuyenLichHenDaQuaHanJob.cs`.
- Implement `IHostedService` voi `PeriodicTimer(TimeSpan.FromMinutes(30))`.
- Logic: join `LichHen` voi `CaLamViec`, filter `CaLamViec.NgayLamViec + CaLamViec.GioKetThuc < UtcNow - 1h` va `LichHen.TrangThai IN (ChoXacNhan, DaXacNhan)`.
- `ExecuteUpdateAsync` set `TrangThai = DaQuaHan`; insert `LichSuLichHen(HetHan)` cho moi ban ghi (can loop vi `ExecuteUpdateAsync` khong ghi phu).
- Log so ban ghi.

**Luu y**: neu so luong ban ghi moi lan chay lon, can batch hoac doi sang Hangfire (Module 4). Giai doan nay gia dinh volume thap.

**Test**: tuong tu B1 voi fixture lich hen + ca lam viec trong qua khu.

#### B3 - Smoke test end-to-end tren Swagger

**Muc tieu**: voi seed data da co, chay mot vong day du cac endpoint Module 1 va xac nhan state chuan.

**Kich ban**:
1. Login `patient001` → `GET /api/lich-hen/cua-toi` → ky vong 2 ban ghi (`4001`, `4002`).
2. Login `receptionist001` → `POST /api/lich-hen/4002/xac-nhan` → `TrangThai = DaXacNhan`.
3. `POST /api/lich-hen/4002/check-in` → tao `HangCho` moi, `SoThuTu = 2` (vi `4001` da co `SoThuTu = 1`).
4. Login `doctor001` → `POST /api/hang-cho/goi-ke-tiep` voi `idCaLamViec = 3001` → chuyen `1` → `DangKham`.
5. `POST /api/hang-cho/{id}/hoan-thanh` → `HoanThanh`.
6. Goi tiep → chuyen `2` → `DangKham`.
7. Verify `LichSuLichHen` ghi day du.

**Deliverable**: `docs/smoke-test-module1.md` ghi lai ket qua tung buoc + screenshot status code.

### Giai doan C - Cho team (Wave 4 blocked)

#### C1 - Swap DI stub sang real service (cho Module 2)

Khi Module 2 push commit `ICaLamViecQueryService` real implementation (co `IncrementSoSlotDaDatAsync` atomic):
- Xoa `CaLamViecQueryServiceStub` (hoac giu lai de dev fallback, co flag).
- Doi registration trong `Infrastructure/DependencyInjection.cs`: `services.AddScoped<ICaLamViecQueryService, CaLamViecQueryService>()`.
- Chay lai toan bo unit test + smoke test.

#### C2 - Integration test + Testcontainers (cho Module 4)

Khi Module 4 lap project `ClinicBooking.Integration.Tests`:
- Them fixture `WebApplicationFactory<Program>` voi Testcontainers SQL Server.
- Viet 2 test concurrency:
  - 2 request `POST tao-lich-hen` song song cung `IdCaLamViec` (gan het slot) → 1 thanh cong, 1 nhan 409.
  - 2 request `POST huy` song song cung `IdLichHen` → 1 thanh cong, 1 nhan 409 do `RowVersion`.

#### C3 - Hangfire / Module 4 swap

Khi Module 4 merge Hangfire:
- Chuyen `QuetGiuChoHetHanJob` va `ChuyenLichHenDaQuaHanJob` tu `IHostedService` sang Hangfire recurring job.
- Chuyen `NotificationServiceStub` sang implementation that cua Module 4 (email/SMS/in-app).

### Giai doan D - UI Module 1 (bonus, ngoai scope goc)

**Da co**:
- `BenhNhan/DanhSachLichHen`, `BenhNhan/LichHen`, `BenhNhan/ThuTuHangCho`.

**Nen lam them neu co thoi gian** (priority thap, sau B/C):
- `LeTan/QuanLyLichHen`: xem danh sach ngay, xac nhan, huy, check-in.
- `BacSi/HangChoCuaToi`: xem hang cho, goi tiep, hoan thanh.
- `Admin/ThongKeLichHen`: dashboard don gian.
- Cai thien UX: them nut "Xem thu tu" tu `DanhSachLichHen` truyen `idCaLamViec` hop le.

## 4) Thu tu trien khai de nghi

1. **Ngay hom nay**: B1 (job quet giu cho) - 2-3h.
2. **Ngay mai**: B2 (job lich hen qua han) + B3 (smoke test) - 3-4h.
3. **Cho Module 2/4 merge**: C1, C2, C3 (khong autonomous duoc).
4. **Thoi gian rong**: D (UI cho le tan, bac si, admin).

## 5) Ghi chu

- Nhanh `docs/module1-notes-wip` hien chua commit cua A (fix/seed/UI/docs/plan). Sau khi hoan tat B, can:
  - Tach commit fix (`2dc2d0b`) sang nhanh `fix/module1/danh-sach-theo-ngay-cua-toi` de PR rieng vao `develop`.
  - Tach commit job (B1/B2) sang nhanh `feature/module1/background-jobs` de PR rieng.
  - UI + seed co the giu lai `docs/module1-notes-wip` (WIP) hoac tach sau.
- Khong dung `.mcp.json` va `.opencode.json` - giu nguyen cau hinh plugin.
- Khong merge truc tiep vao `develop` - luon qua PR + review.
