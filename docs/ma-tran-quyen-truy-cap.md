# Ma tran quyen truy cap API

**Muc dich**: ra soat toan bo endpoint hien co, xac dinh dung — sai — thieu ve `[Authorize]` va ownership check, lam input cho viec gan quyen truoc khi sang front-end.

**Ngay ra soat**: 2026-04-21
**Scope**: 9 controller trong `ClinicBooking.Api/Controllers/`.
**Quy uoc**:
- `Guest` = khach vang lai (chua dang nhap, `[AllowAnonymous]`)
- `BN` = `VaiTroConstants.BenhNhan`
- `LT` = `VaiTroConstants.LeTan`
- `BS` = `VaiTroConstants.BacSi`
- `AD` = `VaiTroConstants.Admin`
- `Auth` = yeu cau dang nhap nhung khong gioi han role
- `Own` = can check ownership trong handler (khong du khi chi authorize role)

Cot **"Trang thai"**:
- ✅ Dung
- ⚠️ Dung nhung can verify (thuong la ownership check trong handler)
- ❌ Sai / thieu
- 🆕 Chua co, can tao moi

---

## 1. AuthController (`api/auth`)

| Method | Route | Hien tai | De xuat | Trang thai | Ghi chu |
|---|---|---|---|---|---|
| POST | `dang-ky` | `Guest` | `Guest` | ✅ | Cho phep self-registration. |
| POST | `dang-nhap` | `Guest` | `Guest` | ✅ | |
| POST | `lam-moi-token` | `Guest` | `Guest` | ✅ | Refresh token la credential, khong can auth header. |
| POST | `dang-xuat` | `Auth` | `Auth` | ✅ | |

---

## 2. DanhMucController (`api/danh-muc`) — Module 2

### 2.1. ChuyenKhoa

| Method | Route | Hien tai | De xuat | Trang thai | Ghi chu |
|---|---|---|---|---|---|
| POST | `chuyen-khoa` | `AD` | `AD` | ✅ | |
| GET | `chuyen-khoa` | `AD, LT, BS` | `Guest` (DTO cong khai, filter `HienThi = true`) + `AD, LT, BS` (DTO full) | ❌ | **Gap nghiem trong**: `BenhNhan` khong the xem danh sach chuyen khoa → khong dat lich duoc. Can endpoint `Guest` rieng. |
| GET | `chuyen-khoa/{id}` | `AD, LT, BS` | `Guest` + `AD, LT, BS` | ❌ | Tuong tu. |
| PUT | `chuyen-khoa/{id}` | `AD` | `AD` | ✅ | |
| DELETE | `chuyen-khoa/{id}` | `AD` | `AD` | ✅ | |

### 2.2. DichVu

| Method | Route | Hien tai | De xuat | Trang thai | Ghi chu |
|---|---|---|---|---|---|
| POST | `dich-vu` | `AD` | `AD` | ✅ | |
| GET | `dich-vu` | `AD, LT, BS` | `Guest` + `AD, LT, BS` | ❌ | **Gap**: `BN` khong xem dc danh sach dich vu. |
| GET | `dich-vu/{id}` | `AD, LT, BS` | `Guest` + `AD, LT, BS` | ❌ | |
| PUT | `dich-vu/{id}` | `AD` | `AD` | ✅ | |
| DELETE | `dich-vu/{id}` | `AD` | `AD` | ✅ | |

### 2.3. Phong

| Method | Route | Hien tai | De xuat | Trang thai | Ghi chu |
|---|---|---|---|---|---|
| POST | `phong` | `AD` | `AD` | ✅ | |
| GET | `phong` | `AD, LT, BS` | `AD, LT, BS` | ✅ | Khong lo cho `Guest` / `BN` — thong tin noi bo. |
| GET | `phong/{id}` | `AD, LT, BS` | `AD, LT, BS` | ✅ | |
| PUT | `phong/{id}` | `AD` | `AD` | ✅ | |
| DELETE | `phong/{id}` | `AD` | `AD` | ✅ | |

### 2.4. DinhNghiaCa

| Method | Route | Hien tai | De xuat | Trang thai | Ghi chu |
|---|---|---|---|---|---|
| POST | `dinh-nghia-ca` | `AD` | `AD` | ✅ | |
| GET | `dinh-nghia-ca` | `AD, LT, BS` | `AD, LT, BS` | ✅ | Template noi bo. |
| GET | `dinh-nghia-ca/{id}` | `AD, LT, BS` | `AD, LT, BS` | ✅ | |
| PUT | `dinh-nghia-ca/{id}` | `AD` | `AD` | ✅ | |
| DELETE | `dinh-nghia-ca/{id}` | `AD` | `AD` | ✅ | |

---

## 3. LichHenController (`api/lich-hen`) — Module 1

| Method | Route | Hien tai | De xuat | Trang thai | Ghi chu |
|---|---|---|---|---|---|
| POST | `tao-lich-hen` | `BN, LT, AD` | `BN, LT, AD` | ✅ | Handler check `IdBenhNhan` theo user khi role=`BN`; cho `LT, AD` truyen `IdBenhNhan`. |
| POST | `{id}/doi-lich` | `BN, LT, AD` | `BN, LT, AD` + `Own` | ⚠️ | Can verify handler `DoiLichHenHandler` check ownership khi role=`BN`. |
| POST | `tao-giu-cho` | `LT, AD` | `LT, AD` | ✅ | |
| POST | `{id}/xac-nhan` | `LT, AD` | `LT, AD` | ✅ | |
| POST | `{id}/huy` | `BN, LT, AD` | `BN, LT, AD` + `Own` | ⚠️ | Handler da check (test `HuyLichHenHandlerTests.ForbiddenBenhNhan` pass) — OK. |
| POST | `{id}/check-in` | `LT` | `LT` | ✅ | Chi le tan tai quay. |
| POST | `giu-cho/{id}/giai-phong` | `LT` | `LT` | ✅ | |
| GET | `{id}` | `Auth` | `Auth` + `Own` | ⚠️ | **Khong co role restriction** — handler `XemLichHenQuery` bat buoc check: `BN` chi xem lich cua minh; `BS` chi xem lich ca minh phu trach; `LT, AD` xem tat ca. Can verify. |
| GET | `cua-toi` | `BN` | `BN` | ✅ | |
| GET | `theo-ngay` | `LT, AD` | `LT, AD, BS` | ⚠️ | **Thieu `BS`**: bac si can xem lich trong ngay cua minh. Co the tao endpoint rieng `theo-ngay/cua-toi` cho `BS`, hoac filter server-side theo user. |

---

## 4. HangChoController (`api/hang-cho`) — Module 1

| Method | Route | Hien tai | De xuat | Trang thai | Ghi chu |
|---|---|---|---|---|---|
| POST | `goi-ke-tiep/{idCa}` | `BS, LT` | `BS, LT` + `Own` | ⚠️ | Handler can check: khi role=`BS` thi chi goi duoc trong ca cua chinh minh. |
| POST | `{id}/hoan-thanh` | `BS` | `BS` + `Own` | ⚠️ | Tuong tu — chi bac si phu trach ca moi complete duoc. |
| GET | `theo-ca/{idCa}` | `LT, BS, AD` | `LT, BS, AD` | ✅ | |

**Gap**: `BN` khong co cach nao xem thu tu cua minh trong hang cho. Can 🆕 `GET api/hang-cho/thu-tu-cua-toi/{idCaLamViec}` cho `BN` — tra `{ SoThuTuCuaBan, SoThuTuDangGoi, SoNguoiConCho }`, KHONG tra danh sach full (tranh lo PII).

---

## 5. BenhNhanController (`api/benh-nhan`) — Module 3

| Method | Route | Hien tai | De xuat | Trang thai | Ghi chu |
|---|---|---|---|---|---|
| GET | `ho-so-cua-toi` | `BN` | `BN` | ✅ | |
| PUT | `ho-so-cua-toi` | `BN` | `BN` | ✅ | |
| GET | `` (list) | `LT, AD` | `LT, AD` | ✅ | DTO phai loc PII nhay cam (mat khau hash khong bao gio lo, da OK). |
| GET | `{id}` | `LT, AD` | `LT, AD` | ✅ | |
| POST | `walk-in` | `LT, AD` | `LT, AD` | ✅ | |

---

## 6. HoSoKhamController (`api/ho-so-kham`) — Module 3

| Method | Route | Hien tai | De xuat | Trang thai | Ghi chu |
|---|---|---|---|---|---|
| POST | `` | `BS` | `BS` + `Own` | ⚠️ | Handler phai check: bac si tao ho so chi cho lich hen ma minh la bac si phu trach. |
| PUT | `{id}` | `BS` | `BS` + `Own` | ⚠️ | Tuong tu — chi bac si tao ho so moi sua duoc (hoac cho phep bat ky `BS` nao? can lam ro quy che nghiep vu). |
| GET | `{id}` | `AD, LT, BS, BN` | `AD, LT, BS, BN` + `Own (BN)` | ⚠️ | **`BN` can ownership check bat buoc**: chi xem ho so kham cua chinh minh. Lo ho so y te nguoi khac la vi pham quyen rieng tu nghiem trong. |
| GET | `benh-nhan/{idBenhNhan}` | `AD, LT, BS` | `AD, LT, BS` | ✅ | Khong mo cho `BN` (da co `cua-toi`). |
| GET | `cua-toi` | `BN` | `BN` | ✅ | |

---

## 7. ThuocController (`api/thuoc`) — Module 3

| Method | Route | Hien tai | De xuat | Trang thai | Ghi chu |
|---|---|---|---|---|---|
| ALL | toan bo | `AD` (class-level) | `AD` cho CUD + `BS` cho GET | ❌ | **Van de**: bac si can GET danh sach thuoc de ke toa, nhung hien tai class-level `AD` chan ho. Can tach: CUD cho `AD`, GET (`DanhSachThuoc`, `LayThuocById`) cho `AD, BS`. |

---

## 8. ToaThuocController (`api/toa-thuoc`) — Module 3

| Method | Route | Hien tai | De xuat | Trang thai | Ghi chu |
|---|---|---|---|---|---|
| POST | `` | `BS` | `BS` + `Own` | ⚠️ | Ownership check: chi bac si tao ho so kham moi ke toa duoc. |
| PUT | `ho-so-kham/{id}` | `BS` | `BS` + `Own` | ⚠️ | Tuong tu. |
| GET | `ho-so-kham/{id}` | `AD, LT, BS, BN` | `AD, LT, BS, BN` + `Own (BN)` | ⚠️ | `BN` phai la chu ho so kham do. |
| GET | `cua-toi` | `BN` | `BN` | ✅ | |

---

## 9. TestController (`api/test`)

| Method | Route | Hien tai | De xuat | Trang thai | Ghi chu |
|---|---|---|---|---|---|
| GET | `` | `Guest` (no attribute) | **Xoa** hoac gioi han `AD` + `#if DEBUG` | ❌ | Endpoint khong co `[Authorize]`, tra `"API is working!"`. Nen xoa truoc khi ship production, hoac chi dinh `Development` environment. |

---

## 10. Endpoint thieu (can tao moi) — Blocker cho front-end

### 10.1. Endpoint cong khai cho trang chu + luong dat lich

Front-end cua `BN` va `Guest` phai goi duoc nhung endpoint sau. Chua co → front-end se block.

| Method | Route de xuat | Role | Muc dich |
|---|---|---|---|
| GET | `api/cong-khai/chuyen-khoa` | `Guest` | Danh sach chuyen khoa `HienThi = true`, DTO giam bot field. |
| GET | `api/cong-khai/chuyen-khoa/{id}` | `Guest` | Detail cong khai. |
| GET | `api/cong-khai/dich-vu` | `Guest` | Danh sach dich vu `HienThi = true`. |
| GET | `api/cong-khai/bac-si` | `Guest` | Danh sach bac si `DangLam`, DTO giam field (`HoTen`, `HocVi`, `ChuyenMon`, `AnhDaiDien`, `IdChuyenKhoa`). **Khong** lo `IdTaiKhoan`, SDT, email, `LoaiHopDong`. |
| GET | `api/cong-khai/bac-si/{id}` | `Guest` | Detail cong khai. |
| GET | `api/cong-khai/ca-lam-viec/slot-trong` | `Guest` | Query `?ngay=yyyy-MM-dd&idBacSi=X&idChuyenKhoa=Y`. Tra `ConTrong` dang **boolean** hoac enum `DayLich/ConIt/ConNhieu`, KHONG lo `SoSlotDaDat` tuyet doi. Chi tra ca `DaDuyet`. |
| GET | `api/ca-lam-viec/slot-trong` | `Auth` | Cung query nhu tren nhung tra `SoSlotDaDat/SoSlotToiDa` thuc. Dung cho `BN` da dang nhap + `LT`. |

### 10.2. Endpoint noi bo con thieu (Module 2 owner)

| Method | Route | Role | Muc dich |
|---|---|---|---|
| GET | `api/bac-si` | `AD, LT, BS` | Danh sach bac si full (admin/le tan). Chua co. |
| GET | `api/bac-si/{id}` | `AD, LT, BS` | Detail bac si noi bo. |
| GET | `api/ca-lam-viec` | `AD, LT, BS` | Danh sach ca, filter theo ngay/bac si/chuyen khoa. |
| POST/PUT/DELETE | `api/ca-lam-viec` | `AD` hoac nguoi co quyen lap lich | Tao/sua ca lam viec. Thuoc Module 2, chua co. |

---

## Tom tat hanh dong can lam (theo do uu tien)

### Blocker (truoc khi front-end bat dau)

1. 🆕 Tao endpoint `api/cong-khai/*` cho `Guest` va `BN` (muc 10.1) — 7 endpoint.
2. 🆕 Tao endpoint `api/bac-si` + `api/ca-lam-viec` cho internal (muc 10.2) — thuoc trach nhiem Module 2, can thao luan voi owner.
3. ❌ Fix `DanhMucController.DanhSachChuyenKhoa` / `DanhSachDichVu` — it nhat mo cho `BN` (neu chua tach endpoint cong khai) **HOAC** tach thanh endpoint rieng (khuyen nghi theo muc 10.1).

### Nghiem trong (lam ngay, khong can front-end)

4. ❌ Tach quyen `ThuocController` — CUD cho `AD`, GET cho `AD, BS`.
5. ⚠️ Verify ownership check trong handler: `XemLichHenQuery`, `DoiLichHenHandler`, `LayHoSoKhamByIdHandler` (role=`BN`), `LayToaTheoHoSoKhamHandler` (role=`BN`). Viet integration test / unit test cho truong hop `BN` truy cap record nguoi khac.
6. ⚠️ Verify ownership check trong handler cua `HangChoController.GoiKeTiep` / `HoanThanh` khi role=`BS`.

### Cai thien (sau khi ship MVP)

7. ❌ `TestController` — xoa hoac gioi han `Development` environment.
8. 🆕 `GET api/hang-cho/thu-tu-cua-toi/{idCaLamViec}` cho `BN`.
9. ⚠️ `LichHenController.TheoNgay` — cho `BS` xem lich ngay cua chinh minh (endpoint rieng hoac filter server-side).
10. Them rate limit cho endpoint `api/cong-khai/*` (IP-based, ~60 req/min).
11. Them `Cache-Control: public, max-age=300` cho endpoint `api/cong-khai/*` + `private, no-store` cho endpoint co auth.

---

## Nguyen tac ap dung

1. **Mac dinh dong**: Controller phai co `[Authorize]` o class-level; `[AllowAnonymous]` dat co the tren tung method can cong khai.
2. **Role check ≠ ownership check**: `[Authorize(Roles = "benh_nhan")]` KHONG bao ve truoc `GET /lich-hen/12345` khi user khac co role `BN` co the xem lich nguoi khac. Handler phai so `IdBenhNhan` voi `ICurrentUserService`.
3. **DTO tach biet guest vs auth**: tao rieng `*CongKhaiResponse` vs `*Response` — khong duoc tai su dung DTO noi bo cho endpoint cong khai.
4. **SoSlotDaDat la thong tin kinh doanh**: khach vang lai chi thay `ConTrong` dang boolean/bucket, khong thay con so tuyet doi.
5. **Query param ≠ authz**: `?idBenhNhan=X` trong request KHONG duoc tin tuong — server lay user tu token, so sanh.
6. **PII filter tai DTO layer**: mat khau hash, token, noi dung y te... khong bao gio lo, ngay ca trong response cho chinh chu so huu (cang it cang tot).
