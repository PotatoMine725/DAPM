# Tien do — Ra soat quyen truy cap truoc khi sang front-end

**Branch**: `feature/module1/phan-a-quyen-truy-cap` (da rebase len `origin/develop` @ `dc75866`)
**Cap nhat**: 2026-04-23

---

## Da hoan thanh

### Phan A — Fix quyen truy cap don gian (commit `a91c065`, rebased tu `a973e8f`)
- Xoa `TestController` (endpoint debug khong bao ve).
- Sua `ThuocController`:
  - Class: `[Authorize]` (thay vi `[Authorize(Roles = Admin)]`).
  - Write (POST/PUT/DELETE): `Admin` only.
  - Read (GET list, GET by id): `Admin` + `BacSi` (truoc day chan `BacSi`).

### Phan B — Ownership audit handler Module 1 (commit `67fdbd7`, rebased tu `a334dac`)
- **Audit xong 5 handler**, ket luan:
  - `XemLichHen`, `DoiLichHen` (Module 1) — da co ownership check, khong can sua.
  - `LayHoSoKhamById`, `LayToaTheoHoSoKham` (Module 3) — da co ownership check, khong can sua.
  - `GoiBenhNhanKeTiep` — **thieu**, da them.
  - `HoanThanhLuotKham` — **thieu**, da them.
- `GoiBenhNhanKeTiep`: BacSi gate theo ca phu trach; LeTan/Admin khong rang buoc (ho dieu phoi quay).
- `HoanThanhLuotKham`: strict BacSi-only (chi bac si phu trach ca moi hoan thanh duoc).
- Them 2 test Forbidden cho moi handler; cap nhat 4 test cu dung `ICurrentUserService` mock.
- Ket qua: **15/15 HangCho tests xanh**.

### Review Module 2 lan 1 (commit `da2f172`, rebased tu `7513897`)
- File: `docs/review-module2-endpoint-cong-khai.md` — review `5dc7ca6`.
- **Chu Module 2 xu ly het 5 blocker o commit `cb706e4` + rename controller.** Chi tiet: xem review doc.
- ~~Chu Module 2 chua xu ly~~: namespace folder + rate limit / cache-control → scope ha tang, defer.

### Handoff Module 2 lan 2 (commit `4f2f628`, rebased tu `71866fa` + enhance)
- File: `docs/nhiem-vu-module2-tiep-theo.md` — nhiem vu tiep theo sau khi pull `cb706e4`.
- Muc §0: ngu canh nghiep vu 2 loai bac si (`NoiTru`/`HopDong`) + `NguonTaoCa` (`TuDong`/`BacSiDangKy`) — thong nhat voi CLAUDE.md §11 naming.
- Muc §1.1 + §1.2: 2 loi can fix (enum `ChinhThuc`/`Admin` khong ton tai, hardcode `IdTaiKhoan` gay FK fail).
- Muc §2.1: optional — them endpoint cong khai `ChuyenKhoa`/`DichVu` cho Guest.
- **Trang thai fix cua chu Module 2** (tren nhanh `origin/feature/module2-doctors-scheduling`, chua merge develop):
  - ✅ §1.1 enum fix — `f674394 test(module2): fix public listing tests for safe enums and seeded keys`.
  - ✅ §1.2 FK seed fix — cung commit `f674394`.
  - ✅ §2.1 public ChuyenKhoa + DichVu — `c765f76 feat(module2): add public danh muc endpoints for booking flow`.
  - ✅ Bonus vuot scope:
    - `TaoBacSi`/`CapNhatBacSi`/`XoaBacSi`/`DanhSachBacSi` — internal BacSi CRUD cho Admin (`de68d09`).
    - `LayHoSoCuaToi` — BS xem ho so cua minh.
    - **`CaLamViecQueryService` thuc te** (`de68d09`) — khong con la stub, du impl real abstraction.
    - `HoSoBacSi` — endpoint profile bac si.
- Viec con lai (nho chu Module 2): PR nhanh `feature/module2-doctors-scheduling` → `develop` de merge 5 commit sau `5dc7ca6`.

### Rebase len develop (2026-04-23)
- `origin/develop` da co: abstractions `ICaLamViecQueryService` + DTOs + `CaLamViecQueryServiceStub` + `NotificationServiceStub` (qua PR #15, #16, #17).
- Patch tam `a60922a` cua minh da bi supersede — tuy rebase standard replay lai (commit `45dfb05`) vi cb706e4 chua len develop. Khi Module 2 PR cac commit moi (bao gom `f674394`) len develop, se rebase lai lan nua va drop.
- Backup branch: `backup/phan-a-before-rebase` giu nguyen trang thai truoc rebase.

---

## Con lai

### Phan C — Endpoint cong khai ChuyenKhoa + DichVu — **~~BI HUY~~ (chu Module 2 lam roi)**

- `c765f76 feat(module2): add public danh muc endpoints for booking flow` da lam xong.
- Handler + DTO + controller endpoint da co. Khong can Module 1 lam.

### Phan D — Doi Module 2 phan hoi — **DA XONG (lan 2)**

- ✅ Chu Module 2 da xu ly het §1.1, §1.2, §2.1 cua handoff `nhiem-vu-module2-tiep-theo.md`.
- Viec con lai: doi chu Module 2 PR cac commit `f674394`, `c765f76`, `544d21c`, `de68d09`, `d12f55b` vao develop. Sau do minh rebase them 1 lan, drop patch `45dfb05`.

### Phan E — Endpoint dac thu theo vai tro (2h, van con)

- `GET api/hang-cho/thu-tu-cua-toi/{idCaLamViec}` cho `BenhNhan` — xem so thu tu cua minh trong hang cho.
- `GET api/lich-hen/theo-ngay/cua-toi?ngay=yyyy-MM-dd` cho `BacSi` — xem lich hen trong ngay cua minh.
- Scope Module 1 (handler + controller o module hang-cho/lich-hen).

### Nhanh moi phat hien: `feature/module4-week1-email-integration`

Chu Module 4 da push nhanh rieng. Nhanh nay chua rebase len develop moi nhat → diff hien tai **dang xoa** cac file cua Module 2. Can dan chu Module 4 rebase truoc khi PR, neu khong se revert work cua Module 2.

Noi dung huu ich:
- `Application/Abstractions/Notifications/IEmailSender.cs` (moi).
- Update `INotificationService` + `NotificationServiceStub`.
- `Infrastructure/Services/Notifications/{EmailSenderStub, EmailSettings}`.
- Project moi `ClinicBooking.IntegrationTests` (voi `TestWebAppFactory`, auth integration tests).
- `Dockerfile`, `docker-compose.yml`, `env.example`, `.github/workflows/ci.yml`, `docs/module4-week1.md`.

→ Khi nay merge xong, **Wave 1 cua plan Module 1 (abstractions + stub + integration test project) hau nhu da duoc dap ung boi Module 2 + Module 4**. Minh co the skip Wave 1 phan lon va nhay sang Wave 2 (handler `TaoLichHen`, `HuyLichHen`, etc.).

### Pre-existing test failures (khong chan front-end)

- **11 tests Thuoc + ToaThuoc failing tren develop** (da flag tu truoc). Can debug rieng khi co thoi gian.
- Lay log: `dotnet test ClinicBooking.Application.UnitTests --filter "FullyQualifiedName~Thuoc|FullyQualifiedName~ToaThuoc"`.

---

## State branch

- Current branch: `feature/module1/phan-a-quyen-truy-cap` — clean tree, da rebase len `origin/develop`, **chua push**.
- 11 commit tren develop:
  - `f114057` docs: them nhac nho vi pham nguyen tac code Module 2
  - `b0f4944` docs: them ma tran quyen truy cap API
  - `2090b24` docs: lam mem ton ngu + them ngu canh `nhac-nho-module2`
  - `a91c065` refactor(api): phan A — fix quyen truy cap don gian
  - `67fdbd7` test(security): verify ownership checks cho handler module 1
  - `da2f172` docs: them review endpoint cong khai Module 2
  - `f788499` docs: luu tien do truoc khi chuyen sang front-end
  - `6169aa6` docs: cap nhat tien do sau khi Module 2 fix review
  - `d588fa4` fix(module2): harden public doctor and schedule endpoints (replay cua `cb706e4`)
  - `45dfb05` fix(tests): module 2 public endpoint tests bien dich + chay (replay `a60922a` — se drop khi Module 2 PR `f674394`)
  - `4f2f628` docs: nhiem vu tiep theo cho chu Module 2
- Backup: `backup/phan-a-before-rebase` (truoc khi rebase, de phong).
---

## Next actions (thu tu uu tien khi quay lai)

### Uu tien 1 — Khong blocker, co the lam ngay (khong can doi ai)

- [ ] **Push nhanh + tao PR `feature/module1/phan-a-quyen-truy-cap` → `develop`** cho Phan A + B + docs. Dung doi chu Module 2 — 2 commit replay (`d588fa4`, `45dfb05`) se duoc GitHub xu ly sach khi merge (va phia Module 2 merge sau se khong conflict vi cung noi dung).
- [ ] Chay `dotnet test ClinicBooking.Application.UnitTests --filter "FullyQualifiedName~HangCho"` truoc khi push, xac nhan 15/15 xanh.

### Uu tien 2 — Sau khi Module 2 PR `d12f55b` vao develop

- [ ] `git fetch origin && git rebase origin/develop` tren nhanh hien tai → drop `d588fa4` + `45dfb05` (2 replay cua Module 2).
- [ ] Force-push neu PR da tao (kem `--force-with-lease`); hoac merge PR truoc roi rebase branch moi.

### Uu tien 3 — Bat dau Wave 2 plan Module 1 (main dev work)

**Context**: Wave 1 cua plan `docs/Module_1_plan.md` ban goc yeu cau tu tao `ICaLamViecQueryService` + `INotificationService` + stub. Nhung develop gio **da co san 100%** (qua Module 2 PR + sap co them tu Module 4). Nen Wave 1 chi con:

- [ ] Migration `Module1_ThemRangBuocLichHen`: unique index `(IdCaLamViec, SoSlot)` tren `LichHen`, `RowVersion` byte[] tren `LichHen`, unique `(IdCaLamViec, SoThuTu)` tren `HangCho`, composite index `(IdCaLamViec, TrangThai, SoThuTu)` cho "goi tiep", index `GiuCho(IdCaLamViec, DaGiaiPhong, GioHetHan)`, index `LichSuLichHen(IdLichHen)`.
- [ ] `LichHenOptions` binding tu `appsettings.json` section `"LichHen"` (`HuyMuonTruocGio`, `GiuChoThoiHanPhut`, `MaLichHenPrefix`).
- [ ] `IMaLichHenGenerator` — format `LH-{yyyyMMdd}-{seq6}`, dung `MAX(MaLichHen)` + parse + 1, retry 3 lan.
- [ ] Test project `ClinicBooking.Application.UnitTests` scaffold (neu chua co — verify da ton tai).

Sau do vao **Wave 2** handler (uu tien theo plan):

1. `TaoLichHen` (command) — critical path, dung `ICaLamViecQueryService.IncrementSoSlotDaDatAsync` + unique index backstop.
2. `HuyLichHen` (command) — notification fire-and-forget + decrement counter.
3. `XacNhanLichHen` (command).
4. `CheckInLichHen` (command) — insert `HangCho` voi `SoThuTu = MAX+1` retry.
5. Query handlers: `XemLichHen`, `DanhSachLichHenCuaToi`, `DanhSachLichHenTheoNgay`.
6. `DoiLichHen`, `TaoGiuCho`, `GiaiPhongGiuCho`.

### Uu tien 4 — Phan E (endpoint dac thu vai tro)

- [ ] `GET api/hang-cho/thu-tu-cua-toi/{idCaLamViec}` cho `BenhNhan`.
- [ ] `GET api/lich-hen/theo-ngay/cua-toi?ngay=yyyy-MM-dd` cho `BacSi`.
- Co the lam ngay sau Phan A+B merge, truoc hoac song song Wave 2 tuy nhu cau front-end.

### Uu tien 5 — Cleanup (khi co thoi gian, khong chan gi)

- [ ] Debug 11 test Thuoc/ToaThuoc fail tren develop (pre-existing, co the bi anh huong boi work cua Module 3).
- [ ] Xem xet chuyen nhanh Module 4 (`feature/module4-week1-email-integration`) — nhac chu Module 4 rebase truoc PR.
