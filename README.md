# ClinicBooking - Hệ Thống Đặt Lịch Khám Bệnh

> **Phiên bản**: 1.0  
> **Framework**: ASP.NET Core 8.0  
> **Database**: SQL Server 2022  
> **Architecture**: Clean Architecture + CQRS

---

## 📋 Giới Thiệu

**ClinicBooking** là hệ thống quản lý đặt lịch khám bệnh trực tuyến, được xây dựng với kiến trúc Clean Architecture và CQRS pattern. Hệ thống hỗ trợ đầy đủ quy trình từ đặt lịch, xác nhận, check-in, khám bệnh, đến kê đơn thuốc.

### ✨ Tính Năng Chính

#### Module 1: Đặt Lịch Hẹn & Hàng Chờ ✅
- ✅ Đặt lịch khám trực tuyến
- ✅ Xác nhận lịch hẹn (Lễ tân)
- ✅ Hủy/Đổi lịch hẹn
- ✅ Giữ chỗ tạm thời (15 phút)
- ✅ Check-in bệnh nhân
- ✅ Quản lý hàng chờ khám
- ✅ Gọi bệnh nhân kế tiếp
- ✅ Đặt lịch hộ khách vãng lai (Walk-in)

#### Module 2: Bác Sĩ, Lịch Làm Việc & Danh Mục 🚧
- 🚧 Quản lý ca làm việc
- 🚧 Quản lý bác sĩ
- 🚧 Quản lý danh mục (Chuyên khoa, Dịch vụ, Phòng)
- 🚧 Nghỉ phép

#### Module 3: Bệnh Nhân, Hồ Sơ Khám & Kê Đơn ✅
- ✅ Tạo/cập nhật hồ sơ khám (Bác sĩ)
- ✅ Kê đơn thuốc (Bác sĩ)
- ✅ Xem lịch sử khám (Bệnh nhân)
- ✅ Xem đơn thuốc (Bệnh nhân)
- ✅ Quản lý danh mục thuốc (Admin)
- ✅ Hạn chế đặt lịch (SoLanHuyMuon >= 3)

#### Module 4: Thông Báo, Admin & Hạ Tầng 🚧
- 🚧 Thông báo in-app
- 🚧 Gửi email thông báo
- 🚧 OTP xác thực
- 🚧 Dashboard Admin
- 🚧 Báo cáo thống kê
- 🚧 Quản lý tài khoản
- 🚧 Background jobs (Hangfire)

---

## 🏗️ Kiến Trúc

### Clean Architecture

```
┌─────────────────────────────────────────────────────────┐
│                  Presentation Layer                      │
│         (ClinicBooking.Web, ClinicBooking.Api)          │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                 Application Layer                        │
│            (ClinicBooking.Application)                   │
│         Commands, Queries, Handlers, DTOs                │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                   Domain Layer                           │
│              (ClinicBooking.Domain)                      │
│         Entities, Enums, Value Objects                   │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│               Infrastructure Layer                       │
│           (ClinicBooking.Infrastructure)                 │
│      EF Core, Repositories, External Services            │
└─────────────────────────────────────────────────────────┘
```

### CQRS Pattern

- **Commands**: Write operations (Create, Update, Delete)
- **Queries**: Read operations (Get, List, Search)
- **Handlers**: Business logic implementation
- **MediatR**: Request/Response pipeline

---

## 🚀 Bắt Đầu

### Yêu Cầu Hệ Thống

- **.NET SDK**: 8.0 hoặc cao hơn
- **SQL Server**: 2022 hoặc cao hơn
- **Visual Studio**: 2022 (khuyến nghị)
- **RAM**: 8GB (khuyến nghị 16GB)

### Cài Đặt Nhanh

```bash
# 1. Clone repository
git clone <repo-url> ClinicBooking
cd ClinicBooking

# 2. Restore packages
dotnet restore

# 3. Cấu hình connection string (sửa appsettings.json)
# Server=localhost;Database=ClinicBooking;Trusted_Connection=True;TrustServerCertificate=True

# 4. Tạo database
cd ClinicBooking.Web
dotnet ef database update

# 5. Chạy ứng dụng
dotnet run
```

Mở trình duyệt: `https://localhost:5001`

### Tài Khoản Test

| Role | Username | Password |
|------|----------|----------|
| Admin | admin | Admin@123456 |
| Lễ Tân | letan | Letan@123456 |
| Bác Sĩ | bacsi_nguyenvana | Letan@123456 |
| Bệnh Nhân | benhnhan_tranthib | Letan@123456 |

---

## 📚 Tài Liệu

- **[SETUP-GUIDE.md](docs/SETUP-GUIDE.md)** - Hướng dẫn cài đặt chi tiết
- **[QUICK-START.md](docs/QUICK-START.md)** - Hướng dẫn nhanh
- **[Module 3 Completion Report](.kiro/specs/module-3-benh-nhan-ho-so-kham-ke-don/COMPLETION-REPORT.md)** - Báo cáo hoàn thành Module 3

---

## 🧪 Testing

### Chạy Unit Tests

```bash
# Chạy tất cả tests
dotnet test

# Chạy tests với coverage
dotnet test --collect:"XPlat Code Coverage"

# Chạy tests của một project
dotnet test ClinicBooking.Application.UnitTests
```

**Test Coverage**: 252 tests (241 unit + 11 integration) - **100% PASS** ✅

### Test Scenarios

- ✅ Đặt lịch hẹn thành công
- ✅ Hủy lịch hẹn (muộn/sớm)
- ✅ Đổi lịch hẹn
- ✅ Check-in và quản lý hàng chờ
- ✅ Tạo hồ sơ khám và kê đơn
- ✅ Authorization (role-based access)
- ✅ Business rules validation

---

## 🛠️ Tech Stack

### Backend
- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core 9.0
- **Database**: SQL Server 2022
- **Authentication**: JWT Bearer Token
- **Validation**: FluentValidation
- **Mediator**: MediatR

### Frontend
- **UI**: Razor Pages
- **CSS**: Bootstrap 5
- **JavaScript**: Vanilla JS

### Testing
- **Unit Testing**: xUnit
- **Mocking**: NSubstitute
- **Assertions**: FluentAssertions
- **In-Memory DB**: SQLite (for tests)

### Tools
- **Migrations**: EF Core Migrations
- **API Documentation**: Swagger/OpenAPI (ClinicBooking.Api)
- **Background Jobs**: IHostedService (sẽ chuyển sang Hangfire)

---

## 📊 Database Schema

### Các Bảng Chính

| Bảng | Mô tả |
|------|-------|
| `TaiKhoan` | Tài khoản người dùng |
| `BenhNhan` | Thông tin bệnh nhân |
| `BacSi` | Thông tin bác sĩ |
| `LichHen` | Lịch hẹn khám |
| `CaLamViec` | Ca làm việc của bác sĩ |
| `HangCho` | Hàng chờ khám |
| `GiuCho` | Giữ chỗ tạm thời |
| `HoSoKham` | Hồ sơ khám bệnh |
| `ToaThuoc` | Đơn thuốc |
| `Thuoc` | Danh mục thuốc |
| `ChuyenKhoa` | Chuyên khoa |
| `DichVu` | Dịch vụ khám |
| `Phong` | Phòng khám |
| `ThongBao` | Thông báo |
| `OtpLog` | Log OTP |
| `LichSuLichHen` | Lịch sử thay đổi lịch hẹn |

---

## 🔐 Security

### Authentication
- JWT Bearer Token
- Access Token: 60 phút
- Refresh Token: 7 ngày
- Password hashing: BCrypt

### Authorization
- Role-based: Admin, LeTan, BacSi, BenhNhan
- Resource-based: Chỉ xem/sửa dữ liệu của mình

### Validation
- FluentValidation cho tất cả Commands
- Business rules enforcement
- Input sanitization

---

## 📈 Roadmap

### Phase 1: Core Features ✅
- [x] Module 1: Đặt lịch & Hàng chờ
- [x] Module 3: Hồ sơ khám & Kê đơn
- [x] Authentication & Authorization
- [x] Unit Tests

### Phase 2: Admin & Infrastructure 🚧
- [ ] Module 2: Bác sĩ & Danh mục
- [ ] Module 4: Thông báo & Admin
- [ ] Email notifications
- [ ] Background jobs (Hangfire)
- [ ] Integration tests

### Phase 3: Advanced Features 📅
- [ ] Payment integration
- [ ] SMS notifications
- [ ] Mobile app (React Native)
- [ ] Telemedicine
- [ ] Analytics dashboard

---

## 🤝 Contributing

### Quy Trình Phát Triển

1. **Clone repository**
2. **Tạo branch mới**: `feature/module-x-feature-name`
3. **Implement feature** theo spec trong `.kiro/specs/`
4. **Viết tests** (unit + integration)
5. **Chạy tests**: `dotnet test` (phải 100% pass)
6. **Tạo PR** vào `develop`
7. **Code review** bởi Module 1 owner
8. **Merge** sau khi approved

### Coding Standards

- **Clean Architecture**: Tuân thủ dependency rules
- **CQRS**: Commands cho write, Queries cho read
- **Naming**: PascalCase cho classes, camelCase cho variables
- **Comments**: Tiếng Việt hoặc English
- **Tests**: Mỗi handler phải có unit tests

---

## 📞 Liên Hệ

- **Project Owner**: Module 1 Team
- **Module 3 Owner**: Member C
- **Documentation**: `.kiro/specs/` và `docs/`

---

## 📄 License

Copyright © 2026 ClinicBooking Team. All rights reserved.

---

## 🎯 Status

| Module | Status | Progress | Deadline |
|--------|--------|----------|----------|
| Module 1 | ✅ Complete | 100% | Done |
| Module 2 | 🚧 In Progress | 30% | 2026-05-16 |
| Module 3 | ✅ Complete | 100% | Done (5 days early) |
| Module 4 | 🚧 In Progress | 20% | 2026-05-16 |

**Overall Progress**: 62.5% (2.5/4 modules complete)

---

**Built with ❤️ using Clean Architecture & CQRS**
