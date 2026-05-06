# Tien do Module 3 - Benh nhan, Ho so kham, Toa thuoc

Cap nhat lan cuoi: 2026-05-06
Nhanh lam viec: `feature/module3`
Trang thai chung: **PHASE 1 COMPLETED (Task 1.1 UNBLOCKED), PHASE 2-5 COMPLETED**

## Dang lam

- Khong co

## Da hoan thanh

### PHASE 1: Schema & Entities + Task 1.1 Xac nhan (100%)
- ✅ BenhNhan entity: co truong SoLanHuyMuon (int, default 0), BiHanChe, NgayHetHanChe
- ✅ HoSoKham entity: co foreign key IdLichHen, IdBacSi
- ✅ ToaThuoc + ChiTietToaThuoc entities
- ✅ Thuoc entity: TenThuoc, HoatChat, DonVi, GhiChu
- ✅ Migration da apply: default value SoLanHuyMuon = 0

**Task 1.1 - Xac nhan SoLanHuyMuon policy (UNBLOCKED):**
- ✅ Nguong toi da: **3 lan/thang**
- ✅ Reset: **hang thang**
- ✅ Rule Module 3: **Benh nhan bi cam (BiHanChe=true va NgayHetHanChe > now) -> khong tao HoSoKham duoc**
- ✅ BenhNhanConstants.cs: Nguong = 3, ChuKyReset = 1 (hang thang)
- ✅ TaoHoSoKhamHandler: them kiem tra BiHanChe

### PHASE 2: HoSoKham CRUD (100%)
- ✅ Command TaoHoSoKham: validate IdLichHen, TrangThai = DangKham|HoanThanh, **kiem tra BehnNhan.BiHanChe**
- ✅ Command CapNhatHoSoKham: cap nhat ChanDoan, KetQuaKham, GhiChu
- ✅ Query LayHoSoKhamById: chi tiet ho so (include BacSi, LichHen, BenhNhan)
- ✅ Query LichSuKhamTheoBenhNhan: danh sach ho so cua 1 benh nhan (paginated)
- ✅ Query LichSuKhamCuaToi: danh sach cho benh nhan xem cua minh (paginated)
- ✅ Validator TaoHoSoKhamValidator: kiem tra idLichHen, length ChanDoan/KetQuaKham/GhiChu
- ✅ Validator CapNhatHoSoKhamValidator
- ✅ Controller HoSoKhamController: POST, PUT, GET (chi tiet + lich su)
- ✅ DTO va Mapping (TaoHoSoKhamResponse, HoSoKhamDto, HoSoKhamTomTatDto)
- ✅ Authorization: BacSi tao/cap nhat, BenhNhan xem cua minh, Admin xem het
- ✅ Unit tests: Tao, Cap nhat, Query, BiHanChe validation (trong ClinicBooking.Application.UnitTests)

### PHASE 3: ToaThuoc CRUD (100%)
- ✅ Command TaoToaThuoc: validate IdHoSoKham, kiem tra thuoc ton tai, khong trung lap
- ✅ Command CapNhatToaThuoc: cap nhat danh sach chi tiet thuoc
- ✅ Query LayToaTheoHoSoKham: danh sach thuoc trong 1 ho so (include Thuoc info)
- ✅ Query LayToaCuaToi: danh sach toa cua benh nhan (paginated)
- ✅ Validator TaoToaThuocValidator: kiem tra IdHoSoKham, DanhSachThuoc khong rong
- ✅ Validator CapNhatToaThuocValidator
- ✅ Controller ToaThuocController: POST, PUT (alternative routes), GET
- ✅ DTO va Mapping (ToaThuocDto, ToaThuocResponse, ToaThuocChiTietInput)
- ✅ Authorization: BacSi ke toa, BenhNhan xem cua minh
- ✅ Unit tests: Tao toa, Cap nhat toa, Query (trong ClinicBooking.Application.UnitTests)

### PHASE 4: Thuoc Danh Muc CRUD (100%)
- ✅ Command TaoThuoc: kiem tra TenThuoc khong trung lap
- ✅ Command CapNhatThuoc: cap nhat TenThuoc, HoatChat, DonVi, GhiChu
- ✅ Command XoaThuoc: kiem tra thuoc khong duoc su dung trong ToaThuoc truoc khi xoa
- ✅ Query LayThuocById: chi tiet 1 thuoc
- ✅ Query DanhSachThuoc: danh sach thuoc (paginated, search by name)
- ✅ Validator TaoThuocValidator: kiem tra TenThuoc khong rong, toi da 300 ky tu
- ✅ Validator CapNhatThuocValidator
- ✅ Validator XoaThuocValidator
- ✅ Controller ThuocController: POST, PUT, DELETE, GET (chi tiet + danh sach)
- ✅ DTO va Mapping (ThuocDto, ThuocResponse)
- ✅ Authorization: Admin CRUD (tao/cap nhat/xoa), BacSi read danh sach
- ✅ Unit tests: CRUD operations, unique name validation, xoa protection (trong ClinicBooking.Application.UnitTests)

### PHASE 5a: Web UI - BacSi QuanLyKham (100%)
- ✅ Trang Razor `/BacSi/QuanLyKham.cshtml` + PageModel
- ✅ Goi API layer (tao HoSoKham, cap nhat ToaThuoc)
- ✅ Noi dung: tao ho so kham, ke don thuoc
- ✅ TempData success message

### PHASE 5b: Web UI - Admin QuanLyThuoc (100%)
- ✅ Trang Razor `/Admin/QuanLyThuoc.cshtml` + PageModel
- ✅ Goi API layer CRUD Thuoc (tao, cap nhat, xoa, danh sach)
- ✅ Form modal (hoac inline) cho tao/cap nhat/xoa
- ✅ Danh sach thuoc (bang paginated)
- ✅ TempData success message

### Unit Tests & Regression
- ✅ Unit tests cho HoSoKham, ToaThuoc, Thuoc (da xem co trong repo)
- ✅ Test handlers va validators
- ⏳ Chay `dotnet test` toan bo de xac nhan khong break Module 1

## Ke hoach tiep theo (NEXT STEPS)

### IMMEDIATE (ngay hom nay)
1. ✅ **UNBLOCK Task 1.1:** Xac nhan voi Module 1 owner
   - Nguong: 3 lan/thang ✅
   - Reset: hang thang ✅
   - Rule Module 3: Benh nhan bi cam khong tao HoSoKham ✅
2. **Chay test toan bo:** `dotnet test` de xac nhan 241 M1 tests + 11 IT tests pass
3. ✅ **Xac nhan Thuoc status:** Khong can them truong TrangThai (logic ke don da check con thuoc)

### NEXT WEEK
- Integrate notification (Module 4) neu ready - log tao ho so kham, ke toa
- Code review & refine
- E2E test: Dat lich -> Check-in -> Kham -> Ke toa -> Xem toa

## Ghi chu quan trong

### Khong break Module 1
- Module 3 **chi doc** LichHen (khong update TrangThai)
- Module 1 set LichHen.TrangThai = DangKham tai check-in
- Module 1 increment BenhNhan.SoLanHuyMuon khi huy lich
- **Phai dam bao:** 241 unit tests + 11 integration tests Module 1 van green

### Phu thuoc Module 1 (COMPLETED)
- `LichHen.TrangThai = DangKham | HoanThanh` - Module 1 set ✅
- `BenhNhan.SoLanHuyMuon` increment logic - Module 1 co ✅
- **Xac nhan:** Nguong = 3, reset = hang thang, rule = benh nhan bi cam khong tao ✅

### Phu thuoc Module 4 (Optional, Phase 3+)
- Notification tao ho so kham
- Notification ke toa
- Notification huy toa
- Hien tai: Stub service (khong gui, chi log)

## Cac file chinh da sua doi

### New Files
- `ClinicBooking.Application/Common/Constants/BenhNhanConstants.cs` - hang so nguong SoLanHuyMuon

### Updated Files
- `ClinicBooking.Application/Features/HoSoKham/Commands/TaoHoSoKham/TaoHoSoKhamHandler.cs`
  - Them `Include(x => x.BenhNhan)` de lay thong tin benh nhan
  - Them validation: Neu BenhNhan.BiHanChe = true va NgayHetHanChe > now -> throw ForbiddenException
- `ClinicBooking.Application.UnitTests/Features/HoSoKham/Commands/TaoHoSoKham/TaoHoSoKhamHandlerTests.cs`
  - Them test case: `Handle_BenhNhanBiHanChe_ThrowForbiddenException()`

## Tong hop thay doi

| Buoc | Noi dung | Gia tri | Trang thai |
|---|---|---|---|
| Constants | Nguong SoLanHuyMuon | 3 lan/thang | ✅ |
| Constants | Chu ky reset | 1 thang | ✅ |
| Handler | Kiem tra BiHanChe | Neu dang cam -> khong tao | ✅ |
| Unit Test | Test BiHanChe | 1 test case moi | ✅ |
| Documentation | Cap nhat tiep theo | Log audit, notification | TODO |

## Status Overall

```
PHASE 1 (Schema & Task 1.1): ✅ COMPLETED
PHASE 2 (HoSoKham):           ✅ COMPLETED
PHASE 3 (ToaThuoc):           ✅ COMPLETED
PHASE 4 (Thuoc):              ✅ COMPLETED
PHASE 5 (Web UI):             ✅ COMPLETED
---
Unit Tests:                   ✅ ADDED
Regression Tests:             ⏳ TODO: Chay dotnet test
```

**Trang thai cuoi cung:** Toan bo cac function core da hoan thanh. Ssan sang cho QA va regression test.
