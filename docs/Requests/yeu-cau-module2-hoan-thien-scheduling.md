# Yêu cầu Module 2 — Hoàn thiện scheduling + nghỉ phép

**Ngày lập:** 2026-05-16
**Người gửi:** Module 1 owner (Potatomine725)
**Người nhận:** Owner Module 2 (Bác sĩ / Lịch làm việc / Danh mục)
**Nguồn:** `docs/Plans/ke-hoach-hoan-thien-he-thong-va-thoat-dev.md` §3.1
**Trạng thái Module 2:** Application layer scheduling + danh mục đã đầy đủ, admin pages đã wire xong (BacSi, CaLamViec, LichNoiTru, DuyetCa, Phong, ChuyenKhoa, DichVu).

---

## 1. Mục tiêu

Đóng 6 việc còn lại của Module 2 trước khi vào staging:

1. Verify ownership + hoàn thiện `CaLamViecQueryService` (đặc biệt `ChayReconSlotAsync`).
2. Hoàn thiện flow nghỉ phép (BS nộp → Admin duyệt → block ca trùng ngày).
3. Fix bug điều kiện trùng trong `SinhCaLamViecTuLichNoiTruHandler`.
4. Verify API CaLamViec public CRUD đủ.
5. **Validate xung đột lịch BS** trong `TaoCaLamViec` + `DuyetCaLamViec` + `SinhCaLamViecTuLichNoiTru` (BS double-book, phòng double-book, trùng đơn nghỉ phép đã duyệt).
6. **Cảnh báo "thiếu BS tại slot"** cho admin/lễ tân khi chuyên khoa không có ca trống trong khung giờ — không lặng lẽ chặn đặt lịch.

---

## 2. Việc cần làm

### 2.1. (🟠 Cao) Verify `CaLamViecQueryService` + impl `ChayReconSlotAsync`

**File:** `ClinicBooking.Infrastructure/Services/Scheduling/CaLamViecQueryService.cs`

Module 1 đã viết tạm impl này. Module 2 cần:

- Đọc kỹ + nhận quyền sở hữu (không đổi signature `ICaLamViecQueryService`).
- Verify `IncrementSoSlotDaDatAsync` thực sự dùng **atomic SQL** (`ExecuteUpdateAsync` hoặc raw SQL), không phải Read-Modify-Write. Nếu chưa, refactor để chống race condition khi nhiều BN đặt cùng ca.
- Impl `ChayReconSlotAsync(CancellationToken ct)` — đếm lại `SoSlotDaDat` từ bảng `LichHen` (status `DaXacNhan` + `DangKham` + `CheckedIn`) cho tất cả `CaLamViec` trong vòng 30 ngày tới, so với giá trị hiện tại, nếu lệch thì cập nhật + log warning. Trả về số ca đã reconcile.

**Test:**
- Unit: race condition giả lập 10 task `IncrementSoSlotDaDatAsync(+1)` song song trên cùng ca → tổng cuối phải = 10, không thấp hơn.
- Integration: tạo ca với `SoSlotToiDa=2`, insert 3 lịch `DaXacNhan` manual → chạy `ChayReconSlotAsync` → `SoSlotDaDat` được sửa thành 3 + log warning.

### 2.2. (🟠 Cao) Flow nghỉ phép — Admin duyệt

**Hiện trạng:** BS có thể nộp đơn (`Features/NghiPhep/Commands/NopDonNghiPhep/` + page `BacSi/NghiPhep`). **Chưa có** flow admin duyệt.

#### Backend mới

`Features/NghiPhep/Commands/DuyetDonNghiPhep/`:

```csharp
public sealed record DuyetDonNghiPhepCommand(
    int IdDonNghiPhep,
    bool ChapNhan,
    string? LyDoTuChoi) : IRequest<Unit>;
```

Handler:
- Lấy `DonNghiPhep` → throw `NotFoundException` nếu null.
- Set `TrangThai = DaDuyet` hoặc `TuChoi`, gán `IdAdminDuyet`, `NgayDuyet`.
- **Nếu duyệt:** query `CaLamViec` có `NgayLamViec` trong khoảng `TuNgay..DenNgay` của BS đó với `TrangThaiDuyet = DaDuyet`:
  - Nếu `SoSlotDaDat = 0` → xoá ca (hoặc set `DaHuy` + log).
  - Nếu `SoSlotDaDat > 0` → **không tự động cancel**, throw `ConflictException` với danh sách ca + số lịch, yêu cầu admin xử lý thủ công trước (huỷ lịch hẹn của BN). Tránh silent data loss.

`Features/NghiPhep/Queries/DanhSachDonNghiPhepChoDuyet/`:
- Filter `TrangThai = ChoDuyet`, sort `NgayNop desc`, join `BacSi` để lấy tên.

#### UI mới

`ClinicBooking.Web/Pages/Admin/NghiPhep.cshtml(.cs)` — copy pattern `Admin/DuyetCa`:
- KPI: số đơn chờ duyệt / số ngày nghỉ tháng này.
- Bảng danh sách đơn chờ → 2 nút Duyệt / Từ chối.
- Modal từ chối với `LyDoTuChoi` textarea required.
- Khi handler throw `ConflictException`: hiển thị `TempData["ErrorMessage"]` với danh sách ca trùng + hướng dẫn vào `Admin/CaLamViec` huỷ trước.

**Sidebar `_AdminLayout`:** thêm link `Admin/NghiPhep` cạnh `Admin/DuyetCa`.

### 2.3. (🟡 Trung) Fix bug `SinhCaLamViecTuLichNoiTruHandler`

**File:** `ClinicBooking.Application/Features/Scheduling/Commands/SinhCaLamViecTuLichNoiTru/...Handler.cs`

Điều kiện trùng hiện tại (theo `docs/Plans/m1-wire-up-admin.md` §"Vấn đề cần follow-up"):

```csharp
exists = await _db.CaLamViec.AnyAsync(c =>
    c.IdBacSi == idBacSi && c.NgayLamViec == ngay);
```

Sai: BS có 2 mẫu Sáng/Chiều cùng ngày sẽ chỉ sinh được 1 ca (mất ca thứ 2).

**Sửa:** thêm `IdDinhNghiaCa` vào điều kiện:

```csharp
exists = await _db.CaLamViec.AnyAsync(c =>
    c.IdBacSi == idBacSi
    && c.NgayLamViec == ngay
    && c.IdDinhNghiaCa == lichNoiTru.IdDinhNghiaCa);
```

**Test:** integration test — tạo 2 mẫu LichNoiTru cho cùng BS, cùng ngày trong tuần, ca Sáng + Chiều → `SinhCaLamViecTuLichNoiTru` → expect 2 record CaLamViec / ngày.

### 2.5. (🔴 Cao) Validate xung đột lịch BS / phòng / nghỉ phép

**Vấn đề hiện tại:** `TaoCaLamViecHandler` chỉ `Add()` rồi `SaveChangesAsync()`, **không có check nào**:
- BS đã có ca khác overlap thời gian (cùng `IdBacSi`, `NgayLamViec`, khoảng `[GioBatDau, GioKetThuc]` giao nhau).
- Phòng đã có ca khác overlap thời gian.
- BS có đơn nghỉ phép đã duyệt phủ ngày này.

Hậu quả: admin tạo 2 ca cho 1 BS cùng giờ → BN đặt được cả 2 → BS xung đột.

#### Helper service mới

`Application/Abstractions/Scheduling/ICaLamViecConflictChecker.cs`:

```csharp
public interface ICaLamViecConflictChecker
{
    /// <summary>
    /// Kiem tra ca moi co xung dot voi ca hien co cua BS, phong, hoac don nghi phep.
    /// Throw ConflictException kem chi tiet xung dot neu co.
    /// </summary>
    Task EnsureKhongXungDotAsync(
        int idBacSi,
        int idPhong,
        DateOnly ngayLamViec,
        TimeOnly gioBatDau,
        TimeOnly gioKetThuc,
        int? idCaLamViecBoQua,   // dùng cho update: bỏ qua chính ca này khi check
        CancellationToken ct);
}
```

Impl `Infrastructure/Services/Scheduling/CaLamViecConflictChecker.cs`:

- Query 3 lần:
  1. `CaLamViec.Any(c => c.IdBacSi == idBacSi && c.NgayLamViec == ngay && c.TrangThaiDuyet != DaHuy && c.IdCaLamViec != idBoQua && OVERLAP(c, gioBatDau, gioKetThuc))` — overlap: `c.GioBatDau < gioKetThuc && c.GioKetThuc > gioBatDau`.
  2. Same logic cho `IdPhong`.
  3. `DonNghiPhep.Any(d => d.IdBacSi == idBacSi && d.TrangThai == DaDuyet && d.TuNgay <= ngay && d.DenNgay >= ngay)`.
- Mỗi loại xung đột → message VN rõ: `"BS Nguyen Van A da co ca khac luc 08:00-12:00 ngay 2026-05-20."` / `"Phong P101 da duoc dat tu 08:00-12:00."` / `"BS dang nghi phep tu 2026-05-19 den 2026-05-25."`.
- Gom tất cả conflict thành 1 `ConflictException` (đừng throw sau check đầu).

#### Wire vào 3 handler

- `TaoCaLamViecHandler`: gọi `EnsureKhongXungDotAsync(... idCaLamViecBoQua: null)` trước khi `Add()`.
- `CapNhatCaLamViecHandler` (nếu có): gọi với `idCaLamViecBoQua: request.IdCaLamViec`.
- `SinhCaLamViecTuLichNoiTruHandler`: cho mỗi ngày sinh, gọi `EnsureKhongXungDotAsync(...)`. Nếu conflict: **skip + log warning**, không throw (vì sinh hàng loạt, 1 ngày fail không nên fail cả batch). Return `KetQuaSinhCa { SoCaSinh, SoCaBoQua, DanhSachXungDot }` để admin biết.
- `DuyetCaLamViecHandler`: trước khi set `DaDuyet`, re-check conflict (ca trong khoảng `ChoDuyet` có thể đã bị overlap với ca khác duyệt trong lúc chờ). Nếu conflict → throw, không duyệt.

#### Test

- Unit: 2 ca BS cùng giờ → thứ 2 throw `ConflictException` với message chứa `BS`.
- Unit: ca + đơn nghỉ phép duyệt cùng ngày → throw với message `nghi phep`.
- Integration: sinh ca từ template với 1 ngày trùng đơn nghỉ phép → `SoCaSinh = 6, SoCaBoQua = 1`, log warning.

### 2.6. (🟠 Cao) Cảnh báo "thiếu BS tại slot"

**Vấn đề:** Khi BN chọn chuyên khoa + ngày + slot → backend trả "không có ca trống" chung chung. Không phân biệt:
- Có ca nhưng full slot.
- Không có ca BS nào của chuyên khoa này.
- Có ca BS nhưng tất cả `ChoDuyet`.

Cũng không cảnh báo admin/lễ tân trước khi BN đặt.

#### Backend mới

`Features/Scheduling/Queries/KiemTraDoPhuBacSi/`:

```csharp
public sealed record KiemTraDoPhuBacSiQuery(
    int IdChuyenKhoa,
    DateOnly TuNgay,
    DateOnly DenNgay) : IRequest<KiemTraDoPhuBacSiResponse>;

public sealed record KiemTraDoPhuBacSiResponse(
    IReadOnlyList<NgayThieuBacSiDto> NgayThieu); // ngày không có ca DaDuyet nào

public sealed record NgayThieuBacSiDto(
    DateOnly Ngay,
    int SoCaChoDuyet,        // có ca chưa duyệt → cảnh báo admin duyệt
    bool HoanToanTrong);     // không có ca nào kể cả ChoDuyet
```

Handler: query `CaLamViec` theo `IdChuyenKhoa` trong khoảng ngày, group by `NgayLamViec`, đếm `DaDuyet`/`ChoDuyet`. Trả ra ngày `DaDuyet == 0`.

#### UI

- **`Admin/Dashboard` widget mới:** "Cảnh báo thiếu bác sĩ 7 ngày tới" — list chuyên khoa × ngày thiếu BS. Click → mở `Admin/CaLamViec?chuyenKhoa=X&tuNgay=Y`.
- **`LeTan/QuanLyLichHen`:** trước khi đặt lịch (modal đặt), gọi `KiemTraDoPhuBacSiQuery` cho chuyên khoa + ngày BN chọn → nếu `HoanToanTrong = true` thì hiện banner đỏ "Khong co BS chuyen khoa nay ngay X. Hay chon ngay khac hoac chuyen khoa khac." trước khi user click submit.
- **`BenhNhan/DatLich`:** trong calendar picker, mark ngày thiếu BS bằng dấu xám + tooltip "Khong co bac si chuyen khoa nay ngay X".

#### Test

- Unit: query trả đúng ngày trống / ngày có ChoDuyet / ngày có DaDuyet.
- Integration: scenario chuyên khoa "Khoa Nhi" không có BS nào nội trú lịch Thứ 7 → query trả T7 trong list `NgayThieu`.

### 2.7. (🟢 Thấp) Verify API CaLamViec CRUD

**File:** `ClinicBooking.Api/Controllers/SchedulingController.cs`

Đảm bảo đủ:

| Method | Path | Authorize |
|---|---|---|
| GET | `/api/lich-lam-viec` | Anonymous (đã có `DanhSachCaLamViecCongKhaiQuery`) |
| POST | `/api/lich-lam-viec` | `admin`, `bac_si` (`TaoCaLamViecCommand`) |
| PUT | `/api/lich-lam-viec/{id}/duyet` | `admin` (`DuyetCaLamViecCommand`) |
| DELETE | `/api/lich-lam-viec/{id}` | `admin` (`XoaCaLamViecCommand`) |
| GET | `/api/lich-lam-viec/{id}` | Anonymous (chi tiết) |

Nếu thiếu → bổ sung. Backend Application đã có hết, chỉ wire controller.

---

## 3. Workflow ship

### 3.1. Branch

```powershell
git fetch origin
git checkout -b feature/module2/scheduling-finishing origin/develop
```

### 3.2. Commit suggest

1. `feat(scheduling): impl ChayReconSlotAsync + atomic IncrementSoSlot`
2. `feat(nghi-phep): admin duyet don nghi phep + UI Admin/NghiPhep`
3. `fix(scheduling): SinhCaLamViecTuLichNoiTru check trung theo IdDinhNghiaCa`
4. `feat(scheduling): validate xung dot BS/phong/nghi phep khi tao/duyet/sinh ca`
5. `feat(scheduling): query KiemTraDoPhuBacSi + canh bao UI admin/le-tan/benh-nhan`
6. `feat(api): wire CRUD CaLamViec public endpoints` (nếu thiếu)

### 3.3. Trước khi push

```powershell
dotnet build DatLichPhongKham.slnx
dotnet test
```

### 3.4. PR

- **Base:** `develop`
- **Reviewer:** Module 1 owner (verify không vỡ flow đặt/huỷ/đổi lịch)
- **Tag:** `module2`

---

## 4. Ràng buộc không được vi phạm

- Không đổi signature `ICaLamViecQueryService`.
- Không đụng vào logic `LichHen.TrangThai` (thuộc Module 1).
- Khi duyệt đơn nghỉ phép có ca đã có BN → **không silent cancel**, throw exception buộc admin xử lý trước.

---

## 5. Ưu tiên

1. **Cao** — §2.5 Validate xung đột BS/phòng/nghỉ phép (data integrity).
2. **Cao** — §2.2 Nghỉ phép admin duyệt (chặn flow demo BS hợp đồng).
3. **Cao** — §2.1 Verify QueryService + ReconSlot.
4. **Cao** — §2.6 Cảnh báo thiếu BS tại slot (UX).
5. **Trung** — §2.3 Fix bug SinhCa.
6. **Thấp** — §2.7 API verify.

---

## 6. Liên hệ

Nhắn Module 1 owner (Potatomine725) trên repo nếu cần làm rõ. Module 1 review PR trong 2 ngày làm việc.
