# Hướng dẫn nhanh demo dự án ClinicBooking

> Tài liệu này viết để dùng khi nhóm đi gặp giảng viên hoặc cần trình bày nhanh tiến độ dự án.
> Mục tiêu là nói ngắn gọn, rõ ràng: dự án đang chạy được gì, demo thế nào, tài khoản nào dùng để thử, và mỗi module đang chịu trách nhiệm gì.

---

## 1) Tóm tắt ngắn để mở đầu khi trình bày

Dự án `ClinicBooking` là hệ thống đặt lịch khám bệnh theo kiến trúc Clean Architecture, chia thành 4 module để 4 thành viên phát triển song song.

Hiện tại dự án đã có:
- nền tảng Auth chạy được
- các luồng lịch hẹn và hàng chờ cho Module 1
- UI Web Razor Pages cho các màn cơ bản của bệnh nhân, lễ tân và bác sĩ
- tài liệu design system và hướng dẫn phát triển UI để bám thống nhất giao diện

Nếu cần nói gọn với giảng viên, có thể nói:

> “Nhóm em đã hoàn thành nền tảng hệ thống, phần xác thực, phần đặt lịch và hàng chờ của Module 1, đồng thời đã có UI demo được cho bệnh nhân, lễ tân và bác sĩ. Các module còn lại đang được chia rõ trách nhiệm để tiếp tục phát triển song song.”

---

## 2) Cách chạy demo dự án

### 2.1 Chạy backend/API

Từ thư mục gốc repo:

```bash
dotnet build DatLichPhongKham.slnx
dotnet run --project ClinicBooking.Api
```

Sau đó mở Swagger để demo API:

- `https://localhost:<port>/swagger`

### 2.2 Chạy UI Razor Pages

```bash
dotnet run --project ClinicBooking.Web
```

Sau đó mở trình duyệt ở địa chỉ mà app in ra trong terminal, thường là:
- `https://localhost:<port>`

### 2.3 Nếu bị lỗi connection string

Tài liệu dự án có nhắc rất rõ là Web và API phải trỏ đúng cùng một database local.

Nếu cần override tạm bằng PowerShell:

```powershell
$env:ConnectionStrings__DefaultConnection="Server=POTATO;Database=ClinicBooking;Trusted_Connection=True;TrustServerCertificate=True"
dotnet run --project ClinicBooking.Web
```

---

## 3) Những phần hiện có thể demo được

### 3.1 Trên Web UI

#### Bệnh nhân
- xem danh sách lịch hẹn của tôi
- xem chi tiết lịch hẹn
- huỷ lịch hẹn
- xem thứ tự hàng chờ

#### Lễ tân
- xem dashboard tổng quan
- xem lịch hẹn theo ngày
- xem hàng chờ theo ca
- xác nhận lịch hẹn
- check-in bệnh nhân
- gọi bệnh nhân tiếp theo
- hoàn thành lượt khám

#### Bác sĩ
- xem hàng chờ của tôi
- xem lịch hẹn theo ngày của tôi
- gọi bệnh nhân tiếp theo
- hoàn thành lượt khám

### 3.2 Trên Swagger/API

Có thể demo các nhóm sau:
- Auth: đăng nhập / đăng ký / đăng xuất / refresh token
- Lịch hẹn: xem chi tiết, danh sách của tôi, theo ngày
- Hàng chờ: xem theo ca, gọi tiếp, hoàn thành
- Check-in / huỷ lịch / đổi lịch / giữ chỗ

---

## 4) Seed và tài khoản dùng để đăng nhập / thao tác

### 4.1 Bộ tài khoản demo hiện tại

Hiện tại cấu hình `Development` đã bật dev fixture để seed bộ tài khoản demo sau:

| Vai trò | Username | Email | Ghi chú |
|---|---|---|---|
| Bệnh nhân | `patient001` | `patient@test.vn` | Dùng để test danh sách lịch hẹn, chi tiết lịch hẹn, thứ tự hàng chờ |
| Bác sĩ | `doctor001` | `doctor@test.vn` | Dùng để test màn bác sĩ |
| Lễ tân | `receptionist001` | `receptionist@test.vn` | Dùng để test màn lễ tân |
| Admin | `admin001` | `admin@test.vn` | Dùng để test màn quản trị |

### 4.2 Mật khẩu đăng nhập

Mật khẩu chung đang dùng cho dev fixture là:

- `Demo@123456`

Tài khoản admin gốc cũng vẫn có cơ chế fix mật khẩu từ config:
- `admin` / `Admin@123456`

### 4.3 Dữ liệu demo đáng chú ý của Module 1

Seed hiện có sẵn để demo:
- ca làm việc test
- lịch hẹn test
- hàng chờ test
- lịch sử trạng thái lịch hẹn test

Điều này đủ để demo luồng:
1. đăng nhập bệnh nhân
2. xem danh sách lịch hẹn
3. vào chi tiết lịch hẹn
4. xem thứ tự hàng chờ
5. chuyển sang lễ tân hoặc bác sĩ để xử lý tiếp

### 4.4 Ghi chú khi demo lần đầu

Nếu vừa chạy lại app mà chưa thấy dữ liệu mới, cần:
- kiểm tra app đã khởi động bằng `ClinicBooking.Api`
- chắc chắn đang dùng môi trường Development
- kiểm tra kết nối đến đúng database local
- nếu cần, restart API để seed chạy lại theo startup

---

## 5) Các màn có thể chạy demo ngay trên UI

### 5.1 Bệnh nhân
- `BenhNhan/DanhSachLichHen`
- `BenhNhan/LichHen/{idLichHen}`
- `BenhNhan/ThuTuHangCho/{idCaLamViec}`

### 5.2 Lễ tân
- `LeTan/Dashboard`
- `LeTan/QuanLyLichHen`
- `LeTan/HangCho`

### 5.3 Bác sĩ
- `BacSi/HangCho`

---

## 6) Kịch bản demo ngắn nên nói trước giảng viên

### Kịch bản 1 — Bệnh nhân
1. Đăng nhập bằng tài khoản bệnh nhân seed.
2. Mở trang lịch khám của tôi.
3. Chọn một lịch hẹn.
4. Vào xem chi tiết lịch hẹn.
5. Mở màn thứ tự hàng chờ để xem số thứ tự của mình.

### Kịch bản 2 — Lễ tân
1. Đăng nhập bằng tài khoản lễ tân seed.
2. Mở dashboard.
3. Xem lịch hẹn theo ngày.
4. Xác nhận một lịch hẹn.
5. Check-in bệnh nhân.
6. Mở hàng chờ và gọi bệnh nhân tiếp theo.

### Kịch bản 3 — Bác sĩ
1. Đăng nhập bằng tài khoản bác sĩ seed.
2. Mở hàng chờ của tôi.
3. Xem danh sách chờ.
4. Gọi bệnh nhân tiếp theo.
5. Hoàn thành lượt khám.

---

## 7) Thuyết trình ngắn về công việc từng module

### Module 1 — Đặt lịch hẹn & Hàng chờ
Đây là phần trung tâm của hệ thống.

Nhóm này phụ trách:
- tạo lịch hẹn
- huỷ / đổi lịch
- giữ chỗ
- check-in
- hàng chờ
- gọi bệnh nhân tiếp theo
- hoàn thành lượt khám

Hiện tại Module 1 đã có:
- backend handler chính cho các luồng lõi
- UI Razor Pages cho bệnh nhân, lễ tân, bác sĩ ở mức demo được
- tài liệu log tiến độ riêng

### Module 2 — Bác sĩ, Lịch làm việc & Danh mục
Module này phụ trách phần vận hành lịch làm việc và danh mục nền.

Nhóm này sẽ làm:
- bác sĩ
- ca làm việc
- chuyên khoa
- dịch vụ
- phòng
- đơn nghỉ phép
- duyệt ca

Nói ngắn với giảng viên:
> “Module 2 là nền vận hành lịch và danh mục, để Module 1 có dữ liệu slot và lịch làm việc chính xác.”

### Module 3 — Bệnh nhân, Hồ sơ khám & Kê đơn
Module này phụ trách dữ liệu chuyên môn của bệnh nhân.

Nhóm này sẽ làm:
- hồ sơ bệnh nhân
- hồ sơ khám
- toa thuốc
- danh mục thuốc

Nói ngắn với giảng viên:
> “Module 3 là phần chuyên môn y khoa, nơi bác sĩ ghi nhận khám và kê toa.”

### Module 4 — Thông báo, Admin & Vận hành
Module này phụ trách lớp nền vận hành và thông báo.

Nhóm này sẽ làm:
- thông báo in-app
- email
- OTP
- báo cáo admin
- background jobs
- hạ tầng vận hành

Nói ngắn với giảng viên:
> “Module 4 là tầng thông báo và vận hành, giúp hệ thống có tự động hóa và báo cáo.”

---

## 8) Điểm nhấn nên nói khi báo cáo

Nếu cần thuyết trình tự nhiên thay mặt bạn, có thể nhấn các ý sau:

- Nhóm đã chia module rõ ràng để tránh đụng nhau khi làm song song.
- Module 1 là phần đã đi được xa nhất về logic nghiệp vụ và demo UI.
- Dự án theo Clean Architecture, nên phần UI, Application, Infrastructure, Domain tách rành mạch.
- Design system UI đã được chuẩn hóa, giúp các màn không bị lệch phong cách.
- Có seed data demo sẵn để kiểm thử luồng người dùng thật.

---

## 9) Gợi ý nói với giảng viên khi hỏi “hiện tại chạy được gì?”

Bạn có thể trả lời ngắn như sau:

> “Hiện tại dự án đã chạy được phần xác thực, danh sách lịch hẹn, xem chi tiết lịch hẹn, thứ tự hàng chờ và các màn lễ tân/bác sĩ cơ bản. Nhóm em cũng đã chuẩn hóa UI theo design system và có seed data để demo ngay. Phần còn lại đang chia theo module để phát triển tiếp mà không đụng nhau.”

---

## 10) Ghi chú cuối

Tài liệu này được viết theo hướng có thể dùng trực tiếp trong buổi gặp giảng viên khi bạn vắng mặt.

Nếu cần rút gọn hơn nữa, chỉ cần giữ 3 ý:
1. Dự án là hệ thống đặt lịch khám theo 4 module.
2. Module 1 đã có backend + UI demo được.
3. Có seed data và luồng demo rõ ràng cho bệnh nhân, lễ tân, bác sĩ.
