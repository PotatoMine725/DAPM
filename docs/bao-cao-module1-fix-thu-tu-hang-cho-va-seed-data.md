# Bao cao Module 1: sua loi thu tu hang cho va them seed data

Ngay cap nhat: 2026-04-23
Nguoi thuc hien: GitHub Copilot

## 1) Muc tieu cong viec

Muc tieu cua dot nay la:
- Xu ly loi ValidationException o trang BenhNhan/ThuTuHangCho.
- Bo sung du lieu test toi thieu de co the kiem tra UI Module 1.
- Ghi lai day du cac buoc da lam, loi gap phai va cach xu ly.

## 2) Nhung viec da lam

### 2.1 Sua trang BenhNhan/ThuTuHangCho

Da cap nhat:
- `ClinicBooking.Web/Pages/BenhNhan/ThuTuHangCho.cshtml.cs`
- `ClinicBooking.Web/Pages/BenhNhan/ThuTuHangCho.cshtml`

Noi dung sua:
- Cho `idCaLamViec` thanh tham so `int?` thay vi bat buoc.
- Neu khong co `idCaLamViec` hoac gia tri <= 0 thi khong nem exception nua, ma hien thi thong bao than thien:
  - `Vui long chon mot ca lam viec de xem thu tu hang cho.`
- Cap nhat route trong view thanh `@page "{idCaLamViec:int?}"`.

### 2.2 Them test data cho Module 1

Da them file moi:
- `ClinicBooking.Infrastructure/Persistence/Migrations/Module1TestDataSeeder.cs`

Va da gan seeder vao:
- `ClinicBooking.Infrastructure/Persistence/SeedData.cs`

Du lieu test gom:
- 4 tai khoan test: benh nhan, bac si, le tan, admin.
- 1 bac si test.
- 1 benh nhan test.
- 3 ca lam viec test.
- 2 lich hen test.
- 1 hang cho test.
- 2 ban ghi lich su lich hen.

### 2.2.1 Du lieu da seed de test

Du lieu co the dung ngay khi test UI:

| Nhom du lieu | ID | Thong tin chinh |
|---|---:|---|
| Tai khoan benh nhan | `2001` | `patient001` / `patient@test.vn` / vai tro `benh_nhan` |
| Tai khoan bac si | `2002` | `doctor001` / `doctor@test.vn` / vai tro `bac_si` |
| Tai khoan le tan | `2003` | `receptionist001` / `receptionist@test.vn` / vai tro `le_tan` |
| Tai khoan admin | `2004` | `admin001` / `admin@test.vn` / vai tro `admin` |
| Bac si test | `2001` | Bac si noi tru, chuyen khoa Tim Mach |
| Benh nhan test | `2001` | Tran Thi B |
| Ca lam viec sang | `3001` | Ngay mai, `07:00-12:00`, da duyet |
| Ca lam viec chieu | `3002` | Ngay mai, `13:00-17:00`, da duyet |
| Ca lam viec tuan sau | `3003` | Sau 7 ngay, `07:00-12:00`, da duyet |
| Lich hen 1 | `4001` | `LH-20260424-001`, trang thai `DaXacNhan`, co trong hang cho |
| Lich hen 2 | `4002` | `LH-20260424-002`, trang thai `ChoXacNhan` |
| Hang cho | `5001` | Gan voi lich hen `4001`, so thu tu `1` |
| Lich su lich hen | `6001`, `6002` | `DatMoi`, `XacNhan` cho lich hen `4001` |

### 2.2.2 Thong tin login de test

Su dung cac tai khoan sau neu can dang nhap de kiem tra UI:

| Vai tro | Username | Email | Ghi chu |
|---|---|---|---|
| Benh nhan | `patient001` | `patient@test.vn` | Dung de test danh sach lich hen, chi tiet lich hen, thu tu hang cho |
| Bac si | `doctor001` | `doctor@test.vn` | Dung de test man bac si neu co man hinh lien quan |
| Le tan | `receptionist001` | `receptionist@test.vn` | Dung de test man quan ly lich hen / hang cho cua le tan |
| Admin | `admin001` | `admin@test.vn` | Dung de test cac man quan tri neu can |

Luu y:
- Mat khau test dang nam trong seed voi gia tri hash gia lap `"$2a$11$encrypted_password"`.
- Neu can dang nhap thuc te, se can dong bo voi logic tao hash/tao tai khoan trong project hien tai.

### 2.3 Tao va ap migration

Da chay thanh cong:
- `dotnet ef migrations add Module1_TestDataSeed --project ClinicBooking.Infrastructure --startup-project ClinicBooking.Api`
- `dotnet ef database update --project ClinicBooking.Infrastructure --startup-project ClinicBooking.Api`

Ket qua: migration da duoc apply thanh cong vao database local.

## 3) Loi gap phai trong qua trinh lam

### 3.1 Loi validation o trang hang cho

Hien tuong:
- Khi mo trang `ThuTuHangCho` ma khong truyen `idCaLamViec`, ung dung nem loi:
  - `IdCaLamViec: Id ca lam viec phai lon hon 0`

Nguyen nhan:
- Page model khai bao `OnGetAsync(int idCaLamViec)`.
- Khi URL khong co tham so, ASP.NET gan mac dinh `0`.
- Validator cua query `ThuTuCuaToiQuery` bat buoc `IdCaLamViec > 0`.

Cach xu ly:
- Doi tham so sang `int?`.
- Them kiem tra hop le truoc khi gui MediatR query.
- Hien thi message cho nguoi dung thay vi nem exception.

### 3.2 Loi build khi gan seeder vao SeedData.cs

Hien tuong:
- Build Infrastructure ban dau bi fail vi `Module1TestDataSeeder` chua duoc import dung namespace.
- Sau do phat hien them loi do dung sai ten enum `HanhDongLichHen`.

Cach xu ly:
- Them `using ClinicBooking.Infrastructure.Persistence.Migrations;` vao `SeedData.cs`.
- Doi `HanhDongLichHen` thanh `HanhDongLichSu`.

### 3.3 Loi design-time cua EF khi scaffold migration

Hien tuong:
- EF khong tao duoc migration vi seed dung anonymous type va thieu gia tri required property.
- Sau khi sua sang CLR entity, EF lai bao loi trung khoa do da seed trung voi du lieu co san.

Cach xu ly:
- Viet lai seeder bang cac entity that (`TaiKhoan`, `BacSi`, `CaLamViec`, `BenhNhan`, `LichHen`, `HangCho`, `LichSuLichHen`).
- Chuyen test IDs sang mot khoang so rieng de tranh trung voi du lieu hien co trong database:
  - `2001` tro len cho tai khoan va benh nhan test
  - `3001` tro len cho ca lam viec test
  - `4001` tro len cho lich hen test
  - `5001` tro len cho hang cho test
  - `6001` tro len cho lich su test
- Xoa migration cu vua sinh ra, sau do scaffold lai migration moi.

### 3.4 Loi trung khoa khi update database

Hien tuong:
- Khi update database lan dau voi test IDs cu, SQL bao:
  - `Violation of PRIMARY KEY constraint 'PK_BenhNhan'`

Nguyen nhan:
- DB da co ban ghi id `1` trong bang `BenhNhan`.
- Seeder moi co cung ID `1` nen bi trung.

Cach xu ly:
- Doi toan bo test data sang key range rieng.
- Tao lai migration va update lai database.

## 4) Cach giai quyet cuoi cung

Ket qua cuoi cung:
- `ThuTuHangCho` khong con nem ValidationException khi thieu `idCaLamViec`.
- Seed data da duoc gan vao `SeedData.cs`.
- Migration `Module1_TestDataSeed` da tao moi va ap thanh cong.
- Database local da co du lieu test phuc vu kiem tra UI Module 1.

## 5) Kiem tra da thuc hien

Da thuc hien cac lenh sau:
- `dotnet build ClinicBooking.Infrastructure\ClinicBooking.Infrastructure.csproj`
- `dotnet ef migrations add Module1_TestDataSeed --project ClinicBooking.Infrastructure --startup-project ClinicBooking.Api`
- `dotnet ef database update --project ClinicBooking.Infrastructure --startup-project ClinicBooking.Api`

Ket qua:
- Build Infrastructure: thanh cong.
- Migration scaffold: thanh cong.
- Database update: thanh cong.

## 6) File da thay doi

- `ClinicBooking.Web/Pages/BenhNhan/ThuTuHangCho.cshtml.cs`
- `ClinicBooking.Web/Pages/BenhNhan/ThuTuHangCho.cshtml`
- `ClinicBooking.Infrastructure/Persistence/SeedData.cs`
- `ClinicBooking.Infrastructure/Persistence/Migrations/Module1TestDataSeeder.cs`
- `ClinicBooking.Infrastructure/Persistence/Migrations/20260423102703_Module1_TestDataSeed.cs`
- `ClinicBooking.Infrastructure/Persistence/Migrations/20260423102703_Module1_TestDataSeed.Designer.cs`
- `ClinicBooking.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs`

## 7) Trang thai hien tai

Co the test ngay:
- Trang danh sach lich hen cua benh nhan.
- Trang chi tiet lich hen.
- Trang thu tu hang cho khi co `idCaLamViec` hop le.

Con can lam tiep neu muon hoan thien UI:
- Gan nut dieu huong ro rang tu lich hen sang trang thu tu hang cho voi `idCaLamViec` hop le.
- Bo sung them du lieu test neu can mo rong kich ban cho le tan / bac si.

## 8) Ghi chu

Ban ghi nay chi tap trung vao dot sua loi + seed data vua lam. Neu can, co the tach tiep thanh:
- report UI Module 1
- report seed data / migration
- report root cause + fix cho trang hang cho
