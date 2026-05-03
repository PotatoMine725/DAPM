Please address the comments from this code review:

## Overall Comments
- In `MaLichHenGenerator` the XML doc mentions retrying up to `LichHenConstants.SoLanThuLaiToiDa` on unique index collisions, but the implementation currently just computes the next sequence and returns without any retry or exception handling, and `SoLanThuLaiToiDa` is never used—either wire in the retry logic around insert or update the comments/constants to match the actual behavior.
- The new `ICaLamViecQueryService.IncrementSoSlotDaDatAsync` stub uses an unconditional `ExecuteUpdateAsync` to adjust `SoSlotDaDat` without checking against `SoSlotToiDa`; if this contract is intended to enforce capacity limits, consider validating the new value in the same atomic statement or clearly documenting that callers must perform a separate capacity check and that `SoSlotDaDat` can temporarily exceed `SoSlotToiDa` under races.

## Individual Comments

### Comment 1
<location path="ClinicBooking.Infrastructure/Persistence/Migrations/20260417001523_Module1_ThemRangBuocLichHen.cs" line_range="46-52" />
<code_context>
+                oldClrType: typeof(string),
+                oldType: "nvarchar(450)");
+
+            migrationBuilder.AddColumn<byte[]>(
+                name: "RowVersion",
+                table: "LichHen",
+                type: "rowversion",
+                rowVersion: true,
+                nullable: false,
+                defaultValue: new byte[0]);
+
+            migrationBuilder.AlterColumn<string>(
</code_context>
<issue_to_address>
**issue (bug_risk):** Thêm `RowVersion` kiểu `rowversion` với `defaultValue: new byte[0]` có thể gây lỗi migration trên SQL Server.

Với cột `rowversion` trên SQL Server, giá trị phải do SQL Server tự sinh, không nên set `defaultValue`. Việc dùng `defaultValue: new byte[0]` dễ khiến EF sinh SQL không hợp lệ và migration lỗi. Nên bỏ `defaultValue` trong `AddColumn` để SQL Server tự quản lý giá trị `rowversion`.
</issue_to_address>

### Comment 2
<location path="ClinicBooking.Infrastructure/Services/MaLichHenGenerator.cs" line_range="10-13" />
<code_context>
+
+namespace ClinicBooking.Application.UnitTests.Common;
+
+/// <summary>
+/// Factory tao <see cref="AppDbContext"/> tren SQLite :memory: de unit test chay nhanh
+/// ma van co unique index + FK constraint thuc su (khac InMemoryDatabase khong co).
</code_context>
<issue_to_address>
**issue (bug_risk):** Docstring nói có retry khi trùng unique index nhưng implementation hiện tại không retry và không dùng `SoLanThuLaiToiDa`.

Method `SinhMaLichHenAsync` hiện chỉ lấy `MAX(MaLichHen)` và trả về 1 giá trị, không hề retry và cũng không bắt/handle `DbUpdateException` do trùng mã, nên không đúng với mô tả.

Nếu nghiệp vụ thực sự cần giảm xác suất lỗi khi nhiều request song song, cần:
- Implement vòng retry (bắt exception khi save và sinh lại mã), **hoặc**
- Cập nhật docstring cho đúng behavior hiện tại và để xử lý va chạm unique index ở lớp gọi.

Cần đồng bộ docstring với behavior thực tế để tránh hiểu nhầm khi tích hợp vào các handler đặt lịch.
</issue_to_address>

### Comment 3
<location path="ClinicBooking.Infrastructure/Services/Scheduling/CaLamViecQueryServiceStub.cs" line_range="125-134" />
<code_context>
+                cancellationToken);
+    }
+
+    public async Task<int> IncrementSoSlotDaDatAsync(
+        int idCaLamViec, int delta, CancellationToken cancellationToken = default)
+    {
+        // Atomic UPDATE — khong Read-Modify-Write tu client.
+        // SQL Server dam bao nguyen tu bang row-level lock tren UPDATE statement.
+        await _db.CaLamViec
+            .Where(c => c.IdCaLamViec == idCaLamViec)
+            .ExecuteUpdateAsync(
+                s => s.SetProperty(c => c.SoSlotDaDat, c => c.SoSlotDaDat + delta),
+                cancellationToken);
+
+        // Doc lai gia tri moi (sau UPDATE da commit)
+        return await _db.CaLamViec
+            .Where(c => c.IdCaLamViec == idCaLamViec)
+            .Select(c => c.SoSlotDaDat)
+            .FirstAsync(cancellationToken);
+    }
+
</code_context>
<issue_to_address>
**suggestion:** `IncrementSoSlotDaDatAsync` không kiểm soát việc giảm dưới 0 hoặc vượt quá `SoSlotToiDa`.

Hiện method chỉ cộng thẳng `delta` vào `SoSlotDaDat`. Nếu caller truyền `delta` âm nhiều lần hoặc dương vượt giới hạn, `SoSlotDaDat` có thể < 0 hoặc > `SoSlotToiDa`.

Nên chuẩn hóa behavior tại đây, ví dụ:
- Áp constraint tối thiểu: clamp về 0 hoặc từ chối `delta` khiến kết quả âm.
- Áp constraint tối đa: đưa điều kiện vào câu `UPDATE` (như `WHERE SoSlotDaDat + @delta BETWEEN 0 AND SoSlotToiDa`) và trả về kết quả cho biết update có thành công hay không.

Như vậy tất cả caller đều tuân theo cùng một logic kiểm soát slot.

Suggested implementation:

```csharp
    public async Task<int?> IncrementSoSlotDaDatAsync(
        int idCaLamViec, int delta, CancellationToken cancellationToken = default)
    {
        // Atomic UPDATE — khong Read-Modify-Write tu client.
        // SQL Server dam bao nguyen tu bang row-level lock tren UPDATE statement.
        // Ap constraint: SoSlotDaDat + delta khong duoc < 0 hoac > SoSlotToiDa.
        var rowsAffected = await _db.CaLamViec
            .Where(c => c.IdCaLamViec == idCaLamViec)
            .Where(c => c.SoSlotDaDat + delta >= 0 && c.SoSlotDaDat + delta <= c.SoSlotToiDa)
            .ExecuteUpdateAsync(
                s => s.SetProperty(c => c.SoSlotDaDat, c => c.SoSlotDaDat + delta),
                cancellationToken);

        // Neu khong co row nao duoc update, co nghia la:
        // - Khong tim thay CaLamViec tuong ung, hoac
        // - delta khien SoSlotDaDat vuot khoi [0, SoSlotToiDa]
        if (rowsAffected == 0)
        {
            return null;
        }

        // Doc lai gia tri moi (sau UPDATE da commit)
        return await _db.CaLamViec
            .Where(c => c.IdCaLamViec == idCaLamViec)
            .Select(c => (int?)c.SoSlotDaDat)
            .FirstAsync(cancellationToken);
    }

```

1. Cap nhat `ICaLamViecQueryService` (va bat ky interface/abstract base nao lien quan) de signature cua `IncrementSoSlotDaDatAsync` tra ve `Task<int?>` thay vi `Task<int>`.
2. Cap nhat tat ca caller cua `IncrementSoSlotDaDatAsync`:
   - Xu ly truong hop tra ve `null` (khong thanh cong do vuot constraint hoac khong tim thay ca lam viec).
   - Neu truoc day caller mac dinh coi la thanh cong, co the chuyen sang pattern:
     `var soSlotDaDatMoi = await service.IncrementSoSlotDaDatAsync(...); if (soSlotDaDatMoi is null) { // handle fail }`.
3. Neu muon phan biet ro rang giua "khong tim thay ca lam viec" va "vi pham constraint slot", co the thay `return null;` bang throw exception cu the, hoac thay doi return type sang mot DTO/record co cac field `Success`, `Reason`, `SoSlotDaDatMoi`.
</issue_to_address>