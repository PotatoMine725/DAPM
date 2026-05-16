# Yêu cầu Module 4 — Wire trang Admin/Accounts (Quản lý tài khoản)

**Ngày lập:** 2026-05-16
**Người gửi:** Module 1 owner (Potatomine725)
**Người nhận:** Thành viên phụ trách Module 4 (Admin UI / Thông báo)
**Trạng thái nguồn:** Phase 2.1 của `docs/Plans/m1-wire-up-admin.md` — Module 1 đã wire xong 6/9 trang admin; trang `Admin/Accounts` thuộc scope Module 4 theo `ke-hoach-hoan-thien-ui-module1.md` (mục "Admin_Van/ accounts.html → 4/Admin").

---

## 1. Mục tiêu

Thay toàn bộ mock-data trong `ClinicBooking.Web/Pages/Admin/Accounts.cshtml(.cs)` bằng MediatR thật. Admin có thể:

1. Xem danh sách tài khoản phân trang + lọc theo vai trò / trạng thái / từ khoá.
2. Tạo tài khoản nhân viên thủ công (LeTan / Admin) — riêng BacSi đã có ở `Admin/BacSi`, BenhNhan tự đăng ký.
3. Khoá / mở khoá tài khoản (kèm lý do, audit log).
4. Reset mật khẩu cho user (gửi mật khẩu tạm hoặc gửi link reset qua email).

> **Phạm vi không bao gồm:** sửa lại `TaiKhoan` entity, đổi cấu trúc bảng. Chỉ thêm Commands/Queries Application + wire Web.

---

## 2. Backend cần tạo mới (Application layer)

### 2.1. Queries

#### `Features/Auth/Queries/DanhSachTaiKhoan/`

```csharp
public sealed record DanhSachTaiKhoanQuery(
    int SoTrang = 1,
    int KichThuocTrang = 50,
    VaiTro? VaiTro = null,
    bool? TrangThai = null,
    string? TuKhoa = null) : IRequest<DanhSachTaiKhoanResponse>;

public sealed record TaiKhoanAdminResponse(
    int IdTaiKhoan,
    string TenDangNhap,
    string Email,
    string SoDienThoai,
    string VaiTro,
    bool TrangThai,
    DateTime? LanDangNhapCuoi,
    DateTime NgayTao,
    string? HoTenHienThi); // join BacSi/BenhNhan/LeTan để lấy họ tên

public sealed record DanhSachTaiKhoanResponse(
    IReadOnlyList<TaiKhoanAdminResponse> Items,
    int TongSo,
    int SoTrang,
    int KichThuocTrang);
```

**Handler chú ý:**
- `AsNoTracking()`.
- Tìm kiếm theo `TuKhoa` trên `TenDangNhap`, `Email`, `SoDienThoai` (Contains).
- Filter `VaiTro` enum, `TrangThai` bool.
- Join navigation `BenhNhan`/`BacSi`/`LeTan` để lấy `HoTen` hiển thị (left join — admin không có entity riêng nên fallback `TenDangNhap`).
- Sắp xếp: `NgayTao desc`.
- Trả về `TongSo` để page hỗ trợ phân trang đúng.

### 2.2. Commands

#### `Features/Auth/Commands/KhoaTaiKhoan/`

```csharp
public sealed record KhoaTaiKhoanCommand(int IdTaiKhoan, string LyDo) : IRequest<Unit>;
```

Handler:
- Lấy `TaiKhoan` theo Id → throw `NotFoundException` nếu null.
- Set `TrangThai = false`. Lưu `LyDo` vào audit log (xem 2.4 bên dưới).
- Revoke toàn bộ `RefreshToken` còn hiệu lực của user (set `DaThuHoi = true`).
- Validator: `LyDo` not empty, max 500 ký tự.

#### `Features/Auth/Commands/MoKhoaTaiKhoan/`

```csharp
public sealed record MoKhoaTaiKhoanCommand(int IdTaiKhoan) : IRequest<Unit>;
```

Handler: set `TrangThai = true`. Ghi audit log "Mo khoa boi admin {IdAdmin}".

#### `Features/Auth/Commands/ResetMatKhauAdmin/`

```csharp
public sealed record ResetMatKhauAdminCommand(
    int IdTaiKhoan,
    string MatKhauMoi,
    bool GuiQuaEmail = false) : IRequest<Unit>;
```

Handler:
- Hash `MatKhauMoi` qua `IPasswordHasher` đang dùng (BCrypt — xem `Infrastructure/Services/Security/PasswordHasher.cs`).
- Update `TaiKhoan.MatKhau`.
- Revoke RefreshToken (user phải đăng nhập lại).
- Nếu `GuiQuaEmail = true`: dùng `INotificationService` Module 4 để gửi email plain-text mật khẩu mới (tạm — production nên đổi sang link reset, ngoài scope này).
- Validator: `MatKhauMoi` ≥ 8 ký tự, có chữ + số (đồng bộ rule với `DangKy`).

#### `Features/Auth/Commands/TaoTaiKhoanNhanVien/`

```csharp
public sealed record TaoTaiKhoanNhanVienCommand(
    string TenDangNhap,
    string Email,
    string SoDienThoai,
    string MatKhau,
    VaiTro VaiTro, // chỉ LeTan hoặc Admin
    string HoTen) : IRequest<int>;
```

Handler:
- Validate `VaiTro` ∈ {`LeTan`, `Admin`} — reject `BacSi` (đi qua `/Admin/BacSi`), `BenhNhan` (tự đăng ký).
- Check duplicate `TenDangNhap` / `Email` / `SoDienThoai`.
- Tạo `TaiKhoan` + `LeTan` entity (nếu `VaiTro = LeTan`) qua navigation, 1 `SaveChangesAsync()`.
- Với `VaiTro = Admin`: hiện entity `Admin` chưa có (xem `clinic.dbml`) — chấp nhận tạo `TaiKhoan` rời, không link entity con.
- Return `IdTaiKhoan` mới.

### 2.3. Validator

Mỗi command dùng FluentValidation, file `<Command>Validator.cs` cùng folder. Pattern theo `TaoBacSiValidator.cs` (Phase 2.2).

### 2.4. Audit log (tuỳ chọn nhưng khuyến nghị)

Module 4 đã có entity `ThongBao` / `MauThongBao`. Đề xuất thêm 1 trong 2 cách:

- **Cách A (nhẹ):** ghi `Microsoft.Extensions.Logging` với prefix `[AUDIT-ACCOUNT]` — không persist, đủ cho dev/demo.
- **Cách B (đầy đủ):** tạo entity `LichSuKhoaTaiKhoan(IdTaiKhoan, IdAdminThucHien, HanhDong, LyDo, ThoiGian)` + migration. Chỉ làm nếu thời gian cho phép.

Khuyến nghị **Cách A** cho phase này — Cách B làm sau khi Module 4 hoàn thiện entity thông báo.

---

## 3. UI cần wire (Web layer)

### 3.1. Files chạm

- `ClinicBooking.Web/Pages/Admin/Accounts.cshtml` — thay mock table bằng `@foreach`.
- `ClinicBooking.Web/Pages/Admin/Accounts.cshtml.cs` — inject `IMediator`, expose properties + 4 handler.

### 3.2. PageModel pattern (copy từ `BacSi.cshtml.cs` Phase 2.2)

```csharp
[Authorize(Roles = VaiTroConstants.Admin)]
public class AccountsModel : PageModel
{
    private readonly IMediator _mediator;

    public AccountsModel(IMediator mediator) => _mediator = mediator;

    public DanhSachTaiKhoanResponse DuLieu { get; private set; } = new(Array.Empty<TaiKhoanAdminResponse>(), 0, 1, 50);
    public VaiTro? VaiTroLoc { get; private set; }
    public bool? TrangThaiLoc { get; private set; }
    public string? TuKhoa { get; private set; }
    public int Trang { get; private set; } = 1;

    [BindProperty] public int IdTaiKhoanThaoTac { get; set; }
    [BindProperty] public string? LyDoKhoa { get; set; }
    [BindProperty] public string? MatKhauMoi { get; set; }
    [BindProperty] public bool GuiQuaEmail { get; set; }

    // Tạo mới
    [BindProperty] public string TenDangNhap { get; set; } = "";
    [BindProperty] public string Email { get; set; } = "";
    [BindProperty] public string SoDienThoai { get; set; } = "";
    [BindProperty] public string MatKhau { get; set; } = "";
    [BindProperty] public string HoTen { get; set; } = "";
    [BindProperty] public VaiTro VaiTroInput { get; set; } = VaiTro.LeTan;

    public async Task OnGetAsync(VaiTro? vaiTro, bool? trangThai, string? tuKhoa, int trang = 1) { ... }
    public Task<IActionResult> OnPostTaoAsync() { ... }
    public Task<IActionResult> OnPostKhoaAsync() { ... }
    public Task<IActionResult> OnPostMoKhoaAsync() { ... }
    public Task<IActionResult> OnPostResetMatKhauAsync() { ... }
}
```

### 3.3. UI yêu cầu

- **Flash message:** dùng `TempData["SuccessMessage"]/["ErrorMessage"]` render inline (pattern `LichNoiTru.cshtml`).
- **Filter bar:** form `method="get"` với 3 select (VaiTro / TrangThai / TuKhoa) + `onchange="this.form.submit()"`.
- **Phân trang:** dùng `?trang=N` GET param. Hiển thị tổng/trang ở footer.
- **Modal tạo:** form `asp-page-handler="Tao"`, dropdown VaiTro chỉ có LeTan + Admin.
- **Modal khoá:** form `asp-page-handler="Khoa"` với `IdTaiKhoanThaoTac` (hidden) + `LyDoKhoa` (textarea required).
- **Modal reset password:** form `asp-page-handler="ResetMatKhau"` với `MatKhauMoi` + checkbox `GuiQuaEmail`.
- **Nút mở khoá inline** trên row đã khoá: form `asp-page-handler="MoKhoa"`, không cần modal.

### 3.4. Quyết định kiến trúc (theo Phase 2.2)

- Modal sửa populate qua JSON inline: `onclick='moModalKhoa(@Html.Raw(JsonSerializer.Serialize(tk)))'`.
- Không gọi REST API riêng — tất cả qua page handlers.
- Không sửa `_AdminLayout` — dùng `class .show` cho modal qua `openModal('id')` / `closeModal('id')` JS có sẵn.

---

## 4. Test

### 4.1. Unit test (Application layer)

File `ClinicBooking.Application.UnitTests/Features/Auth/`:

- `DanhSachTaiKhoanHandlerTests` — verify filter VaiTro/TrangThai/TuKhoa, phân trang, sort.
- `KhoaTaiKhoanHandlerTests` — verify `TrangThai = false` + revoke RefreshToken.
- `MoKhoaTaiKhoanHandlerTests` — verify `TrangThai = true`.
- `ResetMatKhauAdminHandlerTests` — verify hash mới khác cũ, revoke token, gọi `INotificationService` nếu `GuiQuaEmail`.
- `TaoTaiKhoanNhanVienHandlerTests` — verify reject duplicate, reject `VaiTro = BacSi/BenhNhan`, tạo `LeTan` entity khi `VaiTro = LeTan`.

Dùng `InMemoryDb` fixture có sẵn (`TestAppDbContextFactory`).

### 4.2. Integration test (`ClinicBooking.Integration.Tests`)

- `Admin_KhoaTaiKhoan_Should_DenyLogin` — sau khi khoá, login với tài khoản đó → 401/403.
- `Admin_ResetMatKhau_Should_LoginVoiMatKhauMoi` — reset rồi login bằng mật khẩu mới.
- `Admin_TaoTaiKhoanNhanVien_Should_AllowLogin` — tạo lễ tân mới, lễ tân login OK.

### 4.3. Test thủ công UI

- Tạo lễ tân mới qua modal → đăng nhập bằng tài khoản đó.
- Khoá tài khoản → user bị đăng xuất ngay (RefreshToken revoke).
- Mở khoá → login lại OK.
- Reset password → login bằng mật khẩu mới.
- Filter combinations (VaiTro × TrangThai × TuKhoa).

---

## 5. Workflow ship

### 5.1. Branch

```
git fetch origin
git checkout -b feature/module4/admin-accounts-wire origin/develop
```

> **Không nhánh ra từ `feature/module1/portal-sat-demo`** — Module 1 branch sẽ merge develop trước. Khi Module 1 PR đã merge, Module 4 rebase nếu cần.

> **Tham khảo code mẫu:** Module 1 đã mở PR vào `develop` với 6 trang admin đã wire (`Admin/BacSi`, `Admin/CaLamViec`, `Admin/DuyetCa`, `Admin/LichNoiTru`, `Admin/Phong`, `Admin/ChuyenKhoa`, `Admin/DichVu`). Khi PR đó merge, pull `develop` rồi đọc `Admin/BacSi.cshtml(.cs)` làm pattern chính cho `Admin/Accounts`. Nếu cần xem sớm: checkout `feature/module1/portal-sat-demo`.

### 5.2. Commit message convention

```
feat(admin/accounts): wire CRUD tai khoan + khoa/mo khoa + reset mat khau

- Add DanhSachTaiKhoanQuery + TaiKhoanAdminResponse
- Add KhoaTaiKhoanCommand + MoKhoaTaiKhoanCommand
- Add ResetMatKhauAdminCommand + TaoTaiKhoanNhanVienCommand
- Wire Admin/Accounts.cshtml(.cs): list + 3 filter + 3 modal
- Add unit/integration tests

Co-Authored-By: ...
```

Tách 2-3 commit nếu thuận: 1 cho Application, 1 cho Web, 1 cho test.

### 5.3. Trước khi push

```powershell
dotnet build DatLichPhongKham.slnx       # phải 0 errors
dotnet test                                # toàn bộ test pass
```

Nếu có pre-commit hook fail → fix root cause, **không dùng `--no-verify`**.

### 5.4. Pull Request

- **Base branch:** `develop`
- **Title:** `feat(admin/accounts): wire CRUD tai khoan`
- **Body:** copy phần "Mục tiêu" + "Backend cần tạo mới" + checklist "Test thủ công" từ file này.
- **Reviewer:** Module 1 owner (Potatomine725) — để verify không vỡ flow `DangNhap` / `LamMoiToken`.
- **Tag:** `module4`, `admin-portal`.

### 5.5. Sau khi merge develop

Báo lại Module 1 owner để cập nhật `docs/Plans/m1-wire-up-admin.md` (Phase 2.1 DONE).

---

## 6. Backend đã có (không cần code lại)

| Component | Path | Dùng để |
|---|---|---|
| `IPasswordHasher` | `Application/Abstractions/Security/IPasswordHasher.cs` | Hash mật khẩu mới |
| `BCryptPasswordHasher` | `Infrastructure/Services/Security/PasswordHasher.cs` | Implementation |
| `ITokenService` | `Application/Abstractions/Security/ITokenService.cs` | (Không dùng trực tiếp, nhưng tham khảo cách revoke) |
| `RefreshToken` entity | `Domain/Entities/RefreshToken.cs` | Field `DaThuHoi` để revoke |
| `INotificationService` (stub) | `Application/Abstractions/Notifications/` | Gửi email reset (nếu Module 4 đã có impl thật) |
| `VaiTroConstants` | `Application/Common/Constants/VaiTroConstants.cs` | `[Authorize(Roles = VaiTroConstants.Admin)]` |
| `VaiTro` enum | `Domain/Enums/VaiTro.cs` | Filter dropdown |

---

## 7. Rủi ro & lưu ý

| Rủi ro | Cách giảm |
|---|---|
| Khoá tài khoản admin duy nhất → mất quyền truy cập | Validator: reject `KhoaTaiKhoanCommand` nếu user đang khoá là Admin duy nhất còn hoạt động |
| Reset password gửi qua email plaintext | Chấp nhận cho phase này (dev/demo). Production nên đổi sang link 1-lần (ngoài scope) |
| Tạo Admin qua UI có thể bị abuse | UI ẩn option Admin nếu user hiện tại không phải Admin gốc (kiểm tra `ICurrentUserService`) — hoặc dùng claim riêng |
| Filter `TuKhoa` chậm trên DB lớn | Đảm bảo index trên `TenDangNhap`, `Email` (đã có theo migration cũ) |
| `RefreshToken` revoke không đồng bộ với JWT đang còn hạn | JWT access token short-lived (15 phút). Có thể chấp nhận latency 15 phút. Document rõ trong PR. |

---

## 8. Ưu tiên mong muốn

1. **Cao** — Backend Commands/Queries + UI wire (đầy đủ 4 hành động).
2. **Trung** — Unit/integration test.
3. **Thấp** — Audit log persist (Cách B mục 2.4).

---

## 9. Liên hệ

Mở issue trên GitHub repo hoặc nhắn trực tiếp Module 1 owner nếu cần làm rõ:

- Cấu trúc `TaiKhoanAdminResponse` (cần thêm field nào không?)
- Pattern `Admin/BacSi` Phase 2.2 (file `Pages/Admin/BacSi.cshtml(.cs)`) — **dùng làm reference chính** cho trang Accounts.
- Cách integrate `INotificationService` cho reset password.

Module 1 owner sẽ review PR trong vòng 2 ngày làm việc kể từ khi nhận notification.
