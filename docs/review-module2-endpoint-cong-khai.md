# Review commit `5dc7ca6` — endpoint cong khai BacSi + CaLamViec

Chao ban,

Minh da review nhanh `feature/module2-doctors-scheduling` sau khi ban push loat commit moi (xoa Razor Pages, chuan hoa test seed, them endpoint cong khai BacSi + CaLamViec). Phan xoa Razor Pages + chuan test seed **tuyet voi** — `TestSeedSafeValues.cs` va `TestDbContextFactoryBehaviorTests.cs` la huong giai quyet goc re hon de xuat ban dau cua minh.

Rieng commit `5dc7ca6` (endpoint cong khai) co mot so diem can xem lai truoc khi merge vao `develop`, chu yeu lien quan **bao mat va quyen rieng tu**. Minh liet ke theo do uu tien: blocker truoc, gop y sau.

**Ngay review**: 2026-04-22
**Commit ra soat**: `5dc7ca6 feat(module2): add public doctor and schedule listing endpoints`
**Nen doc kem**: `docs/ma-tran-quyen-truy-cap.md` (muc 10.1 + nguyen tac cuoi file)

---

## 1. Can fix truoc khi merge (blocker)

### 1.1. `BacSiPublicResponse` lo thong tin nhan su noi bo

**File:**
- `ClinicBooking.Application/Features/Doctors/Dtos/BacSiPublicResponse.cs`
- `ClinicBooking.Api/Contracts/Doctors/BacSiPublicDto.cs`

Hien tai response (cho `Guest`, `[AllowAnonymous]`) tra:
- `LoaiHopDong` (NoiTru/NgoaiTru) — **thong tin HR noi bo**, khach vang lai khong can biet bac si ky hop dong loai gi.
- `TrangThai` (DangLam/NghiViec/...) — tuong tu, la trang thai nhan su; thay vi lo ra, nen **filter san** chi tra `DangLam`.
- `TieuSu` — can xem lai noi dung nay la "mo ta chuyen mon cong khai" hay "tieu su ca nhan / thong tin nhay cam". Neu la loai 2 thi bo.

**De xuat DTO cong khai:**
```csharp
public sealed record BacSiPublicResponse(
    int IdBacSi,
    int IdChuyenKhoa,
    string HoTen,
    string? AnhDaiDien,
    string? BangCap,
    int? NamKinhNghiem,
    string TenChuyenKhoa);
```

Bo `LoaiHopDong`, `TrangThai`. Giu `TieuSu` neu la noi dung da duoc PM/marketing duyet cho cong khai.

---

### 1.2. `CaLamViecPublicResponse` lo `SoSlotToiDa` + `SoSlotDaDat` tuyet doi

**File:**
- `ClinicBooking.Application/Features/Scheduling/Dtos/CaLamViecPublicResponse.cs`
- `ClinicBooking.Api/Contracts/Scheduling/CaLamViecPublicDto.cs`

DTO cong khai tra **con so tuyet doi** `SoSlotToiDa = 20`, `SoSlotDaDat = 18`. Day la van de minh da flag trong `docs/ma-tran-quyen-truy-cap.md` muc 10.1 + nguyen tac 4:

> "Khach vang lai chi thay `ConTrong` dang boolean/bucket, **khong thay con so tuyet doi**."

Ly do: competitor/bot co the scrape diem endpoint nay theo phut → biet duoc phong kham dong/vang tung ca, suy ra doanh thu, dung cho canh tranh.

**De xuat:** bo 2 field `SoSlotToiDa`, `SoSlotDaDat` khoi DTO cong khai. Chi giu `ConTrong` (boolean, da co san).

Neu can do sau bucket (cho UX tot hon ma khong lo con so thuc): them enum `MucDoSlot { ConNhieu, ConIt, GanHet }` — tinh trong handler dua tren `SoSlotDaDat / SoSlotToiDa` (vd < 50% = ConNhieu, < 90% = ConIt, >= 90% = GanHet). Tra enum thay vi so.

Neu sau nay can con so thuc **cho user da dang nhap** → tao endpoint rieng `GET api/ca-lam-viec/{id}` voi `[Authorize]` va DTO day du, khong mo cho `Guest`.

---

### 1.3. Handler `DanhSachBacSiCongKhai` khong filter mac dinh

**File:** `ClinicBooking.Application/Features/Doctors/Queries/DanhSachBacSiCongKhai/DanhSachBacSiCongKhaiHandler.cs`

Hien tai: neu query **khong truyen** `dangLamViec`, handler tra **tat ca** bac si (bao gom `NghiViec`). Nghia la khach vang lai vao `/api/bac-si/cong-khai` se thay ca bac si da nghi.

Tuong tu, khong co filter `ChuyenKhoa.HienThi == true` → bac si thuoc khoa admin da an cung bi lo.

**De xuat:**
```csharp
// Mac dinh: chi bac si dang lam, thuoc khoa dang hien thi.
var query = _db.BacSi
    .AsNoTracking()
    .Include(x => x.ChuyenKhoa)
    .Where(x => x.TrangThai == TrangThaiBacSi.DangLam)
    .Where(x => x.ChuyenKhoa.HienThi);
```

Bo han parameter `dangLamViec` khoi public query — no cham an ninh, khong co ly do cong khai.

---

### 1.4. Handler `DanhSachCaLamViecCongKhai` khong filter mac dinh

**File:** `ClinicBooking.Application/Features/Scheduling/Queries/DanhSachCaLamViecCongKhai/DanhSachCaLamViecCongKhaiHandler.cs`

Tuong tu 1.3: neu khong truyen `trangThaiDuyet`, handler tra **ca ca `ChoDuyet`, `TuChoi`** — nghia la khach vang lai co the nhin thay pipeline duyet ca noi bo cua admin.

**De xuat:** bo parameter `trangThaiDuyet` khoi public query, hard-code filter `x.TrangThaiDuyet == TrangThaiDuyetCa.DaDuyet` trong handler.

---

### 1.5. `SchedulingController` dung `Enum.Parse` → tra 500 khi input sai

**File:** `ClinicBooking.Api/Controllers/SchedulingController.cs`

```csharp
ClinicBooking.Domain.Enums.TrangThaiDuyetCa? trangThai = string.IsNullOrWhiteSpace(trangThaiDuyet)
    ? null
    : Enum.Parse<ClinicBooking.Domain.Enums.TrangThaiDuyetCa>(trangThaiDuyet, ignoreCase: true);
```

Neu client goi `?trangThaiDuyet=foo` → `Enum.Parse` throw `ArgumentException` → `GlobalExceptionHandler` tra **500 Server Error** thay vi 400 BadRequest.

**De xuat (neu van giu parameter sau fix 1.4):**
- Doi thanh `TryParse` + neu fail thi throw `ValidationException` (da duoc map sang 400).
- Hoac don gian hon: doi signature thanh `TrangThaiDuyetCa? trangThaiDuyet` — ASP.NET model binding se tu validate enum, tra 400 automatically.

Neu accept fix 1.4 (bo parameter nay khoi public), diem nay bien mat.

---

## 2. Gop y (khong chan merge)

### 2.1. Naming convention chua dong bo

**File:**
- `ClinicBooking.Api/Controllers/DoctorsController.cs`
- `ClinicBooking.Api/Controllers/SchedulingController.cs`
- Namespace `ClinicBooking.Application.Features.Doctors.*`
- Namespace `ClinicBooking.Application.Features.Scheduling.*`

Toan bo controller va folder Features hien co trong du an deu dung **tieng Viet khong dau**:

| Hien co | Moi them |
|---|---|
| `DanhMucController`, `LichHenController`, `HangChoController`, `BenhNhanController`, `HoSoKhamController`, `ThuocController`, `ToaThuocController` | `DoctorsController`, `SchedulingController` |
| `Features/DanhMuc/`, `Features/LichHen/`, `Features/HangCho/`, `Features/BenhNhan/`, `Features/Auth/`, `Features/HoSoKham/`, `Features/Thuoc/`, `Features/ToaThuoc/` | `Features/Doctors/`, `Features/Scheduling/` |

`CLAUDE.md` muc **11. Naming Conventions**: *"Class, method, property MUST be named in Vietnamese without diacritics"*. Exception chi cho **suffix** framework (`Controller`, `Service`, `Repository`, `Middleware`), khong phai cho phan tu chinh cua ten.

**De xuat:** doi thanh `BacSiController` / `CaLamViecController`, namespace `Features/BacSi/`, `Features/CaLamViec/`. Route URL `api/bac-si`, `api/ca-lam-viec` thi giu nguyen (dang dung tieng Viet roi, OK).

---

### 2.2. Chua co rate limit + Cache-Control header cho endpoint cong khai

Endpoint `[AllowAnonymous]` thuong la muc tieu DoS + scraping. Hai viec nay nen bo sung truoc khi ship production:

- **Rate limit** (IP-based, ~30-60 req/phut): dung `Microsoft.AspNetCore.RateLimiting` co san trong .NET 8, gan policy len endpoint `[AllowAnonymous]`.
- **Cache-Control**: `public, max-age=300` cho endpoint cong khai (5 phut CDN cache); `private, no-store` cho endpoint yeu cau auth.

Day thuoc scope **ha tang**, khong can fix trong PR hien tai, nhung nen flag va tao issue follow-up.

---

## 3. Diem tot giu nguyen

- `AsNoTracking()` + `Include` dung cho query read-only — perf OK.
- Co ca **unit test** cho 2 handler moi (`DanhSachBacSiCongKhaiHandlerTests`, `DanhSachCaLamViecCongKhaiHandlerTests`) — 100% new-code coverage.
- Pagination day du.
- Mapping tu entity sang DTO qua static factory `TuEntity` sach.
- Tach controller (`DoctorsController`, `SchedulingController`) thay vi dui vao `DanhMucController` — dung tach responsibility.

---

## 4. Ghi chu cuoi

Phan "blocker" muc 1.1-1.5 la van de an ninh/quyen rieng tu, minh nghi **phai fix** truoc khi merge vao `develop` vi front-end se goi endpoint nay va anh tang ben tren kho bi fix sau. Phan "gop y" muc 2.x co the lam trong PR nay hoac follow-up PR rieng, tuy ban.

Neu ban muon, minh co the tu fix luon 1.1-1.5 roi tao PR nho vao nhanh cua ban — de ban khoi phai cham vao nua. Cu reply cho minh biet.
