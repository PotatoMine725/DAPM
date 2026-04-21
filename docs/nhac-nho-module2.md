# Ghi chu trao doi ve Module 2 (Danh muc: ChuyenKhoa / DichVu / Phong / DinhNghiaCa)

Chao ban,

Sau khi merge nhanh `feature/module1/integrate-module2` vao `develop` ngay 2026-04-21, minh co ra soat lai Module 2 de bao dam dong bo voi nhanh Module 1 va voi chuan chung trong `CLAUDE.md`. Noi chung phan handler + controller ban viet **rat tot** (xem muc 3); chi co mot so cho can ban xem qua, phan lon nam o lop Razor Pages moi them vao project API.

Muc dich tai lieu: minh muon gui ban **truoc khi team bat dau front-end va truoc khi Module 2 mo rong scope**, de ca hai thong nhat huong xu ly — tranh tinh trang fix sau khi da co code phu thuoc len tren.

Neu co cho nao ban thay minh hieu sai, cu reply lai — minh se cap nhat tai lieu.

**Ngay ra soat**: 2026-04-21
**Nhanh ra soat**: `feature/module1/integrate-module2` (da merge vao `develop`)
**Pham vi**: toan bo code Module 2 (Application/Features/DanhMuc, Api/Controllers/DanhMucController, Api/Contracts/DanhMuc, Api/Pages/Module2, test tuong ung).

---

## 1. Can xu ly truoc khi ship

### 1.1. Razor Pages lan vao project Web API — lech kien truc du an

**File:**
- `ClinicBooking.Api/Pages/Module2/ChuyenKhoa.cshtml`
- `ClinicBooking.Api/Pages/Module2/ChuyenKhoa.cshtml.cs`
- `ClinicBooking.Api/Pages/Shared/_Layout.cshtml`, `_ViewImports.cshtml`, `_ViewStart.cshtml`
- `ClinicBooking.Api/Program.cs` dong 17 (`AddRazorPages`) va dong 86 (`MapRazorPages`)

**Van de:**
- `CLAUDE.md` muc **2. Architecture** xac dinh ro `ClinicBooking.Api` la **"ASP.NET Core Web API, controllers, middleware"**, khong phai host UI.
- `CLAUDE.md` muc **4. MUST NOT** ghi ro: *"Mix UI logic into backend"*.
- Dinh huong du an (xem `docs/huong-dan-phat-trien-ui.md` va `docs/phan-chia-module-va-huong-dan-du-an.md`) la front-end tach rieng, khong nhung Razor Pages vao API.

**Hau qua:**
- API phu thuoc vao `Microsoft.AspNetCore.Mvc.RazorPages`, them ben rap tren (anti-forgery token, TempData, view engine, static files).
- Lam kho viec containerize sau nay (API-only container).
- Ba thuc the con lai (DichVu/Phong/DinhNghiaCa) khong co trang tuong ung → khong nhat quan.

**Yeu cau khac phuc:**
- Xoa thu muc `ClinicBooking.Api/Pages/` khoi project API.
- Go `AddRazorPages()` + `MapRazorPages()` khoi `Program.cs`.
- Neu muon co UI admin tam thoi de thu nghiem → tach thanh project rieng (vi du `ClinicBooking.AdminWeb`) hoac doi tich hop voi plan front-end chinh thuc.

---

### 1.2. Bat `Exception` chung chung + lo `ex.Message` ra UI

**File:** `ClinicBooking.Api/Pages/Module2/ChuyenKhoa.cshtml.cs`
**Dong:** 58-78 (`OnPostCreateAsync`), 89-109 (`OnPostUpdateAsync`), 112-125 (`OnPostDeleteAsync`)

```csharp
catch (Exception ex)
{
    ErrorMessage = ex.Message;
    ...
}
```

**Van de:**
- `CLAUDE.md` muc **17. Coding Rules → MUST NOT**: *"Ignore error handling"* → bat `Exception` chung va hien thi `ex.Message` truc tiep la hinh thuc xu ly loi hoi hot, **lo stack trace / thong diep noi bo** (vi du loi SQL, loi EF Core) ra nguoi dung.
- Du an da co `GlobalExceptionHandler` (API middleware) map san `ValidationException` → 400, `NotFoundException` → 404, `ConflictException` → 409. Razor Page bypass toan bo co che nay.
- Muc **18. Error Handling**: *"Global Exception Handler is REQUIRED"* + *"Validation messages... MUST be in Vietnamese"*. Hien tai `ex.Message` co the la tieng Anh hoac noi dung ky thuat khong phu hop user.

**Yeu cau khac phuc (neu van giu Razor Page):**
- Bat cu the tung loai exception (`ValidationException`, `NotFoundException`, `ConflictException`) va map sang thong bao tieng Viet than thien.
- Bat `Exception` cuoi cung chi de log, khong hien thi `ex.Message` — hien thi mot thong diep generic ("Co loi xay ra, vui long thu lai.").

---

### 1.3. Razor Page khong co `[Authorize]`

**File:** `ClinicBooking.Api/Pages/Module2/ChuyenKhoa.cshtml.cs` dong 13

```csharp
public class ChuyenKhoaModel : PageModel
```

**Van de:**
- `DanhMucController` cac endpoint CUD yeu cau `[Authorize(Roles = VaiTroConstants.Admin)]`.
- Razor Page goi thang MediatR, **bo qua hoan toan tang Authorization** cua API. Bat ky ai co the truy cap `/module2/chuyen-khoa` va tao/sua/xoa chuyen khoa khi chua dang nhap.
- `CLAUDE.md` muc **5. Authentication** va muc **17. MUST** (implicit qua "Skip layers") — layer bao mat phai xuyen suot.

**Yeu cau khac phuc:**
- Neu giu Razor Page: them `[Authorize(Roles = VaiTroConstants.Admin)]` o class level + test manual co dieu huong dang nhap.
- Tot hon: tuan theo muc 1.1, bo Razor Page.

---

## 2. Cac diem can can nhac

### 2.1. Test data hardcode trung voi `SeedData` toan cuc

**File minh da tam fix de unblock nhanh Module 1 (ban xem lai xem co on khong):**
- `ClinicBooking.Application.UnitTests/Features/DanhMuc/Commands/CapNhatPhong/CapNhatPhongHandlerTests.cs` — ban dau test dung `P301`, `P402` trung seed → 2 test fail sau khi merge. Minh da tam doi thanh `P-UT-301`, `P-UT-401`.
- `ClinicBooking.Application.UnitTests/Features/DanhMuc/Commands/TaoDinhNghiaCa/XoaDinhNghiaCaHandlerTests.cs` — test dung `SoDienThoai = "0900000000"` trung voi tai khoan admin seed. Minh da tam doi thanh `"0911222333"`.

**Van de goc:**
- Unit test dung `TestDbContextFactory` → `EnsureCreated()` → **tat ca `HasData` seed tu `SeedData.cs` deu duoc ap dung**. Test hardcode value trung voi seed se fail ngau nhien, hoac tuong tac ngam voi seed.
- `CLAUDE.md` muc **7. Testing → Unit Tests**: *"Mock dependencies"* — test nen kiem soat state, khong phu thuoc ngam vao seed.

**Goi y (khong bat buoc):**
- **Prefix `*-UT-*`** (vi du `MaPhong = "P-UT-xxx"`, `SoDienThoai = "091xxxxxxx"`) chi la de xuat tam cua minh de tranh trung seed. Ban co the dat quy uoc khac neu thay hop ly hon — mien la gia tri test khac voi seed la duoc.
- Huong lau dai: lam `SeedData` **opt-in** qua config flag hoac tao DbContext "empty" cho test (khong goi seed trong test). Viec nay thuoc scope refactor, co the de sau — khong can lam trong PR hien tai.

---

### 2.2. Input DTOs nhung trong PageModel — bo qua tang Contracts

**File:** `ClinicBooking.Api/Pages/Module2/ChuyenKhoa.cshtml.cs` dong 134-173

```csharp
public sealed class TaoChuyenKhoaInput { ... }
public sealed class CapNhatChuyenKhoaInput { ... }
```

**Van de:**
- Du an co chuan `Contracts/DanhMuc/` (vi du `TaoChuyenKhoaRequest.cs`). Nhung lop Input moi duplicate schema, khong tai su dung.
- `CLAUDE.md` muc **17. MUST**: *"Use DTOs"* — nhung DTO phai thuoc tang Contracts co to chuc, khong rai rac trong UI layer.

**Yeu cau khac phuc:** dung chung `TaoChuyenKhoaRequest` / `CapNhatChuyenKhoaRequest` neu van giu Razor Page, hoac bo Page (xem 1.1).

---

### 2.3. Dung DataAnnotations validate trong Input — trai voi chuan FluentValidation cua du an

**File:** `ClinicBooking.Api/Pages/Module2/ChuyenKhoa.cshtml.cs` dong 134-173

```csharp
[Required]
[MaxLength(450)]
public string TenChuyenKhoa { get; set; }
```

**Van de:**
- Toan du an dung **FluentValidation** (`TaoChuyenKhoaValidator`, `CapNhatChuyenKhoaValidator`, ...) qua `ValidationBehavior` MediatR pipeline.
- Input moi lai dung DataAnnotations + `ModelState.IsValid` → **hai bo validator song song**, thong diep khong nhat quan, de drift.

**Yeu cau khac phuc:** bo DataAnnotations, de ValidationBehavior cua MediatR pipeline tu chay (handler da goi `_mediator.Send`).

---

## 3. Diem tot (de giu va tiep tuc)

De can bang: Module 2 cung co rat nhieu phan tuan thu chuan. Giu duy tri:

- **Handlers** (`TaoDichVuHandler`, `XoaChuyenKhoaHandler`, ...) sach, dung `NotFoundException` / `ConflictException` chuan, khong cos magic string, async/await day du, `CancellationToken` xuyen suot.
- **Validators** FluentValidation dung chuan, thong diep tieng Viet **khong dau** (dung tinh than CLAUDE.md section 11).
- **`DanhMucController`** cuc ki sach: thin controller, `VaiTroConstants` moi noi, `[ProducesResponseType]` day du, `StatusCode(201)` cho create, route tieng Viet khong dau (`api/danh-muc/chuyen-khoa`).
- **Contracts/Mappings** tach rieng (`ChuyenKhoaMappings.cs` voi extension `TuDto()`) — dung chuan Clean Architecture.
- **Test coverage tot**: 93 test xanh sau khi sua seed collision, coverage day du happy path + edge case (trung ten, dang duoc su dung, not found, ...).
- **Soft delete / check ref-integrity truoc khi xoa** (`XoaChuyenKhoaHandler`, `XoaDinhNghiaCaHandler`) — dung nghiep vu.

---

## 4. Checklist ra quyet dinh

Truoc khi Module 2 mo rong scope hoac front-end bat dau:

- [ ] **1.1 Razor Pages**: chot huong xu ly. **Khuyen nghi**: xoa het, front-end di theo plan chung (`docs/huong-dan-phat-trien-ui.md`).
- [ ] **1.2 Generic `catch Exception`**: neu giu Page thi fix, khong giu thi xoa cung `.cshtml.cs`.
- [ ] **1.3 Auth tren Page**: tuong tu 1.2.
- [ ] **2.1 Quy uoc test data `*-UT-*`**: viet vao `docs/phan-chia-module-va-huong-dan-du-an.md` hoac tao `docs/huong-dan-viet-test.md` moi.
- [ ] **2.2, 2.3**: tu dong bien mat neu bo Page.
- [ ] Soft rule: truoc moi PR cross-module, nguoi code Module 2 chay `dotnet test` tren nhanh `develop` moi nhat de phat hien seed collision som.

---

## 5. Loi ket

Chat luong handler + controller ban viet **rat tot** — dung chuan Clean Architecture, CQRS, FluentValidation. Phan dong y kien trong tai lieu nay tap trung o **lop Razor Pages moi them vao project API** hon la o code nghiep vu. Neu thao lop Razor Pages ra (hoac tach sang project rieng), phan con lai cua Module 2 co the lam tham chieu tot cho cac module khac.

Naming tieng Viet khong dau ben vung, khong co business logic trong Controller, khong co magic role string — nhung diem nay deu giu nguyen.

Neu ban thay minh hieu sai cho nao hoac muon thao luan them, reply truc tiep cho minh. Cam on ban!
