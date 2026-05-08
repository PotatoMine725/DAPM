# Kế hoạch hoàn thiện Module 1 — Sau merge feature/module3

> Ngày: 2026-05-08 | Branch: `feature/module1/portal-sat-demo`  
> Supersedes: phần "4. Kế hoạch tiếp theo" trong `ke-hoach-demo-nghiep-vu-web-ui.md`

---

## 1. Trạng thái sau merge

| Hạng mục | Trạng thái |
|---|---|
| Backend Module 1 (Wave 1–3 + Phan E + background jobs) | ✅ Xong |
| Web UI 4 flow cơ bản (BenhNhan / LeTan / BacSi/HangCho) | ✅ Xong |
| Tests (252: 241 unit + 11 integration) | ✅ Pass |
| `BacSi/QuanLyKham` (kê toa + tạo HoSoKham) | ✅ Merge từ module3 |
| `Admin/QuanLyThuoc` | ✅ Merge từ module3 |
| LeTan/QuanLyLichHen — paging + filter | 🔄 WIP, chưa commit |

Module 3 merge mở ra **loop demo đầy đủ**:  
`Đặt lịch → Xác nhận (lễ tân) → Check-in (lễ tân) → Bác sĩ khám + kê toa → Hoàn thành`

---

## 2. Việc còn lại theo thứ tự phụ thuộc

### Task 0 — Commit WIP LeTan/QuanLyLichHen (làm trước mọi thứ)

Các file đang treo cần commit vào nhánh hiện tại:

| File | Ghi chú |
|---|---|
| `Application/Common/PhanTrangKetQua.cs` | Pagination primitive mới |
| `Application/Features/LichHen/Queries/DanhSachTatCaLichHen/` | Handler + Query cho lễ tân |
| `Web/Pages/LeTan/QuanLyLichHen.cshtml` | UI paging + filter |
| `Web/Pages/LeTan/QuanLyLichHen.cshtml.cs` | PageModel paging |
| `docs/Requests/yeu-cau-module-2-3-4-hoan-thien-he-thong.md` | Tài liệu yêu cầu |

**Không commit:** `.agents/`, `AGENTS.md`, `CLAUDE.md`, `.github/`, `.rtk/` (env / tool config, không phải code sản phẩm).

**Commit message đề xuất:**
```
feat(le-tan): them phan trang va bo loc cho QuanLyLichHen
```

---

### Task A — Verify BacSi/QuanLyKham sau merge

`Pages/BacSi/QuanLyKham.cshtml` đến từ module3, cần kiểm tra tích hợp với seed data của module1.

Checklist:
- [ ] Page load đúng, hiện danh sách HangCho đang `DangKham` của ca hôm nay
- [ ] Tạo HoSoKham từ item HangCho
- [ ] Kê ToaThuoc (chọn thuốc từ danh sách)
- [ ] Hoàn thành khám → `HangCho.TrangThai = HoanThanh`

Nếu có lỗi mapping hoặc namespace: sửa tại chỗ, commit riêng.

---

### Task B — Flow demo mở rộng (Flow 5: Khám bệnh & kê toa)

Bổ sung vào kịch bản kiểm tra của `ke-hoach-demo-nghiep-vu-web-ui.md §5`:

```
Flow 5 — Bác sĩ khám bệnh + kê toa (sau Flow 2e)

Điều kiện: có ít nhất 1 HangCho ở trạng thái DangKham (sau Flow 2d)

1. [doctor001] /BacSi/QuanLyKham → chọn Ca #3004 (sáng hôm nay)
2. Bấm "Tạo hồ sơ khám" cho bệnh nhân DangKham
3. Điền ChanDoan, GhiChu
4. Thêm ToaThuoc: chọn thuốc từ danh sách, nhập liều dùng
5. Lưu hồ sơ khám
6. Bấm "Hoàn thành khám"

✅ Mong đợi:
   - HoSoKham mới tạo trong DB
   - ToaThuoc gắn với HoSoKham
   - HangCho.TrangThai = HoanThanh
   - (Optional) ThongBao in-app ghi vào DB (stub đã ghi)
```

---

### Task C — Verify toàn bộ 5 flows

Checklist đầy đủ (bổ sung từ `ke-hoach-demo-nghiep-vu-web-ui.md §5`):

```
[ ] Flow 1 — Đặt lịch thành công, TrangThai=ChoXacNhan
[ ] Flow 2a — Lễ tân xác nhận → DaXacNhan
[ ] Flow 2b — Lễ tân check-in → HangCho.ChoKham (LichHen.TrangThai vẫn DaXacNhan)
[ ] Flow 2c — Bệnh nhân thấy số thứ tự trên ThuTuHangCho
[ ] Flow 2d — Bác sĩ gọi kế tiếp → DangKham
[ ] Flow 2e — Bác sĩ hoàn thành → HoanThanh
[ ] Flow 3  — Đổi lịch, CaLamViec mới, chain LichSuLichHen đúng
[ ] Flow 4  — Hủy, HuyBenhNhan, SoLanHuyMuon không đổi (hủy >24h)
[ ] Flow 5  — BacSi/QuanLyKham: tạo HoSoKham + kê toa + hoàn thành khám
```

---

### Task D — Push branch + tạo PR

Sau khi tất cả flows xanh:

```bash
rtk git push origin feature/module1/portal-sat-demo
# Tạo PR vào develop
```

---

## 3. Out of scope (defer)

| Hạng mục | Lý do |
|---|---|
| Email OTP thật (MailKit / Mailhog) | Giai đoạn 2 — sau khi flows xanh |
| Docker Compose | Giai đoạn wrap-up |
| Wave 4 / Hangfire | Chờ Module 4 owner |
| NotificationService thật | Chờ Module 4 owner |
| Admin dashboard đầy đủ | Out of scope module 1 |

---

## 4. Phụ thuộc cần lưu ý

- `BenhNhan.SoLanHuyMuon` được ghi trong `HuyLichHenHandler` — notify Module 3 owner trước khi merge main.
- `NotificationServiceStub` đã ghi `ThongBao` vào DB (commit `d5edb09`) — Module 4 có thể đọc được ngay.
- Seed data: khởi động app để `[DevFixture]` refresh CaLamViec dates — bắt buộc trước mỗi demo session.
