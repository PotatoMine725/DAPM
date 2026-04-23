# Nhiem vu tiep theo cho Module 2

**Ngay giao**: 2026-04-22
**Nguoi giao**: owner Module 1
**Nhanh hien tai cua ban**: `feature/module2-doctors-scheduling` (commit moi nhat `cb706e4`)

Chao ban,

Cam on ban da xu ly nhanh 5 blocker trong review `docs/review-module2-endpoint-cong-khai.md` — tat ca fix deu chuan, san sang merge vao `develop`. Truoc khi ban PR vao `develop`, co 2 viec can lam khan va 1 viec mo rong scope (optional). Minh tach ro theo do uu tien.

---

## 0. Ngu canh nghiep vu — 2 loai bac si cua phong kham

Phong kham chi co **2 loai bac si**, phan biet boi `LoaiHopDong` (`ClinicBooking.Domain/Enums/LoaiHopDong.cs`):

| Loai | Enum value | Mo ta nghiep vu | Cach tao ca lam viec |
|------|-----------|-----------------|---------------------|
| **Bac si noi tru** | `NoiTru` | Thuong truc tai phong kham theo lich co dinh hang tuan. Lich duoc Admin thiet lap trong bang `lich_noi_tru` (ngay trong tuan + khung ca + phong). | `NguonTaoCa = TuDong` — he thong tu dong sinh `CaLamViec` tu mau `LichNoiTru` dang hieu luc (`NgayKetThuc IS NULL`). |
| **Bac si hop dong** | `HopDong` | Lam viec theo ca da dang ky. Bac si tu chon ngay + khung ca + phong roi gui yeu cau; Admin duyet hoac tu choi. | `NguonTaoCa = BacSiDangKy` — bac si tu tao `CaLamViec` voi `TrangThaiDuyet = ChoDuyet`, Admin duyet thi chuyen `DaDuyet`. |

**Luu y quan trong cho Module 2:**

1. **Khong co loai bac si nao khac** ngoai 2 loai tren. Enum `LoaiHopDong` chi co `NoiTru` va `HopDong` — **khong co** gia tri `ChinhThuc`, `NgoaiTru`, `PartTime`, hay bat ky gia tri nao khac.
2. **`NguonTaoCa`** chi co `TuDong` (tu mau noi tru) va `BacSiDangKy` (hop dong tu dang ky) — **khong co** `Admin`.
3. **Lien he giua `LoaiHopDong` va `NguonTaoCa`:**
   - Bac si `NoiTru` → ca cua ho duoc sinh `TuDong` tu `LichNoiTru`.
   - Bac si `HopDong` → ca cua ho do chinh ho dang ky (`BacSiDangKy`), can Admin duyet.
   - Ngoai le: Admin cung co the tao ca thu cong cho bac si noi tru (vd: ca phat sinh ngoai lich co dinh) — luc do `NguonTaoCa` van la `TuDong` hoac tuy vao flow Admin.
4. **`LichNoiTru`** (entity + bang `lich_noi_tru`) chi ap dung cho bac si `NoiTru`. Bang nay luu lich co dinh theo tuan (`NgayTrongTuan` 0-6 = Thu 2 → Chu nhat) va duoc version hoa bang `NgayApDung`/`NgayKetThuc`. He thong dung mau nay de sinh `CaLamViec` tu dong cho tung tuan.
5. Bac si `HopDong` co the dang ky gio lam **lech so voi gio mac dinh** cua `DinhNghiaCa` (vd: DinhNghiaCa "sang" la 7h-12h, nhung bac si hop dong dang ky 8h-11h). Day la ly do `CaLamViec` luu `GioBatDau`/`GioKetThuc` rieng, khong lay thang tu `DinhNghiaCa`.

---

## 1. Bat buoc fix truoc khi PR vao `develop`

Khi minh merge `origin/feature/module2-doctors-scheduling` vao nhanh Module 1 de dong bo, phat hien 2 loi khien **test project khong bien dich duoc** tren develop hien tai. Doan minh ban chua chay `dotnet test` tren nhanh sau khi rebase/merge develop.

### 1.1. Test tham chieu enum khong ton tai

**File:**
- `ClinicBooking.Application.UnitTests/Features/Doctors/Queries/DanhSachBacSiCongKhai/DanhSachBacSiCongKhaiHandlerTests.cs`
- `ClinicBooking.Application.UnitTests/Features/Scheduling/Queries/DanhSachCaLamViecCongKhai/DanhSachCaLamViecCongKhaiHandlerTests.cs`

**Loi:**
```
error CS0117: 'LoaiHopDong' does not contain a definition for 'ChinhThuc'
error CS0117: 'NguonTaoCa' does not contain a definition for 'Admin'
```

**Nguyen nhan:** enum hien tai trong `ClinicBooking.Domain/Enums/` phan anh dung nghiep vu phong kham (xem muc 0):
- `LoaiHopDong` (`LoaiHopDong.cs`): chi co `NoiTru` (bac si thuong truc lich co dinh), `HopDong` (bac si lam viec theo ca dang ky). **Khong co `ChinhThuc`** — day la gia tri test tu dat, khong phu hop voi domain.
- `NguonTaoCa` (`NguonTaoCa.cs`): chi co `TuDong` (he thong sinh tu `LichNoiTru`), `BacSiDangKy` (bac si hop dong tu dang ky ca). **Khong co `Admin`**.

**Fix — chon gia tri enum dung voi ngu canh nghiep vu cua test:**
- `LoaiHopDong.ChinhThuc` → `LoaiHopDong.NoiTru` (neu test mo phong bac si thuong truc) hoac `LoaiHopDong.HopDong` (neu test mo phong bac si hop dong lam theo ca dang ky).
- `NguonTaoCa.Admin` → `NguonTaoCa.TuDong` (neu test mo phong ca sinh tu dong tu mau noi tru) hoac `NguonTaoCa.BacSiDangKy` (neu test mo phong ca do bac si hop dong tu dang ky).

**Goi y:** voi `DanhSachBacSiCongKhai` test (list bac si cong khai), dung `LoaiHopDong.NoiTru` la phu hop nhat vi da so bac si hien thi cong khai la noi tru. Voi `DanhSachCaLamViecCongKhai` test, `NguonTaoCa.TuDong` phu hop vi ca cong khai thuong la ca da duoc he thong sinh tu dong va Admin duyet.

### 1.2. Hardcode `IdTaiKhoan` gay FK constraint fail

**File:** cung 2 file test tren, cac dong kieu:
```csharp
new BacSi { IdTaiKhoan = 1, IdChuyenKhoa = ..., HoTen = "BS1", ... }
new BacSi { IdTaiKhoan = 2, IdChuyenKhoa = ..., HoTen = "BS2", ... }
```

**Loi:**
```
SQLite Error 19: 'FOREIGN KEY constraint failed'
```

**Nguyen nhan:** test dang gia dinh `TaiKhoan` co san voi `Id = 1, 2, 3`. Nhung `TestDbContextFactory` dung SQLite `:memory:` + EnsureCreated chi seed `HasData` tu `SeedData.cs` — co the khong dam bao co du rows voi Id khop mong doi cua test.

**Fix (de xuat):** dung helper `TestDataSeeder.SeedTaiKhoan` co san tai `ClinicBooking.Application.UnitTests/Common/TestDataSeeder.cs`:

```csharp
// Thay:
new BacSi { IdTaiKhoan = 1, ... }

// Bang:
var tk = TestDataSeeder.SeedTaiKhoan(db, VaiTro.BacSi);
new BacSi { IdTaiKhoan = tk.IdTaiKhoan, ... }
```

Helper nay tao `TaiKhoan` moi voi `TenDangNhap`/`Email`/`SoDienThoai` random-suffix, tranh trung voi seed va tranh FK fail.

### 1.3. Tham chieu snippet fix

Minh da tam patch trong nhanh Module 1 tai commit `a60922a` de CI Module 1 chay duoc. Ban co the xem diff de copy cach fix:

```bash
git fetch origin feature/module1/phan-a-quyen-truy-cap
git show a60922a -- ClinicBooking.Application.UnitTests/Features/Doctors ClinicBooking.Application.UnitTests/Features/Scheduling
```

**Luu y:** ban nen fix truc tiep tren nhanh cua ban (`feature/module2-doctors-scheduling`) roi push — khi do minh merge lai va patch tam `a60922a` se bi supersede mot cach sach se.

### 1.4. Trinh tu PR

Thu tu merge nen la:

1. Ban fix 1.1 + 1.2 tren nhanh cua ban.
2. Ban PR `feature/module2-doctors-scheduling` → `develop`, merge vao.
3. Minh rebase nhanh `feature/module1/phan-a-quyen-truy-cap` len `develop` moi; patch tam `a60922a` bi drop do ban da fix goc.

---

## 2. Xin mo rong scope (optional, duoc thi tot)

### 2.1. Endpoint cong khai cho `ChuyenKhoa` + `DichVu`

Theo `docs/ma-tran-quyen-truy-cap.md` muc 10.1: `Guest` va `BenhNhan` can xem `ChuyenKhoa` + `DichVu` truoc khi dat lich. Hien tai `DanhMucController` chan toan bo boi `[Authorize]`.

Viec nay logic thuoc Module 2 (Danh muc la scope cua ban theo split ban dau) nhung khong urgent truoc khi front-end bat dau. Neu ban con thoi gian, pattern nay 30 phut copy tu `BacSiPublicResponse` + `DanhSachBacSiCongKhaiHandler`:

**Ten convention thong nhat** (da co `bac-si/cong-khai`, `ca-lam-viec/cong-khai`):
- `GET api/danh-muc/chuyen-khoa/cong-khai` — list chuyen khoa cho Guest + BN.
- `GET api/danh-muc/dich-vu/cong-khai` — list dich vu cho Guest + BN.

**Yeu cau de tranh lap loi 1.3/1.4 cua review truoc:**

1. **DTO cong khai** (tach rieng khoi `ChuyenKhoaResponse`/`DichVuResponse`):
   - `ChuyenKhoaPublicResponse`: bo `HienThi` (luon true, khong can tra).
   - `DichVuPublicResponse`: bo `HienThi`, bo `NgayTao` (noi bo).
2. **Handler hardcode filter** — khong nhan `bool? HienThi` trong Query, hardcode `.Where(x => x.HienThi)`:
   - `DanhSachChuyenKhoaCongKhaiHandler`: `Where(x => x.HienThi)`.
   - `DanhSachDichVuCongKhaiHandler`: `Where(x => x.HienThi && x.ChuyenKhoa.HienThi)` — tranh lo dich vu thuoc khoa da an.
3. **Controller:** `[AllowAnonymous]` tren 2 endpoint moi.

### 2.2. Fix dep them (tuy hung)

**Rename file:** commit `cb706e4` ban da doi class `DoctorsController` → `BacSiController` va `SchedulingController` → `CaLamViecController`, nhung file van ten cu `DoctorsController.cs` / `SchedulingController.cs`. C# khong bat loi compile nhung khi tim file se gay nham. Rename file thanh `BacSiController.cs` / `CaLamViecController.cs` de khop class.

**Namespace `Features/Doctors/`, `Features/Scheduling/`:** goi y mem trong review 2.1, con chua doi thanh `Features/BacSi/` + `Features/CaLamViec/`. Neu ban doi thi dong bo tuyet doi; neu khong thi minh chap nhan duoc — khong block.

---

## 3. Con lai (Module 1 phu trach — de ban biet, khong phai viec cua ban)

- `GET api/hang-cho/thu-tu-cua-toi/{idCaLamViec}` cho `BenhNhan` (xem so thu tu minh trong hang cho).
- `GET api/lich-hen/theo-ngay/cua-toi?ngay=yyyy-MM-dd` cho `BacSi` (xem lich hen trong ngay cua minh).

Minh se lam trong nhanh Module 1 sau khi hoan tat 1.x + merge cua ban.

---

## 4. Checklist cho ban

Truoc khi PR vao `develop`:

- [ ] 1.1 Doi `LoaiHopDong.ChinhThuc` → `LoaiHopDong.NoiTru` (bac si thuong truc, lich co dinh) hoac `LoaiHopDong.HopDong` (bac si lam viec theo ca dang ky) — chon theo ngu canh test.
- [ ] 1.1 Doi `NguonTaoCa.Admin` → `NguonTaoCa.TuDong` (ca sinh tu mau noi tru) hoac `NguonTaoCa.BacSiDangKy` (ca do bac si hop dong dang ky) — chon theo ngu canh test.
- [ ] 1.2 Thay hardcode `IdTaiKhoan = 1, 2, 3` bang `TestDataSeeder.SeedTaiKhoan(...).IdTaiKhoan`.
- [ ] Chay `dotnet test ClinicBooking.Application.UnitTests` → 4/4 test `DanhSachBacSiCongKhai` + `DanhSachCaLamViecCongKhai` xanh.
- [ ] PR vao `develop`.

Sau khi merge (optional):

- [ ] 2.1 Endpoint cong khai `chuyen-khoa/cong-khai` + `dich-vu/cong-khai`.
- [ ] 2.2 Rename file controller khop class.

---

Neu cho nao trong 1.x chua ro, reply truc tiep cho minh — minh gui snippet luon. Cam on ban!
