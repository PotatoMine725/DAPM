# Kế hoạch M1 — Wire-up Admin Portal

**Mục tiêu:** Thay toàn bộ mock-data trong `ClinicBooking.Web/Pages/Admin/*` bằng MediatR thật.
**Branch xuất phát:** `feature/module1/portal-sat-demo` (latest `489feaa`).
**Ngôn ngữ:** Tiếng Việt không dấu (theo CLAUDE.md §11).
**Ước tính:** ~5 ngày làm việc.

---

## Tiến độ (cập nhật 2026-05-15)

**Thứ tự ưu tiên đã chốt (cập nhật 2026-05-15):**

1. 🔴 **Cao** — Quản lý ca làm việc bác sĩ: `BacSi`, `CaLamViec`, `LichNoiTru` (✅), `DuyetCa` (✅)
2. 🟡 **Trung** — Danh mục: `Accounts`, `Phong`, `ChuyenKhoa`, `DichVu`
3. 🟢 **Thấp** — Báo cáo & truyền thông: `Dashboard`, `ThongKe`, `ThongBao`

> **Lưu ý:** Tạo BS hợp đồng (cần để test đầy đủ DuyetCa) sẽ thực hiện qua `Admin/BacSi` UI sau khi wire xong — không seed thủ công.

| Phase | Trạng thái | Commit | Ưu tiên |
|---|---|---|---|
| 1.1 Admin/LichNoiTru | ✅ DONE (pushed) | `02e2c57` | 🔴 |
| 1.2 Admin/DuyetCa | ✅ DONE (pushed) | `a98e448` | 🔴 |
| 2.2 Admin/BacSi | ✅ DONE (pushed) | `b8bfb7d` | 🔴 |
| 2.4 Admin/CaLamViec | ✅ DONE (uncommitted) | — | 🔴 |
| 2.1 Admin/Accounts | ⬜ Chưa bắt đầu | — | 🟡 |
| 2.3 Admin/Phong | ⬜ Chưa bắt đầu | — | 🟡 |
| 2.3 Admin/ChuyenKhoa | ⬜ Chưa bắt đầu | — | 🟡 |
| 2.3 Admin/DichVu | ⬜ Chưa bắt đầu | — | 🟡 |
| 2.5 Admin/Dashboard | ⬜ Chưa bắt đầu | — | 🟢 |
| 2.6 Admin/ThongKe | ⬜ Chưa bắt đầu | — | 🟢 |
| 2.7 Admin/ThongBao | ⬜ Chưa bắt đầu | — | 🟢 |
| 3 Verify & integration tests | ⬜ Chưa bắt đầu | — | 🔴 |

**Đã giải quyết blocker:** Admin có thể tự tạo BS hợp đồng qua `/Admin/BacSi` → unblock test đầy đủ Phase 1.2 DuyetCa.

### Files đã tạo/sửa

**Phase 1 — mới:**
- `ClinicBooking.Application/Features/Scheduling/Dtos/LichNoiTruResponse.cs`
- `ClinicBooking.Application/Features/Scheduling/Dtos/CaLamViecAdminResponse.cs`
- `ClinicBooking.Application/Features/Scheduling/Queries/DanhSachLichNoiTruTheoBacSi/{Query,Handler}.cs`
- `ClinicBooking.Application/Features/Scheduling/Queries/DanhSachCaLamViecChoDuyet/{Query,Handler}.cs`
- `ClinicBooking.Application/Features/Scheduling/Queries/ThongKeDuyetCa/{Query,Handler}.cs`

**Phase 1 — sửa:**
- `ClinicBooking.Application/Features/Doctors/Queries/DanhSachBacSiCongKhai/{Query,Handler}.cs` — thêm filter `LoaiHopDong?`
- `ClinicBooking.Web/Pages/Admin/LichNoiTru.cshtml(.cs)` — wire MediatR + bỏ mock UI
- `ClinicBooking.Web/Pages/Admin/DuyetCa.cshtml(.cs)` — wire MediatR + bỏ mock UI

**Phase 2.2 — mới:**
- `ClinicBooking.Application/Features/Doctors/Commands/TaoBacSi/{Command,Validator,Handler}.cs`
- `ClinicBooking.Application/Features/Doctors/Commands/CapNhatBacSi/{Command,Validator,Handler}.cs`
- `ClinicBooking.Application/Features/Doctors/Dtos/BacSiAdminResponse.cs`
- `ClinicBooking.Application/Features/Doctors/Queries/DanhSachBacSiAdmin/{Query,Handler}.cs`

**Phase 2.2 — sửa:**
- `ClinicBooking.Web/Pages/Admin/BacSi.cshtml(.cs)` — wire CRUD + filter + 2 modal (tạo/sửa) + đổi trạng thái

**Phase 2.4 — sửa:**
- `ClinicBooking.Application/Features/Scheduling/Queries/DanhSachCaLamViecChoDuyet/{Query,Handler}.cs` — thêm filter `IdBacSi?` + `IdPhong?` (backward compat, dùng chung query này cho DuyetCa + CaLamViec)
- `ClinicBooking.Web/Pages/Admin/CaLamViec.cshtml(.cs)` — wire MediatR: list + 6 filter (BS/Phòng/CK/TrangThai/TuNgay/DenNgay) + modal tạo (`TaoCaLamViecCommand`) + xoá row (`XoaCaLamViecCommand`)

### Quyết định kiến trúc

**Phase 1:**
- **Không mở rộng** `DanhSachCaLamViecCongKhaiQuery` để thêm filter `TrangThaiDuyet` — thay vào đó tạo `DanhSachCaLamViecChoDuyetQuery` mới + DTO `CaLamViecAdminResponse` riêng để tách field nhạy cảm (`LyDoTuChoi`, `IdAdminDuyet`, `NgayDuyet`) khỏi DTO public.
- **Modal hiện qua class `.show`** + JS thuần (không dùng modal framework) — đồng nhất với pattern hiện có.
- **Flash message** dùng `TempData["SuccessMessage"]/["ErrorMessage"]` render inline trong từng page (vì `_AdminLayout` chỉ có `showToast()` JS).
- **`NgayTrongTuan`** chốt 0=CN, 1=T2, …, 6=T7 (khớp `(int)DayOfWeek` của handler `SinhCaLamViecTuLichNoiTru`).

**Phase 2.2:**
- **Tạo `BacSi` đồng thời với `TaiKhoan`** trong 1 `SaveChangesAsync()` qua navigation property — đảm bảo atomic, không cần transaction explicit.
- **DTO `BacSiAdminResponse` riêng** cho admin (chứa `TenDangNhap/Email/SDT/TaiKhoanHoatDong`), tách khỏi `BacSiPublicResponse` để không expose info nội bộ ở public API.
- **Đổi trạng thái BS** dùng lại `CapNhatBacSiCommand` thay vì tạo command riêng — đơn giản, ít command. Trade-off: phải re-fetch BS hiện tại, có overhead 1 query — chấp nhận được vì action ít gọi.
- **Conflict namespace `BacSi`** (entity vs `Application.Features.BacSi` namespace) → dùng alias `using BacSiEntity = ClinicBooking.Domain.Entities.BacSi;` trong handler.
- **Modal sửa populate qua JSON inline**: `onclick='moModalSua(@Html.Raw(JsonSerializer.Serialize(bs)))'` — đơn giản, không cần fetch API riêng.

### Vấn đề cần follow-up

- ⚠️ `SinhCaLamViecTuLichNoiTruHandler` check trùng theo `(IdBacSi, NgayLamViec)` — **không phân biệt ca Sáng/Chiều**. Một BS có 2 mẫu cùng ngày sẽ chỉ sinh được 1 ca. Cần sửa thêm `IdDinhNghiaCa` vào điều kiện.
- ⚠️ `IdAdminDuyet` đang truyền `IdTaiKhoan`. Cần verify schema (entity `Admin` có FK riêng không?) trước khi vào Phase 2.
- ⚠️ `CaLamViec.IdAdminDuyet` navigation property là `TaiKhoan?` (thấy ở entity) — phù hợp với cách đang truyền.

---

## Hiện trạng

### Backend đã có (không cần code mới)

| Command / Query | Path | Chú thích |
|---|---|---|
| `TaoLichNoiTruCommand` | `Application/Features/Scheduling/Commands/TaoLichNoiTru/` | Tạo template lịch tuần |
| `VoHieuLichNoiTruCommand` | `…/VoHieuLichNoiTru/` | Tắt template |
| `CapNhatLichNoiTruCommand` | `…/CapNhatLichNoiTru/` | Sửa template |
| `SinhCaLamViecTuLichNoiTruCommand` | `…/SinhCaLamViecTuLichNoiTru/` | Sinh `CaLamViec` từ template, slot mặc định `DaDuyet` |
| `DangKyCaLamViecCommand` | `…/DangKyCaLamViec/` | Bác sĩ HopDong đăng ký ca |
| `DuyetCaLamViecCommand` | `…/DuyetCaLamViec/` | Admin duyệt/từ chối |
| `TaoCaLamViecCommand` | `…/TaoCaLamViec/` | Admin tạo ca trực tiếp |
| `XoaCaLamViecCommand` | `…/XoaCaLamViec/` | Xoá ca |
| `DanhSachCaLamViecCongKhaiQuery` | `…/Queries/DanhSachCaLamViecCongKhai/` | Filter theo bác sĩ/khoa/phòng/ngày/conTrong |
| `DanhSachChuyenKhoaQuery`, `DanhSachPhongQuery`, `DanhSachDinhNghiaCaQuery`, `DanhSachDichVuQuery` | `Application/Features/DanhMuc/Queries/` | Read CRUD danh mục |
| `DanhSachBacSiCongKhaiQuery`, `LayHoSoBacSiCuaToiQuery` | `Application/Features/Doctors/Queries/` | List bác sĩ public |

### Web Admin hiện trạng

| Trang | `.cshtml` | `.cshtml.cs` |
|---|---|---|
| `Admin/LichNoiTru` | mock data (5 BS hardcode) | stub `OnGet() {}` |
| `Admin/DuyetCa` | mock data (counts cứng + 5 row) | stub |
| `Admin/Accounts` | mock | stub |
| `Admin/BacSi` | mock | stub |
| `Admin/Phong` | mock | stub |
| `Admin/ChuyenKhoa` | mock | stub |
| `Admin/DichVu` | mock | stub |
| `Admin/CaLamViec` | mock | stub |
| `Admin/Dashboard` | có code (đã wire một phần — verify) | `4.5K` |
| `Admin/ThongKe` | mock | stub |
| `Admin/ThongBao` | mock | stub |
| `Admin/DanhMuc` | đã wire | `7.7K` (verify) |

> `LeTan/QuanLyCaLamViec` đã wire `DuyetCaLamViecCommand` đầy đủ — copy pattern.

### Enum tham chiếu

```csharp
LoaiHopDong   = { NoiTru, HopDong }
NguonTaoCa    = { TuDong, BacSiDangKy }
TrangThaiDuyetCa = { ChoDuyet, DaDuyet, DaHuy, HoanThanh }
TrangThaiBacSi   = { …xem file… }
```

---

## Phase 1 — Hai trang trọng tâm (1.5 ngày)

### 1.1. `Admin/LichNoiTru` — Tạo thời khoá biểu bác sĩ nội trú

**File chính:** `ClinicBooking.Web/Pages/Admin/LichNoiTru.cshtml(.cs)`

#### Cần tạo mới (Application layer)

1. `Features/Scheduling/Queries/DanhSachLichNoiTruTheoBacSi/`
   - `DanhSachLichNoiTruTheoBacSiQuery(int IdBacSi) : IRequest<IReadOnlyList<LichNoiTruDto>>`
   - Handler: `_db.LichNoiTru.Include(BacSi/Phong/DinhNghiaCa).Where(IdBacSi==).ToListAsync()`
   - DTO `LichNoiTruDto`: `IdLichNoiTru, IdBacSi, TenBacSi, IdPhong, TenPhong, IdDinhNghiaCa, TenDinhNghiaCa, NgayTrongTuan, NgayApDung, NgayKetThuc, TrangThai`

2. `Features/Doctors/Queries/DanhSachBacSiTheoLoaiHopDong/`
   - `DanhSachBacSiTheoLoaiHopDongQuery(LoaiHopDong Loai, string? Search) : IRequest<IReadOnlyList<BacSiTomTatDto>>`
   - Handler: filter `LoaiHopDong`, search theo `HoTen`
   - DTO: `IdBacSi, HoTen, IdChuyenKhoa, TenChuyenKhoa, AnhDaiDien, TrangThai`

#### Page Model `LichNoiTruModel`

```csharp
[Authorize(Roles = VaiTroConstants.Admin)]
public class LichNoiTruModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public IReadOnlyList<BacSiTomTatDto> DanhSachBacSi { get; private set; } = [];
    public IReadOnlyList<LichNoiTruDto> DanhSachLich { get; private set; } = [];
    public IReadOnlyList<DinhNghiaCaDto> DanhSachDinhNghiaCa { get; private set; } = [];
    public IReadOnlyList<PhongResponse> DanhSachPhong { get; private set; } = [];
    public int? IdBacSiDangChon { get; private set; }

    [BindProperty] public int IdBacSiInput { get; set; }
    [BindProperty] public int IdPhongInput { get; set; }
    [BindProperty] public int IdDinhNghiaCaInput { get; set; }
    [BindProperty] public int NgayTrongTuanInput { get; set; }
    [BindProperty] public DateOnly NgayApDungInput { get; set; }
    [BindProperty] public DateOnly? NgayKetThucInput { get; set; }
    [BindProperty] public int IdLichXoa { get; set; }
    [BindProperty] public int SoNgaySinh { get; set; } = 7;

    public async Task OnGetAsync(int? idBacSi = null) { /* load all + lich nếu idBacSi */ }
    public async Task<IActionResult> OnPostTaoMauAsync()  // → TaoLichNoiTruCommand
    public async Task<IActionResult> OnPostVoHieuAsync()  // → VoHieuLichNoiTruCommand
    public async Task<IActionResult> OnPostSinhCaAsync()  // → SinhCaLamViecTuLichNoiTruCommand
}
```

#### `.cshtml` cần sửa

- Thay block `bs-list-item` hardcode bằng `@foreach (var bs in Model.DanhSachBacSi)` + link `?idBacSi=@bs.IdBacSi`.
- Thay bảng lịch hardcode bằng `@foreach (var l in Model.DanhSachLich)` group theo `NgayTrongTuan`.
- Thêm form modal "Tạo mẫu lịch":
  - `<select asp-for="IdBacSiInput">` populate từ `DanhSachBacSi`.
  - `<select asp-for="IdDinhNghiaCaInput">` (Sáng/Chiều/…).
  - `<select asp-for="IdPhongInput">`.
  - Chip 2-CN cho `NgayTrongTuanInput` (giá trị 1-7 hoặc 0-6 — verify với handler hiện tại dùng `(int)ngay.DayOfWeek`).
  - `<input type="date" asp-for="NgayApDungInput">` + `NgayKetThucInput`.
  - Submit `asp-page-handler="TaoMau"`.
- Nút "Sinh ca làm việc 7 ngày tới" → form `asp-page-handler="SinhCa"` với `SoNgaySinh=7`. Hiển thị toast số ca đã sinh từ `TempData["SoCaSinh"]`.
- Nút "Vô hiệu mẫu" trên từng row → form `asp-page-handler="VoHieu"` với `IdLichXoa`.

**Cảnh báo NgayTrongTuan:** Handler `SinhCaLamViecTuLichNoiTruHandler` dùng `(int)DayOfWeek` (Sun=0, Sat=6). UI cần khớp. Validator cũng nên giới hạn 0-6.

---

### 1.2. `Admin/DuyetCa` — Duyệt ca bác sĩ hợp đồng

**File chính:** `ClinicBooking.Web/Pages/Admin/DuyetCa.cshtml(.cs)`

#### Cần tạo mới

1. `Features/Scheduling/Queries/ThongKeDuyetCa/`
   - `ThongKeDuyetCaQuery(DateOnly TuNgay, DateOnly DenNgay) : IRequest<ThongKeDuyetCaDto>`
   - DTO: `int SoChoDuyet, SoDaDuyet, SoTuChoi, SoBacSiHopDong`
   - Handler: 4 query song song (count theo `TrangThaiDuyet` + count `BacSi.Where(LoaiHopDong==HopDong)`).

2. (Tuỳ chọn) `Features/Scheduling/Commands/DuyetCaLamViecHangLoat/`
   - `DuyetCaLamViecHangLoatCommand(int[] IdCaLamViecs, bool ChapNhan, string? LyDoTuChoi, int IdAdminDuyet) : IRequest<int>`
   - Handler: loop hoặc bulk update.

#### Page Model `DuyetCaModel`

```csharp
[Authorize(Roles = VaiTroConstants.Admin)]
public class DuyetCaModel : PageModel
{
    public ThongKeDuyetCaDto ThongKe { get; private set; } = new();
    public IReadOnlyList<CaLamViecPublicResponse> DanhSachChoDuyet { get; private set; } = [];

    [BindProperty] public int IdCaLamViecChon { get; set; }
    [BindProperty] public string? LyDoTuChoi { get; set; }
    [BindProperty] public int[] IdsHangLoat { get; set; } = [];

    public async Task OnGetAsync() { /* load thongke + DanhSachCaLamViecCongKhaiQuery(trangThai=ChoDuyet) */ }
    public async Task<IActionResult> OnPostDuyetAsync()      // ChapNhan=true
    public async Task<IActionResult> OnPostTuChoiAsync()     // ChapNhan=false
    public async Task<IActionResult> OnPostDuyetTatCaAsync() // hàng loạt
}
```

> **Lưu ý:** `DanhSachCaLamViecCongKhaiQuery` hiện không có filter `TrangThaiDuyet`. Cần **bổ sung tham số** `TrangThaiDuyetCa? TrangThaiDuyet = null` hoặc tạo query mới `DanhSachCaLamViecChoDuyetQuery`. Khuyến nghị: bổ sung tham số (ít invasive hơn). Update cả `SchedulingController` + `LeTan/QuanLyCaLamViec`.

#### `.cshtml` cần sửa

- 4 KPI card → `@Model.ThongKe.SoChoDuyet`, `SoDaDuyet`, `SoTuChoi`, `SoBacSiHopDong`.
- Bảng pending → `@foreach (var ca in Model.DanhSachChoDuyet)`.
- Modal duyệt → form `asp-page-handler="Duyet"` với hidden `IdCaLamViecChon`.
- Modal từ chối → form `asp-page-handler="TuChoi"` với `LyDoTuChoi` textarea (required).
- Nút "Duyệt tất cả" → form gửi `IdsHangLoat` từ checkbox.
- `IdAdminDuyet` lấy từ `_currentUser.IdTaiKhoan` (hoặc `IdAdmin` — verify entity `Admin` có sẵn không).

---

## Phase 2 — Admin CRUD đầy đủ (3 ngày)

Áp dụng pattern: 1 entity = 3 commands (Tao/CapNhat/Xoa) + 1 query list + 1 query chi tiết + page model wire form.

### 2.1. `Admin/Accounts` (TaiKhoan)

**Commands mới** (`Features/Auth/Commands/`):
- `KhoaTaiKhoanCommand(int IdTaiKhoan, string LyDo)`
- `MoKhoaTaiKhoanCommand(int IdTaiKhoan)`
- `ResetMatKhauAdminCommand(int IdTaiKhoan, string MatKhauMoi)` — bcrypt hash
- (Optional) `TaoTaiKhoanThuCongCommand` — admin tạo nhân viên

**Query mới** (`Features/Auth/Queries/`):
- `DanhSachTaiKhoanQuery(string? VaiTro, bool? TrangThai, string? Search, int Page, int Size) : IRequest<PagedResult<TaiKhoanAdminDto>>`

### 2.2. `Admin/BacSi`

**Commands mới** (`Features/Doctors/Commands/`):
- `TaoBacSiCommand(string HoTen, int IdChuyenKhoa, LoaiHopDong, string? BangCap, int? NamKinhNghiem, string? TieuSu, int IdTaiKhoan)` — link với tài khoản đã có hoặc tạo kèm.
- `CapNhatBacSiCommand(int IdBacSi, …)` — partial update.
- `XoaBacSiCommand(int IdBacSi)` — soft delete (set `TrangThai=NgungLamViec`).

**Query mới:**
- `DanhSachBacSiAdminQuery(LoaiHopDong?, TrangThaiBacSi?, int? IdChuyenKhoa, string? Search, int Page, int Size)` — version đầy đủ hơn `DanhSachBacSiCongKhai`.
- `LayChiTietBacSiQuery(int IdBacSi)` — bao gồm số ca tháng, số bệnh nhân, đánh giá.

### 2.3. `Admin/Phong`, `Admin/ChuyenKhoa`, `Admin/DichVu`

Pattern lặp:
- `Features/DanhMuc/Commands/Tao{Entity}/`, `CapNhat{Entity}/`, `Xoa{Entity}/`
- Validator: `RuleFor(x => x.Ten).NotEmpty().MaximumLength(100)`.
- Xử lý conflict: Xoá `Phong` đang có `CaLamViec` chưa kết thúc → throw `ConflictException`.
- Wire vào page model giống pattern `LichNoiTruModel`.

### 2.4. `Admin/CaLamViec`

- Đã có `TaoCaLamViecCommand`, `XoaCaLamViecCommand`. Wire trực tiếp.
- Thêm `DanhSachCaLamViecAdminQuery` (đầy đủ hơn `CongKhai`, bao gồm cả `ChoDuyet`/`DaHuy` không lọc).

### 2.5. `Admin/Dashboard`

**Query mới** `Features/Admin/Queries/ThongKeTongHopAdmin/`:
- DTO: `int LichHomNay, LichTuanNay, DoanhThuTuanNay, DoanhThuThangNay, SoBacSiHoatDong, SoBenhNhanMoiTuan, SoCaChoDuyet, SoToaThuocChoCap`
- Handler: 8 query song song (`Task.WhenAll`).

> Lưu ý: chưa có module hoá đơn → `DoanhThu` trả 0. Chờ M3.

### 2.6. `Admin/ThongKe`

**Query mới** `Features/Admin/Queries/BaoCaoThongKe/`:
- `BaoCaoThongKeQuery(DateOnly TuNgay, DateOnly DenNgay, LoaiBaoCao Loai)` với `LoaiBaoCao = { LichHen, BacSi, ChuyenKhoa, DichVu }`.
- Trả `IReadOnlyList<RowBaoCaoDto>` để render bảng + chart.

### 2.7. `Admin/ThongBao`

- Đã có entity `ThongBao` + `MauThongBao`.
- **Command mới** `Features/ThongBao/Commands/GuiThongBaoBroadcast/` — gửi tới một role (BacSi/BenhNhan/LeTan/All).
- Page wire: form chọn role + tiêu đề + nội dung → submit.

---

## Phase 3 — Verify & integration (0.5 ngày)

### Test thủ công end-to-end

**Flow nội trú (admin):**
1. Đăng nhập admin → `Admin/BacSi` → Tạo BS Nguyen Van Test, `LoaiHopDong=NoiTru`.
2. `Admin/LichNoiTru` → chọn BS → Tạo mẫu Thứ 2, ca Sáng, P101, từ hôm nay → Lưu.
3. Nhấn "Sinh ca 7 ngày" → kiểm tra toast "Đã sinh N ca".
4. Đăng nhập bệnh nhân → đặt lịch ngày Thứ 2 gần nhất, slot Sáng → đặt thành công.

**Flow hợp đồng (bác sĩ + admin):**
1. Tạo BS hợp đồng + tài khoản tương ứng.
2. Login bác sĩ → `BacSi/QuanLyCaLamViec` → Tạo ca mới (status auto = `ChoDuyet`).
3. Login admin → `Admin/DuyetCa` → ca mới hiện trong bảng pending.
4. Duyệt → ca chuyển `DaDuyet`, refresh `KPI` count.
5. Đặt lịch bệnh nhân khớp slot vừa duyệt → thành công.

### Tests tự động

- Bổ sung integration tests:
  - `Admin_LichNoiTru_TaoMau_Should_PersistEntity`
  - `Admin_LichNoiTru_SinhCa_Should_GenerateExpectedCount`
  - `Admin_DuyetCa_Should_TransitionStatus`
  - `Admin_DuyetCa_TuChoi_Should_StoreLyDo`
  - `Admin_BacSi_TaoMoi_Should_LinkTaiKhoan`
- Chạy `dotnet test` toàn solution, đảm bảo không regression.

### Build & GitNexus

- `dotnet build DatLichPhongKham.slnx` không warning.
- `npx gitnexus analyze` để refresh index sau khi merge.

---

## Thứ tự commit đề xuất

1. `feat(admin/lich-noi-tru): wire CRUD template + sinh ca`
2. `feat(admin/duyet-ca): wire approval flow + thong ke`
3. `feat(admin/accounts): full CRUD tai khoan + reset mat khau`
4. `feat(admin/bac-si): full CRUD bac si + filter + chi tiet`
5. `feat(admin/danh-muc): wire Phong + ChuyenKhoa + DichVu CRUD`
6. `feat(admin/ca-lam-viec): wire CRUD + filter`
7. `feat(admin/dashboard-thong-ke): wire bao cao tong hop`
8. `feat(admin/thong-bao): wire broadcast notification`
9. `test(admin): integration tests cho admin portal`

Mỗi commit kèm screenshot UI vào `docs/screenshots/admin-*.png` (optional).

---

## Rủi ro & lưu ý

| Rủi ro | Cách giảm |
|---|---|
| `DanhSachCaLamViecCongKhaiQuery` đang được lễ tân dùng — bổ sung filter có thể vỡ test | Default param `null`, viết test backward-compat trước khi đổi |
| `IdAdminDuyet` cần entity `Admin` riêng hay `IdTaiKhoan` đủ? | Verify schema `clinic.dbml` — handler hiện tại nhận `int` nên ưu tiên `IdTaiKhoan` |
| `NgayTrongTuan` 0-6 vs 1-7 không nhất quán giữa UI/handler | Chốt 0-6 (theo `DayOfWeek`), update validator |
| Sinh ca trùng — handler skip nếu `(IdBacSi, NgayLamViec)` đã tồn tại, nhưng không check `IdDinhNghiaCa` | Một BS có 2 ca (Sáng/Chiều) cùng ngày sẽ bị bỏ — sửa điều kiện exists trong `SinhCaLamViecTuLichNoiTruHandler` |
| Soft-delete vs hard-delete `BacSi` | Dùng `TrangThai=NgungLamViec`, không xoá vật lý vì có `HoSoKham` tham chiếu |
| Reset mật khẩu — gửi email hay hiện màn hình? | Đề xuất: hiện modal cho admin nhập mật khẩu mới + bắt user đổi lại lần đăng nhập đầu (cờ `BatBuocDoiMatKhau`) |

---

## Định nghĩa hoàn thành (DoD)

- [x] `Pages/Admin/LichNoiTru.cshtml.cs` wire MediatR đầy đủ.
- [x] `Pages/Admin/DuyetCa.cshtml.cs` wire MediatR đầy đủ.
- [x] `Pages/Admin/BacSi.cshtml.cs` wire MediatR đầy đủ (CRUD + đổi trạng thái).
- [x] `Pages/Admin/CaLamViec.cshtml.cs` wire MediatR đầy đủ (list + filter + tạo + xoá).
- [ ] Còn 5 trang stub: `Accounts`, `Phong`, `ChuyenKhoa`, `DichVu`, `ThongBao`, `ThongKe` (Dashboard có một phần).
- [x] Mock data trong `LichNoiTru.cshtml` + `DuyetCa.cshtml` + `BacSi.cshtml` + `CaLamViec.cshtml` đã thay bằng `@foreach` từ Model.
- [ ] Còn mock trong các trang admin khác.
- [x] `dotnet build` xanh sau cả 3 phase.
- [ ] `dotnet test` chưa chạy lại sau Phase 1+2.2 — cần verify regression.
- [x] Admin có thể CRUD BS (Phase 2.2) → tạo được BS hợp đồng qua UI.
- [ ] Admin tạo BS nội trú → cấu hình lịch → sinh ca → bệnh nhân đặt — chờ verify UI thủ công.
- [ ] Admin thấy ca pending → duyệt/từ chối → trạng thái thay đổi đúng — chờ verify UI thủ công.
- [ ] Admin CRUD được Account/Phong/ChuyenKhoa/DichVu (BacSi đã xong).
- [ ] Dashboard hiển thị số liệu thực (không hardcode), trừ phần doanh thu (chờ M3).
