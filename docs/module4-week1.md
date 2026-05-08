# 📦 Module 4 – Week 1: Infrastructure Setup & Integration

## 🎯 Mục tiêu

Trong tuần này, hệ thống được mở rộng với các thành phần nền tảng nhằm phục vụ cho việc phát triển và kiểm thử trong các giai đoạn tiếp theo:

* Thiết kế abstraction cho gửi email
* Cài đặt stub service (chưa gửi email thật)
* Thiết lập Integration Test
* Cấu hình Docker để chạy hệ thống
* Thiết lập CI với GitHub Actions

---

## 🧩 1. Email Abstraction & Stub Implementation

### 📌 Mục đích

Tách logic gửi email ra khỏi business logic → dễ test, dễ thay thế sau này.

---

### 🏗️ Interface: `IEmailSender`

```csharp
public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body);
}
```

👉 Đây là abstraction cho việc gửi email.

---

### ⚙️ Implementation: `EmailSenderStub`

```csharp
public class EmailSenderStub : IEmailSender
{
    public Task SendAsync(string to, string subject, string body)
    {
        // TODO: integrate real SMTP later
        return Task.CompletedTask;
    }
}
```

👉 Hiện tại chỉ là stub (giả lập), chưa gửi email thật.

---

### 📌 EmailSettings

```csharp
public class EmailSettings
{
    public string SmtpHost { get; set; }
    public int SmtpPort { get; set; }
    public string TenNguoiGui { get; set; }
    public string DiaChiGui { get; set; }
    public string TenDangNhap { get; set; }
    public string MatKhau { get; set; }
    public bool DungSsl { get; set; }
}
```

👉 Chuẩn bị sẵn cho việc cấu hình SMTP sau này.

---

### 🔁 NotificationServiceStub

* Đã cập nhật để sử dụng `IEmailSender`
* Hiện tại chỉ mock logic

---

## 🧪 2. Integration Test Framework

### 📌 Mục tiêu

Test toàn bộ flow (API → Application → Infrastructure)

---

### 📁 Cấu trúc

```
ClinicBooking.IntegrationTests/
│
├── Auth/
│   └── AuthControllerIntegrationTests.cs
│
├── Common/
│   ├── IntegrationTestBase.cs
│   └── TestWebAppFactory.cs
│
└── ClinicBooking.IntegrationTests.csproj
```

---

### 🔧 TestWebAppFactory

* Dùng để khởi tạo test server
* Override config nếu cần

---

### 🧱 IntegrationTestBase

* Base class cho các test
* Setup HttpClient

---

### ✅ Ví dụ test

```csharp
[Fact]
public async Task DangKy_ThanhCong()
{
    var response = await Client.PostAsJsonAsync("/api/auth/dang-ky", new { ... });
    response.EnsureSuccessStatusCode();
}
```

---

## 🐳 3. Docker Setup

### 📄 Dockerfile

* Build và chạy ứng dụng .NET

---

### 📄 docker-compose.yml

* Dùng để chạy nhiều service (API, DB...)

---

### 📄 env.example

* Template biến môi trường
* KHÔNG chứa thông tin nhạy cảm

---

## 🔄 4. CI – GitHub Actions

### 📁 `.github/workflows/ci.yml`

Pipeline gồm:

* Restore
* Build
* Test

👉 Giúp đảm bảo code luôn chạy đúng khi push

---

## ⚠️ 5. Lưu ý về cấu hình

* Không commit `appsettings.json`
* Sử dụng:

  * `.env`
  * `env.example`

---

## 🚀 6. Cách chạy project

### 🔹 Chạy test

```bash
dotnet test
```

---

### 🔹 Chạy Docker

```bash
docker-compose up --build
```

---

## 📌 7. Hướng phát triển tiếp theo

* Implement EmailSender thật (SMTP)
* Kết nối EmailSettings vào Dependency Injection
* Hoàn thiện Integration Test
* Fix lỗi UNIQUE constraint trong test

---

## ✅ Kết luận

Tuần 1 đã hoàn thành:

* Email abstraction ✔
* Integration Test setup ✔
* Docker setup ✔
* CI pipeline ✔

👉 Sẵn sàng cho các feature tiếp theo.
