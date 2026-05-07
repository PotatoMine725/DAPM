# Báo cáo: Tích hợp Integration Test cho Module 1 & Sửa lỗi Authentication

**Ngày ghi nhận:** 2026-05-03  
**Phạm vi:** `ClinicBooking.Integration.Tests`, `ClinicBooking.Api`, `ClinicBookingApiFactory`

---

## 1. Mục tiêu ban đầu
Thiết lập bộ Integration Test hoàn chỉnh và chạy thực tế (không dùng mock hay stub đối với DbContext) cho Module 1, đặc biệt tập trung vào các luồng quan trọng: Đặt lịch, Hủy lịch, Đổi lịch và Check-in.
- **Yêu cầu:** Gửi HTTP request thực qua `WebApplicationFactory`, kết nối tới Database cấu trúc thật, vượt qua được luồng xác thực JWT `[Authorize]`.

## 2. Những gì đã làm
- Khởi tạo thư mục và dự án `ClinicBooking.Integration.Tests`.
- Thiết lập file rập khuôn `ClinicBookingApiFactory` kế thừa từ `WebApplicationFactory<Program>`.
- Ghi đè `IAppDbContext` để sử dụng SQL Server LocalDB (`(localdb)\MSSQLLocalDB`) riêng biệt cho test, đồng thời tự động gọi `Database.Migrate()` khi host khởi động để dựng schema mới nhất.
- Bổ sung logic dịch ngày của các `CaLamViec` có trong Db Seed lên tương lai (1-7 ngày) để không bị vi phạm constraint về thời gian cho các bài test tạo lịch hẹn.
- Tạo class `UnitTest1` và code bộ client gọi đến `/api/auth/dang-nhap` để tự động giả lập luồng Đăng nhập lấy `AccessToken` Bearer, phục vụ gọi các endpoint tiếp theo.

## 3. Lỗi đã gặp
Trong quá trình thực thi, hệ thống liên tiếp gặp nhiều cản trở làm Test Run bị Fail:
1. **Lỗi EF Core Provider Conflict:** `MissingMethodException` do ban đầu test project cài `Microsoft.EntityFrameworkCore.Sqlite` v10 nhưng application base đang dùng EF Core 9 SQL Server.
2. **Lỗi chuyển hướng HTTPS làm rơi Authorization Header:** API trả `401 Unauthorized` ngay cả khi đã add Header.
3. **Lỗi sai mật khẩu Seed Data khi test:** Đăng nhập `/api/auth/dang-nhap` báo `"Tài khoản hoac mat khau khong dung"` khi sử dụng mật khẩu quen thuộc `Demo@123456`.
4. **Lỗi 401 Unauthorized do Token Validation Mismatch:** Sau khi qua được bước đăng nhập (với mật khẩu gượng ép `Test@123456`), gọi API `/api/lich-hen/tao-lich-hen` vẫn văng lỗi `401 Unauthorized` mặc dù API này chỉ yêu cầu vai trò `BenhNhan`.

## 4. Kết quả điều tra
- **Tra cứu SQLite:** Sự chênh lệch version (EF v9 vs EF Core Sqlite v10) phá vỡ tính tương thích khi DbContext khởi tạo.
- **Tra cứu HTTPS Redirect:** Bằng mặc định, Test Host chạy http và chuyển hướng (redirect 302) sang https. HttpClient bị rớt `Authorization` Header trong pha redirect này.
- **Điều tra mật khẩu Seed:** Mật khẩu trong môi trường Development được fix là `Demo@123456` (theo báo cáo *bao-cao-loi-dang-nhap-seed-port.md*). Thế nhưng bên trong `ClinicBookingApiFactory.cs`, lệnh ghi đè (override) Dictionary Setting lại chứa `["Admin:DevFixture:MatKhauChung"] = "Test@123456"`.
- **Tra cứu lỗi 401 chập chờn khi verify Token:** Mặc dù luồng lấy Token thành công, token đó lại trượt bài test của `JwtBearerMiddleware`. Lý do được phát hiện là `ClinicBookingApiFactory` đã override `["Jwt:Issuer"]`, `["Jwt:Audience"]`, `["Jwt:Key"]` bằng các hardcode string dành riêng cho Test (`ClinicBooking.Tests`), **nhưng** API khi khởi tạo middleware `.AddJwtAuthentication(builder.Configuration)` vẫn lấy config JWT gốc từ `appsettings.json` thật của WebHost. Dẫn đến Hệ thống Test phát token một đằng, Web API nội bộ lại verify bằng chữ ký một nẻo => Fail chữ ký => 401.

## 5. Những gì đã làm để sửa lỗi
1. **Xử lý Provider:** Gỡ bỏ package `.Sqlite` khỏi `ClinicBooking.Integration.Tests.csproj`, giữ lại hoàn toàn stack SQL Server LocalDB.
2. **Xử lý Redirect:** Khi tạo Client (`factory.CreateClient()`), luôn đính kèm `BaseAddress = new Uri("https://localhost")` nhằm tránh trigger redirect.
3. **Xử lý đồng nhất config JWT & Password:** 
   - Thay đổi các value config trong `ClinicBookingApiFactory.cs` từ `Test@123456` thành `Demo@123456` để đồng nhất với bộ User Fixture (`patient001`, `doctor001`...).
   - **Xóa bỏ** block override settings liên quan đến `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key` khỏi Test Factory. Cho phép cả bộ render Token lẫn bộ Verify Middleware dùng chung một Config từ ASP.NET Host (chính là nội dung thật của môi trường Development appsettings).
4. Cập nhật test case trong `UnitTest1.cs` để login bằng `patient001` / `Demo@123456` và kiểm chứng tạo mới lịch hẹn.

## 6. Kết quả hiện tại
- Luồng Integration Test đầu tiên: **Đặt Lịch Hẹn (Module1_Smoke_Dat_Lich_BangApiThat)** đã **Passed 100%**.
- Token được sinh và xác thực thành công.
- Bản ghi `LichHen` và lịch sử `LichSuLichHen` được seed xuống DB thật một cách nhất quán.
- Cấu hình hạ tầng Test (Test Infrastructure) đã sạch và ổn định, sẵn sàng làm nền tảng phát triển các test case tiếp theo (Hủy Lịch, Đổi Lịch).
