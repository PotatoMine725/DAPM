# Hướng Dẫn Cài Đặt & Khởi Chạy ClinicBooking

> **Phiên bản**: 1.0  
> **Ngày cập nhật**: 2026-05-11  
> **Hệ thống**: ClinicBooking - Hệ thống đặt lịch khám bệnh

---

## 📋 Mục Lục

1. [Yêu Cầu Hệ Thống](#yêu-cầu-hệ-thống)
2. [Cài Đặt Môi Trường](#cài-đặt-môi-trường)
3. [Clone & Cấu Hình Project](#clone--cấu-hình-project)
4. [Cấu Hình Database](#cấu-hình-database)
5. [Chạy Ứng Dụng](#chạy-ứng-dụng)
6. [Tài Khoản Test](#tài-khoản-test)
7. [Troubleshooting](#troubleshooting)

---

## 🖥️ Yêu Cầu Hệ Thống

### Phần Mềm Bắt Buộc

| Phần mềm | Phiên bản | Link tải |
|----------|-----------|----------|
| **.NET SDK** | 8.0 hoặc cao hơn | [Download](https://dotnet.microsoft.com/download/dotnet/8.0) |
| **SQL Server** | 2022 hoặc cao hơn | [Download](https://www.microsoft.com/sql-server/sql-server-downloads) |
| **Visual Studio** | 2022 (khuyến nghị) | [Download](https://visualstudio.microsoft.com/) |
| **Git** | Latest | [Download](https://git-scm.com/) |

### Phần Mềm Tùy Chọn

- **SQL Server Management Studio (SSMS)** - Quản lý database
- **Visual Studio Code** - Editor nhẹ
- **Postman** - Test API endpoints

### Cấu Hình Tối Thiểu

- **RAM**: 8GB (khuyến nghị 16GB)
- **Ổ cứng**: 5GB trống
- **OS**: Windows 10/11, macOS, hoặc Linux

---

## 🔧 Cài Đặt Môi Trường

### Bước 1: Cài Đặt .NET SDK

1. Tải .NET 8.0 SDK từ [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0)
2. Chạy file cài đặt và làm theo hướng dẫn
3. Kiểm tra cài đặt thành công:

```bash
dotnet --version
```

**Kết quả mong đợi**: `8.0.x` hoặc cao hơn

### Bước 2: Cài Đặt SQL Server

#### Option A: SQL Server 2022 (Khuyến nghị cho Production)

1. Tải SQL Server 2022 Developer Edition (miễn phí)
2. Chọn **Basic Installation**
3. Ghi nhớ **Server Name** (thường là `localhost` hoặc `.\SQLEXPRESS`)
4. Chọn **Mixed Mode Authentication** và đặt mật khẩu cho `sa`

#### Option B: SQL Server Express (Nhẹ hơn)

1. Tải SQL Server Express
2. Cài đặt với tên instance: `SQLEXPRESS`
3. Bật **SQL Server Browser** service

#### Kiểm Tra SQL Server

```bash
# Kiểm tra SQL Server đang chạy
sqlcmd -S localhost -U sa -P YourPassword
```

Hoặc mở **SQL Server Management Studio (SSMS)** và kết nối.

### Bước 3: Cài Đặt Visual Studio (Khuyến nghị)

1. Tải Visual Studio 2022 Community (miễn phí)
2. Chọn workload: **ASP.NET and web development**
3. Chọn workload: **.NET desktop development**
4. Cài đặt

---

## 📦 Clone & Cấu Hình Project

### Bước 1: Clone Repository

```bash
# Clone project từ Git
git clone <repository-url> ClinicBooking
cd ClinicBooking

# Hoặc nếu đã có source code, giải nén vào thư mục
```

### Bước 2: Restore Dependencies

```bash
# Restore tất cả NuGet packages
dotnet restore

# Hoặc trong Visual Studio: Right-click Solution → Restore NuGet Packages
```

### Bước 3: Cấu Hình Connection String

#### Cách 1: Sửa file `appsettings.json` (Không khuyến nghị cho production)

Mở file `ClinicBooking.Web/appsettings.json` và sửa connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=ClinicBooking;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

**Thay thế `YOUR_SERVER_NAME`** bằng:
- `localhost` - Nếu dùng SQL Server default instance
- `.\SQLEXPRESS` - Nếu dùng SQL Server Express
- `POTATO` - Tên máy của bạn (nếu dùng Windows Authentication)

#### Cách 2: Dùng User Secrets (Khuyến nghị)

```bash
# Di chuyển vào thư mục Web project
cd ClinicBooking.Web

# Khởi tạo user secrets
dotnet user-secrets init

# Set connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost;Database=ClinicBooking;Trusted_Connection=True;TrustServerCertificate=True"

# Quay lại thư mục root
cd ..
```

#### Cách 3: Dùng SQL Authentication (Nếu không dùng Windows Auth)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ClinicBooking;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
  }
}
```

---

## 🗄️ Cấu Hình Database

### Bước 1: Tạo Database

#### Option A: Dùng EF Core Migrations (Khuyến nghị)

```bash
# Di chuyển vào thư mục Web project
cd ClinicBooking.Web

# Tạo database và chạy migrations
dotnet ef database update

# Nếu gặp lỗi "dotnet ef not found", cài đặt tool:
dotnet tool install --global dotnet-ef

# Sau đó chạy lại:
dotnet ef database update
```

#### Option B: Dùng Visual Studio Package Manager Console

1. Mở Visual Studio
2. Mở **Tools → NuGet Package Manager → Package Manager Console**
3. Chọn **Default project**: `ClinicBooking.Infrastructure`
4. Chạy lệnh:

```powershell
Update-Database
```

### Bước 2: Kiểm Tra Database

Sau khi chạy migrations, database `ClinicBooking` sẽ được tạo với:

**Các bảng chính:**
- `TaiKhoan` - Tài khoản người dùng
- `BenhNhan` - Thông tin bệnh nhân
- `BacSi` - Thông tin bác sĩ
- `LichHen` - Lịch hẹn khám
- `CaLamViec` - Ca làm việc của bác sĩ
- `HangCho` - Hàng chờ khám
- `HoSoKham` - Hồ sơ khám bệnh
- `ToaThuoc` - Đơn thuốc
- `Thuoc` - Danh mục thuốc
- ... và nhiều bảng khác

**Data mẫu (Test Data):**
- 1 tài khoản Admin
- 1 tài khoản Lễ tân
- 1 tài khoản Bác sĩ (Dr. Nguyen Van A)
- 1 tài khoản Bệnh nhân (Tran Thi B)
- 3 ca làm việc mẫu (ID: 3001, 3002, 3003)
- 2 lịch hẹn mẫu

### Bước 3: Verify Database

Kết nối vào SQL Server và kiểm tra:

```sql
-- Kiểm tra database đã tạo
USE ClinicBooking;

-- Kiểm tra số lượng bảng
SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE';
-- Kết quả mong đợi: ~20 bảng

-- Kiểm tra data mẫu
SELECT * FROM TaiKhoan;
SELECT * FROM BacSi;
SELECT * FROM BenhNhan;
SELECT * FROM CaLamViec;
```

---

## 🚀 Chạy Ứng Dụng

### Cách 1: Chạy bằng Visual Studio (Khuyến nghị)

1. Mở file `DatLichPhongKham.slnx` trong Visual Studio
2. Set **ClinicBooking.Web** làm Startup Project:
   - Right-click `ClinicBooking.Web` → **Set as Startup Project**
3. Nhấn **F5** hoặc click nút **Run** (▶️)
4. Trình duyệt sẽ tự động mở: `https://localhost:5001` hoặc `http://localhost:5000`

### Cách 2: Chạy bằng Command Line

```bash
# Di chuyển vào thư mục Web project
cd ClinicBooking.Web

# Chạy ứng dụng
dotnet run

# Hoặc chạy với watch mode (tự động reload khi có thay đổi)
dotnet watch run
```

**Output mong đợi:**

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### Cách 3: Chạy với IIS Express

1. Trong Visual Studio, chọn **IIS Express** từ dropdown
2. Nhấn **F5**
3. Ứng dụng sẽ chạy trên port được cấu hình trong `launchSettings.json`

---

## 👤 Tài Khoản Test

### Tài Khoản Admin (Quản trị viên)

```
Username: admin
Password: Admin@123456
Role: Admin
```

**Quyền hạn:**
- Quản lý tất cả danh mục (Chuyên khoa, Dịch vụ, Phòng, Thuốc)
- Quản lý tài khoản
- Quản lý ca làm việc
- Xem báo cáo thống kê

### Tài Khoản Lễ Tân

```
Username: letan
Password: Letan@123456
Role: LeTan
```

**Quyền hạn:**
- Xác nhận lịch hẹn
- Check-in bệnh nhân
- Quản lý hàng chờ
- Đặt lịch hộ khách vãng lai

### Tài Khoản Bác Sĩ

```
Username: bacsi_nguyenvana
Password: Letan@123456
Role: BacSi
ID: 2001
```

**Quyền hạn:**
- Xem lịch làm việc
- Quản lý hàng chờ
- Tạo/cập nhật hồ sơ khám
- Kê đơn thuốc

### Tài Khoản Bệnh Nhân

```
Username: benhnhan_tranthib
Password: Letan@123456
Role: BenhNhan
ID: 2001
```

**Quyền hạn:**
- Đặt lịch khám
- Xem lịch sử lịch hẹn
- Xem lịch sử khám bệnh
- Xem đơn thuốc

---

## 🧪 Kiểm Tra Ứng Dụng

### Bước 1: Truy Cập Trang Chủ

Mở trình duyệt và truy cập: `https://localhost:5001`

**Trang chủ hiển thị:**
- Form đăng nhập
- Link đăng ký tài khoản mới

### Bước 2: Đăng Nhập

1. Nhập username: `admin`
2. Nhập password: `Admin@123456`
3. Click **Đăng nhập**

**Sau khi đăng nhập thành công:**
- Redirect về trang Dashboard tương ứng với role
- Admin → `/Admin/Dashboard`
- Lễ tân → `/LeTan/Dashboard`
- Bác sĩ → `/BacSi/Dashboard`
- Bệnh nhân → `/BenhNhan/Dashboard`

### Bước 3: Test Các Chức Năng

#### Test Module 1 (Đặt lịch hẹn & Hàng chờ)

**Với tài khoản Bệnh nhân:**
1. Truy cập `/BenhNhan/DatLich`
2. Chọn chuyên khoa, bác sĩ, ngày khám
3. Đặt lịch thành công
4. Xem lịch hẹn tại `/BenhNhan/LichHen`

**Với tài khoản Lễ tân:**
1. Truy cập `/LeTan/XacNhanLichHen`
2. Xác nhận lịch hẹn
3. Check-in bệnh nhân tại `/LeTan/CheckIn`
4. Xem hàng chờ tại `/LeTan/HangCho`

**Với tài khoản Bác sĩ:**
1. Truy cập `/BacSi/HangCho`
2. Gọi bệnh nhân kế tiếp
3. Hoàn thành khám

#### Test Module 3 (Hồ sơ khám & Kê đơn)

**Với tài khoản Bác sĩ:**
1. Truy cập `/BacSi/QuanLyKham?idLichHen=4001`
2. Tạo hồ sơ khám (nhập chẩn đoán, kết quả khám)
3. Kê đơn thuốc (chọn thuốc, liều lượng, cách dùng)
4. Lưu hồ sơ

**Với tài khoản Bệnh nhân:**
1. Truy cập `/BenhNhan/LichSuKham` - Xem lịch sử khám
2. Truy cập `/BenhNhan/ToaThuoc` - Xem đơn thuốc

### Bước 4: Chạy Unit Tests

```bash
# Chạy tất cả tests
dotnet test

# Chạy tests với coverage
dotnet test --collect:"XPlat Code Coverage"

# Chạy tests của một project cụ thể
dotnet test ClinicBooking.Application.UnitTests/ClinicBooking.Application.UnitTests.csproj
```

**Kết quả mong đợi:**
```
Passed!  - Failed:     0, Passed:   252, Skipped:     0, Total:   252
```

---

## 🔍 Troubleshooting

### Lỗi 1: "Cannot connect to SQL Server"

**Nguyên nhân:** Connection string sai hoặc SQL Server không chạy

**Giải pháp:**

1. Kiểm tra SQL Server đang chạy:
   ```bash
   # Windows: Mở Services và tìm "SQL Server"
   # Hoặc chạy:
   sqlcmd -S localhost -U sa -P YourPassword
   ```

2. Kiểm tra connection string trong `appsettings.json`:
   - Server name đúng chưa?
   - Database name đúng chưa?
   - Authentication mode đúng chưa?

3. Test connection bằng SSMS trước khi chạy app

### Lỗi 2: "dotnet ef not found"

**Nguyên nhân:** EF Core tools chưa được cài đặt

**Giải pháp:**

```bash
# Cài đặt EF Core tools globally
dotnet tool install --global dotnet-ef

# Hoặc update nếu đã cài:
dotnet tool update --global dotnet-ef

# Verify:
dotnet ef --version
```

### Lỗi 3: "Migration already applied"

**Nguyên nhân:** Database đã tồn tại với schema cũ

**Giải pháp:**

```bash
# Option 1: Drop database và tạo lại
dotnet ef database drop
dotnet ef database update

# Option 2: Xóa database bằng SSMS và chạy lại migration
```

### Lỗi 4: "Port 5000/5001 already in use"

**Nguyên nhân:** Port đã được sử dụng bởi ứng dụng khác

**Giải pháp:**

1. Tìm và kill process đang dùng port:
   ```bash
   # Windows
   netstat -ano | findstr :5000
   taskkill /PID <PID> /F
   ```

2. Hoặc đổi port trong `launchSettings.json`:
   ```json
   "applicationUrl": "https://localhost:7001;http://localhost:7000"
   ```

### Lỗi 5: "JWT token invalid"

**Nguyên nhân:** JWT key trong `appsettings.json` không đủ dài

**Giải pháp:**

Đảm bảo `Jwt:Key` có ít nhất 32 ký tự:

```json
{
  "Jwt": {
    "Key": "THAY_BANG_KHOA_BI_MAT_TOI_THIEU_32_KY_TU_CHO_HMAC_SHA256!!"
  }
}
```

### Lỗi 6: "Build failed" với lỗi NuGet

**Nguyên nhân:** NuGet packages chưa được restore đúng

**Giải pháp:**

```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore lại
dotnet restore

# Build lại
dotnet build
```

### Lỗi 7: "Cannot find ClinicBooking.Web.dll"

**Nguyên nhân:** Project chưa được build

**Giải pháp:**

```bash
# Build solution
dotnet build

# Hoặc build và chạy luôn
dotnet run --project ClinicBooking.Web
```

### Lỗi 8: "Access denied" khi tạo database

**Nguyên nhân:** User không có quyền tạo database

**Giải pháp:**

1. Dùng SQL Authentication với user `sa`
2. Hoặc grant quyền cho Windows user:
   ```sql
   USE master;
   CREATE LOGIN [DOMAIN\Username] FROM WINDOWS;
   ALTER SERVER ROLE sysadmin ADD MEMBER [DOMAIN\Username];
   ```

---

## 📚 Tài Liệu Bổ Sung

### Cấu Trúc Project

```
ClinicBooking/
├── ClinicBooking.Web/              # Web UI (Razor Pages)
├── ClinicBooking.Api/              # REST API (Controllers)
├── ClinicBooking.Application/      # Business Logic (CQRS)
├── ClinicBooking.Domain/           # Domain Entities
├── ClinicBooking.Infrastructure/   # Data Access (EF Core)
├── ClinicBooking.Application.UnitTests/
├── ClinicBooking.Integration.Tests/
├── database/                       # Database schema
└── docs/                          # Documentation
```

### Tech Stack

- **Framework**: ASP.NET Core 8.0
- **ORM**: Entity Framework Core 9.0
- **Database**: SQL Server 2022
- **Architecture**: Clean Architecture + CQRS
- **Authentication**: JWT Bearer Token
- **Validation**: FluentValidation
- **Testing**: xUnit, NSubstitute, FluentAssertions

### Các Module Chính

1. **Module 1**: Đặt lịch hẹn & Hàng chờ ✅
2. **Module 2**: Bác sĩ, Lịch làm việc & Danh mục (Đang phát triển)
3. **Module 3**: Bệnh nhân, Hồ sơ khám & Kê đơn ✅
4. **Module 4**: Thông báo, Admin & Hạ tầng (Đang phát triển)

### Links Hữu Ích

- [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core)
- [Clean Architecture Guide](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)

---

## 🆘 Hỗ Trợ

Nếu gặp vấn đề không có trong hướng dẫn này:

1. Kiểm tra file `COMPLETION-REPORT.md` trong `.kiro/specs/module-3-benh-nhan-ho-so-kham-ke-don/`
2. Xem logs trong console khi chạy ứng dụng
3. Kiểm tra SQL Server logs
4. Liên hệ team phát triển

---

## ✅ Checklist Hoàn Thành Setup

- [ ] Đã cài đặt .NET SDK 8.0
- [ ] Đã cài đặt SQL Server 2022
- [ ] Đã clone/giải nén source code
- [ ] Đã restore NuGet packages (`dotnet restore`)
- [ ] Đã cấu hình connection string
- [ ] Đã chạy migrations (`dotnet ef database update`)
- [ ] Đã verify database có data mẫu
- [ ] Đã chạy ứng dụng thành công (`dotnet run`)
- [ ] Đã đăng nhập được với tài khoản admin
- [ ] Đã test các chức năng cơ bản
- [ ] Đã chạy unit tests thành công (`dotnet test`)

**Chúc bạn setup thành công! 🎉**
