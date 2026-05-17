# Yêu cầu Module 3 — Finalize Hồ sơ khám + Tra cứu bệnh án

**Ngày lập:** 2026-05-16
**Người gửi:** Module 1 owner (Potatomine725)
**Người nhận:** Owner Module 3 (Hồ sơ khám / Toa thuốc / Bệnh nhân)
**Nguồn:** `docs/Plans/ke-hoach-hoan-thien-he-thong-va-thoat-dev.md` §3.2
**Trạng thái Module 3:** Backend `HoSoKham` + `ToaThuoc` đầy đủ. UI BN (`LichSuKham`, `ToaThuoc`, `ThongBao`) + BS (`QuanLyKham`, `HoSo`, `LichLamViec`) đã wire.

---

## 1. Mục tiêu

Đóng 5 việc trước staging:

1. Chốt logic `BenhNhan.SoLanHuyMuon` — ngưỡng cấm đặt lịch + reset chu kỳ.
2. Wire CRUD Thuốc trong `Admin/QuanLyDanhMuc` (verify nếu đã wire).
3. Trang tra cứu bệnh án cho lễ tân (search theo BN bất kỳ).
4. Panel lịch sử khám trong `BacSi/QuanLyKham`.
5. **Enforce "1 bệnh nhân = 1 hồ sơ cá nhân, N bệnh án"** — verify constraint + trang "Hồ sơ tổng hợp" gộp profile + danh sách bệnh án.

---

## 2. Việc cần làm

### 2.1. (🔴 Cao — chặn flow đặt lịch) Chốt logic `SoLanHuyMuon`

**Hiện trạng:** `HuyLichHenHandler` (Module 1) đang increment `BenhNhan.SoLanHuyMuon += 1` khi BN huỷ muộn (trong 24h trước giờ khám). Field hiện chỉ ghi, **chưa dùng để chặn gì**.

**Quyết định cần chốt** (báo lại Module 1 owner để wire validator):

1. Ngưỡng cấm? Đề xuất `>= 5` → chặn đặt mới trong N ngày.
2. Reset định kỳ? Đề xuất reset hàng tháng (cron job, Module 4 Hangfire).
3. Có warning trước khi cấm (ví dụ `>= 3` thì hiện cảnh báo)?
4. Lễ tân có override được không (walk-in case)?

**Sau khi chốt:**

- Thêm validator trong `TaoLichHenCommand` + `LeTan/DatLichKhachVangLai` (Module 1 sẽ wire — báo Module 1 owner sau khi chốt).
- Hoặc Module 3 expose `Features/BenhNhan/Queries/KiemTraQuyenDatLich/` (`Query(int IdBenhNhan) → KetQua { ChoPhep, LyDo }`) để Module 1 gọi — clean cross-module boundary.

**Test:** unit test handler huỷ muộn 5 lần → lần thứ 6 đặt mới bị reject với thông báo VN.

### 2.2. (🟠 Cao) Verify wire CRUD Thuốc

**File:** `ClinicBooking.Web/Pages/Admin/QuanLyDanhMuc.cshtml(.cs)` (đã có 7.7K code).

Verify:
- Có wire `TaoThuocCommand` / `CapNhatThuocCommand` / `XoaThuocCommand` chưa? (Module 3 own backend `Features/Thuoc/Commands/...`).
- Filter: tên thuốc, đơn vị, hoạt chất.
- Validator: tên không rỗng, đơn vị không rỗng.

Nếu chưa wire — bổ sung theo pattern `Admin/DichVu`.

### 2.3. (🟡 Trung) Trang tra cứu bệnh án cho lễ tân

**Mục tiêu:** Lễ tân nhập SĐT / mã BN / CCCD → ra danh sách BN match → click 1 BN → xem lịch sử khám + đơn thuốc của BN đó.

#### Backend mới

`Features/HoSoKham/Queries/LichSuKhamTheoBenhNhan/`:

```csharp
public sealed record LichSuKhamTheoBenhNhanQuery(
    int IdBenhNhan,
    int SoTrang = 1,
    int KichThuocTrang = 20) : IRequest<LichSuKhamTheoBenhNhanResponse>;
```

Handler:
- `AsNoTracking()`, join `BacSi`, `ChuyenKhoa`, `ToaThuoc` (count).
- Sort `NgayKham desc`.
- Authorize: `[Authorize(Roles = "le_tan,bac_si,admin")]` (BN dùng query `LichSuKhamCuaToi` riêng).

`Features/BenhNhan/Queries/TimKiemBenhNhan/` (nếu chưa có):

```csharp
public sealed record TimKiemBenhNhanQuery(string TuKhoa, int Limit = 20)
    : IRequest<IReadOnlyList<BenhNhanTomTatDto>>;
```

Handler: search `Contains` trên `HoTen`, `SoDienThoai`, `Cccd`, `MaBN`. Limit 20.

#### UI mới

`ClinicBooking.Web/Pages/LeTan/TraCuuBenhAn.cshtml(.cs)`:

- Search bar `?tuKhoa=...` GET — POST không cần.
- Bảng kết quả: HoTen / NgaySinh / SDT / CCCD / Mã BN.
- Click row → `?idBenhNhan=N` → load panel phải:
  - Profile BN (đọc-only).
  - Bảng lịch sử khám (date, BS, chẩn đoán, link xem chi tiết hồ sơ).
  - Section đơn thuốc gần nhất.

Sidebar `_Layout.cshtml` block `le_tan`: thêm link "Tra cứu bệnh án".

### 2.4. (🟡 Trung) Panel lịch sử khám trong `BacSi/QuanLyKham`

**File:** `ClinicBooking.Web/Pages/BacSi/QuanLyKham.cshtml(.cs)`.

Khi BS đang khám 1 BN (có `idLichHen` selected) → render panel phải hiển thị 5 lần khám gần nhất của BN đó:
- Reuse `LichSuKhamTheoBenhNhanQuery` (§2.3) với `KichThuocTrang = 5`.
- UI: card timeline gọn (date, chẩn đoán short, link expand modal xem full).

Mục tiêu: giúp BS ra quyết định nhanh, không phải mở trang khác.

### 2.5. (🔴 Cao) Enforce "1 BN = 1 hồ sơ cá nhân, N bệnh án"

**Nguyên tắc nghiệp vụ:**
- Mỗi `TaiKhoan` vai trò `benh_nhan` ↔ **đúng 1** record `BenhNhan` (hồ sơ cá nhân).
- Mỗi `BenhNhan` có **N** `HoSoKham` (bệnh án), mỗi lần khám tạo 1 record mới — không sửa cộng dồn vào record cũ.

**Hiện trạng (verified 2026-05-16):**
- `BenhNhan.IdTaiKhoan: int` — chưa verify có unique index ở DB chưa.
- `HoSoKham` có `IdLichHen` + `IdBacSi`, **không có `IdBenhNhan` trực tiếp** → phải traverse `LichHen.IdBenhNhan` để gom bệnh án theo BN. Query qua nhiều joins.
- `HoSoKham.IdLichHen` chưa verify có unique (1 lịch hẹn ↔ 1 bệnh án).

#### 2.5.1. Verify DB constraint (nếu thiếu, thêm migration)

1. **Unique `BenhNhan.IdTaiKhoan`** — đảm bảo 1 TK chỉ có 1 record BenhNhan:
   ```csharp
   // Trong AppDbContext.OnModelCreating
   modelBuilder.Entity<BenhNhan>()
       .HasIndex(b => b.IdTaiKhoan)
       .IsUnique();
   ```
2. **Unique `HoSoKham.IdLichHen`** — mỗi lịch hẹn tạo tối đa 1 bệnh án:
   ```csharp
   modelBuilder.Entity<HoSoKham>()
       .HasIndex(h => h.IdLichHen)
       .IsUnique();
   ```
3. Migration: `AddUniqueConstraintsBenhAn` — nếu DB hiện có duplicate, viết script cleanup trước.

#### 2.5.2. Thêm denormalize `HoSoKham.IdBenhNhan` (optional nhưng khuyến nghị)

Hiện gom bệnh án theo BN phải join `HoSoKham → LichHen → BenhNhan`. Thêm `HoSoKham.IdBenhNhan` (FK redundant, populate trong `TaoHoSoKhamHandler` từ `LichHen.IdBenhNhan`) → query lịch sử khám nhanh hơn + index được.

Migration: `AddIdBenhNhanToHoSoKham` + backfill `UPDATE HoSoKham SET IdBenhNhan = (SELECT IdBenhNhan FROM LichHen WHERE LichHen.IdLichHen = HoSoKham.IdLichHen)`.

> Nếu chọn KHÔNG denormalize: chấp nhận join nhưng phải đảm bảo `LichSuKhamTheoBenhNhanQuery` (§2.3) có index trên `LichHen.IdBenhNhan` (đã có theo FK).

#### 2.5.3. Validator `TaoHoSoKhamCommand`

Bổ sung check:
- `LichHen.TrangThai ∈ {DangKham, HoanThanh}` (giữ nguyên).
- `LichHen.IdLichHen` chưa có `HoSoKham` nào (`!await _db.HoSoKham.AnyAsync(h => h.IdLichHen == request.IdLichHen, ct)`) → throw `ConflictException("Lich hen nay da co benh an, khong the tao moi.")`.

> Nếu UC nghiệp vụ cho phép "sửa bệnh án sau khám" thì dùng `CapNhatHoSoKhamCommand` riêng, không tạo mới.

#### 2.5.4. Trang `LeTan/HoSoCaNhan` (hoặc tab trong `TraCuuBenhAn` §2.3)

Mục tiêu: 1 màn hình duy nhất khi mở 1 BN, bố cục:

```
┌─ Hồ sơ cá nhân (1) ────────────────┐
│ HoTen, NgaySinh, GioiTinh, Cccd,   │
│ DiaChi, SDT, Email                 │
│ [Sửa]                              │
└────────────────────────────────────┘
┌─ Bệnh án (N) ─ tab/timeline ───────┐
│ ▾ 2026-05-15 — BS Nguyen Van A — Khoa Noi │
│   Chẩn đoán: ... | Toa thuốc: 3 thuốc     │
│ ▾ 2026-04-22 — BS Le Thi B  — Khoa Tai-Mui│
│ ...                                       │
└────────────────────────────────────┘
```

Reuse:
- Profile: `Features/BenhNhan/Queries/LayChiTietBenhNhan/` (tạo mới nếu chưa có).
- Bệnh án list: `LichSuKhamTheoBenhNhanQuery` (§2.3).

#### Test

- Integration: tạo `BenhNhan` rồi cố tạo `BenhNhan` thứ 2 cùng `IdTaiKhoan` → DB throw unique constraint.
- Integration: tạo `HoSoKham` cho `LichHen` đã có bệnh án → handler throw `ConflictException`.
- Integration: 1 BN có 3 lịch hẹn `HoanThanh` + 3 bệnh án → query lịch sử trả đúng 3 record.

---

## 3. Workflow ship

### 3.1. Branch

```powershell
git fetch origin
git checkout -b feature/module3/finalize-medical-records origin/develop
```

### 3.2. Commit suggest

1. `feat(benh-nhan): chot logic SoLanHuyMuon + query KiemTraQuyenDatLich`
2. `feat(admin/thuoc): verify wire CRUD thuoc trong QuanLyDanhMuc`
3. `feat(le-tan): trang TraCuuBenhAn + LichSuKhamTheoBenhNhan query`
4. `feat(bac-si/quan-ly-kham): panel lich su kham BN dang kham`
5. `feat(benh-an): unique BenhNhan.IdTaiKhoan + HoSoKham.IdLichHen + (optional) HoSoKham.IdBenhNhan denormalize`
6. `feat(le-tan/ho-so-ca-nhan): trang gop profile BN + danh sach benh an`

### 3.3. Trước khi push

```powershell
dotnet build DatLichPhongKham.slnx
dotnet test
```

### 3.4. PR

- **Base:** `develop`
- **Reviewer:** Module 1 owner (verify cross-module write `SoLanHuyMuon` đúng kỳ vọng)
- **Tag:** `module3`

---

## 4. Ràng buộc

- **Không update `LichHen.TrangThai`** (thuộc Module 1).
- Khi expose `KiemTraQuyenDatLich` query: signature stable sau khi M1 đã wire.
- `HoSoKham` chỉ tạo được khi `LichHen.TrangThai ∈ {DangKham, HoanThanh}`.

---

## 5. Ưu tiên

1. **Cao** — §2.5 Enforce 1 BN = 1 hồ sơ + N bệnh án (data integrity, blocking).
2. **Cao** — §2.1 SoLanHuyMuon (chặn flow đặt lịch staging).
3. **Cao** — §2.2 CRUD Thuốc (BS không kê được đơn nếu thuốc rỗng).
4. **Trung** — §2.3 Tra cứu bệnh án lễ tân.
5. **Trung** — §2.4 Panel BS/QuanLyKham.

---

## 6. Liên hệ

Nhắn Module 1 owner (Potatomine725). Module 1 review PR trong 2 ngày làm việc, đặc biệt phần `SoLanHuyMuon` vì cross-module.
