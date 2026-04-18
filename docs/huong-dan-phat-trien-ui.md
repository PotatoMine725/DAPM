# Hướng dẫn phát triển UI — ClinicBooking.Web

> Tài liệu này mô tả kiến trúc, quy ước và quy trình thêm trang mới cho project `ClinicBooking.Web` (Razor Pages).  
> **Đọc kỹ trước khi bắt đầu thiết kế UI cho module của bạn.**

---

## 1. Tổng quan kiến trúc

`ClinicBooking.Web` là một project **ASP.NET Core Razor Pages** nằm trong cùng solution với các project backend.  
Nó giao tiếp trực tiếp với Application layer qua **MediatR** — **không** gọi HTTP đến API.

```
ClinicBooking.Web (Razor Pages)
    │
    ├── IMediator.Send(Command/Query) ──▶ Application layer
    │                                         ↓
    │                               Handler → Infrastructure → DB
    │
    └── Cookie Authentication (không dùng JWT)
```

### Lý do chọn Razor Pages thay vì React/Angular
- Không cần cài node/npm trên máy dev
- Tích hợp thẳng với DI container của .NET
- Phù hợp với nghiệp vụ form-heavy (đặt lịch, quản lý hàng chờ)
- Dễ cho team .NET không quen frontend framework

---

## 2. Cấu trúc thư mục

```
ClinicBooking.Web/
├── Pages/
│   ├── Shared/
│   │   ├── _Layout.cshtml          ← Layout chung (sidebar + topbar)
│   │   └── _LoginLayout.cshtml     ← Layout trang đăng nhập
│   ├── Auth/
│   │   ├── DangNhap.cshtml(.cs)    ← Đăng nhập
│   │   └── DangXuat.cshtml.cs      ← Đăng xuất
│   ├── LeTan/
│   │   ├── Dashboard.cshtml(.cs)
│   │   ├── QuanLyLichHen.cshtml(.cs)
│   │   └── HangCho.cshtml(.cs)
│   ├── BacSi/
│   │   └── HangCho.cshtml(.cs)
│   └── BenhNhan/
│       └── DanhSachLichHen.cshtml(.cs)
├── Helpers/
│   └── BadgeHelper.cs              ← Render HTML badge cho enum trạng thái
├── wwwroot/
│   ├── css/
│   │   ├── common.css              ← CSS variables, reset, typography
│   │   ├── components.css          ← Button, badge, card, table, form, modal...
│   │   └── layout.css              ← App shell, sidebar, topbar
│   └── js/
│       └── app.js                  ← openModal/closeModal/showToast helpers
└── Program.cs                      ← DI setup, cookie auth, AddApplication(), AddInfrastructure()
```

---

## 3. Quy trình thêm trang mới cho module của bạn

### Bước 1 — Xác định quyền truy cập

Mỗi trang thuộc về một role cụ thể:

| Role | Folder | Ví dụ |
|---|---|---|
| `le_tan` hoặc `admin` | `Pages/LeTan/` | Dashboard, QuanLyLichHen |
| `bac_si` | `Pages/BacSi/` | HangCho |
| `benh_nhan` | `Pages/BenhNhan/` | DanhSachLichHen |
| `admin` | `Pages/Admin/` | (chưa có, module Admin sẽ tạo) |

### Bước 2 — Tạo cặp file `.cshtml` + `.cshtml.cs`

**File `.cshtml.cs` (Page Model):**

```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using YourQuery;  // import từ Application layer

namespace ClinicBooking.Web.Pages.TenModule;

[Authorize(Roles = "ten_role")]       // LUÔN đặt Authorize
public class TenTrangModel : PageModel
{
    private readonly IMediator _mediator;

    public TenTrangModel(IMediator mediator) => _mediator = mediator;

    // Properties để cshtml đọc
    public SomeResponseDto Data { get; private set; } = default!;

    // GET handler
    public async Task OnGetAsync(/* query params */)
    {
        Data = await _mediator.Send(new SomeQuery(/* params */));
    }

    // POST handler (nếu có action)
    public async Task<IActionResult> OnPostTenHanhDongAsync(int id)
    {
        try
        {
            await _mediator.Send(new SomeCommand(id));
            TempData["SuccessMessage"] = "Thao tác thành công.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToPage();
    }
}
```

**File `.cshtml`:**

```razor
@page
@model ClinicBooking.Web.Pages.TenModule.TenTrangModel
@{
    ViewData["Title"] = "Tên trang hiển thị";
}

<div class="page-header">
    <h1 class="page-title">Tên trang</h1>
</div>

<!-- Nội dung trang -->
```

### Bước 3 — Thêm link vào sidebar

Mở `Pages/Shared/_Layout.cshtml`, tìm block `@if (vaiTro == "ten_role")` tương ứng và thêm:

```html
<li>
    <a class="sidebar-nav-item @(currentPath.StartsWith("/TenModule/TenTrang") ? "active" : "")"
       href="/TenModule/TenTrang">
        <i class="ph ph-ten-icon"></i> Tên mục
    </a>
</li>
```

> Xem danh sách icon tại: https://phosphoricons.com — copy class name dạng `ph ph-ten-icon`

---

## 4. Hệ thống CSS — component có sẵn

### 4.1 Layout trang

```html
<!-- Header trang -->
<div class="page-header">
    <h1 class="page-title">Tiêu đề</h1>
    <!-- Các nút hành động bên phải -->
    <button class="btn btn-primary">Thêm mới</button>
</div>

<!-- Card container -->
<div class="card">
    <div class="card-header">
        <span class="card-title">Tiêu đề card</span>
        <span>Phần bên phải header</span>
    </div>
    <!-- Nội dung -->
</div>
```

### 4.2 Button

```html
<button class="btn btn-primary">Hành động chính</button>
<button class="btn btn-secondary">Hành động phụ</button>
<button class="btn btn-danger">Xoá / Huỷ</button>
<button class="btn btn-ghost">Đóng / Bỏ qua</button>

<!-- Kích thước nhỏ (trong bảng) -->
<button class="btn btn-sm btn-primary">Xác nhận</button>

<!-- Icon only -->
<button class="btn btn-icon btn-ghost"><i class="ph ph-x"></i></button>
```

### 4.3 Badge trạng thái

Dùng `BadgeHelper` trong cshtml:

```razor
@Html.Raw(BadgeHelper.TrangThaiLichHen(lh.TrangThai))
@Html.Raw(BadgeHelper.TrangThaiHangCho(hc.TrangThai))
```

Nếu cần thêm badge cho enum mới của module, mở `Helpers/BadgeHelper.cs` và thêm method tương tự.

Các class badge có sẵn: `badge-success`, `badge-warning`, `badge-danger`, `badge-info`, `badge-active`, `badge-neutral`.

### 4.4 Bảng dữ liệu

```html
<table class="data-table">
    <thead>
        <tr>
            <th>Cột 1</th>
            <th>Trạng thái</th>
            <th>Thao tác</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var item in Model.DanhSach)
        {
            <tr>
                <td><span class="table-code">@item.MaCode</span></td>
                <td>@Html.Raw(BadgeHelper.TrangThaiLichHen(item.TrangThai))</td>
                <td class="table-actions">
                    <!-- Các button hành động -->
                </td>
            </tr>
        }
    </tbody>
</table>
```

### 4.5 Form

```html
<div class="form-group">
    <label class="form-label">Tên trường <span style="color:red">*</span></label>
    <input type="text" class="form-input" asp-for="TenTruong" />
    <span asp-validation-for="TenTruong" class="text-danger"></span>
</div>

<div class="form-group">
    <label class="form-label">Dropdown</label>
    <select class="form-select" asp-for="TrangThai">
        <option value="">— Chọn —</option>
        <!-- Options -->
    </select>
</div>
```

### 4.6 Modal

```html
<!-- Nút mở modal -->
<button type="button" class="btn btn-primary" onclick="openModal('modal-them-moi')">
    Thêm mới
</button>

<!-- Modal -->
<div id="modal-them-moi" class="modal-overlay" hidden>
    <div class="modal-box">
        <div class="modal-header">
            <span class="modal-title">Tiêu đề modal</span>
            <button type="button" class="btn btn-ghost btn-icon" onclick="closeModal('modal-them-moi')">
                <i class="ph ph-x"></i>
            </button>
        </div>
        <div style="padding: var(--space-4) var(--space-5);">
            <form method="post" asp-page-handler="ThemMoi">
                <!-- Form fields -->
                <div style="display:flex; gap:var(--space-3); justify-content:flex-end; margin-top:var(--space-4);">
                    <button type="button" class="btn btn-ghost" onclick="closeModal('modal-them-moi')">Đóng</button>
                    <button type="submit" class="btn btn-primary">Lưu</button>
                </div>
            </form>
        </div>
    </div>
</div>
```

**Lưu ý ID modal**: Khi render trong vòng lặp (mỗi row có modal riêng), thêm ID vào để tránh trùng:

```razor
@foreach (var item in Model.DanhSach)
{
    <button onclick="openModal('modal-huy-@item.Id')">Huỷ</button>
    <div id="modal-huy-@item.Id" class="modal-overlay" hidden>
        <!-- ... -->
    </div>
}
```

### 4.7 Stats grid (KPI cards)

```html
<div class="stats-grid" style="grid-template-columns: repeat(4,1fr);">
    <div class="stat-card">
        <div class="stat-icon" style="background:var(--color-primary-light); color:var(--color-primary);">
            <i class="ph ph-calendar-check"></i>
        </div>
        <div>
            <div class="stat-value">@Model.SoLuong</div>
            <div class="stat-label">Nhãn mô tả</div>
        </div>
    </div>
    <!-- Thêm stat-card khác -->
</div>
```

### 4.8 Alert (thông báo inline)

```html
<div class="alert alert-success">Thao tác thành công.</div>
<div class="alert alert-danger">Có lỗi xảy ra.</div>
<div class="alert alert-warning">Cảnh báo.</div>
```

> TempData["SuccessMessage"] và TempData["ErrorMessage"] được render tự động bởi `_Layout.cshtml` — không cần tự render trong từng trang.

### 4.9 Toast (thông báo nổi)

```javascript
// Từ JS
showToast("Thao tác thành công!", "success");
showToast("Có lỗi xảy ra.", "danger");
```

### 4.10 Empty state

```html
<div class="empty-state">
    <div class="empty-state-icon"><i class="ph ph-folder-open"></i></div>
    <p>Không có dữ liệu.</p>
    <a href="/TenTrang/ThemMoi" class="btn btn-primary">Tạo mới</a>
</div>
```

---

## 5. CSS Variables — bảng màu và spacing

Tất cả màu và spacing được định nghĩa trong `wwwroot/css/common.css` dưới `:root`.  
**Không hardcode màu hex hoặc pixel trong inline style** — hãy dùng variable.

### Màu chính

| Variable | Mô tả |
|---|---|
| `--color-primary` | Xanh y tế chính (`#1a5fa8`) |
| `--color-primary-dark` | Hover/active trạng thái |
| `--color-primary-light` | Nền icon, highlight nhẹ |
| `--color-bg` | Nền trang (`#f5f6f8`) |
| `--color-surface` | Nền card (`#ffffff`) |
| `--color-border` | Viền (`#dde3ed`) |
| `--color-text-primary` | Chữ chính (`#1e2a3a`) |
| `--color-text-secondary` | Chữ phụ (`#5f6b7c`) |

### Màu trạng thái

| Variable | Dùng cho |
|---|---|
| `--color-success-bg` / `--color-success-text` | Thành công, đã duyệt |
| `--color-warning-bg` / `--color-warning-text` | Chờ duyệt, cảnh báo |
| `--color-danger-bg` / `--color-danger-text` | Huỷ, lỗi |
| `--color-active-bg` / `--color-active-text` | Đang hoạt động (khám) |
| `--color-neutral-bg` / `--color-neutral-text` | Trạng thái trung tính |

### Spacing

Dùng `var(--space-1)` đến `var(--space-10)` (tương ứng 4px đến 40px):

```
--space-1: 4px
--space-2: 8px
--space-3: 12px
--space-4: 16px
--space-5: 20px
--space-6: 24px
--space-8: 32px
--space-10: 40px
```

---

## 6. Authentication và phân quyền

### Cookie Auth
- Đăng nhập qua `POST /Auth/DangNhap` → tạo cookie phiên 8 giờ
- Đăng xuất qua `POST /Auth/DangXuat`
- Trang 403 hiển thị tại `/Auth/TuChoi`

### Phân quyền trong Page Model

```csharp
// Một role
[Authorize(Roles = "bac_si")]

// Nhiều role
[Authorize(Roles = "le_tan,admin")]

// Tất cả đã đăng nhập
[Authorize]
```

### Lấy thông tin user hiện tại trong Page Model

```csharp
// Trong handler:
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
var role   = User.FindFirst(ClaimTypes.Role)?.Value;
var name   = User.Identity?.Name;
```

### Lấy thông tin user trong `.cshtml`

```razor
@{
    var vaiTro = User.FindFirst(ClaimTypes.Role)?.Value;
    var name = User.Identity?.Name;
}
```

---

## 7. Giao tiếp với Application layer

### Sử dụng IMediator trực tiếp (KHÔNG gọi HTTP)

```csharp
// Query (đọc dữ liệu)
var result = await _mediator.Send(new TenQuery(param1, param2));

// Command (thay đổi dữ liệu)
await _mediator.Send(new TenCommand(param1, param2));
```

### Xử lý lỗi chuẩn

```csharp
try
{
    await _mediator.Send(new TenCommand(...));
    TempData["SuccessMessage"] = "Thao tác thành công.";
}
catch (NotFoundException)
{
    TempData["ErrorMessage"] = "Không tìm thấy bản ghi.";
}
catch (ForbiddenException)
{
    return Forbid();
}
catch (ConflictException ex)
{
    TempData["ErrorMessage"] = ex.Message;
}
catch (Exception ex)
{
    TempData["ErrorMessage"] = ex.Message;
}
return RedirectToPage();
```

> `NotFoundException`, `ConflictException`, `ForbiddenException` đều có trong `ClinicBooking.Application.Common.Exceptions`.

---

## 8. Trang nào cần tạo thêm (roadmap)

Dưới đây là danh sách trang chưa có, mỗi module tự phụ trách phần của mình:

| Module | Trang cần tạo | Queries/Commands cần dùng |
|---|---|---|
| Module 1 (đã xong) | `BenhNhan/DatLich` | `TaoLichHenCommand` (blocked: cần Module 2 thật) |
| Module 2 (Ca làm việc) | `BacSi/CaLamViec`, `LeTan/QuanLyCa` | Queries/Commands của Module 2 |
| Module 3 (Hồ sơ BN) | `BenhNhan/HoSo`, `LeTan/HoSoBenhNhan` | Queries/Commands của Module 3 |
| Module 4 (Thông báo) | `BenhNhan/ThongBao` | Queries của Module 4 |
| Admin (chung) | `Admin/Dashboard`, `Admin/QuanLyTaiKhoan` | Queries tổng hợp |

### Khi Module 2 (ca làm việc) hoàn thành

Trang `BenhNhan/DatLich` cần:
1. Hiển thị danh sách chuyên khoa / bác sĩ → cần Query từ Module 2/3
2. Chọn ca làm việc có slot trống → `ICaLamViecQueryService.KiemTraSlotTrongAsync` đã có
3. Submit → `TaoLichHenCommand` từ Module 1 (đã sẵn sàng)

---

## 9. Lưu ý quan trọng

### Naming convention
- **Tên file, class, method**: tiếng Việt không dấu, PascalCase cho class, camelCase cho variable
- **Không dùng tiếng Anh** cho tên nghiệp vụ (xem `CLAUDE.md` mục 11)
- Page Model class: `TenTrangModel` trong namespace `ClinicBooking.Web.Pages.TenFolder`

### Icon
- Dùng **Phosphor Icons** (đã load qua CDN trong `_Layout.cshtml`)
- Class format: `ph ph-ten-icon` (outlined) hoặc `ph-fill ph-ten-icon` (filled)
- Tìm icon: https://phosphoricons.com

### Không thêm CSS riêng vào từng trang
- Tất cả style dùng chung class trong `components.css`
- Nếu cần style đặc biệt, dùng inline CSS với CSS variable — không hardcode màu
- Nếu cần style chỉ cho 1 trang, đặt trong `@section Scripts { <style>...</style> }`

### Form POST và CSRF
- Razor Pages tự động thêm anti-forgery token vào form `method="post"`
- Không cần làm gì thêm

### Redirect sau POST
- Luôn `return RedirectToPage()` sau POST để tránh double-submit khi refresh
- Truyền thông báo qua `TempData["SuccessMessage"]` hoặc `TempData["ErrorMessage"]`

---

## 10. Chạy project

```bash
# Từ root của solution
dotnet run --project ClinicBooking.Web

# Mặc định chạy trên http://localhost:5xxx (xem launchSettings.json)
```

Tài khoản seed mặc định (dev only) xem trong `DatabaseSeeder.cs`.

---

*Cập nhật lần cuối: 2026-04-18 — Module 1 Wave 3 hoàn tất.*
