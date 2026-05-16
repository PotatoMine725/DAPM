# Ke hoach hoan thien he thong + thoat moi truong dev

**Ngay lap:** 2026-05-16
**Nguoi lap:** Module 1 owner (Potatomine725)
**Pham vi:** Ra soat phan con lai sau khi ban giao `Admin/Accounts` cho Module 4 (commit `afd73e6`, `996d425`).
**Branch hien tai:** `feature/module1/portal-sat-demo`

---

## 1. Hien trang (verified 2026-05-16)

### 1.1. Da hoan thien va wire data that

| Module | Phan da xong |
|---|---|
| M1 | Toan bo flow dat / huy / doi lich + hang cho + LeTan/HoSoBenhNhan + BacSi portal overhaul + OTP email walk-in |
| M2 | `ICaLamViecQueryService` impl that, scheduling commands/queries day du. Admin pages: `BacSi`, `CaLamViec`, `LichNoiTru`, `DuyetCa`, `Phong`, `ChuyenKhoa`, `DichVu` da wire MediatR |
| M3 | Application layer `HoSoKham` + `ToaThuoc` day du. UI BenhNhan (`LichSuKham`, `ToaThuoc`, `ThongBao`) + BacSi (`QuanLyKham`, `HoSo`, `LichLamViec`, `NghiPhep`, `YeuCauTaoCa`, `DanhSach`) da wire |
| M4 | `EmailService`, `OtpService`, `NotificationService` THAT da register trong `Infrastructure/DependencyInjection.cs:64-66` (`Stub*` con file nhung khong dung) |

### 1.2. Stub / mock con sot

| Item | File | Phai lam gi |
|---|---|---|
| `Admin/Accounts` | `Web/Pages/Admin/Accounts.cshtml.cs` (14 dong stub) | Da ban giao M4 — `docs/Requests/yeu-cau-module4-admin-accounts.md` |
| `Admin/Dashboard` | `Web/Pages/Admin/Dashboard.cshtml.cs` con class `MockDashboardStatistics` | M4 / M1 |
| `Admin/ThongKe` | 14 dong stub | M4 |
| `Admin/ThongBao` | 14 dong stub | M4 |
| `NotificationService` | hardcode `IdMau = 1..6` (`Infrastructure/Services/Notifications/NotificationService.cs`) | M4 — seed bang `MauThongBao` va tra cuu theo enum |
| `OtpServiceStub.cs`, `NotificationServiceStub.cs`, `EmailSenderStub.cs` | `Infrastructure/Services/...` | M4 — xoa khi confirm khong test nao import |
| `[OTP-DEV]` console fallback | `Infrastructure/Services/Security/OtpService.cs` | M4 — gate theo `IHostEnvironment.IsDevelopment()` chinh xac |

### 1.3. Rui ro moi truong dev "ro ri" ra prod

| # | Issue | File | Muc do |
|---|---|---|---|
| D1 | Gmail App Password (`iipgcgmjtslsdjes`) o `appsettings.Development.json` (uncommitted, `M`) | `ClinicBooking.Api/appsettings.Development.json` | 🔴 Khong duoc commit |
| D2 | `Admin:MatKhauMacDinh: Admin@123456` + `DevFixture:MatKhauChung: Demo@123456` | `appsettings.Development.json` | 🔴 Roi vao prod la mat tk admin |
| D3 | `DevFixture.Enabled = true` seed luon admin/le_tan/bac_si/benh_nhan | `Infrastructure/Persistence/DatabaseSeeder.cs:82+` | 🟠 OK voi dev, MUST disable prod |
| D4 | Walk-in ghost email `walkin_{sdt}@local.invalid` | code dat lich + `LienKetHoSo` | 🟡 Co LienKetHoSo flow → OK, tai lieu hoa |
| D5 | Seed `CaLamViec` ID 3001-3003 | `DatabaseSeeder` | 🟡 Idempotent, OK dev / khong sinh prod |
| D6 | Background jobs `IHostedService` (chua chuyen Hangfire) | `Infrastructure/DependencyInjection.cs:73-74` | 🟡 Chay duoc, nhung scale-out se lap |
| D7 | Khong co `Dockerfile` / `docker-compose.yml` / GitHub Actions | repo root | 🟠 Khong deploy duoc |
| D8 | File `nul` o repo root (rac do PowerShell `2>nul`) | `./nul` | 🟢 Xoa |
| D9 | Folder `code-review-graph/` untracked | repo root | 🟢 Add `.gitignore` neu local tool, hoac xoa |
| D10 | `*.csproj.lscache` untracked toan project | 7 file | 🟢 Them vao `.gitignore` |
| D11 | Production `appsettings.json` chua tach key SMTP/JWT bi mat | `Api/appsettings.json` | 🟠 Doc keys phai dung env / user-secrets / Azure Key Vault |
| D12 | `LienKetHoSo` hien thi OTP qua `TempData` o dev fallback | `OtpService` + page | 🟠 Verify khong leak khi `ASPNETCORE_ENVIRONMENT=Production` |

---

## 2. Cong viec M1 owner can lam ngay (1 ngay)

### 2.1. Don rac repo (15 phut)

```powershell
git rm -f nul
# Them vao .gitignore:
echo "*.csproj.lscache" >> .gitignore
echo "code-review-graph/" >> .gitignore
echo ".rtk/" >> .gitignore
echo ".mcp.json" >> .gitignore  # neu khong share
```

### 2.2. Tach secrets ra user-secrets (30 phut)

```powershell
cd ClinicBooking.Api
dotnet user-secrets init
dotnet user-secrets set "EmailSettings:Username" "wgspartan7@gmail.com"
dotnet user-secrets set "EmailSettings:Password" "iipgcgmjtslsdjes"
# Roi sua appsettings.Development.json — xoa Username/Password chi giu Host/Port
git checkout -- ClinicBooking.Api/appsettings.Development.json  # neu password chua tu modify
```

> Sau khi tach: chay lai `dotnet run --project ClinicBooking.Api` de verify OTP email van gui duoc.

### 2.3. Tao `appsettings.Production.json` template (15 phut)

```json
{
  "Logging": { "LogLevel": { "Default": "Information", "Microsoft.AspNetCore": "Warning" } },
  "Admin": {
    "DevFixture": { "Enabled": false }
  },
  "EmailSettings": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "EnableSsl": true,
    "TenHienThi": "ClinicBooking"
  }
}
```

`Admin:MatKhauMacDinh`, `EmailSettings:Username/Password`, `ConnectionStrings:DefaultConnection`, `Jwt:Key` → set qua env var hoac Azure Key Vault.

### 2.4. Verify dev-only paths (1h)

- Grep `IHostEnvironment.IsDevelopment` → moi `[OTP-DEV]` console log + `TempData["MaOtpDev"]` (neu co) phai gated.
- Add integration test `OtpService_Should_NotLeakOtp_In_Production`.

### 2.5. Reset seed admin password sau deploy (doc-only)

Vao `docs/SETUP-GUIDE.md` them muc: "Sau lan dau dang nhap prod voi `Admin@123456` (set qua env), DOI ngay qua `/Admin/Accounts` (sau khi M4 wire) hoac `dotnet user-secrets set Admin:MatKhauMacDinh ...`".

---

## 3. Phan giao Module — chi tiet

### 3.1. Module 2 — Member B (Lich lam viec / Danh muc)

**Branch de xuat:** `feature/module2/scheduling-finishing`
**Request chi tiet:** `docs/Requests/yeu-cau-module2-hoan-thien-scheduling.md`

**Viec con lai:**

1. 🟠 **Verify ownership `CaLamViecQueryService`** — bo sung `ChayReconSlotAsync` + atomic SQL `IncrementSoSlotDaDatAsync`.
2. 🟠 **CRUD Nghi phep duyet** — `DuyetDonNghiPhepCommand` + page `Admin/NghiPhep`. Khi duyet: kiem tra ca trung ngay nghi, neu co BN da dat → throw `ConflictException`.
3. 🔴 **Validate xung dot lich BS** (MOI) — `ICaLamViecConflictChecker` check BS double-book, phong double-book, trung don nghi phep da duyet. Wire vao `TaoCaLamViec`/`DuyetCaLamViec`/`SinhCaLamViecTuLichNoiTru`.
4. 🟠 **Canh bao thieu BS tai slot** (MOI) — `KiemTraDoPhuBacSiQuery` + widget `Admin/Dashboard` + banner UI le_tan/benh_nhan khi ngay khong co BS chuyen khoa.
5. 🟢 **`SinhCaLamViecTuLichNoiTruHandler` bug** — fix dieu kien trung them `IdDinhNghiaCa`.
6. 🟢 **API CaLamViec public** — verify `SchedulingController` co `GET/POST/PUT/DELETE`.

**Test thu cong end-to-end:**
- Admin tao 2 ca cung BS cung gio → ca thu 2 bi reject voi message "BS X da co ca khac luc ...".
- BS hop dong nop don nghi → admin duyet → ca ngay do bi block.
- Chuyen khoa khong co BS lich T7 → BN vao DatLich thay calendar mark T7 xam, le_tan thay banner do.

---

### 3.2. Module 3 — Member C (Ho so kham / Toa thuoc)

**Branch de xuat:** `feature/module3/finalize-medical-records`
**Request chi tiet:** `docs/Requests/yeu-cau-module3-finalize-medical.md`

**Viec con lai:**

1. 🔴 **Enforce "1 BN = 1 ho so + N benh an"** (MOI) — them unique index `BenhNhan.IdTaiKhoan` + `HoSoKham.IdLichHen`; (optional) denormalize `HoSoKham.IdBenhNhan`; validator `TaoHoSoKham` chan duplicate; trang `LeTan/HoSoCaNhan` gop profile + timeline benh an.
2. 🟠 **Chot logic `SoLanHuyMuon`** — nguong cam dat lich + reset chu ky + query `KiemTraQuyenDatLich`. Bao M1 owner de wire validator vao `TaoLichHen`.
3. 🟠 **CRUD Thuoc trong `Admin/QuanLyDanhMuc`** — verify, neu chua wire thi bo sung.
4. 🟡 **Trang tra cuu benh an le tan** — `LichSuKhamTheoBenhNhanQuery` + UI `LeTan/TraCuuBenhAn`.
5. 🟡 **Panel lich su kham BS/QuanLyKham** — reuse query tren, limit 5, render timeline ben phai.

**Test thu cong:**
- BN huy lich 5 lan thang → lan thu 6 bi tu choi (sau khi M3 chot logic).
- Le_tan vao trang tra cuu, search SDT → ra lich su kham + don thuoc.

---

### 3.3. Module 4 — Member D (Thong bao / Admin / Ha tang)

**Branch de xuat:** `feature/module4/admin-and-ops-finish`

#### 3.3.1. Priority CAO (1 tuan)

1. 🔴 **`Admin/Accounts` wire** — theo `docs/Requests/yeu-cau-module4-admin-accounts.md`. Lam **truoc**.
2. 🔴 **Seed `MauThongBao` + sua `NotificationService` hardcode `IdMau`** — hien `IdMau = 1..6` la magic number, neu DB seed thieu se FK fail:
   - Tao migration `SeedMauThongBaoMacDinh` (6 record cho 6 event)
   - Doi `IdMau = 1` → enum `LoaiMauThongBao.TaoLichHen` → tra cuu `_db.MauThongBao.FirstAsync(x => x.Loai == ...)`
3. 🔴 **`Admin/ThongBao` wire** — broadcast notification:
   - `Features/ThongBao/Commands/GuiThongBaoBroadcast/` (target: All / BacSi / BenhNhan / LeTan)
   - CRUD `MauThongBao` (admin sua tieu de/noi dung template, ho tro variable substitution `{TenBenhNhan}`, `{NgayHen}`)

#### 3.3.2. Priority TRUNG (1 tuan)

4. 🟠 **`Admin/Dashboard` thay mock data** — query that tu DB:
   - `Features/Admin/Queries/ThongKeTongHopAdmin/` (8 query song song qua `Task.WhenAll`)
   - Field: `LichHomNay`, `LichTuanNay`, `SoBacSiHoatDong`, `SoBenhNhanMoiTuan`, `SoCaChoDuyet`, `SoToaThuocChoCap`, `DoanhThuTuanNay = 0` (cho M3 hoa don), `DoanhThuThangNay = 0`
5. 🟠 **`Admin/ThongKe` wire** — bao cao chi tiet:
   - `Features/Admin/Queries/BaoCaoThongKe/` (`LoaiBaoCao = LichHen | BacSi | ChuyenKhoa | DichVu`)
   - UI: filter ngay + dropdown LoaiBaoCao + bang + chart Chart.js
6. 🟠 **Dockerize** — `Dockerfile` cho `Api` + `Web` + `docker-compose.yml` voi SQL Server + Mailhog (de test SMTP local khong gui mail that).

#### 3.3.3. Priority THAP (sau MVP)

7. 🟡 **Hangfire migration** — chuyen `QuetGiuChoHetHanJob` + `ChuyenLichHenDaQuaHanJob` tu `IHostedService` sang recurring job. Them job moi: nhac lich truoc 1h, cleanup OtpLog >24h, `ChayReconSlotAsync` 15 phut/lan.
8. 🟡 **CI/CD GitHub Actions** — `.github/workflows/build-test.yml`:
   ```yaml
   on: [push, pull_request]
   jobs:
     build:
       runs-on: ubuntu-latest
       steps:
         - uses: actions/checkout@v4
         - uses: actions/setup-dotnet@v4
           with: { dotnet-version: '8.0.x' }
         - run: dotnet restore DatLichPhongKham.slnx
         - run: dotnet build --no-restore
         - run: dotnet test --no-build --verbosity normal
   ```
9. 🟡 **Auth bo sung** — `quen-mat-khau` + `dat-lai-mat-khau` qua OTP email (reuse `IOtpService` voi `MucDichOtp` moi).
10. 🟢 **Don file stub** — sau khi confirm `OtpServiceStub.cs`, `NotificationServiceStub.cs`, `EmailSenderStub.cs` khong duoc import o test nao, xoa file (don 3 file).

---

## 4. Trat tu uu tien tong (de timeline)

```
Week 1 (2026-05-17 → 2026-05-23) — DATA INTEGRITY
- M1 owner: §2.1-2.4 don rac + tach secret + production template — DONE 2026-05-16
- M4: Admin/Accounts + Seed MauThongBao
- M2: Validate xung dot BS/phong/nghi phep (§3.1.3) + Verify CaLamViecQueryService (§3.1.1)
- M3: Enforce 1 BN = 1 ho so + N benh an (§3.2.1) + Chot SoLanHuyMuon (§3.2.2)

Week 2 (2026-05-24 → 2026-05-30) — UX + BUSINESS FLOW
- M4: Admin/ThongBao + Admin/Dashboard + Admin/ThongKe
- M2: Nghi phep flow Admin/NghiPhep (§3.1.2) + Canh bao thieu BS (§3.1.4) + fix SinhCa (§3.1.5)
- M3: CRUD Thuoc + Tra cuu benh an le_tan + panel BS/QuanLyKham (§3.2.3-5)

Week 3 (2026-05-31 → 2026-06-06) — INFRA + DEPLOY
- M4: Dockerize + CI/CD + Hangfire (neu kip)
- All: regression test toan he thong, prepare staging deploy
```

---

## 5. DoD ra mat staging

- [ ] Khong stub nao register trong `Infrastructure/DependencyInjection.cs`
- [ ] Khong page `.cshtml.cs` nao co class `Mock*` hoac `data-mock`
- [ ] `appsettings.Development.json` khong chua secret (toan bo qua user-secrets)
- [ ] `appsettings.Production.json` ton tai, `DevFixture.Enabled = false`
- [ ] `Admin:MatKhauMacDinh` doc tu env var trong prod
- [ ] `dotnet test` toan solution: 0 failure
- [ ] `dotnet build` 0 warning
- [ ] `docker-compose up` chay duoc tu repo sach (chi can `.env`)
- [ ] Integration test login + dat lich + check-in + kham + ke don E2E pass
- [ ] OTP email gui that qua Gmail (test thu 1 lan)
- [ ] Background job `QuetGiuChoHetHanJob` chay it nhat 1 lan (xem log)

---

## 6. Tham chieu

- `docs/Plans/m1-wire-up-admin.md` — tien do wire admin
- `docs/Requests/yeu-cau-module4-admin-accounts.md` — ban giao M4 (gan nhat)
- `docs/Plans/yeu-cau-module-2-3-4-hoan-thien-he-thong.md` — yeu cau goc 2026-05-05 (file nay update)
- `docs/Requests/yeu-cau-module2-implement-calamviecqueryservice.md`
- `docs/Requests/yeu-cau-module3-ho-tro-module1.md`
- `docs/Requests/yeu-cau-module4-ho-tro-module1.md`
- `docs/huong-dan-setup-gmail-otp.md` — huong dan SMTP credential
