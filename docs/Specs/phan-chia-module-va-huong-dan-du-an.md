# ClinicBooking — Phân chia module & Hướng dẫn dự án

> Tài liệu này mô tả cách chia dự án `DatLichPhongKham` (ClinicBooking API) thành 4 module độc lập
> cho 4 thành viên, kèm theo hướng dẫn cài đặt, build, migration, chạy và commit.
> Đọc kèm `CLAUDE.md` ở gốc repo (nguồn "luật chơi" chính thức).

---

## 1. Tổng quan dự án

- **Tên:** ClinicBooking (DatLichPhongKham)
- **Mục tiêu:** Web API đặt lịch khám cho phòng khám, quản lý ca làm bác sĩ, tiếp tân check-in,
  hồ sơ khám + toa thuốc, thông báo.
- **Nền tảng:** ASP.NET Core 8 Web API, EF Core 9 + SQL Server, JWT Auth, Clean Architecture.
- **Solution:** `DatLichPhongKham.slnx` gồm 4 project:

```text
ClinicBooking.Domain         # Entity + Enum thuần, không phụ thuộc
ClinicBooking.Application    # Use case (CQRS/MediatR), DTO, Validator, Abstractions
ClinicBooking.Infrastructure # EF Core, Security (JWT/BCrypt), Seeder
ClinicBooking.Api            # Controller, Middleware, Swagger, DI wiring
```

Luồng phụ thuộc: `Api → Application + Infrastructure → Application → Domain`.

- **Source of truth schema:** `database/clinic.dbml`
- **Ngôn ngữ đặt tên code:** Tiếng Việt không dấu (ví dụ `BenhNhan`, `taoLichHen`); ngoại lệ:
  framework/standard terms giữ nguyên tiếng Anh (Controller, Service, Token, Cache...).
- **Ngôn ngữ message trả về client:** Tiếng Việt có dấu.

---

## 2. Hiện trạng (tính đến 2026-04-16)

### Đã hoàn thành (foundation do cặp đôi User + Claude làm)

- Toàn bộ Entity + Enum trong `Domain` (22 entity, map đúng `clinic.dbml`).
- `AppDbContext` + Fluent API cho toàn bộ bảng + relationship.
- 3 migration: `InitialCreate`, `SeedDanhMuc`, `ThemRefreshToken`.
- Module **Auth** đầy đủ: `DangKy`, `DangNhap`, `LamMoiToken`, `DangXuat` (CQRS + Validator + Handler + Controller).
- JWT access token + refresh token (có xoay vòng, thu hồi, lưu DB).
- Password hashing BCrypt (workFactor 11).
- `IPasswordHasher`, `ITokenService`, `ICurrentUserService`, `IDateTimeProvider`.
- `GlobalExceptionHandler` + ProblemDetails (400/401/403/404/409/500).
- `ValidationBehavior` (MediatR pipeline gọi FluentValidation).
- `DatabaseSeeder` idempotent thay hash admin giả lập bằng BCrypt thật qua config.
- Swagger UI có nút Authorize JWT.

### Chưa làm (nội dung 4 module bên dưới)

- Đặt lịch hẹn, hủy/đổi lịch, lịch sử lịch hẹn, giữ chỗ, hàng chờ, check-in.
- Quản lý bác sĩ, ca làm việc, đơn nghỉ phép, danh mục (chuyên khoa/dịch vụ/phòng/định nghĩa ca).
- Quản lý bệnh nhân, hồ sơ khám, toa thuốc, danh mục thuốc.
- Thông báo (email + in-app), template thông báo, OTP, báo cáo admin.
- Background jobs (dọn `GiuCho` hết hạn, nhắc lịch hẹn, cleanup OTP).
- Docker/compose, CI GitHub Actions, bộ khung Integration Test.

---

## 3. Phân chia 4 module

> Nguyên tắc chia:
> - Mỗi module = một nhóm "Feature" trong `Application/Features/` + `Contracts/` + `Controllers/` tương ứng.
> - Tránh chồng lấn write trên cùng entity; nếu cần thì dùng **abstraction** (interface trong `Application/Abstractions`) để decouple.
> - Mỗi người sở hữu migration của entity thuộc module mình (xem §4 để tránh conflict).

### Module 1 — Đặt lịch hẹn & Hàng chờ (shared: **User + Claude**)

Đây là trái tim nghiệp vụ, phụ thuộc mạnh vào Auth + `CurrentUserService` → phù hợp cho cặp đã nắm rõ foundation.

**Phạm vi entity chính:** `LichHen`, `LichSuLichHen`, `GiuCho`, `HangCho`.

**Use case:**

| Actor | Feature | Command/Query |
|---|---|---|
| Bệnh nhân | Đặt lịch hẹn online | `Features/Appointments/Commands/TaoLichHen` |
| Bệnh nhân | Huỷ lịch hẹn | `Features/Appointments/Commands/HuyLichHen` (ghi `LichSuLichHen`, tăng `SoLanHuyMuon` nếu huỷ muộn) |
| Bệnh nhân | Đổi lịch hẹn | `Features/Appointments/Commands/DoiLichHen` |
| Bệnh nhân / Lễ tân | Xem chi tiết + lịch sử 1 lịch hẹn | `Features/Appointments/Queries/XemLichHen` |
| Bệnh nhân | Danh sách lịch hẹn của tôi | `Features/Appointments/Queries/DanhSachLichHenCuaToi` |
| Lễ tân / Admin | Danh sách lịch hẹn theo ngày/ca | `Features/Appointments/Queries/DanhSachLichHenTheoNgay` |
| Lễ tân | Giữ chỗ tạm (giữ 5–10 phút) | `Features/Reception/Commands/TaoGiuCho` |
| Lễ tân | Giải phóng giữ chỗ | `Features/Reception/Commands/GiaiPhongGiuCho` |
| Lễ tân | Check-in bệnh nhân → đẩy vào hàng chờ | `Features/Reception/Commands/CheckInLichHen` |
| Lễ tân / Bác sĩ | Gọi bệnh nhân tiếp theo | `Features/Reception/Commands/GoiBenhNhanKeTiep` |
| Bác sĩ | Đánh dấu hoàn thành lượt khám | `Features/Reception/Commands/HoanThanhLuotKham` |
| Lễ tân | Xem hàng chờ theo ca | `Features/Reception/Queries/XemHangChoTheoCa` |

**Background job sở hữu:** dọn `GiuCho` hết hạn + chuyển trạng thái `LichHen` sang `DaQuaHan`.

**Phụ thuộc:** `CaLamViec` (từ Module 2) → dùng interface đọc slot còn trống; kiểm tra vai trò qua `ICurrentUserService` + `[Authorize(Roles=...)]`.

**Authorization sample:**

```csharp
[Authorize(Roles = VaiTroConstants.BenhNhan)]
[HttpPost("dat-lich")]
public Task<ActionResult<LichHenResponse>> TaoLichHen(...) { ... }
```

---

### Module 2 — Bác sĩ, Lịch làm việc & Danh mục (Member B)

Khối "master data" + lịch bác sĩ, không có user-facing booking flow.

**Phạm vi entity chính:** `BacSi`, `CaLamViec`, `DinhNghiaCa`, `LichNoiTru`, `DonNghiPhep`, `ChuyenKhoa`, `DichVu`, `Phong`.

**Use case:**

| Actor | Feature |
|---|---|
| Admin | CRUD `ChuyenKhoa`, `DichVu`, `Phong`, `DinhNghiaCa` (`Features/DanhMuc/...`) |
| Admin | CRUD bác sĩ (liên kết với `TaiKhoan` vai trò `bac_si`) |
| Bác sĩ | Cập nhật hồ sơ cá nhân (`Features/Doctors/Commands/CapNhatHoSoBacSi`) |
| Bác sĩ | Xem ca làm của tôi (theo tuần/tháng) |
| Bác sĩ | Gửi yêu cầu tạo ca (`Features/Scheduling/Commands/YeuCauTaoCa`) |
| Admin | Tạo/duyệt ca làm việc (`Features/Scheduling/Commands/TaoCaLamViec`, `DuyetCaLamViec`) |
| Admin | Xoá/đóng ca (chỉ khi `SoSlotDaDat = 0`) |
| Lễ tân/Admin | Xem lịch nội trú theo phòng/ngày |
| Bác sĩ | Gửi đơn nghỉ phép (`Features/NghiPhep/Commands/TaoDonNghiPhep`) |
| Admin | Duyệt/từ chối đơn nghỉ phép (`Features/NghiPhep/Commands/DuyetDonNghiPhep`) |

**API mà các module khác sẽ cần (cần expose qua Application abstraction):**

```csharp
// ClinicBooking.Application/Abstractions/Scheduling/
public interface ICaLamViecQueryService
{
    Task<CaLamViecInfo?> LayCaLamViecAsync(int idCaLamViec, CancellationToken ct);
    Task<bool> ConSlotTrongAsync(int idCaLamViec, CancellationToken ct);
}
```

Module 1 dùng interface này để đặt lịch; implementation do Module 2 viết trong `Infrastructure`.

---

### Module 3 — Bệnh nhân, Hồ sơ khám & Kê đơn (Member C)

**Phạm vi entity chính:** `BenhNhan`, `HoSoKham`, `ToaThuoc`, `Thuoc`.

**Use case:**

| Actor | Feature |
|---|---|
| Bệnh nhân | Xem/cập nhật hồ sơ cá nhân (`Features/Patients/Commands/CapNhatHoSoBenhNhan`) |
| Lễ tân / Admin | Tìm kiếm + xem danh sách bệnh nhân |
| Lễ tân | Tạo hồ sơ bệnh nhân walk-in (không có tài khoản) |
| Bác sĩ | Tạo hồ sơ khám từ một `LichHen` đã check-in |
| Bác sĩ | Cập nhật hồ sơ khám (chẩn đoán, lời dặn) |
| Bác sĩ / Bệnh nhân | Xem lịch sử khám của bệnh nhân |
| Bác sĩ | Kê toa thuốc (1 `HoSoKham` n `ToaThuoc` → n dòng thuốc) |
| Bệnh nhân | Xem toa thuốc của tôi |
| Admin | CRUD danh mục `Thuoc` |

**Phụ thuộc:** `LichHen` (Module 1). Module 3 **chỉ đọc** `LichHen` (không sửa trạng thái),
dùng một `IAppDbContext.LichHen.Where(...)` hoặc interface do Module 1 expose.

---

### Module 4 — Thông báo, Admin & Hạ tầng vận hành (Member D)

**Phạm vi entity chính:** `ThongBao`, `MauThongBao`, `OtpLog` + cross-cutting.

**Use case:**

| Actor | Feature |
|---|---|
| Hệ thống | Gửi thông báo (email + in-app) qua `INotificationService` |
| Admin | CRUD `MauThongBao` |
| User | Xem danh sách thông báo của tôi, đánh dấu đã đọc |
| Hệ thống | Gửi OTP email (đổi mật khẩu / xác thực email) |
| User | `POST /api/auth/quen-mat-khau` → phát OTP |
| User | `POST /api/auth/dat-lai-mat-khau` (dùng OTP) |
| User | `POST /api/auth/doi-mat-khau` (đã đăng nhập) |
| Admin | Báo cáo: số lượt khám theo ngày/bác sĩ/chuyên khoa, tỷ lệ huỷ |
| Admin | Danh sách tài khoản, khoá/mở tài khoản |

**Abstraction phải định nghĩa SỚM (tuần đầu) để module khác fire event:**

```csharp
// ClinicBooking.Application/Abstractions/Notifications/INotificationService.cs
public interface INotificationService
{
    Task GuiAsync(
        int idTaiKhoanNhan,
        LoaiThongBao loai,
        object duLieu,       // payload để render template (ví dụ { TenBacSi, GioKham })
        LoaiThamChieu? loaiThamChieu = null,
        int? idThamChieu = null,
        CancellationToken ct = default);
}
```

**Hạ tầng vận hành:**

- `Dockerfile` cho `ClinicBooking.Api` + `docker-compose.yml` (API + `mcr.microsoft.com/mssql/server:2022-latest`).
- GitHub Actions pipeline: `restore → build → test → (tuỳ chọn) docker build`.
- Background worker host: chọn **Hangfire** (SQL Server storage — không cần Redis) hoặc `BackgroundService` thủ công.
  - Jobs: nhắc lịch hẹn trước 1 giờ, cleanup `OtpLog` > 24h, cleanup `GiuCho` quá hạn (phối hợp Module 1).
- Bộ khung **Integration Test** dùng `WebApplicationFactory` + SQL Server Testcontainers (hoặc `Microsoft.EntityFrameworkCore.Sqlite` cho CI nhanh).

---

## 4. Phụ thuộc giữa các module & thứ tự triển khai

```text
Module 4 (skeleton) ─ định nghĩa INotificationService, skeleton test, CI
        │
Module 2 ──► Module 1 ──► Module 3
(danh mục,           (lịch hẹn,         (hồ sơ khám
 ca làm)              hàng chờ)          dựa trên LichHen)
        │                │                   │
        └────────┬───────┴───────────────────┘
                 ▼
            Module 4 (notification handlers, reports)
```

**Lộ trình đề xuất (~6 tuần):**

| Tuần | Module 1 | Module 2 | Module 3 | Module 4 |
|---|---|---|---|---|
| 1 | Design DTO + validator `TaoLichHen` | CRUD `ChuyenKhoa`/`DichVu`/`Phong`/`DinhNghiaCa` | Hồ sơ bệnh nhân self-service | `INotificationService` skeleton + Docker skeleton + CI skeleton |
| 2 | Handler `TaoLichHen` + đồng bộ `SoSlotDaDat` | CRUD bác sĩ + `ICaLamViecQueryService` | Danh sách bệnh nhân cho lễ tân | In-app notification + `ThongBao` query |
| 3 | Huỷ/đổi lịch + `LichSuLichHen` | `CaLamViec` tạo/duyệt | `HoSoKham` create/update | Email adapter (MailKit hoặc SMTP builder) |
| 4 | `GiuCho` + `HangCho` + check-in | `DonNghiPhep` | `ToaThuoc` | OTP + đổi mật khẩu + quên mật khẩu |
| 5 | Gọi bệnh nhân + hoàn thành lượt khám | `LichNoiTru` | CRUD `Thuoc` | Báo cáo admin + Hangfire jobs |
| 6 | Hardening + integration tests | Hardening | Hardening | Docker compose production-ready, Hangfire dashboard |

**Quy tắc migration để không đá nhau:**

- Mỗi PR chỉ tạo migration cho entity thuộc **module của PR đó**.
- Trước khi `dotnet ef migrations add`, luôn `git pull origin develop` và apply migration mới nhất.
- Đặt tên migration theo prefix module: `Module2_CaLamViec_AddDuyetFields`, `Module1_LichHen_ThemCotMaLichHen`, v.v.
- **Không** sửa migration đã merge vào `develop`; tạo migration mới để bù.

---

## 5. Tech stack & NuGet packages

### Đã cài sẵn

| Project | Package | Phiên bản | Mục đích |
|---|---|---|---|
| Application | `MediatR` | 14.1.0 | CQRS + pipeline behavior |
| Application | `FluentValidation` + `.DependencyInjectionExtensions` | 12.1.1 | Validate command/query |
| Application | `AutoMapper` | 16.1.1 | Map entity ↔ DTO |
| Application | `Microsoft.EntityFrameworkCore` | 9.0.10 | `IQueryable` trong handler |
| Infrastructure | `Microsoft.EntityFrameworkCore.SqlServer` | 9.0.10 | Provider SQL Server |
| Infrastructure | `Microsoft.EntityFrameworkCore.Design` | 9.0.10 | Tool migration |
| Infrastructure | `BCrypt.Net-Next` | 4.0.3 | Hash password |
| Infrastructure | `System.IdentityModel.Tokens.Jwt` | 8.2.1 | Ký/xác thực JWT |
| Infrastructure | `Microsoft.Extensions.Options.ConfigurationExtensions` | 10.0.0 | IOptions binding |
| Api | `Microsoft.AspNetCore.Authentication.JwtBearer` | 8.0.26 | JWT middleware |
| Api | `Swashbuckle.AspNetCore` | 8.0.0 | Swagger UI |

### Sẽ cần thêm (mỗi module tự add khi cần)

| Module | Package đề xuất | Mục đích |
|---|---|---|
| M4 | `MailKit` | Gửi email SMTP |
| M4 | `Hangfire.AspNetCore` + `Hangfire.SqlServer` | Background job có dashboard |
| M4 (test) | `Microsoft.AspNetCore.Mvc.Testing` | `WebApplicationFactory` |
| M4 (test) | `xunit`, `FluentAssertions`, `NSubstitute` | Unit/integration test |
| M4 (test) | `Testcontainers.MsSql` | SQL Server trong integration test |
| Tuỳ chọn | `Serilog.AspNetCore` + `Serilog.Sinks.Console` | Logging structured |

**Lệnh add package chuẩn:**

```bash
dotnet add ClinicBooking.Infrastructure package MailKit --version 4.8.0
dotnet add ClinicBooking.Api package Hangfire.AspNetCore --version 1.8.17
```

> Luôn cố định version (`--version`) để tránh "works on my machine".

---

## 6. Shell commands quan trọng

Chạy **từ thư mục gốc repo** (`D:\Code\C#\DatLichPhongKham`).

### Build & chạy

```bash
# Restore + build toàn bộ solution
dotnet build DatLichPhongKham.slnx

# Chạy API (Swagger: https://localhost:<port>/swagger)
dotnet run --project ClinicBooking.Api

# Chạy API ở chế độ Development explicit
dotnet run --project ClinicBooking.Api --environment Development

# Watch mode (hot reload khi sửa code)
dotnet watch --project ClinicBooking.Api run
```

### EF Core Migrations

**Luôn** chỉ định `--project` (nơi chứa `AppDbContext`) và `--startup-project` (nơi chứa `Program.cs` + config).

```bash
# Cài global tool (1 lần / máy)
dotnet tool install --global dotnet-ef --version 9.0.10
# hoặc cập nhật
dotnet tool update --global dotnet-ef --version 9.0.10

# Liệt kê migration
dotnet ef migrations list \
  --project ClinicBooking.Infrastructure \
  --startup-project ClinicBooking.Api

# Thêm migration mới (đặt tên theo convention module)
dotnet ef migrations add Module2_CaLamViec_AddDuyetFields \
  --project ClinicBooking.Infrastructure \
  --startup-project ClinicBooking.Api \
  --output-dir Persistence/Migrations

# Apply migration vào DB
dotnet ef database update \
  --project ClinicBooking.Infrastructure \
  --startup-project ClinicBooking.Api

# Rollback 1 migration (về migration ngay trước)
dotnet ef database update <TenMigrationTruoc> \
  --project ClinicBooking.Infrastructure \
  --startup-project ClinicBooking.Api

# Xoá migration chưa apply (khi đang viết dở)
dotnet ef migrations remove \
  --project ClinicBooking.Infrastructure \
  --startup-project ClinicBooking.Api

# Drop database (CHỈ dùng khi môi trường local)
dotnet ef database drop --force \
  --project ClinicBooking.Infrastructure \
  --startup-project ClinicBooking.Api

# Sinh script SQL idempotent (để deploy production)
dotnet ef migrations script --idempotent \
  --project ClinicBooking.Infrastructure \
  --startup-project ClinicBooking.Api \
  --output ./database/migrations.sql
```

### Test

```bash
# Chạy toàn bộ test
dotnet test DatLichPhongKham.slnx

# Chạy 1 project test
dotnet test tests/ClinicBooking.Application.Tests/ClinicBooking.Application.Tests.csproj

# Filter theo class/method
dotnet test --filter "FullyQualifiedName~TaoLichHenHandlerTests"
```

### Docker (Module 4 sẽ setup)

```bash
# Build image API
docker build -t clinicbooking-api:dev -f ClinicBooking.Api/Dockerfile .

# Chạy full stack
docker compose up -d
docker compose logs -f api
docker compose down            # dừng
docker compose down -v         # dừng + xoá volume (wipe DB)
```

### User Secrets (dev, thay cho env var khi debug local)

```bash
cd ClinicBooking.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=.;Database=ClinicBooking;Trusted_Connection=True;TrustServerCertificate=True"
dotnet user-secrets set "Jwt:Key" "<chuoi-ngau-nhien-toi-thieu-32-ky-tu>"
dotnet user-secrets set "Jwt:Issuer" "ClinicBooking"
dotnet user-secrets set "Jwt:Audience" "ClinicBookingClients"
dotnet user-secrets set "Admin:MatKhauMacDinh" "Admin@123456"
dotnet user-secrets list
```

---

## 7. Cấu hình (appsettings)

### Cấu trúc bắt buộc

```jsonc
// appsettings.json (KHÔNG chứa secret, commit vào git)
{
  "ConnectionStrings": {
    "DefaultConnection": ""   // override bằng user-secrets / env var
  },
  "Jwt": {
    "Issuer": "ClinicBooking",
    "Audience": "ClinicBookingClients",
    "Key": "",                                    // BẮT BUỘC override
    "AccessTokenMinutes": 60,
    "RefreshTokenDays": 7
  },
  "Admin": {
    "MatKhauMacDinh": ""                          // chỉ set ở Development/User Secrets
  }
}
```

### Quy tắc

- `appsettings.json`: mặc định không nhạy cảm, commit vào git.
- `appsettings.Development.json`: override cho dev, chỉ chứa giá trị non-secret (ví dụ log level).
- **Secret** (connection string thật, `Jwt:Key`, `Admin:MatKhauMacDinh`):
  - Local: **User Secrets**.
  - CI/CD: **Environment variables** hoặc GitHub Secrets.
  - **KHÔNG** bao giờ commit vào git.
- Đọc config qua `IOptions<TSettings>` với `SectionName` hằng số:

```csharp
services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
```

---

## 8. Git workflow

### Branch

- `main`: production-ready, merge từ `develop` khi release.
- `develop`: default integration branch.
- `feature/<module>/<ten-ngan>`: nhánh tính năng, ví dụ `feature/module1/tao-lich-hen`.
- `fix/<mo-ta>`, `chore/<mo-ta>`: sửa lỗi / việc vặt.

### Commit message

Theo Conventional Commits, **chủ thể ngắn gọn tiếng Việt không dấu**:

```text
feat(appointments): them lenh TaoLichHen + validator
fix(auth): xu ly refresh token da thu hoi
chore(ci): them workflow dotnet test
docs(modules): cap nhat phan chia module
```

Prefix hay dùng: `feat | fix | chore | docs | refactor | test | perf | build | ci`.

### Quy trình PR

1. `git checkout develop && git pull`
2. `git checkout -b feature/moduleX/ten-feature`
3. Code → `dotnet build` → `dotnet test` → self-review.
4. Nếu có migration: verify `dotnet ef database update` ok trên DB local sạch.
5. `git add <file>` (tránh `git add -A` để không dính secrets).
6. `git commit -m "feat(...): ..."`.
7. `git push -u origin feature/moduleX/ten-feature`.
8. Mở PR → target `develop` → request review.
9. Review xanh + CI xanh → **Squash merge**.

---

## 9. Quy ước code (tóm tắt — chi tiết xem `CLAUDE.md`)

### Naming

| Loại | Ví dụ đúng | Ví dụ sai |
|---|---|---|
| Entity/Class domain | `BenhNhan`, `LichHen`, `HoSoKham` | `Patient`, `Appointment`, `BệnhNhân` |
| Command | `TaoLichHenCommand` | `CreateAppointmentCommand` |
| Handler | `TaoLichHenHandler` | `CreateAppointmentHandler` |
| DTO API (request) | `TaoLichHenRequest` | `CreateAppointmentDto` |
| DTO API (response) | `LichHenResponse` | `AppointmentResult` |
| Thuộc tính | `NgayKham`, `TrangThai` | `AppointmentDate`, `Status` |
| Biến cục bộ | `tenBenhNhan` | `patientName`, `tmp`, `bn` |

Ngoại lệ giữ tiếng Anh: `Controller`, `Service`, `Repository`, `Middleware`, `Token`, `Cache`,
và toàn bộ ký hiệu framework/thư viện.

### Message trả về client

Tiếng Việt có dấu: `"Lịch hẹn không tồn tại hoặc đã bị hủy."`.

### Layout feature (MUST follow)

```text
ClinicBooking.Application/
└── Features/
    └── Appointments/
        ├── Dtos/
        │   └── LichHenResponse.cs
        ├── Commands/
        │   └── TaoLichHen/
        │       ├── TaoLichHenCommand.cs
        │       ├── TaoLichHenValidator.cs
        │       └── TaoLichHenHandler.cs
        └── Queries/
            └── DanhSachLichHenCuaToi/
                ├── DanhSachLichHenCuaToiQuery.cs
                └── DanhSachLichHenCuaToiHandler.cs
```

Controller tương ứng: `ClinicBooking.Api/Controllers/AppointmentsController.cs` (thin, chỉ map
`Request → Command → _mediator.Send`).

### Quy tắc kỹ thuật

- **Async all the way** cho I/O (`await _db.SaveChangesAsync(ct)`).
- **Không** truy cập `DbContext` từ Controller.
- **Không** đặt business logic trong Controller hay Infrastructure.
- Dùng **constant / enum**, không dùng magic string.
- Dùng `IDateTimeProvider` thay vì `DateTime.UtcNow` trực tiếp (để test được).
- Trả lỗi bằng `throw new NotFoundException("...")`, `throw new ConflictException("...")`,
  `throw new ForbiddenException("...")`; `GlobalExceptionHandler` sẽ map sang ProblemDetails đúng status.
- Validator ném `FluentValidation.ValidationException` → tự động 400.
- Kiểm soát quyền bằng `[Authorize(Roles = VaiTroConstants.XXX)]`, không hardcode chuỗi `"admin"` rời rạc.

---

## 10. Authorization matrix (áp dụng chung)

| Endpoint | `admin` | `le_tan` | `bac_si` | `benh_nhan` | Anonymous |
|---|:-:|:-:|:-:|:-:|:-:|
| `POST /api/auth/dang-ky` | | | | | ✅ |
| `POST /api/auth/dang-nhap` | | | | | ✅ |
| `POST /api/auth/dang-xuat` | ✅ | ✅ | ✅ | ✅ | |
| `POST /api/appointments` (đặt lịch) | | | | ✅ | |
| `POST /api/appointments/{id}/huy` | ✅ | ✅ | | ✅ (của mình) | |
| `POST /api/reception/check-in` | | ✅ | | | |
| `POST /api/reception/goi-tiep` | | ✅ | ✅ | | |
| `POST /api/doctors` (tạo BS) | ✅ | | | | |
| `POST /api/scheduling/ca-lam-viec` | ✅ | | | | |
| `POST /api/scheduling/ca-lam-viec/yeu-cau` | | | ✅ | | |
| `POST /api/medical-records` | | | ✅ | | |
| `POST /api/prescriptions` | | | ✅ | | |
| `GET /api/admin/reports/*` | ✅ | | | | |
| `GET /api/notifications` | ✅ | ✅ | ✅ | ✅ | |

> Mỗi module nên kẹp file `Authorization.md` trong `Features/<Module>/` nếu ma trận quyền phức tạp hơn bảng trên.

---

## 11. Definition of Done cho mỗi feature

Trước khi mở PR, mỗi feature phải:

- [ ] Có `Command`/`Query`, `Handler`, `Validator` (nếu command có input).
- [ ] Có DTO Request/Response riêng (`Contracts/` cho request, `Features/.../Dtos/` cho response).
- [ ] Endpoint Controller có `[Authorize(Roles = ...)]` + `[ProducesResponseType]` cho status code chính.
- [ ] Không có magic string; dùng `VaiTroConstants`, Enum hoặc hằng số.
- [ ] Lỗi business ném qua exception chuẩn (`NotFoundException`, `ConflictException`, `ForbiddenException`).
- [ ] Migration (nếu có) chạy sạch trên DB trống: `dotnet ef database drop -f && dotnet ef database update`.
- [ ] Unit test cho handler (happy path + ít nhất 1 nhánh lỗi).
- [ ] `dotnet build` không warning mới, `dotnet test` xanh.
- [ ] Smoke test qua Swagger: đăng nhập đúng role → gọi endpoint → trả đúng status.
- [ ] Commit message theo Conventional Commits.

---

## 12. Điểm liên lạc & trách nhiệm

| Module | Chủ sở hữu | Liên hệ chính khi cần merge dependency |
|---|---|---|
| 1 — Lịch hẹn & Hàng chờ | User + Claude | cần `ICaLamViecQueryService` từ M2; cần `INotificationService` từ M4 |
| 2 — Bác sĩ, Ca, Danh mục | Member B | cung cấp abstraction cho M1, M3 |
| 3 — Bệnh nhân, Hồ sơ, Toa thuốc | Member C | đọc `LichHen` của M1 qua `IAppDbContext` |
| 4 — Thông báo, Admin, DevOps | Member D | cung cấp `INotificationService`, `IEmailSender`, Hangfire, CI |

**Quy tắc vàng khi chỉnh code ngoài module của mình:**

- Chỉ đụng **abstraction/interface** trong `Application/Abstractions/`, **không** sửa entity/DbContext/Migration của module khác.
- Nếu bắt buộc phải đổi chữ ký interface → mở PR nhỏ riêng `refactor(api): ...`, review chéo trước.

---

## 13. Tham khảo nhanh

- `CLAUDE.md` — luật code chính thức.
- `database/clinic.dbml` — schema source of truth.
- `docs/code-review-graph.md` — checklist review.
- Swagger (sau khi `dotnet run`): `https://localhost:<port>/swagger`.

> Khi có xung đột giữa tài liệu này và `CLAUDE.md`, **`CLAUDE.md` thắng**.
