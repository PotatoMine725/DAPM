# Kế hoạch cải thiện UI/UX - Phía Bệnh Nhân

**Ngày tạo:** 2026-05-09  
**Phạm vi:** Portal bệnh nhân — trang Lịch Hẹn và Đặt Lịch Hẹn

---

## 1. Lọc trạng thái lịch hẹn bằng Tab (thay thế Dropdown + Nút Lọc)

**Hiện trạng:** Người dùng phải chọn trạng thái từ dropdown rồi bấm nút "Lọc" mới lọc được danh sách.

**Yêu cầu:**
- Thay thế bộ lọc dropdown + nút bằng các **tab ngang** (ví dụ: Tất cả | Chờ xác nhận | Đã xác nhận | Đã khám | Đã hủy).
- Click tab là lọc ngay, không cần thao tác thêm.
- Tab đang active hiển thị nổi bật (màu nền / gạch chân).
- Giữ nguyên logic lọc phía backend — chỉ thay đổi phần giao diện gửi tham số lọc.

---

## 2. Hiển thị giờ khám theo giờ bệnh nhân muốn đến (thay vì ca làm việc hệ thống)

**Hiện trạng:** Chi tiết lịch hẹn hiện khung giờ theo ca làm việc của hệ thống (ví dụ: Ca sáng 7:00–11:30).

**Yêu cầu:**
- Hiển thị **giờ bệnh nhân dự kiến đến khám** được tính từ thời điểm đặt lịch.
- Biên độ giờ ước tính: **+30 phút đến +1 giờ** tùy tình trạng nghiệp vụ (hàng chờ, số thứ tự, v.v.).
- Ví dụ hiển thị: *"Dự kiến khám: 09:00 – 09:30"* hoặc *"Dự kiến khám: khoảng 09:00"*.
- Không bỏ thông tin ca làm việc — có thể hiển thị phụ (chữ nhỏ hơn) để tham khảo.

---

## 3. Chuẩn hóa tiếng Việt có dấu cho các trường dữ liệu

**Hiện trạng:** Một số nhãn và giá trị trong trang chi tiết lịch hẹn và trang đặt lịch hẹn hiển thị không dấu hoặc viết dính liền.

**Yêu cầu:**
- Rà soát toàn bộ nhãn (label), placeholder, thông báo lỗi, tiêu đề trường trong:
  - Trang chi tiết lịch hẹn
  - Trang đặt lịch hẹn
- Thay thế tất cả văn bản không dấu / viết dính thành tiếng Việt có dấu chuẩn.
- Ví dụ cần sửa:

| Hiện tại | Chuẩn hóa |
|---|---|
| `Benhnh Nhan` | `Bệnh Nhân` |
| `Ngaykham` | `Ngày Khám` |
| `Trangthai` | `Trạng Thái` |
| `Chuyenkhoa` | `Chuyên Khoa` |
| `Bacsi` | `Bác Sĩ` |

---

## 4. Quy tắc bổ sung JS cải thiện UI

**Phạm vi:** Cho phép dùng JavaScript để cải thiện trải nghiệm người dùng nhưng phải tuân thủ nghiêm ngặt các ràng buộc sau.

### Được phép
- Xử lý chuyển tab lọc lịch hẹn (mục 1) bằng JS thuần hoặc Alpine.js nếu dự án đã dùng.
- Hiệu ứng hiển thị / ẩn, animation nhẹ.
- Validate phía client (chỉ bổ sung, không thay thế validate phía server).

### Bắt buộc tránh
- **Không được chặn hoặc override sự kiện `submit` của form** theo cách làm hỏng luồng gửi dữ liệu lên server.
- **Không can thiệp vào các input ẩn** liên quan đến token, anti-forgery, hay các tham số backend.
- **Không dùng `event.preventDefault()` trên form submit** trừ khi logic async đã được kiểm tra kỹ và gọi lại submit thủ công.
- Không dùng thư viện JS nặng nếu dự án chưa có — ưu tiên JS thuần.

### Cấu trúc thư mục JS

Tất cả file JS viết mới phải đặt trong thư mục riêng, tách khỏi các file JS có sẵn:

```
ClinicBooking.Web/wwwroot/
└── js/
    └── portal/           ← thư mục JS riêng cho portal bệnh nhân
        ├── lich-hen-tabs.js     (tab lọc trạng thái)
        └── [tên-chức-năng].js  (các file JS khác theo chức năng)
```

- Mỗi file JS chỉ chịu trách nhiệm một chức năng.
- Đặt tên file theo kebab-case, mô tả rõ chức năng.
- Include file vào page tương ứng bằng `@section Scripts`, không include toàn cục.

---

## 5. Thứ tự ưu tiên thực hiện

| # | Hạng mục | Độ ưu tiên |
|---|---|---|
| 1 | Chuẩn hóa tiếng Việt có dấu | Cao — ảnh hưởng trực tiếp đến demo |
| 2 | Tab lọc trạng thái lịch hẹn | Cao — UX rõ ràng hơn |
| 3 | Giờ khám dự kiến theo bệnh nhân | Trung bình — cần xác nhận nghiệp vụ |
| 4 | Tách thư mục JS | Thực hiện song song khi viết JS mới |
