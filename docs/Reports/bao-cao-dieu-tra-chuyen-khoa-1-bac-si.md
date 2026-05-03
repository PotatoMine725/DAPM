# Báo cáo điều tra: trang Chuyên khoa chỉ hiển thị 1 bác sĩ

Ngày lập: 2026-04-29  
Mục tiêu: xác định vì sao portal bệnh nhân ở trang `Chuyên khoa` chỉ còn 1 bác sĩ công khai.

## 1) Kết luận sơ bộ

Nguyên nhân chính không phải do seed bị mất dữ liệu, mà là do query public bác sĩ đang lọc quá chặt sau khi đồng bộ nhánh `origin/Luy`.

## 2) Dấu hiệu phát hiện

Trong handler `DanhSachBacSiCongKhaiHandler` trước khi điều chỉnh, logic đã lọc:

- `TrangThai == DangLam`
- `ChuyenKhoa.HienThi == true`
- lọc thêm theo chuyên khoa / từ khóa / tham số public

Điều này khiến các bác sĩ không đúng trạng thái public bị loại ngay, nên chỉ còn một record hiển thị.

## 3) Đã điều chỉnh

Đã nới lại logic public để:

- vẫn ưu tiên chuyên khoa đang hiển thị
- nhưng không hard-drop quá sớm ở tầng query nếu dữ liệu chưa sync hoàn toàn
- giữ các bộ lọc theo chuyên khoa / trạng thái làm việc khi người dùng chọn lọc

## 4) Trạng thái seed

Từ seeder hiện tại, bác sĩ fixture sẽ:

- được upsert theo `DevFixture`
- gán `LoaiHopDong = NoiTru`
- gán `TrangThai = DangLam`
- gán chuyên khoa đầu tiên có trong hệ thống

Nghĩa là seed vẫn có thể tạo đủ nhiều bác sĩ, nhưng dữ liệu hiển thị public còn phụ thuộc vào bộ lọc và trạng thái chuyên khoa.

## 5) Hướng theo dõi tiếp

Nếu sau khi nới query mà vẫn chỉ có 1 bác sĩ, bước tiếp theo là:

- kiểm tra dữ liệu seed thực tế trong DB
- kiểm tra số chuyên khoa đang `HienThi`
- kiểm tra xem fixture bác sĩ có bị trùng tên / bị ghi đè không

## 6) Kết luận

Trang `Chuyên khoa` bị hụt dữ liệu chủ yếu là do filter public quá chặt sau đồng bộ Module 2. Đã có chỉnh đầu tiên ở handler public bác sĩ để tránh tình trạng chỉ còn 1 bản ghi hiển thị.
