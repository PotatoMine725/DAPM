# Tien do — Ra soat quyen truy cap truoc khi sang front-end

**Branch**: `feature/module1/phan-a-quyen-truy-cap`
**Cap nhat**: 2026-04-22

---

## Da hoan thanh

### Phan A — Fix quyen truy cap don gian (commit `a973e8f`)
- Xoa `TestController` (endpoint debug khong bao ve).
- Sua `ThuocController`:
  - Class: `[Authorize]` (thay vi `[Authorize(Roles = Admin)]`).
  - Write (POST/PUT/DELETE): `Admin` only.
  - Read (GET list, GET by id): `Admin` + `BacSi` (truoc day chan `BacSi`).

### Phan B — Ownership audit handler Module 1 (commit `a334dac`)
- **Audit xong 5 handler**, ket luan:
  - `XemLichHen`, `DoiLichHen` (Module 1) — da co ownership check, khong can sua.
  - `LayHoSoKhamById`, `LayToaTheoHoSoKham` (Module 3) — da co ownership check, khong can sua.
  - `GoiBenhNhanKeTiep` — **thieu**, da them.
  - `HoanThanhLuotKham` — **thieu**, da them.
- `GoiBenhNhanKeTiep`: BacSi gate theo ca phu trach; LeTan/Admin khong rang buoc (ho dieu phoi quay).
- `HoanThanhLuotKham`: strict BacSi-only (chi bac si phu trach ca moi hoan thanh duoc).
- Them 2 test Forbidden cho moi handler; cap nhat 4 test cu dung `ICurrentUserService` mock.
- Ket qua: **15/15 HangCho tests xanh**.

### Review Module 2 (commit `7513897`, **da gui cho chu Module 2**)
- File: `docs/review-module2-endpoint-cong-khai.md`.
- Review commit `5dc7ca6` tren nhanh remote `feature/module2-doctors-scheduling`.
- 5 blocker (1.1-1.5) + 2 gop y (2.1, 2.2).
- Trong tam: `BacSiPublicResponse` lo `LoaiHopDong`/`TrangThai`/`TieuSu`, `CaLamViecPublicResponse` lo con so slot tuyet doi, handler khong filter mac dinh, `Enum.Parse` tra 500.

---

## Con lai (lam khi quay lai)

### Phan C — Endpoint cong khai cho ChuyenKhoa + DichVu (3-4h, doc lap)

**Ly do:** `docs/ma-tran-quyen-truy-cap.md` muc 10.1 liet ke — `Guest` va `BenhNhan` can xem danh sach chuyen khoa + dich vu truoc khi dat lich; hien tai ca 2 bi chan boi `[Authorize]` tren `DanhMucController`.

**Viec can lam:**
- Tao `Features/DanhMuc/Queries/DanhSachChuyenKhoaCongKhai/` + `DanhSachDichVuCongKhai/`.
- Filter mac dinh: `HienThi == true`.
- DTO cong khai: bo field noi bo (vd `NgayTao`, counters).
- Endpoint: `GET api/cong-khai/chuyen-khoa`, `GET api/cong-khai/dich-vu` voi `[AllowAnonymous]`.
- Hoac theo huong Module 2 dang di: `GET api/chuyen-khoa/cong-khai` (dong bo voi `api/bac-si/cong-khai` + `api/ca-lam-viec/cong-khai`) — chot convention.

### Phan E — Endpoint dac thu theo vai tro (2h)

- `GET api/hang-cho/thu-tu-cua-toi/{idCaLamViec}` cho `BenhNhan` — xem so thu tu cua minh trong hang cho (chi so thu tu va vi tri, khong lo thong tin BN khac).
- `GET api/lich-hen/theo-ngay/cua-toi?ngay=yyyy-MM-dd` cho `BacSi` — xem lich hen trong ngay cua minh (hien `api/lich-hen/theo-ngay` chi cho LeTan/Admin).

### Phan D (doi Module 2 phan hoi)

- Sau khi chu Module 2 xu ly review `docs/review-module2-endpoint-cong-khai.md`, pull lai branch va verify 1.1-1.5 da fix.
- Neu co, test smoke endpoint cong khai BacSi + CaLamViec khi develop da co fix.

### Pre-existing test failures (khong chan front-end, nhung can fix)

- **11 tests Thuoc + ToaThuoc failing tren develop** (phat hien khi chay suite day du). Likely seed collision hoac SQLite constraint — can debug rieng, KHONG lien quan den Phan A/B.
- Lay log: `dotnet test ClinicBooking.Application.UnitTests --filter "FullyQualifiedName~Thuoc|FullyQualifiedName~ToaThuoc" 2>&1 > /tmp/thuoc-fails.log`.

---

## State branch

- Current branch: `feature/module1/phan-a-quyen-truy-cap`, clean, **chua push**.
- Commits chua merge vao `develop`: `43447b7`, `0bfb887`, `ac9d849`, `a973e8f`, `a334dac`, `7513897`.
- Recommend: khi quay lai, push branch len + tao PR vao `develop` cho Phan A + B + docs.
