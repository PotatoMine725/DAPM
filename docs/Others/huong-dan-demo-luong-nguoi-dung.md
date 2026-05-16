# Hướng dẫn Demo — Luồng Bệnh nhân, Lễ tân, Bác sĩ

> **Đối tượng:** Thành viên team chạy demo trước giảng viên hoặc bàn giao công việc.  
> **Branch:** `feature/module1/portal-sat-demo`  
> **Cập nhật:** 2026-05-08

---

## Mục lục nhanh

1. [Khởi động ứng dụng](#1-khởi-động-ứng-dụng)
2. [Tài khoản demo](#2-tài-khoản-demo)
3. [Dữ liệu seed tự động](#3-dữ-liệu-seed-tự-động)
4. [Luồng Bệnh nhân](#4-luồng-bệnh-nhân)
5. [Luồng Lễ tân](#5-luồng-lễ-tân)
6. [Luồng Bác sĩ](#6-luồng-bác-sĩ)
7. [Luồng Admin](#7-luồng-admin)
8. [Kịch bản demo liên tục (end-to-end)](#8-kịch-bản-demo-liên-tục-end-to-end)
9. [Lỗi thường gặp và cách xử lý](#9-lỗi-thường-gặp-và-cách-xử-lý)

---

## 1. Khởi động ứng dụng

### Yêu cầu

| Công cụ | Phiên bản |
|---|---|
| .NET SDK | 8.0+ |
| SQL Server | 2019+ (Express đủ dùng) |

### Chạy lần đầu

```powershell
# Từ thư mục gốc repo
dotnet build DatLichPhongKham.slnx
dotnet run --project ClinicBooking.Web
```

> **Seed tự động:** Khi app khởi động ở môi trường `Development`, seeder tự chạy và tạo đủ dữ liệu demo.  
> Kiểm tra terminal có log `[DevFixture]` — nếu thấy nghĩa là seed thành công.

### Nếu cần đổi connection string

```powershell
$env:ConnectionStrings__DefaultConnection = "Server=TEN_MAY_BAN;Database=ClinicBooking;Trusted_Connection=True;TrustServerCertificate=True"
dotnet run --project ClinicBooking.Web
```

> Thay `TEN_MAY_BAN` bằng tên SQL Server trên máy của bạn (ví dụ: `LAPTOP\SQLEXPRESS`).

---

## 2. Tài khoản demo

| Vai trò | Tên đăng nhập | Mật khẩu | Ghi chú |
|---|---|---|---|
| Bệnh nhân | `patient001` | `Demo@123456` | Dùng để đặt lịch, xem lịch, xem hàng chờ |
| Lễ tân | `receptionist001` | `Demo@123456` | Xác nhận, check-in, gọi bệnh nhân |
| Bác sĩ | `doctor001` | `Demo@123456` | Xem hàng chờ, gọi kế tiếp, hoàn thành khám |
| Admin | `admin` | `Admin@123456` | Quản trị hệ thống |

> **Lưu ý OTP:** OTP đang được bypass trong môi trường Development (`BatBuocChoDatLich=false`). Khi đặt lịch không cần nhập OTP — submit thẳng.

---

## 3. Dữ liệu seed tự động

Mỗi lần khởi động app, seeder tạo lại các ca làm việc và lịch hẹn sau:

### Ca làm việc (CaLamViec)

| ID | Giờ | Ngày | Ghi chú |
|---|---|---|---|
| 3004 | 07:00 – 12:00 | **Hôm nay** | Ca sáng hôm nay — có 3 lịch demo sẵn |
| 3005 | 13:00 – 17:00 | **Hôm nay** | Ca chiều hôm nay |
| 3001 | 07:00 – 12:00 | **Ngày mai** | Ca sáng ngày mai — có 1 lịch demo |
| 3002 | 13:00 – 17:00 | **Ngày mai** | Ca chiều ngày mai |
| 3003 | 07:00 – 12:00 | **Hôm nay +7 ngày** | Ca xa — dùng để demo đổi lịch |

### Lịch hẹn demo (seed mỗi startup)

| Mã lịch hẹn | Ca | Trạng thái | Dùng để demo |
|---|---|---|---|
| `DEMO-{today}-01` | 3004 (sáng hôm nay) | `ChoXacNhan` | Lễ tân xác nhận |
| `DEMO-{today}-02` | 3004 (sáng hôm nay) | `DaXacNhan` | Lễ tân check-in |
| `DEMO-{today}-03` | 3004 (sáng hôm nay) | `DaXacNhan` + HangCho `ChoKham` | Bệnh nhân xem số thứ tự, bác sĩ gọi kế tiếp |
| `DEMO-{tomorrow}-01` | 3001 (sáng ngày mai) | `ChoXacNhan` | Demo flow ngày mai |

---

## 4. Luồng Bệnh nhân

### Trang bệnh nhân có thể demo

| Đường dẫn | Chức năng |
|---|---|
| `/BenhNhan/DatLich` | Đặt lịch hẹn mới |
| `/BenhNhan/DanhSachLichHen` | Xem danh sách lịch hẹn của tôi |
| `/BenhNhan/LichHen?id={id}` | Xem chi tiết một lịch hẹn |
| `/BenhNhan/DoiLich?id={id}` | Đổi lịch hẹn sang ca khác |
| `/BenhNhan/ThuTuHangCho` | Xem số thứ tự hàng chờ hiện tại |

---

### Flow B1 — Đặt lịch hẹn mới

**Mục đích:** Bệnh nhân tự đặt lịch qua portal.

```
1. Đăng nhập tài khoản patient001
2. Vào /BenhNhan/DatLich
3. Chọn chuyên khoa và dịch vụ
4. Chọn ngày (từ ngày mai trở đi)
5. Chọn giờ trong khung ca: 07:00–12:00 hoặc 13:00–17:00
6. Bấm Submit (không cần nhập OTP)
7. Kết quả mong đợi:
   - Chuyển về DanhSachLichHen
   - Lịch mới xuất hiện với trạng thái "Chờ xác nhận"
```

> ⚠️ **Lưu ý quan trọng:**
> - Ngày hôm nay đã có 3 slot demo — **chỉ đặt từ ngày mai trở đi**.
> - Giờ mong muốn phải nằm trong khung ca (07:00–12:00 hoặc 13:00–17:00).
> - Nếu chọn giờ ngoài khung ca sẽ báo lỗi "Không tìm thấy slot phù hợp" — đây là đúng nghiệp vụ.

---

### Flow B2 — Xem danh sách và chi tiết lịch hẹn

```
1. Đăng nhập patient001
2. Vào /BenhNhan/DanhSachLichHen
3. Thấy danh sách lịch hẹn: gồm các lịch DEMO và lịch mới vừa đặt
4. Bấm vào một lịch hẹn
5. Vào /BenhNhan/LichHen?id={id}
6. Kết quả mong đợi:
   - Hiển thị đầy đủ thông tin: mã lịch, ngày giờ, ca làm việc, trạng thái
   - Có nút "Đổi lịch" (nếu còn trong thời hạn)
   - Có nút "Hủy lịch" (nếu còn trong thời hạn — trước 24h)
```

---

### Flow B3 — Đổi lịch hẹn

**Điều kiện:** Lịch hẹn có trạng thái `ChoXacNhan` hoặc `DaXacNhan`, chưa đến giờ khám.

```
1. Đặt lịch mới (Flow B1) hoặc dùng DEMO-{today}-01 (ChoXacNhan)
2. Vào chi tiết lịch hẹn /BenhNhan/LichHen?id={id}
3. Bấm "Đổi lịch"
4. Vào /BenhNhan/DoiLich?id={id}
5. Chọn ngày và giờ khác (ví dụ: Ca 3003 — hôm nay +7 ngày)
6. Submit
7. Kết quả mong đợi:
   - Lịch cũ: TrangThai = HuyBenhNhan (vì đổi lịch tạo lịch mới và hủy lịch cũ)
   - Lịch mới: TrangThai = ChoXacNhan
   - Lịch sử lịch hẹn ghi nhận chuỗi thay đổi
```

---

### Flow B4 — Hủy lịch hẹn

**Điều kiện:** Hủy trước 24 giờ so với giờ khám → không bị tính lần hủy muộn.

```
1. Đặt lịch mới cho ngày mai (Flow B1)
2. Vào DanhSachLichHen
3. Bấm "Hủy" trên lịch vừa đặt
4. Nhập lý do hủy
5. Xác nhận
6. Kết quả mong đợi:
   - TrangThai = HuyBenhNhan
   - SoLanHuyMuon không tăng (vì hủy trước 24h)
   - Slot được trả lại cho ca làm việc
```

---

### Flow B5 — Xem số thứ tự hàng chờ

```
1. Đăng nhập patient001
2. Vào /BenhNhan/ThuTuHangCho
3. Kết quả mong đợi:
   - Thấy số thứ tự của DEMO-{today}-03 (đã check-in, đang trong hàng)
   - Hiển thị ca làm việc, phòng khám, thứ tự chờ
```

---

## 5. Luồng Lễ tân

### Trang lễ tân có thể demo

| Đường dẫn | Chức năng |
|---|---|
| `/LeTan/QuanLyLichHen` | Danh sách lịch hẹn theo ngày, xác nhận, check-in |
| `/LeTan/HangCho` | Quản lý hàng chờ, gọi bệnh nhân tiếp theo |

---

### Flow L1 — Xác nhận lịch hẹn

**Dùng:** `DEMO-{today}-01` đang ở trạng thái `ChoXacNhan`.

```
1. Đăng nhập receptionist001
2. Vào /LeTan/QuanLyLichHen
3. Tìm lịch hẹn DEMO-{today}-01 (trạng thái "Chờ xác nhận")
4. Bấm "Xác nhận"
5. Kết quả mong đợi:
   - TrangThai = DaXacNhan
   - Dòng lịch hẹn cập nhật trạng thái trên UI
   - Bệnh nhân nhận thông báo in-app "Lịch hẹn đã được xác nhận"
```

---

### Flow L2 — Check-in bệnh nhân

**Dùng:** `DEMO-{today}-02` đang ở trạng thái `DaXacNhan`.

```
1. Đăng nhập receptionist001
2. Vào /LeTan/QuanLyLichHen
3. Tìm lịch hẹn DEMO-{today}-02 (trạng thái "Đã xác nhận")
4. Bấm "Check-in"
5. Kết quả mong đợi:
   - Một bản ghi HangCho được tạo với TrangThaiHangCho = ChoKham
   - LichHen.TrangThai vẫn là DaXacNhan (đây là đúng — check-in không đổi TrangThai lịch hẹn)
   - Bệnh nhân nhận thông báo in-app "Check-in thành công, số thứ tự: X"
```

> **Lưu ý nghiệp vụ quan trọng:** Check-in chỉ tạo bản ghi hàng chờ, không đổi trạng thái lịch hẹn.

---

### Flow L3 — Xem và quản lý hàng chờ

```
1. Đăng nhập receptionist001
2. Vào /LeTan/HangCho
3. Chọn ca làm việc từ dropdown (Ca 3004 — sáng hôm nay)
4. Kết quả mong đợi:
   - Thấy danh sách bệnh nhân trong hàng chờ
   - DEMO-{today}-03 đang ở trạng thái "Chờ khám" (ChoKham)
   - Thứ tự hàng chờ hiển thị đúng
5. (Tùy chọn) Bấm "Gọi tiếp" để gọi bệnh nhân kế tiếp
```

---

### Flow L4 — Demo chuỗi đầy đủ (Lễ tân)

Chạy liền theo thứ tự để demo nghiệp vụ hoàn chỉnh của lễ tân:

```
Bước 1: Xác nhận DEMO-{today}-01
         → QuanLyLichHen → Xác nhận → TrangThai = DaXacNhan

Bước 2: Check-in DEMO-{today}-02
         → QuanLyLichHen → Check-in → HangCho được tạo (ChoKham)

Bước 3: Xem hàng chờ Ca 3004
         → HangCho → chọn Ca 3004 → thấy 2 bệnh nhân trong hàng
```

---

## 6. Luồng Bác sĩ

### Trang bác sĩ có thể demo

| Đường dẫn | Chức năng |
|---|---|
| `/BacSi/HangCho` | Xem hàng chờ theo ca, gọi bệnh nhân kế tiếp, hoàn thành khám |

---

### Flow D1 — Xem hàng chờ và gọi bệnh nhân

**Điều kiện:** Phải có ít nhất 1 bệnh nhân đã check-in (HangCho.TrangThaiHangCho = ChoKham).  
Seed sẵn: `DEMO-{today}-03` đã check-in.

```
1. Đăng nhập doctor001
2. Vào /BacSi/HangCho
3. Chọn ca làm việc từ dropdown (Ca 3004 — sáng hôm nay)
4. Thấy danh sách bệnh nhân chờ khám
5. Bấm "Gọi tiếp"
6. Kết quả mong đợi:
   - Bệnh nhân đầu tiên (số thứ tự nhỏ nhất) chuyển sang TrangThaiHangCho = DangKham
   - Row đó highlight khác màu trên UI
   - Bệnh nhân nhận thông báo in-app "Đến lượt khám của bạn"
```

---

### Flow D2 — Hoàn thành lượt khám

**Điều kiện:** Có bệnh nhân đang ở trạng thái `DangKham` (từ Flow D1).

```
1. Đăng nhập doctor001 (hoặc tiếp nối từ Flow D1)
2. Vào /BacSi/HangCho → chọn ca 3004
3. Thấy bệnh nhân đang "Đang khám" (DangKham)
4. Bấm "Hoàn thành"
5. Kết quả mong đợi:
   - TrangThaiHangCho = HoanThanh
   - LichHen.TrangThai = HoanThanh
   - Slot được xử lý, ca tiếp tục với bệnh nhân tiếp theo trong hàng
```

---

### Flow D3 — Demo chuỗi đầy đủ (Bác sĩ)

```
Bước 1: Chọn ca 3004 → thấy DEMO-{today}-03 trong hàng (ChoKham)
Bước 2: Bấm "Gọi tiếp" → DangKham, highlight
Bước 3: Bấm "Hoàn thành" → HoanThanh
Bước 4: (Nếu có bệnh nhân tiếp): Bấm "Gọi tiếp" lần nữa
```

---

## 7. Luồng Admin

### Trang admin có thể demo

| Đường dẫn | Chức năng |
|---|---|
| `/Admin/Dashboard` | Dashboard tổng quan (mock data) |
| `/Admin/Accounts` | Quản lý tài khoản người dùng |
| `/Admin/ChuyenKhoa` | Quản lý chuyên khoa |
| `/Admin/BacSi` | Quản lý bác sĩ |
| `/Admin/DichVu` | Quản lý dịch vụ |
| `/Admin/Phong` | Quản lý phòng khám |
| `/Admin/CaLamViec` | Lịch bác sĩ nội trú |
| `/Admin/DuyetCa` | Duyệt ca hợp đồng |
| `/Admin/ThongKe` | Thống kê bệnh nhân |
| `/Admin/ThongBao` | Thông báo hệ thống |

**Đăng nhập:** `admin` / `Admin@123456`

> **Lưu ý:** Admin dashboard hiện dùng mock data để demo giao diện. Data thật sẽ được tích hợp trong Module 4.

---

## 8. Kịch bản demo liên tục (end-to-end)

Đây là kịch bản đầy đủ từ đầu đến cuối, phù hợp để trình bày trước giảng viên hoặc stakeholder. Tổng thời gian: **~8–10 phút**.

---

### Giai đoạn 1 — Bệnh nhân đặt lịch (2 phút)

```
[patient001] /Auth/DangNhap → đăng nhập

[patient001] /BenhNhan/DatLich
  → Chọn ngày mai, giờ 09:00
  → Submit (không cần OTP)
  → ✅ DanhSachLichHen: lịch mới TrangThai=ChoXacNhan

Nói với giảng viên: "Bệnh nhân vừa đặt lịch qua portal. Hệ thống ghi nhận
và tự gửi thông báo in-app cho bệnh nhân."
```

---

### Giai đoạn 2 — Lễ tân xử lý lịch (3 phút)

```
[receptionist001] /Auth/DangNhap → đăng nhập

[receptionist001] /LeTan/QuanLyLichHen
  → Tìm DEMO-{today}-01 (ChoXacNhan)
  → Bấm "Xác nhận"
  → ✅ TrangThai = DaXacNhan

  → Tìm DEMO-{today}-02 (DaXacNhan)
  → Bấm "Check-in"
  → ✅ Tạo HangCho (ChoKham)

[receptionist001] /LeTan/HangCho
  → Chọn Ca 3004
  → ✅ Thấy danh sách hàng chờ

Nói với giảng viên: "Lễ tân xác nhận lịch hẹn, cho bệnh nhân check-in,
và theo dõi hàng chờ theo từng ca làm việc."
```

---

### Giai đoạn 3 — Bác sĩ khám bệnh (2 phút)

```
[doctor001] /Auth/DangNhap → đăng nhập

[doctor001] /BacSi/HangCho
  → Chọn Ca 3004
  → ✅ Thấy DEMO-{today}-03 đang ChoKham

  → Bấm "Gọi tiếp"
  → ✅ DangKham — row highlight

  → Bấm "Hoàn thành"
  → ✅ HoanThanh

Nói với giảng viên: "Bác sĩ gọi bệnh nhân theo thứ tự hàng chờ. Khi hoàn
thành, hệ thống tự chuyển sang bệnh nhân tiếp theo."
```

---

### Giai đoạn 4 — Bệnh nhân xem thứ tự (1 phút, song song với giai đoạn 2/3)

```
[patient001] /BenhNhan/ThuTuHangCho
  → ✅ Thấy số thứ tự của DEMO-{today}-03

Nói với giảng viên: "Bệnh nhân có thể theo dõi số thứ tự của mình
theo thời gian thực."
```

---

## 9. Lỗi thường gặp và cách xử lý

### Không thấy dữ liệu demo khi vào lần đầu

**Nguyên nhân:** Seeder chưa chạy hoặc kết nối database sai.  
**Xử lý:**
```powershell
# Kiểm tra terminal có log [DevFixture] không
# Nếu không, restart app:
dotnet run --project ClinicBooking.Web
```

---

### "Không tìm thấy slot phù hợp" khi đặt lịch hôm nay

**Nguyên nhân:** Slot của ngày hôm nay đã dùng cho lịch demo (SoSlotDaDat=3).  
**Xử lý:** Chọn ngày mai hoặc ca 3003 (hôm nay +7 ngày).

---

### Đăng xuất xong vẫn bị redirect về trang admin

**Nguyên nhân:** Đã được fix. Đảm bảo đang chạy code mới nhất từ branch `feature/module1/portal-sat-demo`.  
**Xử lý:** Restart browser hoặc xóa cookie thủ công.

---

### Dropdown ca làm việc trống

**Nguyên nhân:** CaLamViec hôm nay không có trong DB hoặc seeder chưa chạy.  
**Xử lý:** Restart app để seeder cập nhật ngày cho các ca.

---

### Avatar dropdown trên admin không mở

**Nguyên nhân:** Đã được fix trong commit mới nhất.  
**Xử lý:** Hard refresh trình duyệt (`Ctrl+Shift+R`) để load lại CSS mới.

---

## Ghi chú cuối

- **OTP bypass** chỉ bật ở môi trường Development. Không ảnh hưởng production.
- **Thông báo in-app** đang dùng stub — ghi vào DB nhưng không gửi email. Email sẽ do Module 4 implement.
- **Admin dashboard** dùng mock data — data thật sẽ được Module 4 kết nối.
- Nếu cần reset toàn bộ dữ liệu demo về trạng thái ban đầu: restart app.
