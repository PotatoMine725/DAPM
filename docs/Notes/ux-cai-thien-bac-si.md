# Ghi chú cải thiện UX — Trang Bác sĩ

> Ghi nhận sau demo 2026-05-08. Defer sang sprint sau.

## Vấn đề hiện tại

- Chỉ có 2 trang: `HangCho` + `QuanLyKham` — thiếu ngữ cảnh
- `QuanLyKham` yêu cầu nhập `IdHoSoKham` thủ công để xem — không thân thiện
- Sau tạo HoSoKham + kê toa, bác sĩ phải quay thủ công về HangCho để bấm "Hoàn thành"

## Gợi ý cải thiện

| Hạng mục | Mô tả | Ưu tiên |
|---|---|---|
| `BacSi/LichSuKham` | Danh sách HoSoKham của bác sĩ, filter theo ngày | Cao |
| Inline flow | Click DangKham ở HangCho → expand/modal tạo HoSoKham + kê toa ngay tại chỗ | Cao |
| `BacSi/HoSoBenhNhan` | Xem toàn bộ lịch sử khám 1 bệnh nhân | Trung |
| Link "Hoàn thành" trong QuanLyKham | Sau kê toa, nút Hoàn thành gọi thẳng `HoanThanhLuotKham` khỏi cần quay HangCho | Cao |
