# Bao cao ra soat Module 3 so voi phan chia du an

Ngay lap bao cao: 2026-05-03
Nguon doi chieu:
- `docs/Others/phan-chia-module-va-huong-dan-du-an.md`
- `docs/Plans/tien-do-module3.md`

## 1. Ket luan nhanh

- Backend Module 3 da trien khai phan lon nghiep vu cot loi (BenhNhan, HoSoKham, ToaThuoc, Thuoc) o cac lop Application + API + UnitTests.
- Con it nhat 2 diem chua khop hoan toan voi dac ta trong file phan chia:
  - `walk-in` dang tao `TaiKhoan` he thong, trong khi dac ta ghi ro "walk-in (khong co tai khoan)".
  - Frontend Web chua thay cac man hinh/chuc nang rieng cho HoSoKham va ToaThuoc cua Module 3 (hien moi thay HoSoCaNhan).
- File tien do Module 3 dang co noi dung chua nhat quan (muc "Da hoan thanh" va "Ke hoach tiep theo" bi trung lap ToaThuoc).

## 2. Tong hop cong viec can lam cua Module 3 (theo file phan chia)

### 2.1 Nhom nghiep vu chinh

1. Benh nhan xem/cap nhat ho so ca nhan.
2. Le tan/Admin tim kiem + xem danh sach benh nhan.
3. Le tan tao ho so benh nhan walk-in (khong co tai khoan).
4. Bac si tao ho so kham tu lich hen da check-in.
5. Bac si cap nhat ho so kham (chan doan, loi dan/ghi chu).
6. Bac si/Benh nhan xem lich su kham benh.
7. Bac si ke toa thuoc cho HoSoKham (1 ho so co nhieu dong toa).
8. Benh nhan xem toa thuoc cua toi.
9. Admin CRUD danh muc Thuoc.

### 2.2 Rang buoc ky thuat lien quan

1. Module 3 phu thuoc LichHen tu Module 1 theo huong chi doc.
2. Tuan 6 co muc "Hardening" cho Module 3.
3. Definition of Done yeu cau day du Command/Query/Handler/Validator, endpoint authorize, unit test handler, build/test xanh.

## 3. Doi chieu voi code hien tai cua Module 3

Phan vi ra soat code:
- `ClinicBooking.Application/Features/BenhNhan`
- `ClinicBooking.Application/Features/HoSoKham`
- `ClinicBooking.Application/Features/ToaThuoc`
- `ClinicBooking.Application/Features/Thuoc`
- `ClinicBooking.Api/Controllers/BenhNhanController.cs`
- `ClinicBooking.Api/Controllers/HoSoKhamController.cs`
- `ClinicBooking.Api/Controllers/ToaThuocController.cs`
- `ClinicBooking.Api/Controllers/ThuocController.cs`
- Unit test trong `ClinicBooking.Application.UnitTests/Features/{BenhNhan,HoSoKham,ToaThuoc,Thuoc}`
- Giao dien web trong `ClinicBooking.Web/Pages/BenhNhan`

### 3.1 Bang doi chieu

| Cong viec theo phan chia | Trang thai | Bang chung code |
|---|---|---|
| 1) Benh nhan xem/cap nhat ho so ca nhan | Da co | `LayHoSoCuaToi*`, `CapNhatHoSoCuaToi*`, `BenhNhanController` (`GET/PUT ho-so-cua-toi`) |
| 2) Le tan/Admin tim kiem + danh sach benh nhan | Da co | `DanhSachBenhNhan*`, `LayBenhNhanById*`, `BenhNhanController` (`GET /api/benh-nhan`, `GET /{id}`) |
| 3) Tao benh nhan walk-in khong co tai khoan | Co, nhung lech dac ta | `TaoBenhNhanWalkInHandler` dang tao moi `TaiKhoan` + tra `IdTaiKhoan` |
| 4) Bac si tao ho so kham tu lich hen da check-in | Da co (co rang buoc trang thai) | `TaoHoSoKhamHandler` kiem tra lich hen ton tai, dung bac si, trang thai `DangKham/HoanThanh`, va chua co ho so |
| 5) Bac si cap nhat ho so kham | Da co | `CapNhatHoSoKham*`, endpoint `PUT /api/ho-so-kham/{id}` |
| 6) Bac si/Benh nhan xem lich su kham | Da co | `LichSuKhamTheoBenhNhan*`, `LichSuKhamCuaToi*`, endpoint `GET /api/ho-so-kham/benh-nhan/{id}` + `GET /api/ho-so-kham/cua-toi` |
| 7) Bac si ke toa theo HoSoKham | Da co | `TaoToaThuoc*`, `CapNhatToaThuoc*`, kiem tra trung thuoc + quyen bac si |
| 8) Benh nhan xem toa cua toi | Da co | `LayToaCuaToi*`, endpoint `GET /api/toa-thuoc/cua-toi` |
| 9) Admin CRUD Thuoc | Da co | `Tao/CapNhat/Xoa/DanhSach/LayThuocById`, `ThuocController` |

### 3.2 Danh gia theo Definition of Done

- Command/Query/Handler/Validator: Da co day du cho cac feature chinh.
- Phan quyen endpoint: Da khai bao `[Authorize(Roles = ...)]` tren cac controller Module 3.
- Unit tests: Da co test cho BenhNhan, HoSoKham, ToaThuoc, Thuoc (command/query + validator).
- Build/Test: Chua chay lai trong lan ra soat nay (bao cao dua tren static code scan).

## 4. Phan con thieu hoac chua khop

1. Lech dac ta walk-in:
- Dac ta: walk-in khong co tai khoan.
- Thuc te: `TaoBenhNhanWalkInHandler` tao `TaiKhoan` moi voi `TenDangNhap/Email` he thong.
- Tac dong: can chot lai voi nghiep vu xem giu huong hien tai (de dang chuyen doi sau nay) hay sua ve dung dac ta goc.

2. Frontend cho nghiep vu Module 3 chua day du:
- Hien thay trang benh nhan chu yeu lien quan dat lich + ho so ca nhan.
- Chua thay trang rieng cho lich su kham/chi tiet ho so kham/toa thuoc cua benh nhan.
- Neu pham vi Module 3 tinh ca Web portal thi day la khoang trong can bo sung.

3. Tai lieu tien do chua nhat quan:
- `docs/Plans/tien-do-module3.md` ghi ToaThuoc vua "Da hoan thanh" vua "Ke hoach tiep theo".
- Nen cap nhat lai de phan anh dung backlog con lai.

## 5. De xuat backlog tiep theo cho Module 3

1. Chot nghiep vu walk-in voi team (co/khong tao tai khoan) va cap nhat code + test + tai lieu theo quyet dinh.
2. Neu scope gom frontend: bo sung man hinh Benh nhan xem lich su kham va toa thuoc cua toi; Bac si xem/nhap ho so kham + toa thuoc.
3. Dong bo file `docs/Plans/tien-do-module3.md` thanh checklist ro rang theo trang thai: Done / In Progress / Todo.
4. Chay lai `dotnet build` + `dotnet test` de xac nhan baseline sau khi chot thay doi tren.
