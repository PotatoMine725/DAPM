# Yêu cầu Module 2 / 3 / 4 — Hoàn thiện hệ thống ClinicBooking

> Ngày lập: 2026-05-05  
> Tác giả: Module 1 (User + Claude)  
> Mục đích: Liệt kê rõ từng việc mỗi module còn nợ, interface đã định nghĩa sẵn, và điểm phối hợp cụ thể giữa các module

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

### 2.2 Việc Module 2 cần làm

#### A. Verify và nhận quyền sở hữu CaLamViecQueryService

- [ ] Review `ClinicBooking.Infrastructure/Services/Scheduling/CaLamViecQueryService.cs` — đây là implementation Module 1 đã viết tạm
- [ ] Nếu cần refactor, làm trên branch Module 2 và PR vào develop
- [ ] Đảm bảo `ChayReconSlotAsync` implement đúng (hiện chưa đầy đủ)

#### B. CRUD CaLamViec qua API

- [ ] `POST /api/lich-lam-viec` — admin/bác sĩ tạo ca (`TaoCaLamViec`)
- [ ] `PUT /api/lich-lam-viec/{id}/duyet` — admin duyệt ca (`DuyetCaLamViec`)
- [ ] `DELETE /api/lich-lam-viec/{id}` — xóa ca (chỉ khi `SoSlotDaDat = 0`)
- [ ] `GET /api/lich-lam-viec` — danh sách ca (lọc theo ngày, bác sĩ, chuyên khoa)

> Các CaLamViec ID 3001–3003 đã được seed sẵn bởi Module 1 (dùng cho test/dev). Module 2 KHÔNG xóa các record này.

#### C. CRUD Danh mục

- [ ] `ChuyenKhoa`: CRUD (`Features/DanhMuc/Commands/...`)
- [ ] `DichVu`: CRUD
- [ ] `Phong`: CRUD
- [ ] `DinhNghiaCa`: CRUD

#### D. Quản lý Bác sĩ

- [ ] CRUD bác sĩ (liên kết `TaiKhoan` vai trò `bac_si`)
- [ ] Bác sĩ xem ca làm của mình (theo tuần/tháng)
- [ ] Bác sĩ yêu cầu tạo ca (`YeuCauTaoCa`)

#### E. Nghỉ phép

- [ ] `TaoDonNghiPhep` — bác sĩ gửi
- [ ] `DuyetDonNghiPhep` — admin duyệt/từ chối

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

### 3.2 Việc Module 3 cần làm

#### A. Hồ sơ khám (`HoSoKham`)

- [ ] `POST /api/ho-so-kham` — bác sĩ tạo hồ sơ từ `LichHen` đã check-in (`TaoHoSoKham`)
  - Input: `IdLichHen`, `ChanDoan`, `LoiDan`, v.v.
  - Constraint: chỉ tạo được khi `LichHen.TrangThai = DangKham`
- [ ] `PUT /api/ho-so-kham/{id}` — bác sĩ cập nhật (`CapNhatHoSoKham`)
- [ ] `GET /api/ho-so-kham/{id}` — xem chi tiết
- [ ] `GET /api/benh-nhan/{idBenhNhan}/lich-su-kham` — lịch sử khám

#### B. Kê đơn thuốc (`ToaThuoc`)

- [ ] `POST /api/ho-so-kham/{id}/toa-thuoc` — bác sĩ kê đơn (`KeToa`)
- [ ] `GET /api/ho-so-kham/{id}/toa-thuoc` — xem đơn thuốc
- [ ] `GET /api/benh-nhan/{idBenhNhan}/toa-thuoc` — tất cả đơn của bệnh nhân

#### C. Danh mục Thuốc

- [ ] `GET/POST/PUT/DELETE /api/danh-muc/thuoc` — admin quản lý

#### D. Web UI đã có sẵn (cần nối backend)

Trang `/BenhNhan/ThongBao` đã có Razor page. Module 3 (hoặc Module 4) cần nối data thật.

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

### 4.2 Hạ tầng vận hành — Module 4 cần làm

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

#### C. Admin features

- [ ] Báo cáo: số lượt khám theo ngày/bác sĩ/chuyên khoa, tỷ lệ hủy (`Features/Admin/Reports/...`)
- [ ] Danh sách tài khoản, khoá/mở (`Features/Admin/Commands/KhoaTaiKhoan`)
- [ ] CRUD `MauThongBao`

#### D. Auth bổ sung

- [ ] `POST /api/auth/quen-mat-khau` → phát OTP email
- [ ] `POST /api/auth/dat-lai-mat-khau` → dùng OTP
- [ ] `POST /api/auth/doi-mat-khau` → đã đăng nhập

---

## Tóm tắt — Ma trận ưu tiên

| # | Module | Việc | Ưu tiên | Ảnh hưởng |
|---|---|---|---|---|
| 1 | **M4** | Implement `INotificationService` thật (ghi DB ThongBao + email) | 🔴 Cao | Unblock in-app notification toàn hệ thống |
| 2 | **M4** | Implement `IOtpService` thật (gửi OTP qua email) | 🔴 Cao | Unblock flow đặt lịch thật sự |
| 3 | **M2** | Verify & nhận quyền sở hữu `CaLamViecQueryService` | 🔴 Cao | Đảm bảo slot logic đúng |
| 4 | **M2** | API tạo/duyệt CaLamViec | 🟠 Trung bình | Mới có thể test thêm ca ngoài seed |
| 5 | **M3** | `TaoHoSoKham` từ LichHen đã check-in | 🟠 Trung bình | Hoàn thiện flow bác sĩ |
| 6 | **M3** | Kê đơn thuốc | 🟠 Trung bình | Hoàn thiện Module 3 |
| 7 | **M4** | Chuyển background jobs sang Hangfire | 🟡 Thấp | Hiện `BackgroundService` đã chạy đủ |
| 8 | **M4** | Docker + CI/CD | 🟡 Thấp | Cần cho deployment thật |
| 9 | **M4** | Admin reports | 🟡 Thấp | Nice-to-have |
| 10 | **M3** | Confirm `SoLanHuyMuon` logic | 🟡 Cần xác nhận | Module 1 đang write, cần verify |

---

## Điều Module 1 cam kết không thay đổi (để tránh breaking changes)

- Signature của `ICaLamViecQueryService` — stable
- Signature của `INotificationService` — stable
- `OtpLog` schema (bảng DB) — Module 4 có thể extend nhưng không drop cột
- `LichHen.TrangThai` enum values và transition logic — stable
- Seed CaLamViec ID 3001–3003 — không xóa, Module 2 có thể thêm mới

## Liên hệ & phối hợp

Trước khi thay thế bất kỳ stub nào, tạo PR nhỏ và tag Module 1 owner để review DI registration change. Tránh merge thay đổi breaking vào `develop` mà không thông báo.
