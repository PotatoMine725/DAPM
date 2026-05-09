# Yêu cầu Module 2 / 3 / 4 — Hoàn thiện hệ thống ClinicBooking

> Ngày lập: 2026-05-05  
> **Cập nhật: 2026-05-09 — Thêm deadline, yêu cầu BE+FE, và yêu cầu mới từ feature Walk-in**  
> Tác giả: Module 1 (User + Claude)  
> Mục đích: Liệt kê rõ từng việc mỗi module còn nợ, interface đã định nghĩa sẵn, và điểm phối hợp cụ thể giữa các module

---

## ⏰ DEADLINE CHUNG: **2026-05-16** (1 tuần kể từ hôm nay)

Mỗi module phải hoàn thành **cả backend lẫn frontend (Web UI)** cho các use case lõi của mình trước ngày này. Xem chi tiết từng module bên dưới.

Tiêu chí "hoàn thành":
- Handler/Command/Query tương ứng đã implement (không phải stub)
- Razor Page hoặc API endpoint đã nối data thật (không phải hardcode/mock)
- Build không lỗi, không break test hiện có
- Đã tạo PR vào `develop` và tag Module 1 để review DI changes

---

## 🆕 Yêu cầu khẩn từ Module 1 — Feature Walk-in (cần trước 2026-05-12)

Module 1 đang phát triển tính năng **Đặt lịch hộ khách vãng lai tại quầy lễ tân** (xem `docs/Plans/ke-hoach-dat-lich-khach-vang-lai-le-tan.md`). Feature này cần một artifact từ Module 2 chưa tồn tại:

### Cần từ Module 2 — `DanhSachDichVuQuery`

**Vị trí:** `ClinicBooking.Application/Features/DichVu/Queries/DanhSachDichVu/`

**Yêu cầu tối thiểu (Module 1 cần để load dropdown dịch vụ trong modal đặt lịch vãng lai):**

```csharp
// Query
public record DanhSachDichVuQuery() : IRequest<List<DichVuResponse>>;

// DTO — chỉ cần 3 trường này
public record DichVuResponse(int IdDichVu, string TenDichVu, int IdChuyenKhoa);

// Handler — query bảng dich_vu, filter hien_thi = true, sort TenDichVu
```

**Nếu Module 2 chưa làm kịp trước 2026-05-12:** Module 1 sẽ tự tạo query tạm trong folder `Features/DichVu/` để unblock. Module 2 sau đó merge vào, sửa/mở rộng theo nhu cầu của mình — **không xóa, không đổi tên DTO đã có**.

---

## Bối cảnh chung

Module 1 (Đặt lịch hẹn & Hàng chờ) đã hoàn thành toàn bộ backend logic và Web UI cho 4 flow chính: đặt / hủy / đổi lịch / hàng chờ. Trong quá trình triển khai, Module 1 đã ship **các stub tạm thời** cho các interface thuộc phạm vi module khác để hệ thống compile và chạy được. Các module tương ứng cần thay thế stub này bằng implementation thật.

**Nguyên tắc thay thế stub:**
- Không đổi tên interface hay signature method (Module 1 đã dùng)
- Chỉ thay DI registration trong `DependencyInjection.cs` — từ `*Stub` sang class thật
- Không break 252 tests hiện tại (241 unit + 11 integration)

---

## Module 2 — Bác sĩ, Lịch làm việc & Danh mục

**Owner:** Member B  
**Branch hiện tại:** `feature/module2-doctors-scheduling` (đã merge vào develop một phần)

### 2.1 Contract Module 1 đang dùng — CẦN GIỮ NGUYÊN

Module 1 dùng `ICaLamViecQueryService` để đặt/hủy/đổi lịch. Interface này Module 1 đã định nghĩa sẵn tại:

```
ClinicBooking.Application/Abstractions/Scheduling/ICaLamViecQueryService.cs
```

Module 2 hiện đã có `CaLamViecQueryService` thật trong Infrastructure (Module 1 đã ship để flow chạy được). Tuy nhiên Module 2 cần **verify và sở hữu** implementation này:

```csharp
public interface ICaLamViecQueryService
{
    Task<ThongTinCaLamViecDto?> LayThongTinCaAsync(int idCaLamViec, CancellationToken ct);
    Task<KetQuaKiemTraSlotDto> KiemTraSlotTrongAsync(int idCaLamViec, CancellationToken ct);
    Task<bool> LaCaDuocDuyetAsync(int idCaLamViec, CancellationToken ct);

    // QUAN TRỌNG: atomic UPDATE — không Read-Modify-Write từ client
    // delta > 0 khi đặt lịch, delta < 0 khi hủy
    // Trả về null nếu vi phạm constraint [0, SoSlotToiDa]
    Task<int?> IncrementSoSlotDaDatAsync(int idCaLamViec, int delta, CancellationToken ct);

    // Dùng bởi background job reconciliation (Wave 4)
    Task<int> ChayReconSlotAsync(CancellationToken ct);
}
```

**Điểm phối hợp quan trọng:** `IncrementSoSlotDaDatAsync` phải dùng atomic SQL UPDATE, không phải Read-Modify-Write, để tránh race condition khi nhiều bệnh nhân đặt cùng ca.

### 2.2 Việc Module 2 cần làm — deadline 2026-05-16

> **Yêu cầu:** Mỗi mục phải có cả backend (Handler/Query) lẫn frontend (Razor Page/Admin UI) hoàn chỉnh.

#### A. Verify và nhận quyền sở hữu CaLamViecQueryService

- [ ] Review `ClinicBooking.Infrastructure/Services/Scheduling/CaLamViecQueryService.cs` — đây là implementation Module 1 đã viết tạm
- [ ] Nếu cần refactor, làm trên branch Module 2 và PR vào develop
- [ ] Đảm bảo `ChayReconSlotAsync` implement đúng (hiện chưa đầy đủ)

#### B. `DanhSachDichVuQuery` — **Khẩn, cần trước 2026-05-12**

- [ ] Tạo `Features/DichVu/Queries/DanhSachDichVu/` với DTO `DichVuResponse(IdDichVu, TenDichVu, IdChuyenKhoa)` (xem yêu cầu chi tiết ở phần "Yêu cầu khẩn" bên trên)

#### C. CRUD CaLamViec — BE + Admin UI

- [ ] **BE:** `POST /api/lich-lam-viec` — admin/bác sĩ tạo ca (`TaoCaLamViec`)
- [ ] **BE:** `PUT /api/lich-lam-viec/{id}/duyet` — admin duyệt ca (`DuyetCaLamViec`)
- [ ] **BE:** `DELETE /api/lich-lam-viec/{id}` — xóa ca (chỉ khi `SoSlotDaDat = 0`)
- [ ] **BE:** `GET /api/lich-lam-viec` — danh sách ca (lọc theo ngày, bác sĩ, chuyên khoa)
- [ ] **FE (Admin):** Trang `/Admin/QuanLyCaLamViec` — hiển thị lịch theo tuần, tạo/duyệt/hủy ca trực tiếp

> Các CaLamViec ID 3001–3003 đã được seed sẵn bởi Module 1 (dùng cho test/dev). Module 2 KHÔNG xóa các record này.

#### D. CRUD Danh mục — BE + Admin UI

- [ ] **BE + FE (Admin):** `ChuyenKhoa` — trang `/Admin/QuanLyChuyenKhoa`
- [ ] **BE + FE (Admin):** `DichVu` — trang `/Admin/QuanLyDichVu`
- [ ] **BE + FE (Admin):** `Phong` — trang `/Admin/QuanLyPhong`
- [ ] **BE + FE (Admin):** `DinhNghiaCa` — trang `/Admin/QuanLyDinhNghiaCa`

#### E. Quản lý Bác sĩ — BE + FE

- [ ] **BE + FE (Admin):** CRUD bác sĩ — trang `/Admin/QuanLyBacSi` (liên kết `TaiKhoan` vai trò `bac_si`)
- [ ] **BE + FE (BacSi portal):** Bác sĩ xem ca làm của mình — trang `/BacSi/LichLamViec` (theo tuần/tháng)
- [ ] **BE:** Bác sĩ yêu cầu tạo ca (`YeuCauTaoCa`)

#### F. Nghỉ phép — BE + FE

- [ ] **BE + FE (BacSi):** `TaoDonNghiPhep` — bác sĩ gửi đơn từ portal
- [ ] **BE + FE (Admin):** `DuyetDonNghiPhep` — admin duyệt/từ chối tại `/Admin/QuanLyNghiPhep`

### 2.3 Ràng buộc không được vi phạm

- `CaLamViec.SoSlotDaDat` chỉ được cập nhật qua `IncrementSoSlotDaDatAsync` — KHÔNG write trực tiếp từ handler Module 2
- Ca `DaDuyet` mà `SoSlotDaDat > 0` thì không được xóa/đóng trực tiếp — cần flow hủy lịch trước

---

## Module 3 — Bệnh nhân, Hồ sơ khám & Kê đơn

**Owner:** Member C  
**Branch hiện tại:** `feature/module3` (đã merge vào develop)

### 3.1 Điểm phối hợp Module 1 đang chờ

#### Cross-module write: `BenhNhan.SoLanHuyMuon`

Trong `HuyLichHenHandler.cs` (Module 1) có đoạn:

```csharp
// Cross-module write: BenhNhan.SoLanHuyMuon thuoc Module 3.
// Coordination point — notify Module 3 owner.
var benhNhan = await _db.BenhNhan
    .FirstOrDefaultAsync(x => x.IdBenhNhan == lichHen.IdBenhNhan, cancellationToken);
if (benhNhan is not null)
    benhNhan.SoLanHuyMuon += 1;
```

Module 3 cần **verify logic này còn đúng** với nghiệp vụ của mình (ngưỡng cấm đặt lịch, reset định kỳ, v.v.) và báo lại nếu cần điều chỉnh trước khi merge main.

### 3.2 Việc Module 3 cần làm — deadline 2026-05-16

> **Yêu cầu:** Mỗi mục phải có cả backend (Handler/Query) lẫn frontend (Razor Page) hoàn chỉnh.

#### A. Hồ sơ khám (`HoSoKham`) — BE + BacSi UI

- [ ] **BE + FE (BacSi):** `TaoHoSoKham` từ lịch hẹn đã check-in
  - Constraint: chỉ tạo được khi `LichHen.TrangThai = DangKham`
  - FE: Form tạo hồ sơ tích hợp trên trang `/BacSi/QuanLyKham` (đã có skeleton)
- [ ] **BE + FE (BacSi):** `CapNhatHoSoKham` — bác sĩ cập nhật chẩn đoán, kết quả
- [ ] **BE + FE (BenhNhan):** Bệnh nhân xem lịch sử khám — trang `/BenhNhan/LichSuKham`

#### B. Kê đơn thuốc (`ToaThuoc`) — BE + BacSi UI

- [ ] **BE + FE (BacSi):** `KeToa` — form kê đơn ngay trong hồ sơ khám (đã có skeleton tại `/BacSi/QuanLyKham`)
- [ ] **BE + FE (BenhNhan):** Bệnh nhân xem đơn thuốc của mình — trang `/BenhNhan/ToaThuoc`

#### C. Danh mục Thuốc — BE + Admin UI

- [ ] **BE + FE (Admin):** CRUD Thuốc — trang `/Admin/QuanLyThuoc`

#### D. Web UI đã có sẵn (cần nối backend)

- [ ] Trang `/BenhNhan/ThongBao` đã có Razor page — nối data thật từ bảng `ThongBao` (phối hợp với Module 4)

### 3.3 Ràng buộc

- Module 3 **chỉ đọc** `LichHen` — không update `TrangThai` của lịch hẹn (đó là Module 1)
- `HoSoKham` phải trỏ về `IdLichHen` hợp lệ có `TrangThai = DangKham` hoặc `HoanThanh`
- `BenhNhan.SoLanHuyMuon` hiện đang được Module 1 increment — Module 3 cần confirm ngưỡng nghiệp vụ và báo nếu cần thêm validation

---

## Module 4 — Thông báo, Admin & Hạ tầng vận hành

**Owner:** Member D  
**Branch hiện tại:** `feature/module4-week1-email-integration` (một phần)

### 4.1 Contract Module 1 đang chờ — KHẨN

#### `INotificationService` — stub cần thay bằng implementation thật

Interface đã định nghĩa sẵn tại `ClinicBooking.Application/Abstractions/Notifications/INotificationService.cs`.  
Hiện tại `NotificationServiceStub` chỉ log ra console, KHÔNG ghi vào bảng `ThongBao`.

**Module 4 cần implement `NotificationService`:**

```csharp
// Gợi ý implementation (đặt tại Infrastructure/Services/Notifications/)
public class NotificationService : INotificationService
{
    // Inject: IAppDbContext, IEmailService (hoặc ISmtpClient), IDateTimeProvider

    public async Task GuiThongBaoTaoLichHenAsync(int idLichHen, CancellationToken ct)
    {
        // 1. Query LichHen + BenhNhan + CaLamViec để lấy thông tin
        // 2. Ghi record vào bảng ThongBao (in-app)
        // 3. Gửi email qua IEmailService (async, không block)
    }

    // Tương tự cho 5 method còn lại...
}
```

**Sau khi xong, đổi DI registration tại `DependencyInjection.cs`:**
```csharp
// Xóa dòng này:
services.AddScoped<INotificationService, NotificationServiceStub>();

// Thay bằng:
services.AddScoped<INotificationService, NotificationService>();
```

**6 events Module 1 đã fire (cần handle):**

| Method | Trigger | Payload cần query |
|---|---|---|
| `GuiThongBaoTaoLichHenAsync(idLichHen)` | Sau khi đặt lịch thành công | LichHen → BenhNhan.Email, CaLamViec.NgayLamViec + GioBatDau |
| `GuiThongBaoXacNhanLichHenAsync(idLichHen)` | Lễ tân xác nhận | LichHen → BenhNhan.Email |
| `GuiThongBaoHuyLichHenAsync(idLichHen, lyDo, doPhongKhamHuy)` | Hủy lịch (bệnh nhân hoặc phòng khám) | LichHen → BenhNhan.Email, lý do |
| `GuiThongBaoDoiLichHenAsync(idLichHenCu, idLichHenMoi)` | Sau khi đổi lịch thành công | Cả 2 LichHen |
| `GuiThongBaoCheckInAsync(idHangCho)` | Sau check-in | HangCho → LichHen → BenhNhan.Email, SoThuTu |
| `GuiThongBaoGoiBenhNhanAsync(idHangCho)` | Bác sĩ gọi kế tiếp | HangCho → BenhNhan.Email, TenBacSi |

**Semantic bắt buộc:** Tất cả method phải là fire-and-forget — KHÔNG throw exception làm hỏng flow chính. Lỗi gửi thông báo chỉ được log, không được propagate.

#### `IOtpService` — OTP email thật (tách khỏi stub)

Interface tại `ClinicBooking.Application/Abstractions/Security/IOtpService.cs`:

```csharp
public interface IOtpService
{
    Task<string> TaoVaGuiOtpDatLichAsync(int idTaiKhoan, string soDienThoai, CancellationToken ct = default);
    Task<bool> XacThucOtpDatLichAsync(int idTaiKhoan, string maOtp, CancellationToken ct = default);
}
```

`OtpServiceStub` hiện đã ghi DB thật (`OtpLog`), có expiry và rate limit — logic đúng rồi. Việc cần làm là:
- Thêm gửi email thật (dùng `IEmailService` / MailKit)
- Không hiển thị mã OTP ra `TempData` nữa

**Đề xuất package:** `MailKit` (đã có trong danh sách tech stack cho phép)

**Cấu hình SMTP mẫu (thêm vào `appsettings.json`, giá trị thật dùng user-secrets):**
```json
"Email": {
  "SmtpHost": "smtp.gmail.com",
  "SmtpPort": 587,
  "StartTls": true,
  "FromAddress": "noreply@phongkham.vn",
  "FromName": "Phòng Khám Demo"
}
```

### 4.2 Use case lõi Admin — BE + Admin UI — deadline 2026-05-16

> **Yêu cầu:** Mỗi use case phải có cả handler (BE) lẫn Razor Page trong `Pages/Admin/` (FE) hoàn chỉnh. Không chấp nhận hardcode data hay trang placeholder.

#### A. Báo cáo & Thống kê — **Core**

- [ ] **BE:** `BaoCaoLichHenQuery` — số lượt khám theo ngày, theo bác sĩ, theo chuyên khoa; tỷ lệ hủy
- [ ] **FE (Admin):** Trang `/Admin/BaoCao` — biểu đồ hoặc bảng tổng hợp, lọc theo khoảng ngày
- [ ] **BE:** `ThongKeTrangThaiLichHenQuery` — tỷ lệ ChoXacNhan / DaXacNhan / HoanThanh / Huy

#### B. Quản lý tài khoản — **Core**

- [ ] **BE:** `KhoaTaiKhoanCommand` — admin khóa/mở tài khoản bất kỳ vai trò
- [ ] **BE:** `DanhSachTaiKhoanQuery` — lọc theo vai trò, trang thái, từ khóa tên
- [ ] **FE (Admin):** Trang `/Admin/QuanLyTaiKhoan` — bảng danh sách, nút Khóa/Mở

#### C. Quản lý mẫu thông báo — **Core**

- [ ] **BE:** CRUD `MauThongBao`
- [ ] **FE (Admin):** Trang `/Admin/QuanLyMauThongBao`

#### D. Dashboard Admin — **Core**

- [ ] **FE (Admin):** Trang `/Admin/Dashboard` (hiện là placeholder) — nối số liệu thật: lịch hẹn hôm nay, hàng chờ, tỷ lệ hoàn thành tuần này

### 4.3 Hạ tầng vận hành — Module 4 cần làm

#### A. Background Jobs — chuyển từ BackgroundService sang Hangfire

Hiện Module 1 đang chạy 2 `IHostedService` tạm thời:
- `QuetGiuChoHetHanJob` — quét giữ chỗ hết hạn mỗi 1 phút
- `ChuyenLichHenDaQuaHanJob` — chuyển lịch quá hạn sang `DaQua` mỗi 5 phút

Khi Module 4 triển khai Hangfire, cần:

```csharp
// Xóa 2 dòng này trong DependencyInjection.cs:
services.AddHostedService<QuetGiuChoHetHanJob>();
services.AddHostedService<ChuyenLichHenDaQuaHanJob>();

// Thay bằng Hangfire recurring jobs tương đương
RecurringJob.AddOrUpdate<QuetGiuChoHetHanJob>("quet-giu-cho", j => j.ChayAsync(...), "*/1 * * * *");
RecurringJob.AddOrUpdate<ChuyenLichHenDaQuaHanJob>("chuyen-qua-han", j => j.ChayAsync(...), "*/5 * * * *");
```

Thêm job mới:
- [ ] Nhắc lịch hẹn trước 1 giờ (gọi `INotificationService`)
- [ ] Cleanup `OtpLog` hết hạn (> 24h)
- [ ] Gọi `ICaLamViecQueryService.ChayReconSlotAsync()` định kỳ để đồng bộ slot

#### B. Docker & CI/CD

- [ ] `Dockerfile` cho `ClinicBooking.Web` (hoặc `ClinicBooking.Api`)
- [ ] `docker-compose.yml`: Web/API + SQL Server 2022 + (tùy chọn) Mailhog
- [ ] GitHub Actions pipeline: `restore → build → test → docker build`

**Gợi ý docker-compose tối thiểu:**
```yaml
services:
  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      SA_PASSWORD: "Dev@123456"
      ACCEPT_EULA: "Y"
    ports: ["1433:1433"]

  web:
    build: .
    environment:
      ASPNETCORE_ENVIRONMENT: Development
      ConnectionStrings__DefaultConnection: "Server=db;Database=ClinicBooking;User=sa;Password=Dev@123456;TrustServerCertificate=True"
    ports: ["5000:8080"]
    depends_on: [db]
```

#### B. Auth bổ sung

- [ ] `POST /api/auth/quen-mat-khau` → phát OTP email
- [ ] `POST /api/auth/dat-lai-mat-khau` → dùng OTP
- [ ] `POST /api/auth/doi-mat-khau` → đã đăng nhập

---

## Tóm tắt — Ma trận ưu tiên

| # | Module | Việc | Yêu cầu | Deadline | Ảnh hưởng |
|---|---|---|---|---|---|
| 1 | **M2** | `DanhSachDichVuQuery` (DTO tối thiểu) | BE | **2026-05-12** | Unblock feature Walk-in Module 1 |
| 2 | **M4** | Implement `INotificationService` thật (ghi DB ThongBao + email) | BE | 2026-05-16 | Unblock in-app notification toàn hệ thống |
| 3 | **M4** | Implement `IOtpService` thật (gửi OTP qua email) | BE | 2026-05-16 | Unblock flow đặt lịch thật sự |
| 4 | **M2** | Verify & nhận quyền sở hữu `CaLamViecQueryService` | BE | 2026-05-16 | Đảm bảo slot logic đúng |
| 5 | **M2** | CRUD CaLamViec + Admin UI `/Admin/QuanLyCaLamViec` | BE + FE | 2026-05-16 | Mới có thể test thêm ca ngoài seed |
| 6 | **M2** | CRUD Danh mục (ChuyenKhoa, DichVu, Phong) + Admin UI | BE + FE | 2026-05-16 | Hoàn thiện danh mục hệ thống |
| 7 | **M2** | CRUD BacSi (Admin) + BacSi xem lịch `/BacSi/LichLamViec` | BE + FE | 2026-05-16 | Hoàn thiện Module 2 |
| 8 | **M3** | `TaoHoSoKham` + `KeToa` từ trang `/BacSi/QuanLyKham` | BE + FE | 2026-05-16 | Hoàn thiện flow bác sĩ |
| 9 | **M3** | Bệnh nhân xem lịch sử khám + đơn thuốc | BE + FE | 2026-05-16 | Hoàn thiện Module 3 |
| 10 | **M4** | Admin Dashboard thật + Báo cáo + Quản lý tài khoản | BE + FE | 2026-05-16 | Core admin use cases |
| 11 | **M4** | CRUD MauThongBao | BE + FE | 2026-05-16 | Quản lý thông báo |
| 12 | **M3** | Confirm `SoLanHuyMuon` logic | BE | 2026-05-16 | Module 1 đang write, cần verify |
| 13 | **M4** | Chuyển background jobs sang Hangfire | BE | sau deadline | Hiện `BackgroundService` đã chạy đủ |
| 14 | **M4** | Docker + CI/CD | Infra | sau deadline | Cần cho deployment thật |

---

## Điều Module 1 cam kết không thay đổi (để tránh breaking changes)

- Signature của `ICaLamViecQueryService` — stable
- Signature của `INotificationService` — stable
- `OtpLog` schema (bảng DB) — Module 4 có thể extend nhưng không drop cột
- `LichHen.TrangThai` enum values và transition logic — stable
- Seed CaLamViec ID 3001–3003 — không xóa, Module 2 có thể thêm mới

## Liên hệ & phối hợp

Trước khi thay thế bất kỳ stub nào, tạo PR nhỏ và tag Module 1 owner để review DI registration change. Tránh merge thay đổi breaking vào `develop` mà không thông báo.
