# Kế hoạch: Liên kết tài khoản cho bệnh nhân vãng lai

> **Ngày lập:** 2026-05-09 | **Cập nhật:** 2026-05-13  
> **Branch:** chưa xác định — tách từ `develop` khi triển khai  
> **Trạng thái:** Kế hoạch — chưa code (chờ Module 4 merge + SMS provider)  
> **Người thực hiện:** Module 1

---

## 1. Bối cảnh & Vấn đề

### Vấn đề

Khi lễ tân đặt lịch hộ khách vãng lai (mới), hệ thống tự sinh một **tài khoản ghost**:
- `TenDangNhap = walkin_<timestamp><random>`
- `Email = walkin_<timestamp><random>@local.invalid`
- `TrangThai = false` (không thể đăng nhập)
- Mật khẩu là hash ngẫu nhiên, không ai biết

Bệnh nhân sau đó **không thể tự quản lý lịch hẹn online** vì không có credentials thực.  
Nếu họ cố đăng ký tài khoản mới với SĐT cũ → `ConflictException` (SĐT đã tồn tại trong ghost account).

### Mục tiêu

Cho phép bệnh nhân vãng lai **claim hồ sơ đã có** bằng cách:
1. Xác minh quyền sở hữu số điện thoại (OTP)
2. Xác minh danh tính (CCCD hoặc Ngày sinh + Họ tên)
3. Nâng cấp ghost account thành tài khoản thực — **in-place, giữ nguyên `IdTaiKhoan` và `IdBenhNhan`**

---

## 2. Quyết định kiến trúc

### In-place promotion (không tạo account mới)

**Lý do:** `IdBenhNhan` được tham chiếu bởi nhiều bảng:
- `LichHen.IdBenhNhan`
- `HoSoKham.IdBenhNhan`
- `ToaThuoc` (qua HoSoKham)
- `ThongBao.IdNguoiNhan`
- `RefreshToken.IdTaiKhoan`

Tạo tài khoản mới + merge FK là nguy cơ mất dữ liệu lịch sử. **In-place promotion** giữ toàn bộ FK graph nguyên vẹn.

### Tách use case riêng

`DangKyHandler` throw `ConflictException` khi SDT trùng — đây là **hành vi đúng**. Flow liên kết là ý định khác → tách thành `KichHoatTaiKhoanWalkInCommand` riêng.  
`DangKyHandler` **không được sửa** — chỉ ở tầng UI (trang đăng ký) ta redirect sang link flow khi phát hiện ghost account.

---

## 3. Thiết kế luồng

### Entry point 1 — Từ trang Đăng ký

```
Người dùng nhập SDT tại trang Đăng ký
  ↓
Kiểm tra TaiKhoan theo SDT
  ├─ Không tồn tại → flow đăng ký bình thường
  └─ Tồn tại VÀ là ghost (TrangThai=false, TenDangNhap LIKE 'walkin_%')
       → Redirect sang trang "Liên kết hồ sơ" với SDT đã điền sẵn
       (không throw lỗi, không tiết lộ thông tin tài khoản)
```

### Entry point 2 — Từ trang Đăng nhập

```
Link "Đã từng khám tại phòng khám? Liên kết hồ sơ →"
  → Trang /Auth/LienKetHoSo
```

### Luồng liên kết hoàn chỉnh

```
[Bước 1 - Nhập SĐT]
  Người dùng nhập SĐT
    → Hệ thống kiểm tra ghost account tồn tại không (không trả lỗi chi tiết)
    → Hiển thị form OTP nếu tìm thấy ghost account

[Bước 2 - Xác minh OTP]
  Gửi OTP đến SĐT (SMS)
    → Người dùng nhập OTP
    → OTP hợp lệ → tiếp tục bước 3
    → OTP sai/hết hạn → báo lỗi

[Bước 3 - Xác minh danh tính]
  Nhập một trong hai:
    - CCCD (nếu bệnh nhân cung cấp khi đặt lịch tại quầy)
    - Ngày sinh + Họ tên (phương án dự phòng)
  → Match với BenhNhan record → xác nhận danh tính

[Bước 4 - Đặt credentials]
  Nhập TenDangNhap (mới, không trùng), Email thực, MatKhau
    → Validate uniqueness
    → Update TaiKhoan in-place:
        TenDangNhap = <input>
        Email = <input>
        MatKhau = hash(<input>)
        TrangThai = true
        NgayKichHoat = now
    → Tạo RefreshToken mới
    → Trả về XacThucResponse (auto-login)
```

---

## 4. Thay đổi cần thực hiện

### 4.1 Domain / Migration

| Thay đổi | Mô tả |
|---|---|
| `TaiKhoan.NgayKichHoat` (nullable DateTime) | Audit — ghi lại khi nào ghost được kích hoạt. **Có thể defer** sang sau. |

### 4.2 Application Layer

| Feature | Files | Ghi chú |
|---|---|---|
| `KiemTraGhostTheoSdt` query | `Query.cs`, `Handler.cs` | Trả `bool IsGhost` — dùng ở trang đăng ký để redirect |
| `KichHoatTaiKhoanWalkIn` command | `Command.cs`, `Validator.cs`, `Handler.cs`, `KetQua.cs` | Core logic: OTP verify + identity check + in-place update |

**`KichHoatTaiKhoanWalkInHandler` logic:**
```
1. Load TaiKhoan by SoDienThoai — throw NotFoundException nếu không có
2. Guard: TrangThai phải false, TenDangNhap phải match 'walkin_%' — throw nếu không phải ghost
3. Xác minh OTP đã dùng trước đó (token từ bước 2 của UI)
4. Verify danh tính: BenhNhan.Cccd == input.Cccd
         HOẶC (BenhNhan.NgaySinh == input.NgaySinh && normalize(BenhNhan.HoTen) == normalize(input.HoTen))
5. Check TenDangNhap mới không trùng với non-ghost accounts
6. Check Email mới không trùng với non-ghost accounts
7. Update TaiKhoan in-place (TenDangNhap, Email, MatKhau, TrangThai=true, NgayKichHoat=now)
8. Tạo RefreshToken mới
9. SaveChanges — catch DbUpdateException (unique constraint) → throw ConflictException
10. Trả XacThucResponse (auto-login)
```

### 4.3 Web Layer

| Page | Route | Mô tả |
|---|---|---|
| `LienKetHoSo.cshtml` | `/Auth/LienKetHoSo` | Trang 4 bước, dùng session/TempData để lưu trạng thái xác minh |
| Sửa `DangKy.cshtml.cs` | — | Sau khi submit: nếu SDT là ghost → redirect `/Auth/LienKetHoSo?sdt=xxx` thay vì throw lỗi 409 |
| Sửa `DangNhap.cshtml` | — | Thêm link "Liên kết hồ sơ phòng khám" |

---

## 5. Phụ thuộc & Rủi ro

### Phụ thuộc

| Thứ | Trạng thái | Ghi chú |
|---|---|---|
| SMS OTP provider | ❌ Chưa có — hiện chỉ có `OtpServiceStub` (email) | **Blocker chính.** OTP phải gửi qua SMS vì ghost account không có email thực. Có thể tạm chấp nhận email-OTP với caveat bảo mật nếu SMS chưa có. |
| `OtpServiceStub` interface | ✅ Đã có | Cần SMS implementation |

### Rủi ro & Giới hạn

| Rủi ro | Xử lý |
|---|---|
| SĐT của bệnh nhân khác với SĐT lễ tân nhập vào | **Ngoài phạm vi** — cần admin tool "merge hai hồ sơ" riêng |
| Concurrent registration cùng SĐT trong lúc đang link | Catch `DbUpdateException` ở step 9, trả ConflictException |
| `BenhNhan.BiHanChe` bị ảnh hưởng | Không — flag này ở `BenhNhan`, không ở `TaiKhoan`, tự động giữ nguyên |
| OTP email bị chặn / không đến | Caveat bảo mật nếu dùng email tạm thay SMS — cần ghi rõ trong UI |

---

## 6. Ngoài phạm vi (Out of Scope)

- SMS provider thực (cần mua/tích hợp Twilio, Stringee, ESMS…)
- Admin UI "merge hai hồ sơ BenhNhan"
- Trường hợp SĐT bệnh nhân thay đổi sau khi đặt lịch tại quầy
- Đổi SĐT / Email sau khi đã kích hoạt

---

## 7. Độ ưu tiên & Gợi ý thời điểm

Tính năng này **không block** bất kỳ module nào hiện tại. Nên làm sau khi:
1. SMS provider được tích hợp (Module 1 background jobs wave)
2. Các module 2/3/4 đã merge ổn định vào develop

**Blast radius khi triển khai:** Thấp — 1 command mới + 1 page mới + có thể 1 migration nhỏ. Không sửa `DangKyHandler` hay `DangNhapHandler`.

---

## 8. Tiến độ phối hợp các module (cập nhật 2026-05-13)

### Trạng thái merge vào develop

| Thành viên | Module | Commit mới nhất | Ngày | Trạng thái |
|---|---|---|---|---|
| Potatomine725 | 1 — Lễ tân / Bác sĩ portal | `feat(module1): add LeTan/HoSoBenhNhan page + fix sidebar nav` | 2026-05-13 | ✅ Merged PR #43 |
| Luy2011 | 2 — Lịch & scheduling bác sĩ | `feat(module2): finish admin and doctor frontend flows` | 2026-05-10 | ✅ Merged PR #41 |
| vominhhoang-2218 | 3 — Bệnh nhân portal | `feat(module3): Thêm trang Bệnh nhân xem đơn thuốc / lịch sử khám` | 2026-05-11 | ✅ Merged PR #42 |
| NGuyenVAn21 | 4 — Admin UI | `feat(admin): fix giao diện admin UI module 4` | 2026-05-08 | ⏳ Chưa merge — branch `feature/module4/admin-ui-fixes` |

### Điều kiện để triển khai walk-in linking

| Điều kiện | Trạng thái |
|---|---|
| Module 2 merge ổn định vào develop | ✅ Xong (PR #41, 2026-05-10) |
| Module 3 merge ổn định vào develop | ✅ Xong (PR #42, 2026-05-11) |
| Module 4 merge ổn định vào develop | ⏳ Chờ NGuyenVAn21 — `feature/module4/admin-ui-fixes` chưa có PR |
| SMS provider tích hợp | ❌ Chưa có — `OtpServiceStub` vẫn là email. Blocker chính. |

### Gợi ý hành động

- **NGuyenVAn21**: Mở PR từ `feature/module4/admin-ui-fixes` → `develop` để unblock điều kiện cuối.
- **Module 1**: Có thể bắt đầu skeleton `KichHoatTaiKhoanWalkIn` (Application layer) ngay — không phụ thuộc Module 4.
- **SMS provider**: Cân nhắc tích hợp Stringee hoặc ESMS trước khi hoàn chỉnh flow OTP. Tạm thời dùng email-OTP với caveat bảo mật nếu cần demo sớm.
