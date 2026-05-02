# Tiến độ Module 1 — Nhật ký thực thi

## Mốc vừa hoàn tất

- Thêm endpoint cho bệnh nhân xem thứ tự của mình trong hàng chờ: `GET /api/hang-cho/thu-tu-cua-toi/{idCaLamViec}`.
- Thêm endpoint cho bác sĩ xem lịch hẹn theo ngày của chính mình: `GET /api/lich-hen/theo-ngay/cua-toi?ngay=yyyy-MM-dd`.
- Bổ sung validator, handler, DTO/mapping và unit test cho các luồng mới.
- Commit và push code Module 1 lên remote.
- Mở PR vào `develop` và chỉnh base đúng về `develop`.
- Merge PR thủ công trên GitHub.
- Đồng bộ local với `origin/develop` để giữ nhánh chính an toàn.
- Tách các thay đổi docs/config còn treo sang nhánh riêng `docs/module1-notes-wip`.
- Chuẩn hóa lại log Module 1 để phản ánh đúng trạng thái “code đã xong, docs còn tiếp tục”.

## Mốc UI vừa thực hiện

- Chuẩn hóa phần lọc của `BenhNhan/DanhSachLichHen` theo design system.
- Tạo trang `BenhNhan/LichHen` để xem chi tiết lịch hẹn.
- Tạo trang `BenhNhan/ThuTuHangCho` để xem thứ tự của mình trong hàng chờ.
- Bổ sung các helper layout CSS dùng chung cho UI Module 1 (`section-stack`, `filter-bar`, `filter-actions`, `action-bar`, `panel-grid-2`, `panel-grid-3`).
- Thêm điều hướng sidebar cho mục “Thứ tự hàng chờ”.

## Ý nghĩa của mốc

Mốc này khép lại phần code trực tiếp của Module 1 đã làm gần nhất và mở thêm lớp UI đầu tiên cho bệnh nhân. Từ đây trở đi, phần còn lại sẽ tiếp tục theo hướng chuẩn hóa giao diện, nâng cấp các màn lễ tân/bác sĩ và mở rộng các flow thao tác, nhưng vẫn không ảnh hưởng tới `develop`.

## Trạng thái nhánh

- `develop`: an toàn, đã đồng bộ với remote.
- `docs/module1-notes-wip`: giữ phần ghi chú/tài liệu cần xử lý tiếp.
- `feature/module1/phan-a-quyen-truy-cap`: nhánh feature cũ đã dùng để commit/push/PR trước đó.

## Hướng tiếp theo

- Tiếp tục chuẩn hóa tài liệu Module 1 trên nhánh docs riêng.
- Nếu có code Module 1 mới, chốt theo từng mốc rõ ràng trước khi commit/push/PR.
- Không đụng trực tiếp `develop` cho tới khi có thay đổi code thực sự cần merge.
- Không chỉnh `.mcp.json` và `.opencode.json` nữa; giữ nguyên như cấu hình plugin.
