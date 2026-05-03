# Báo cáo fix 20 unit test thất bại sau merge `Module1_TestDataSeed`

Ngày lập: 2026-05-03  
Nguyên nhân: migration `Module1_TestDataSeed` seed dữ liệu cố định vào SQLite `:memory:`, xung đột với dữ liệu test hardcode và logic assert sai.

---

## 1) Triệu chứng

Sau khi merge nhánh có migration `20260423102703_Module1_TestDataSeed`, chạy `dotnet test ClinicBooking.Application.UnitTests` báo **20/231 test thất bại**:

- Nhóm Auth (`DangKy`, `DangNhap`): `UNIQUE constraint failed: TaiKhoan.SoDienThoai / Email / Cccd`
- Nhóm BenhNhan (`TaoBenhNhanWalkIn`, `DanhSachBenhNhan`, `CapNhatHoSo`, `LayHoSo`): tương tự UNIQUE constraint
- Nhóm LichHen (`TaoLichHen`): `NullReferenceException` và message mismatch
- Nhóm Doctors (`DanhSachBacSiCongKhai`): `Expected 1 item but found 2`
- `TestDbContextFactoryTests`: count không về 0 sau cleanup

---

## 2) Nguyên nhân gốc rễ

### 2.1 SQLite `EnsureCreated()` áp dụng toàn bộ `HasData`

`TestDbContextFactory` dùng `EnsureCreated()` thay vì migration thật. EF Core áp dụng tất cả `HasData()` từ model snapshot, bao gồm dữ liệu seed của `Module1_TestDataSeed`:

| Bảng | ID được seed |
|---|---|
| `TaiKhoan` | 2001–2004 (SoDienThoai cố định: `0912345678`, `0987654321`, ...) |
| `BenhNhan` | 2001 (Cccd: `123456789012`) |
| `BacSi` | 2001 (TrangThai: DangLam, IdChuyenKhoa: 1) |
| `CaLamViec` | 3001–3003 |
| `LichHen` | 4001–4002 |
| `HangCho` | 5001 |
| `LichSuLichHen` | 6001–6002 |

Các test (đặc biệt module 3) hardcode cùng số điện thoại/CCCD → UNIQUE constraint fail ngay khi `SaveChanges`.

### 2.2 Multi-statement trong một `ExecuteNonQuery` không chạy hết

Lần fix đầu dùng chuỗi ghép nhiều lệnh DELETE bằng `;`:

```csharp
cmd.CommandText = "DELETE FROM HangCho ...;" + "DELETE FROM LichHen ...;";
cmd.ExecuteNonQuery(); // SQLite chỉ chạy lệnh đầu tiên!
```

SQLite với `Microsoft.Data.Sqlite` chỉ thực thi statement đầu tiên trong một lần gọi `ExecuteNonQuery()`. Các bảng sau không bị xóa.

### 2.3 Thiếu `LichSuLichHen` trong cleanup sequence

Ngay cả khi tách thành lệnh riêng, nếu không xóa `LichSuLichHen` trước `LichHen`, SQLite FK enforcement (`PRAGMA foreign_keys = ON` được EF Core bật trên connection) sẽ chặn `DELETE FROM LichHen` vì rows 6001–6002 tham chiếu rows 4001–4002.

### 2.4 `DanhSachBacSiCongKhaiHandler` không filter mặc định

Handler chỉ filter `TrangThai == DangLam` khi `DangLamViec.HasValue`. Khi `DangLamViec=null` (default), handler trả về cả bác sĩ `NghiViec` → test expect 1 nhưng nhận 2.

### 2.5 `TaoLichHenHandler` không dùng `LayThongTinCaAsync`

Handler query DB trực tiếp (filter `TrangThaiDuyet == DaDuyet`) thay vì gọi `ICaLamViecQueryService.LayThongTinCaAsync`. Test `Handle_CaChuaDuyet_ThrowConflict` mock `LayThongTinCaAsync` → mock không có tác dụng → `KiemTraSlotTrongAsync` không được mock → NSubstitute trả `null` → `NullReferenceException`.

### 2.6 Message mismatch

Test `Handle_HetSlot_IncrementTraVeNull_ThrowConflict` expect message cũ `"Ca lam viec da het slot."`, nhưng handler đã được cập nhật thành `"Ca lam viec da het slot hoac bi xung dot cap nhat."`.

---

## 3) Đã sửa

### 3.1 `TestDbContextFactory` — tách DELETE + thêm đúng thứ tự dependency

File: `ClinicBooking.Application.UnitTests/Common/TestDbContextFactory.cs`

Thay chuỗi ghép bằng vòng `foreach`, mỗi lệnh một `ExecuteNonQuery()`. Thêm `LichSuLichHen` và `GiuCho` vào đầu sequence để đảm bảo FK không bị vi phạm:

```csharp
var deleteStatements = new[]
{
    "DELETE FROM LichSuLichHen WHERE IdLichSu >= 6000",  // -> LichHen
    "DELETE FROM GiuCho WHERE IdGiuCho >= 1",            // -> CaLamViec
    "DELETE FROM HangCho WHERE IdHangCho >= 5000",
    "DELETE FROM LichHen WHERE IdLichHen >= 4000",
    "DELETE FROM CaLamViec WHERE IdCaLamViec >= 3000",
    "DELETE FROM BenhNhan WHERE IdBenhNhan >= 2000",
    "DELETE FROM BacSi WHERE IdBacSi >= 2000",
    "DELETE FROM LeTan WHERE IdLeTan >= 2000",
    "DELETE FROM TaiKhoan WHERE IdTaiKhoan >= 2000",
};
foreach (var sql in deleteStatements)
{
    using var cmd = _connection.CreateCommand();
    cmd.CommandText = sql;
    cmd.ExecuteNonQuery();
}
```

### 3.2 `DanhSachBacSiCongKhaiHandler` — default filter DangLam

File: `ClinicBooking.Application/Features/Doctors/Queries/DanhSachBacSiCongKhai/DanhSachBacSiCongKhaiHandler.cs`

Khi `DangLamViec=null`, mặc định chỉ trả bác sĩ đang làm việc (phù hợp portal bệnh nhân):

```csharp
if (request.DangLamViec.HasValue)
    query = query.Where(x => (x.TrangThai == TrangThaiBacSi.DangLam) == request.DangLamViec.Value);
else
    query = query.Where(x => x.TrangThai == TrangThaiBacSi.DangLam);
```

### 3.3 `TaoLichHenHandlerTests` — mock đúng method + cập nhật message

File: `ClinicBooking.Application.UnitTests/Features/LichHen/Commands/TaoLichHen/TaoLichHenHandlerTests.cs`

**`Handle_CaChuaDuyet_ThrowConflict`**: thay mock `LayThongTinCaAsync` bằng mock `KiemTraSlotTrongAsync` trả `{CoTheDat=false, LyDoTuChoi=CaChuaDuyet}`.

**`Handle_HetSlot_IncrementTraVeNull_ThrowConflict`**: cập nhật expected message:

```csharp
// Truoc:
.WithMessage("Ca lam viec da het slot.")
// Sau:
.WithMessage("Ca lam viec da het slot hoac bi xung dot cap nhat.")
```

---

## 4) Kết quả

```
Passed! - Failed: 0, Passed: 231, Skipped: 0, Total: 231, Duration: 3s
```

---

## 5) Ghi chú kỹ thuật

| Vấn đề | Bài học |
|---|---|
| SQLite multi-statement | Luôn tách mỗi statement thành một lần gọi `ExecuteNonQuery()` |
| FK enforcement trong test | EF Core bật `PRAGMA foreign_keys = ON` — cleanup phải đúng thứ tự child → parent |
| Mock vs DB query | Trước khi viết test mock, kiểm tra handler thực sự gọi method nào trên interface |
| Message trong test | Test nên dùng hằng số hoặc lấy từ một source of truth, tránh hardcode string trùng lặp |

Khi có thêm migration seed data mới, **cần cập nhật** `TestDbContextFactory.cs` bổ sung DELETE tương ứng (đúng bảng, đúng ngưỡng ID, đúng thứ tự).
