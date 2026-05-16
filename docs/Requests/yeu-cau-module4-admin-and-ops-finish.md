# Yêu cầu Module 4 — Admin pages + ops infra (Dashboard/ThongKe/ThongBao + Docker/CI)

**Ngày lập:** 2026-05-16
**Người gửi:** Module 1 owner (Potatomine725)
**Người nhận:** Owner Module 4 (Thông báo / Admin / Hạ tầng)
**Nguồn:** `docs/Plans/ke-hoach-hoan-thien-he-thong-va-thoat-dev.md` §3.3
**Trạng thái Module 4:** `EmailService`, `OtpService`, `NotificationService` đã impl thật + registered. Stub files (`*Stub.cs`) còn trong repo nhưng không register. Admin pages còn 4 trang stub.

> **Lưu ý:** File này tổng hợp các nhiệm vụ M4 còn lại, **không bao gồm** `Admin/Accounts` (đã bàn giao tại `docs/Requests/yeu-cau-module4-admin-accounts.md`). Ưu tiên làm Accounts xong trước, sau đó tới các mục dưới.

---

## 1. Mục tiêu

| # | Mục tiêu | Ưu tiên |
|---|---|---|
| 1 | Seed `MauThongBao` mặc định + sửa `NotificationService` hardcode `IdMau` | 🔴 Cao |
| 2 | Wire `Admin/ThongBao` — broadcast notification + CRUD MauThongBao | 🔴 Cao |
| 3 | Wire `Admin/Dashboard` — query thống kê thật | 🟠 Trung |
| 4 | Wire `Admin/ThongKe` — báo cáo chi tiết theo loại | 🟠 Trung |
| 5 | Dockerize (Dockerfile + docker-compose) | 🟠 Trung |
| 6 | GitHub Actions CI (restore/build/test) | 🟡 Thấp |
| 7 | Migration `IHostedService` → Hangfire | 🟡 Thấp |
| 8 | Auth: quên mật khẩu + đổi mật khẩu | 🟡 Thấp |
| 9 | Dọn 3 file `*Stub.cs` trong Infrastructure | 🟢 Cleanup |

---

## 2. Việc cần làm

### 2.1. (🔴 Cao) Seed `MauThongBao` + fix `NotificationService` hardcode `IdMau`

**Vấn đề:** `Infrastructure/Services/Notifications/NotificationService.cs` đang hardcode `IdMau = 1..6` trong 6 method (`GuiThongBaoTaoLichHen`, `XacNhanLichHen`, `HuyLichHen`, `DoiLichHen`, `CheckIn`, `GoiBenhNhan`). Nếu DB không có 6 record `MauThongBao` với ID khớp → FK fail runtime.

#### Giải pháp

**Bước 1:** Thêm enum `LoaiMauThongBao` vào `Domain/Enums/`:

```csharp
public enum LoaiMauThongBao
{
    TaoLichHen = 1,
    XacNhanLichHen = 2,
    HuyLichHen = 3,
    DoiLichHen = 4,
    CheckIn = 5,
    GoiBenhNhan = 6
}
```

**Bước 2:** Thêm field `Loai` (enum, unique) vào entity `MauThongBao` (migration mới).

**Bước 3:** Seed 6 record default trong `DatabaseSeeder.cs`:

```csharp
private async Task SeedMauThongBaoAsync(CancellationToken ct)
{
    var defaults = new[]
    {
        new MauThongBao { Loai = LoaiMauThongBao.TaoLichHen, TieuDe = "Xac nhan lich hen kham", NoiDung = "..." },
        // ... 5 records còn lại
    };
    foreach (var mau in defaults)
        if (!await _db.MauThongBao.AnyAsync(m => m.Loai == mau.Loai, ct))
            _db.MauThongBao.Add(mau);
    await _db.SaveChangesAsync(ct);
}
```

**Bước 4:** Sửa `NotificationService` — tra cứu ID theo enum:

```csharp
var mau = await _db.MauThongBao.FirstAsync(m => m.Loai == LoaiMauThongBao.TaoLichHen, ct);
var thongBao = new ThongBao { IdMau = mau.IdMau, ... };
```

Cache vào `IMemoryCache` (đã có) với key `mau-thong-bao:{loai}` để tránh query lặp.

### 2.2. (🔴 Cao) `Admin/ThongBao` — broadcast + CRUD MauThongBao

**File:** `ClinicBooking.Web/Pages/Admin/ThongBao.cshtml(.cs)` (hiện 14 dòng stub).

#### Backend mới

`Features/ThongBao/Commands/GuiThongBaoBroadcast/`:

```csharp
public sealed record GuiThongBaoBroadcastCommand(
    VaiTro? VaiTro,           // null = tất cả; nếu set thì gửi cho role đó
    string TieuDe,
    string NoiDung,
    KenhGui KenhGui) : IRequest<int>;  // return số TK đã nhận
```

Handler:
- Query `TaiKhoan` theo `VaiTro` (active).
- Bulk insert `ThongBao` (chunk 500 record/batch).
- Nếu `KenhGui = Email`: enqueue gửi email background (đừng block — fire-and-forget qua `Task.Run` hoặc Hangfire nếu §2.7 xong).

`Features/MauThongBao/Commands/` — `Tao/CapNhat/Xoa MauThongBaoCommand`.
`Features/MauThongBao/Queries/DanhSachMauThongBao/`.

#### UI

- Tab 1: Broadcast — form chọn `VaiTro` dropdown (All/BacSi/BenhNhan/LeTan) + tiêu đề + nội dung (textarea) + nút gửi. Hiển thị `TempData["SuccessMessage"]` với số TK đã nhận.
- Tab 2: Quản lý mẫu — table CRUD `MauThongBao` (Loai readonly cho 6 mẫu hệ thống, chỉ sửa TieuDe + NoiDung).

### 2.3. (🟠 Trung) `Admin/Dashboard` — query thống kê thật

**File:** `ClinicBooking.Web/Pages/Admin/Dashboard.cshtml(.cs)` (hiện có class `MockDashboardStatistics`).

#### Backend mới

`Features/Admin/Queries/ThongKeTongHopAdmin/`:

```csharp
public sealed record ThongKeTongHopAdminQuery() : IRequest<ThongKeTongHopAdminResponse>;

public sealed record ThongKeTongHopAdminResponse(
    int LichHomNay,
    int LichTuanNay,
    int SoBacSiHoatDong,
    int SoBenhNhanMoiTuan,
    int SoCaChoDuyet,
    int SoToaThuocChoCap,
    decimal DoanhThuTuanNay,    // tạm 0 (chờ M3 hoá đơn)
    decimal DoanhThuThangNay);  // tạm 0
```

Handler: 6 query song song qua `Task.WhenAll` trên `IDbContextFactory<AppDbContext>` (mỗi task 1 context tránh concurrent issue).

#### UI

Bỏ class `MockDashboardStatistics`, replace bằng `@Model.ThongKe.XXX` trong 6-8 KPI card. Thêm chú thích "Doanh thu - chua co module hoa don" cho 2 field tạm.

### 2.4. (🟠 Trung) `Admin/ThongKe` — báo cáo chi tiết

**File:** `ClinicBooking.Web/Pages/Admin/ThongKe.cshtml(.cs)` (hiện 14 dòng stub).

#### Backend mới

`Features/Admin/Queries/BaoCaoThongKe/`:

```csharp
public sealed record BaoCaoThongKeQuery(
    DateOnly TuNgay,
    DateOnly DenNgay,
    LoaiBaoCao Loai) : IRequest<IReadOnlyList<RowBaoCaoDto>>;

public enum LoaiBaoCao { LichHen, BacSi, ChuyenKhoa, DichVu }

public sealed record RowBaoCaoDto(
    string Nhom,         // "BS Nguyen Van A" / "Khoa Noi" / "2026-05-16"
    int TongSo,
    int HoanThanh,
    int Huy,
    int KhongDen);
```

#### UI

- Filter: 2 input date + dropdown `LoaiBaoCao` → form GET.
- Bảng kết quả + Chart.js bar chart.
- Nút "Xuất CSV" (optional, gen từ rows server-side).

### 2.5. (🟠 Trung) Dockerize

**Files mới:**

- `Dockerfile` (multi-stage build cho cả Api + Web, hoặc 2 file riêng).
- `docker-compose.yml`:

```yaml
services:
  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: "${SA_PASSWORD:?required}"
      ACCEPT_EULA: "Y"
    ports: ["1433:1433"]
    volumes: [mssql_data:/var/opt/mssql]

  api:
    build:
      context: .
      dockerfile: ClinicBooking.Api/Dockerfile
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ConnectionStrings__DefaultConnection: "Server=db;Database=ClinicBooking;User=sa;Password=${SA_PASSWORD};TrustServerCertificate=True"
      Jwt__Key: "${JWT_KEY:?required}"
      Email__Username: "${SMTP_USERNAME}"
      Email__Password: "${SMTP_PASSWORD}"
      Email__FromAddress: "${SMTP_FROM}"
      Admin__MatKhauMacDinh: "${ADMIN_PASSWORD:?required}"
    ports: ["5000:8080"]
    depends_on: [db]

  web:
    build:
      context: .
      dockerfile: ClinicBooking.Web/Dockerfile
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ConnectionStrings__DefaultConnection: "Server=db;Database=ClinicBooking;User=sa;Password=${SA_PASSWORD};TrustServerCertificate=True"
      Jwt__Key: "${JWT_KEY}"
      Email__Username: "${SMTP_USERNAME}"
      Email__Password: "${SMTP_PASSWORD}"
      Admin__MatKhauMacDinh: "${ADMIN_PASSWORD}"
    ports: ["5001:8080"]
    depends_on: [db, api]

  mailhog:
    image: mailhog/mailhog:latest
    ports: ["1025:1025", "8025:8025"]

volumes:
  mssql_data:
```

- `.env.example` — list các biến cần set (không commit `.env` thật).
- Update `docs/SETUP-GUIDE.md` với hướng dẫn `docker-compose up`.

### 2.6. (🟡 Thấp) GitHub Actions CI

**File mới:** `.github/workflows/build-test.yml`

```yaml
name: build-test
on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main, develop]
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      - run: dotnet restore DatLichPhongKham.slnx
      - run: dotnet build --no-restore --configuration Release
      - run: dotnet test --no-build --configuration Release --verbosity normal
```

(Optional) Thêm job `docker-build` push image lên GHCR khi merge `main`.

### 2.7. (🟡 Thấp) Hangfire migration

Thay 2 `IHostedService` (`QuetGiuChoHetHanJob`, `ChuyenLichHenDaQuaHanJob`) bằng Hangfire recurring jobs:

```csharp
// Trong DependencyInjection.cs (Infrastructure)
services.AddHangfire(c => c.UseSqlServerStorage(connectionString));
services.AddHangfireServer();

// Trong Program.cs (Api/Web) sau app.Run() prep:
RecurringJob.AddOrUpdate<QuetGiuChoHetHanJob>("quet-giu-cho", j => j.ChayAsync(default), "*/1 * * * *");
RecurringJob.AddOrUpdate<ChuyenLichHenDaQuaHanJob>("chuyen-qua-han", j => j.ChayAsync(default), "*/30 * * * *");
RecurringJob.AddOrUpdate<ReconSlotJob>("recon-slot", j => j.ChayAsync(default), "*/15 * * * *");
RecurringJob.AddOrUpdate<CleanupOtpLogJob>("cleanup-otp", j => j.ChayAsync(default), "0 3 * * *");
RecurringJob.AddOrUpdate<NhacLichTruoc1HJob>("nhac-lich", j => j.ChayAsync(default), "*/15 * * * *");
```

Xoá 2 `AddHostedService` cũ trong `DependencyInjection.cs:73-74`.

Dashboard Hangfire: mount tại `/hangfire`, authorize `admin` only.

### 2.8. (🟡 Thấp) Auth — quên mật khẩu

`Features/Auth/Commands/`:
- `GuiOtpQuenMatKhau` (input: email/SĐT → tra cứu TK → gửi OTP với `MucDichOtp.DatLaiMatKhau` mới)
- `DatLaiMatKhauBangOtp` (input: SĐT + OTP + mật khẩu mới)
- `DoiMatKhauCuaToi` (input: mật khẩu cũ + mới — yêu cầu đăng nhập)

Thêm enum `MucDichOtp.DatLaiMatKhau = 4`.

UI Auth: `QuenMatKhau.cshtml` (2 bước giống `LienKetHoSo`).

### 2.9. (🟢 Cleanup) Dọn file stub

Sau khi confirm không test nào import:

```powershell
git rm ClinicBooking.Infrastructure/Services/Notifications/EmailSenderStub.cs
git rm ClinicBooking.Infrastructure/Services/Notifications/NotificationServiceStub.cs
git rm ClinicBooking.Infrastructure/Services/Security/OtpServiceStub.cs
```

Verify: `dotnet build` + `dotnet test` xanh.

---

## 3. Workflow ship

### 3.1. Branch

Tách thành nhiều branch nhỏ (dễ review):

1. `feature/module4/admin-accounts` — đã có (theo `yeu-cau-module4-admin-accounts.md`)
2. `feature/module4/mau-thong-bao-seed` — §2.1
3. `feature/module4/admin-thong-bao-broadcast` — §2.2
4. `feature/module4/admin-dashboard-thongke` — §2.3 + §2.4
5. `feature/module4/dockerize-and-ci` — §2.5 + §2.6
6. `feature/module4/hangfire-migration` — §2.7
7. `feature/module4/auth-quen-mat-khau` — §2.8
8. `chore/module4/remove-stubs` — §2.9

### 3.2. Trước khi push mỗi branch

```powershell
dotnet build DatLichPhongKham.slnx
dotnet test
```

### 3.3. PR

- **Base:** `develop`
- **Reviewer:** Module 1 owner
- **Tag:** `module4`

---

## 4. Ràng buộc

- Không đổi signature `INotificationService`, `IOtpService`, `IEmailService`.
- `NotificationService` không throw — chỉ log (semantic fire-and-forget).
- Khi seed `MauThongBao`, chạy được trên DB cũ (idempotent, check exists trước insert).
- `appsettings.Production.json` đã có (Module 1 tạo) — không sửa, chỉ extend qua env var.
- KHÔNG commit `.env` thật, chỉ `.env.example`.

---

## 5. Liên hệ

Nhắn Module 1 owner (Potatomine725). Module 1 review PR trong 2 ngày làm việc.
