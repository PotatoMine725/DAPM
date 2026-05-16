# Kế hoạch: Đặt lịch hộ khách vãng lai tại quầy lễ tân

> **Ngày lập:** 2026-05-08  
> **Branch:** sẽ tách từ `feature/module1/portal-sat-demo`  
> **Trạng thái:** Kế hoạch — chưa code  
> **Người thực hiện:** Module 1

---

## 1. Bối cảnh & Vấn đề

### Vấn đề hiện tại

Trang `/LeTan/QuanLyLichHen` hiện chỉ cho lễ tân xem, xác nhận, check-in và hủy lịch hẹn đã có sẵn. **Không có chức năng tạo lịch hẹn mới tại quầy** cho khách vãng lai (người chưa có tài khoản).

### Phân tích code sẵn có

| Thành phần | Trạng thái | Ghi chú |
|---|---|---|
| `TaoBenhNhanWalkInCommand` + Handler | ✅ Đã có | Tạo TaiKhoan + BenhNhan mới cho vãng lai |
| `TaoBenhNhanWalkInValidator` | ✅ Đã có | Validate HoTen, SDT, CCCD, NgaySinh |
| `TaoLichHenCommand` + Handler | ✅ Đã có | Hỗ trợ `HinhThucDat.TaiQuay`, nhận `IdBenhNhan` |
| Query tìm BenhNhan theo SDT | ❌ Chưa có | Cần thêm để kiểm tra bệnh nhân đã đăng ký |
| UI modal trên QuanLyLichHen | ❌ Chưa có | |
| Page handler `OnPostDatLichVangLai` | ❌ Chưa có | |

### Ràng buộc quan trọng từ schema DB

- `TaiKhoan.TenDangNhap`, `Email`, `SoDienThoai` đều có **UNIQUE constraint**.
- `TaoBenhNhanWalkInHandler` hiện **throw ConflictException** khi SDT đã tồn tại — không phù hợp cho flow tìm-và-tái-sử-dụng.
- `TaiKhoan` walk-in được tạo với `TrangThai = false` (không thể đăng nhập) — đúng về nghiệp vụ.
- `BenhNhan.Cccd` có UNIQUE constraint — cần kiểm tra trùng khi có CCCD.

---

## 2. Thiết kế luồng

### Luồng tổng quan

```
[Lễ tân] → Bấm "Đặt lịch mới" trên QuanLyLichHen
  ↓
[Modal bước 1 — Tìm kiếm bệnh nhân]
  → Nhập số điện thoại → Bấm "Tìm"
  ↓
  ┌─ SDT đã có trong hệ thống ─────────────────────────────┐
  │  → Hiện thông tin: HoTen, NgaySinh                     │
  │  → Lễ tân xác nhận "Dùng hồ sơ này"                   │
  │  → Lưu IdBenhNhan vào hidden field                     │
  └────────────────────────────────────────────────────────┘
  ┌─ SDT chưa có ──────────────────────────────────────────┐
  │  → Mở form nhập thông tin bệnh nhân mới               │
  │  → HoTen (bắt buộc), NgaySinh, GioiTinh, CCCD (tùy)  │
  └────────────────────────────────────────────────────────┘
  ↓
[Modal bước 2 — Thông tin lịch hẹn]
  → Ngày khám, Giờ mong muốn, Dịch vụ, Triệu chứng
  → Bấm "Xác nhận đặt lịch"
  ↓
[POST OnPostDatLichVangLaiAsync]
  → Nếu IdBenhNhan có sẵn → gọi TaoLichHenCommand trực tiếp
  → Nếu không có → gọi TaoBenhNhanWalkIn → lấy IdBenhNhan → gọi TaoLichHenCommand
  ↓
✅ Redirect về QuanLyLichHen, hiển thị lịch hẹn vừa tạo (TrangThai = ChoXacNhan)
```

---

## 3. Quyết định thiết kế

### 3.1 Tìm kiếm bệnh nhân theo SDT

**Quyết định:** Thêm query endpoint `TimBenhNhanTheoSdtQuery` trong Application layer.  
**Lý do:** Tránh tạo tài khoản trùng, cho phép lễ tân nhận diện bệnh nhân cũ (dù họ không nhớ tên đăng nhập).  
**Nằm ở đâu:** `ClinicBooking.Application/Features/BenhNhan/Queries/TimBenhNhanTheoSdt/`

### 3.2 Xử lý SDT đã tồn tại

**Quyết định:** Không throw ConflictException, trả về thông tin bệnh nhân cũ để lễ tân xác nhận dùng lại.  
**Cách xử lý:**
- Thêm query riêng (không sửa handler hiện tại)
- Page handler kiểm tra SDT trước → nếu tìm thấy BenhNhan, dùng IdBenhNhan đó
- Nếu SDT thuộc TaiKhoan nhưng **không có BenhNhan** (ví dụ: tài khoản BacSi/LeTan) → báo lỗi "Số điện thoại này thuộc vai trò khác, không thể đặt lịch"

### 3.3 TaiKhoan cho khách vãng lai

**Quyết định:** Giữ nguyên thiết kế trong `TaoBenhNhanWalkInHandler`:
- `TrangThai = false` — tài khoản inactive, không thể đăng nhập
- Username: `walkin_{timestamp}{random}` — tránh collision
- Email: `walkin_{timestamp}{random}@local.invalid` — placeholder hợp lệ về format, không gửi email
- Password: hashed random — không cấp cho bệnh nhân

**Lý do:** Bệnh nhân vãng lai có thể "nhận" tài khoản sau (bằng cách reset password qua SDT) — đây là out of scope và sẽ làm ở Module 4.

### 3.4 Cấu trúc UI

**Quyết định:** Modal 2 bước trực tiếp trên trang `QuanLyLichHen` — không tạo trang riêng.  
**Lý do:** Giảm số bước điều hướng cho lễ tân, phù hợp với UX hiện tại.

### 3.5 Flow tạo + đặt lịch

**Quyết định:** Xử lý 2 lệnh riêng biệt tại Page handler, **không** tạo command mới kết hợp.  
**Lý do:** Tái dụng `TaoBenhNhanWalkInHandler` và `TaoLichHenHandler` đã có và đã test. Tránh duplicate logic slot.

---

## 4. Thay đổi Application Layer

### 4.1 Query mới — `TimBenhNhanTheoSdtQuery`

**File:** `ClinicBooking.Application/Features/BenhNhan/Queries/TimBenhNhanTheoSdt/`

```
TimBenhNhanTheoSdtQuery.cs     — record(string SoDienThoai) : IRequest<TimBenhNhanKetQua?>
TimBenhNhanTheoSdtHandler.cs   — query TaiKhoan by SDT, include BenhNhan, trả về null nếu không tìm thấy
TimBenhNhanKetQua.cs           — record(int IdBenhNhan, string HoTen, DateOnly? NgaySinh, GioiTinh? GioiTinh)
```

**Logic handler:**
1. Tìm `TaiKhoan` theo `SoDienThoai`
2. Nếu không có → trả về `null`
3. Nếu có `TaiKhoan` nhưng `BenhNhan == null` → throw `ConflictException` ("Số điện thoại này không thuộc bệnh nhân")
4. Nếu có `BenhNhan` → trả về `TimBenhNhanKetQua`

### 4.2 Không sửa `TaoBenhNhanWalkInHandler`

Handler hiện tại throw ConflictException khi SDT trùng — vẫn giữ nguyên vì đây là behavior đúng cho API. Page handler sẽ gọi query trước để kiểm tra, không bao giờ gọi `TaoBenhNhanWalkIn` khi SDT đã tồn tại.

### 4.3 Không sửa `TaoLichHenHandler`

Đã hỗ trợ `LeTan + IdBenhNhan` với `HinhThucDat.TaiQuay`. Không cần thay đổi.

---

## 5. Thay đổi Web Layer

### 5.1 Page Model — `QuanLyLichHenModel`

Thêm các BindProperty và handler mới:

```csharp
// BindProperty cho form đặt lịch vãng lai
[BindProperty] public DatLichVangLaiInput VangLai { get; set; } = new();

public class DatLichVangLaiInput
{
    // Thông tin bệnh nhân (dùng khi tạo mới)
    public int? IdBenhNhanCu { get; set; }   // null = tạo mới
    public string? HoTen { get; set; }
    public string SoDienThoai { get; set; } = string.Empty;
    public DateOnly? NgaySinh { get; set; }
    public GioiTinh? GioiTinh { get; set; }
    public string? Cccd { get; set; }

    // Thông tin lịch hẹn
    public DateOnly NgayLamViec { get; set; }
    public TimeOnly GioMongMuon { get; set; }
    public int IdDichVu { get; set; }
    public string? TrieuChung { get; set; }
}

// Handler mới
public async Task<IActionResult> OnPostDatLichVangLaiAsync()
{
    // 1. Xác định IdBenhNhan
    // 2. Nếu IdBenhNhanCu null → gọi TaoBenhNhanWalkInCommand → lấy IdBenhNhan
    // 3. Gọi TaoLichHenCommand với IdBenhNhan
    // 4. Redirect với SuccessMessage
}
```

Thêm handler lookup (AJAX hoặc form POST):

```csharp
// Trả JSON để modal gọi lookup bệnh nhân theo SDT
public async Task<IActionResult> OnGetTimBenhNhanAsync(string sdt)
{
    var ketQua = await _mediator.Send(new TimBenhNhanTheoSdtQuery(sdt));
    return new JsonResult(ketQua);
}
```

### 5.2 UI — Modal "Đặt lịch mới"

**Button trigger** (thêm vào header của bảng QuanLyLichHen):
```html
<button class="btn-primary" onclick="openModal('modalDatLichVangLai')">
    <i class="fa-solid fa-plus"></i> Đặt lịch mới
</button>
```

**Modal layout:**

```
┌─────────────────────────────────────────────┐
│  Đặt lịch mới tại quầy                  [X] │
├─────────────────────────────────────────────┤
│  ── BƯỚC 1: Thông tin bệnh nhân ──          │
│                                             │
│  Số điện thoại: [____________] [Tìm kiếm]  │
│                                             │
│  ┌── Nếu tìm thấy: ──────────────────────┐ │
│  │  ✓ Bệnh nhân: Nguyễn Văn A            │ │
│  │    Ngày sinh: 01/01/1990              │ │
│  │  [Dùng hồ sơ này] [Nhập mới]         │ │
│  └───────────────────────────────────────┘ │
│                                             │
│  ┌── Nếu chưa có: ────────────────────────┐ │
│  │  Họ tên: [___________________] (*)     │ │
│  │  Ngày sinh: [__/__/____]               │ │
│  │  Giới tính: [Nam / Nữ / Khác]         │ │
│  │  CCCD: [____________] (tùy chọn)      │ │
│  └───────────────────────────────────────┘ │
│                                             │
│  ── BƯỚC 2: Thông tin lịch hẹn ──          │
│                                             │
│  Ngày khám:   [__/__/____]  (*)            │
│  Giờ mong muốn: [07:00 ▼]  (*)            │
│  Dịch vụ:     [___________ ▼]  (*)        │
│  Triệu chứng: [_________________________]  │
│               [________________________]  │
│                                             │
├─────────────────────────────────────────────┤
│              [Hủy]  [Xác nhận đặt lịch]    │
└─────────────────────────────────────────────┘
```

**Dropdown dịch vụ:** cần load danh sách DichVu từ DB vào `PageModel.DanhSachDichVu`.  
**Dropdown giờ:** hiển thị các mốc giờ hợp lệ (07:00, 07:30, 08:00 ... 11:30, 13:00 ... 16:30).

---

## 6. Edge cases & Xử lý lỗi

| Tình huống | Xử lý |
|---|---|
| SDT đã có trong hệ thống (bệnh nhân cũ) | Query trả thông tin, lễ tân xác nhận dùng lại — không tạo mới |
| SDT thuộc TaiKhoan BacSi/LeTan | Báo lỗi: "Số điện thoại này không thuộc hồ sơ bệnh nhân" |
| CCCD trùng với bệnh nhân khác | Validator báo lỗi trên field Cccd trong modal |
| Slot không còn (ca đã đầy) | TaoLichHenHandler throw ConflictException → hiện ErrorMessage trên modal, không đóng modal |
| Ngày chọn là hôm nay, slot demo đã chiếm | Báo lỗi "Không tìm thấy slot phù hợp" — đúng nghiệp vụ |
| IdBenhNhan được reuse nhưng bệnh nhân đang bị hạn chế | TaoLichHenHandler throw ConflictException → hiện ErrorMessage |
| Form submit thiếu HoTen khi tạo mới | Validator phía client + server đều báo lỗi |
| Form submit thiếu NgayLamViec / IdDichVu | Báo lỗi required field |

---

## 7. Danh sách file cần tạo / sửa

### Tạo mới

```
ClinicBooking.Application/Features/BenhNhan/Queries/TimBenhNhanTheoSdt/
├── TimBenhNhanTheoSdtQuery.cs
├── TimBenhNhanTheoSdtHandler.cs
└── TimBenhNhanKetQua.cs

ClinicBooking.Application/Features/LichHen/Queries/LayChiTietLichHenLeTan/
├── LayChiTietLichHenLeTanQuery.cs
├── LayChiTietLichHenLeTanHandler.cs
└── ChiTietLichHenLeTanResponse.cs

ClinicBooking.Application/Features/LichHen/Queries/TimBacSiKhaDung/
├── TimBacSiKhaDungChoLichHenQuery.cs
├── TimBacSiKhaDungChoLichHenHandler.cs
└── BacSiKhaDungItem.cs

ClinicBooking.Application/Features/LichHen/Commands/GanBacSiChoLichHen/
├── GanBacSiChoLichHenCommand.cs
├── GanBacSiChoLichHenHandler.cs
└── GanBacSiKetQua.cs
```

### Sửa đổi

```
ClinicBooking.Web/Pages/LeTan/QuanLyLichHen.cshtml.cs
  + thêm DanhSachDichVu property (load DichVu)
  + thêm BindProperty VangLai
  + thêm OnGetTimBenhNhanAsync (AJAX lookup)
  + thêm OnPostDatLichVangLaiAsync
  + thêm OnGetChiTietLichHenAsync (AJAX — trả JSON chi tiết lịch hẹn)
  + thêm OnGetTimBacSiKhaDungAsync (AJAX — tìm bác sĩ theo tên + idLichHen)
  + thêm OnPostGanBacSiAsync (gán bác sĩ chỉ định)

ClinicBooking.Web/Pages/LeTan/QuanLyLichHen.cshtml
  + thêm button "Đặt lịch mới"
  + thêm modal #modalDatLichVangLai
  + thêm modal #modalChiTietLichHen (xem chi tiết + tìm/gán bác sĩ)
  + sửa nút Check-in: render có điều kiện theo HangCho != null
  + thêm JS: tìm kiếm SDT via AJAX, toggle form mới/cũ, lookup chi tiết, tìm bác sĩ, gán bác sĩ
```

---

## 8. Test cases cần viết

### Unit tests (Application layer)

**Đặt lịch vãng lai:**
- `TimBenhNhanTheoSdtHandler`: SDT có trong DB → trả `TimBenhNhanKetQua`
- `TimBenhNhanTheoSdtHandler`: SDT không có → trả `null`
- `TimBenhNhanTheoSdtHandler`: SDT thuộc BacSi → throw ConflictException
- `OnPostDatLichVangLaiAsync` (mock): IdBenhNhanCu null → gọi `TaoBenhNhanWalkIn` trước
- `OnPostDatLichVangLaiAsync` (mock): IdBenhNhanCu có giá trị → bỏ qua `TaoBenhNhanWalkIn`

**Gán bác sĩ chỉ định:**
- `GanBacSiChoLichHenHandler`: TrangThai không hợp lệ (DangKham) → throw ConflictException
- `GanBacSiChoLichHenHandler`: CaLamViecMoi hết slot → return ThanhCong=false
- `GanBacSiChoLichHenHandler`: thành công → IdCaLamViec đổi, SoSlotDaDat điều chỉnh, LichSuLichHen được insert
- `GanBacSiChoLichHenHandler`: concurrency conflict → return ThanhCong=false với lý do "thử lại"
- `TimBacSiKhaDungChoLichHenHandler`: trả đúng danh sách bác sĩ cùng ChuyenKhoa, cùng ngày, còn slot

### Manual test checklist

```
[ ] Nhập SDT chưa có → hiện form mới → điền đủ → đặt được lịch
[ ] Nhập SDT đã có (bệnh nhân cũ) → hiện thông tin cũ → xác nhận → đặt được lịch
[ ] Nhập SDT thuộc BacSi → hiện thông báo lỗi đúng
[ ] Bỏ trống HoTen khi tạo mới → báo lỗi, không submit
[ ] Chọn giờ ngoài khung ca → báo lỗi từ server
[ ] Sau đặt thành công → lịch hẹn xuất hiện trên bảng với TrangThai=ChoXacNhan
[ ] Lịch vừa tạo có HinhThucDat=TaiQuay (phân biệt với đặt online)

[ ] Nút Check-in hiển thị bình thường khi TrangThai=DaXacNhan và chưa check-in
[ ] Nút Check-in chuyển sang "Đã check-in" (disabled) sau khi bấm xong
[ ] Nút Check-in ẩn khi TrangThai không phải DaXacNhan

[ ] Bấm icon chi tiết → modal hiện đúng thông tin, bao gồm BacSiMongMuonNote
[ ] Tìm bác sĩ → chỉ hiện bác sĩ cùng ChuyenKhoa với DichVu lịch hẹn
[ ] Tìm bác sĩ không khớp tên → danh sách rỗng
[ ] Gán bác sĩ có slot → cập nhật thành công, modal refresh hiện tên bác sĩ đã gán
[ ] Gán bác sĩ hết slot → hiện thông báo lỗi, lịch hẹn không thay đổi
[ ] Gán bác sĩ khi TrangThai=DangKham → server từ chối, hiện lỗi
```

---

## 9. Out of scope (không làm trong feature này)

- Cho phép bệnh nhân vãng lai "nhận" tài khoản sau (activate bằng SDT + OTP) — defer Module 4
- Tìm kiếm bệnh nhân theo tên hoặc CCCD trên modal
- Sửa thông tin bệnh nhân cũ ngay trong modal
- Thêm bệnh nhân vào hệ thống mà không đặt lịch luôn
- Gửi thông báo cho bác sĩ khi được gán thêm lịch hẹn — defer Module 4
- Tìm bác sĩ theo CCCD hoặc ID
- Auto-parse tên bác sĩ từ `BacSiMongMuonNote` — lễ tân tự đọc và tìm thủ công

---

## 10. Phụ thuộc

| Phụ thuộc | Trạng thái | Ghi chú |
|---|---|---|
| `TaoBenhNhanWalkInHandler` | ✅ Đã có | Không sửa |
| `TaoLichHenHandler` (path LeTan) | ✅ Đã có | Không sửa — hỗ trợ `HinhThucDat.TaiQuay` từ line 66 |
| Danh sách DichVu để dropdown | ✅ Đã có trong DB | Cần thêm query trong PageModel |
| `DanhSachDichVuQuery` hoặc tương đương | ❌ Module 2/3 chưa có | Tạo query đơn giản trong feature này |
| `RowVersion` trên `LichHen` cho concurrency | ✅ Đã có | Dùng lại cho `GanBacSiChoLichHenHandler` |
| `CaLamViec.SoSlotDaDat` atomicity | ✅ Đã có pattern | Xem `TaoLichHenHandler` — decrement/increment trong transaction |
| Quan hệ BacSi ↔ DichVu (để lọc) | ✅ Qua ChuyenKhoa | `bac_si.id_chuyen_khoa = dich_vu.id_chuyen_khoa` — join 2 bảng |
| `LichSuLichHen.HanhDong` giá trị mới | ❌ Cần thêm | Thêm `'gan_bac_si'` vào constant/enum `HanhDong` trong Domain |

---

## 11. Toggle trạng thái nút Check-in

### Vấn đề

Sau khi lễ tân bấm Check-in, nút vẫn hiển thị bình thường — không phân biệt được đã check-in hay chưa.

### Thiết kế

**Không cần thêm `TrangThaiLichHen` mới.** Trạng thái check-in xác định qua navigation property `LichHen.HangCho != null`.

**Điều kiện render nút:**

| Điều kiện | Hiển thị |
|---|---|
| `TrangThai == DaXacNhan` AND `HangCho == null` | Nút "Check-in" (enabled, style primary) |
| `HangCho != null` | Nút "Đã check-in" (disabled, style muted) |
| `TrangThai` khác (ChoXacNhan, DangKham, v.v.) | Ẩn nút |

**Razor template:**

```razor
@if (lh.TrangThai == TrangThaiLichHen.DaXacNhan)
{
    @if (lh.HangCho == null)
    {
        <button class="btn-sm btn-primary" asp-page-handler="CheckIn" ...>Check-in</button>
    }
    else
    {
        <button class="btn-sm" disabled>Đã check-in</button>
    }
}
```

**Backend:** Không thay đổi. `OnPostCheckInAsync` giữ nguyên.

**File sửa:** Chỉ `QuanLyLichHen.cshtml` — Razor conditional render. Query cần `Include(x => x.HangCho)` nếu chưa có.

---

## 12. Lễ tân xem chi tiết lịch hẹn

### Mục đích

Lễ tân cần đọc `BacSiMongMuonNote` (ghi chú bệnh nhân đề cập tên bác sĩ) để quyết định có gán bác sĩ chỉ định không. Hiện chưa có UI cho phép lễ tân xem chi tiết từng lịch hẹn.

### Query mới — `LayChiTietLichHenLeTanQuery`

**Vị trí:** `ClinicBooking.Application/Features/LichHen/Queries/LayChiTietLichHenLeTan/`

**DTO `ChiTietLichHenLeTanResponse`:**

```csharp
record ChiTietLichHenLeTanResponse(
    int IdLichHen,
    string MaLichHen,
    string TenBenhNhan,
    string SoDienThoai,
    string TenDichVu,
    string TenChuyenKhoa,
    int IdChuyenKhoa,        // cần cho TimBacSiKhaDung
    DateOnly NgayLamViec,
    TimeOnly GioBatDau,
    TimeOnly GioKetThuc,
    TrangThaiLichHen TrangThai,
    HinhThucDat HinhThucDat,
    string? TrieuChung,
    string? BacSiMongMuonNote,
    int? IdBacSiMongMuon,
    string? TenBacSiDuocGan,
    bool DaCheckIn           // = HangCho != null
);
```

**Authorization:** `[Authorize(Roles = "le_tan,admin")]` — query riêng, không reuse query của bệnh nhân.

**Endpoint trên PageModel:**

```csharp
public async Task<IActionResult> OnGetChiTietLichHenAsync(int id)
{
    var ketQua = await _mediator.Send(new LayChiTietLichHenLeTanQuery(id));
    return new JsonResult(ketQua);
}
```

**UI:** Nút/icon "Xem chi tiết" trên mỗi dòng bảng → AJAX → modal `#modalChiTietLichHen`. Modal 2 vùng:
- Vùng trên: thông tin đọc (tên, dịch vụ, ngày giờ, triệu chứng, ghi chú bác sĩ mong muốn, bác sĩ đã gán).
- Vùng dưới: form tìm/gán bác sĩ (xem Section 13).

---

## 13. Tìm kiếm và gán bác sĩ chỉ định

### Phân tích schema

- `BacSi.IdChuyenKhoa` và `DichVu.IdChuyenKhoa` cùng FK tới `ChuyenKhoa` — không có bảng M:N trực tiếp.
- Lọc bác sĩ phù hợp dịch vụ: `JOIN ca_lam_viec ON bac_si.id_bac_si = ca_lam_viec.id_bac_si WHERE ca_lam_viec.id_chuyen_khoa = @idChuyenKhoaCuaDichVu`.
- **"Gán bác sĩ" = swap `LichHen.IdCaLamViec`** — bác sĩ thực tế = `CaLamViec.IdBacSi`. `IdBacSiMongMuon` chỉ là preference, không xác định ai khám thực tế.

### Query — `TimBacSiKhaDungChoLichHenQuery`

**Vị trí:** `ClinicBooking.Application/Features/LichHen/Queries/TimBacSiKhaDung/`

**Input:** `record(int IdLichHen, string TenBacSi) : IRequest<List<BacSiKhaDungItem>>`

**Output:** `record BacSiKhaDungItem(int IdBacSi, string HoTen, int SoSlotConLai, int IdCaLamViec)`

**Logic handler:**
1. Load lịch hẹn → lấy `DichVu.IdChuyenKhoa` và `CaLamViec.NgayLamViec`
2. Query `ca_lam_viec` cùng `NgayLamViec`, cùng `IdChuyenKhoa`, `trang_thai_duyet = 'da_duyet'`, `so_slot_da_dat < so_slot_toi_da`
3. JOIN `bac_si` WHERE `ho_ten LIKE %TenBacSi%` (EF.Functions.Like, case-insensitive)
4. Filter: `bac_si.trang_thai = 'dang_lam'`
5. Loại trừ `ca_lam_viec.id_ca_lam_viec == LichHen.IdCaLamViec` hiện tại
6. Trả ≤ 10 kết quả, sort `SoSlotConLai` descending

**Endpoint:**

```csharp
public async Task<IActionResult> OnGetTimBacSiKhaDungAsync(int idLichHen, string tenBacSi)
{
    var ds = await _mediator.Send(new TimBacSiKhaDungChoLichHenQuery(idLichHen, tenBacSi));
    return new JsonResult(ds);
}
```

### Command — `GanBacSiChoLichHenCommand`

**Vị trí:** `ClinicBooking.Application/Features/LichHen/Commands/GanBacSiChoLichHen/`

**Input:** `record(int IdLichHen, int IdCaLamViecMoi) : IRequest<GanBacSiKetQua>`

**Output:** `record GanBacSiKetQua(bool ThanhCong, string? LyDoThatBai)`

**Logic handler:**

```
1. Load LichHen (with RowVersion) + CaLamViecCu
2. State guard:
   - TrangThai NOT IN (ChoXacNhan, DaXacNhan) → throw ConflictException("Không thể gán bác sĩ ở trạng thái này")
   - HangCho != null → throw ConflictException("Bệnh nhân đã check-in, không thể đổi ca")
3. Load CaLamViecMoi — verify:
   - trang_thai_duyet = 'da_duyet'
   - so_slot_da_dat < so_slot_toi_da
   - Nếu fail → return GanBacSiKetQua(false, "Bác sĩ không còn slot phù hợp trong ca này")
4. Transaction:
   a. CaLamViecCu.SoSlotDaDat--
   b. CaLamViecMoi.SoSlotDaDat++
   c. LichHen.IdCaLamViec = IdCaLamViecMoi
   d. LichHen.IdBacSiMongMuon = CaLamViecMoi.IdBacSi  (đồng bộ field preference)
   e. INSERT lich_su_lich_hen { hanh_dong = "gan_bac_si", id_nguoi_thuc_hien = IdLeTanHienTai }
5. Catch DbUpdateConcurrencyException → return GanBacSiKetQua(false, "Dữ liệu vừa thay đổi, vui lòng thử lại")
6. return GanBacSiKetQua(true, null)
```

**Lưu ý:** `lich_su_lich_hen.hanh_dong` cần thêm giá trị `"gan_bac_si"` vào Domain constant/enum. Kiểm tra xem hiện tại có dùng enum hay string constant không trước khi sửa.

**Endpoint:**

```csharp
public async Task<IActionResult> OnPostGanBacSiAsync(int idLichHen, int idCaLamViecMoi)
{
    var ketQua = await _mediator.Send(new GanBacSiChoLichHenCommand(idLichHen, idCaLamViecMoi));
    return new JsonResult(ketQua);
}
```

### Fallback behavior

Command **không** tự fallback về auto-match. Nếu `ThanhCong = false`:
- UI hiện thông báo lỗi với `LyDoThatBai`, modal không đóng.
- Lệ tân tự chọn: thử bác sĩ khác hoặc giữ nguyên lịch hẹn hiện tại (auto-match vẫn hoạt động như cũ).

### Luồng UI trong modal chi tiết

```
Lễ tân đọc BacSiMongMuonNote
  → [Tìm bác sĩ] → input tên → AJAX → danh sách bác sĩ phù hợp + số slot
  → Chọn 1 bác sĩ → [Gán bác sĩ này]
  → POST → ThanhCong: refresh modal, hiện "Đã gán: [Tên bác sĩ]"
          → Thất bại: hiện lý do, danh sách vẫn mở
```

### Ràng buộc UI

- Vùng tìm/gán bác sĩ chỉ hiện khi `TrangThai IN (ChoXacNhan, DaXacNhan)` AND `DaCheckIn = false`.
- Sau khi gán thành công, nút [Gán] disable để tránh double-submit.
