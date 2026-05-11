# Hướng Dẫn Nhanh - ClinicBooking

> **Thời gian setup**: ~15-30 phút

---

## 🚀 Setup Nhanh (5 Bước)

### 1️⃣ Cài Đặt Môi Trường

```bash
# Kiểm tra .NET SDK (cần 8.0+)
dotnet --version

# Nếu chưa có, tải tại: https://dotnet.microsoft.com/download/dotnet/8.0
```

**SQL Server**: Cài SQL Server 2022 hoặc Express

### 2️⃣ Clone & Restore

```bash
# Clone project
git clone <repo-url> ClinicBooking
cd ClinicBooking

# Restore packages
dotnet restore
```

### 3️⃣ Cấu Hình Database

**Sửa connection string** trong `ClinicBooking.Web/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=ClinicBooking;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Thay `localhost` bằng tên SQL Server của bạn.

### 4️⃣ Tạo Database

```bash
# Cài EF Core tools (nếu chưa có)
dotnet tool install --global dotnet-ef

# Tạo database
cd ClinicBooking.Web
dotnet ef database update
cd ..
```

### 5️⃣ Chạy Ứng Dụng

```bash
# Chạy app
cd ClinicBooking.Web
dotnet run

# Hoặc dùng Visual Studio: F5
```

Mở trình duyệt: `https://localhost:5001`

---

## 👤 Đăng Nhập

### Admin
```
Username: admin
Password: Admin@123456
```

### Lễ Tân
```
Username: letan
Password: Letan@123456
```

### Bác Sĩ
```
Username: bacsi_nguyenvana
Password: Letan@123456
```

### Bệnh Nhân
```
Username: benhnhan_tranthib
Password: Letan@123456
```

---

## 🧪 Test Nhanh

```bash
# Chạy tất cả tests
dotnet test

# Kết quả mong đợi: 252 tests passed
```

---

## ❌ Lỗi Thường Gặp

### "Cannot connect to SQL Server"
```bash
# Kiểm tra SQL Server đang chạy
sqlcmd -S localhost -U sa -P YourPassword
```

### "dotnet ef not found"
```bash
dotnet tool install --global dotnet-ef
```

### "Port already in use"
Đổi port trong `launchSettings.json` hoặc kill process:
```bash
# Windows
netstat -ano | findstr :5000
taskkill /PID <PID> /F
```

---

## 📖 Tài Liệu Đầy Đủ

Xem file `SETUP-GUIDE.md` để có hướng dẫn chi tiết hơn.

---

## ✅ Checklist

- [ ] .NET SDK 8.0 installed
- [ ] SQL Server installed
- [ ] `dotnet restore` done
- [ ] Connection string configured
- [ ] `dotnet ef database update` done
- [ ] App running on `https://localhost:5001`
- [ ] Login successful with admin account

**Done! 🎉**
