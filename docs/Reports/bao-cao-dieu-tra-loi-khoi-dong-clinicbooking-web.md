# Bao cao dieu tra loi khoi dong ClinicBooking.Web

Ngay dieu tra: 2026-04-19
Cap nhat: 2026-04-19 (sau khi user xac nhan da co DB ClinicBooking tren SSMS)
Nguoi thuc hien: GitHub Copilot

## 1) Mo ta hien tuong

Khi chay lenh:

```bash
dotnet run --project ClinicBooking.Web
```

Ung dung bi dung o giai doan startup voi loi:

- `Cannot open database "ClinicBookingDb" requested by the login`
- `Login failed for user 'POTATO\\Wotbl'`
- Stack trace dung tai `DatabaseSeeder.FixMatKhauAdminAsync(...)`

## 2) Ket qua kiem tra

### 2.1 Build va run

- `dotnet build DatLichPhongKham.slnx`: **thanh cong** (khong co loi compile).
- `dotnet run --project ClinicBooking.Web`: **that bai runtime** do ket noi DB.

=> Day **khong phai loi build/code compile**, ma la loi cau hinh ket noi CSDL khi startup.

### 2.1.1 Cap nhat theo thong tin user

- User xac nhan DB `ClinicBooking` da ton tai tren SSMS va truoc day Web tung chay binh thuong.
- Ket qua nay **phu hop** voi phan dieu tra: loi hien tai xay ra do Web dang doc connection string tro den DB/instance khac.

### 2.2 Bang chung ve cau hinh ket noi bi lech

1. `ClinicBooking.Web/appsettings.json` dang dung:

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ClinicBookingDb;Trusted_Connection=true;MultipleActiveResultSets=true"
```

2. `ClinicBooking.Api/appsettings.json` dang dung:

```json
"DefaultConnection": "Server=POTATO;Database=ClinicBooking;Trusted_Connection=True;TrustServerCertificate=True"
```

3. Kiem tra LocalDB:

- Instance `(localdb)\\mssqllocaldb` co ton tai.
- Nhung truy van DB `ClinicBookingDb` tra ve **khong co du lieu** (database nay khong ton tai tren instance do).

4. Kiem tra SQL Server `POTATO`:

- Database `ClinicBooking` ton tai (khop voi xac nhan tu SSMS).

5. Chay thu voi override tam thoi chuoi ket noi cho Web:

```powershell
$env:ConnectionStrings__DefaultConnection="Server=POTATO;Database=ClinicBooking;Trusted_Connection=True;TrustServerCertificate=True"
dotnet run --project ClinicBooking.Web
```

Ket qua: ung dung khoi dong thanh cong, nghe tai `http://localhost:5181`.

## 3) Nguyen nhan goc (root cause)

`ClinicBooking.Web` dang tro den DB `ClinicBookingDb` tren LocalDB, trong khi du lieu thuc te dang nam o DB `ClinicBooking` tren SQL Server `POTATO`.

Noi cach khac: **sai khop giua ten DB/server trong connection string cua Web va he CSDL dang dung thuc te**.

Giai thich ngan gon: ton tai DB trong SSMS khong dam bao Web se dung DB do; Web se dung dung chuoi ket noi no duoc nap luc runtime.

## 4) Vi tri code gay fail

- `ClinicBooking.Web/Program.cs`: goi `await app.SeedDatabaseAsync();` ngay khi startup.
- `ClinicBooking.Infrastructure/Persistence/DatabaseSeeder.cs`: method `FixMatKhauAdminAsync(...)` truy van bang `TaiKhoan`.

Vi DB khong mo duoc, seeder nem exception va lam app dung ngay luc khoi dong.

## 5) De xuat cach giai quyet

### Phuong an A (khuyen nghi cho may dev hien tai)

Dong bo connection string cua `ClinicBooking.Web` voi DB dang ton tai (`POTATO` + `ClinicBooking`).

Cap nhat `ClinicBooking.Web/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=POTATO;Database=ClinicBooking;Trusted_Connection=True;TrustServerCertificate=True"
}
```

Hoac dat qua Environment Variable/User Secrets de tranh hardcode theo may:

```powershell
$env:ConnectionStrings__DefaultConnection="Server=POTATO;Database=ClinicBooking;Trusted_Connection=True;TrustServerCertificate=True"
```

Tinh trang kiem chung: da test override theo cach nay, Web khoi dong thanh cong.

### Phuong an B (neu muon dung LocalDB)

Giu connection string LocalDB, sau do tao DB dung ten `ClinicBookingDb` bang migration:

```bash
dotnet ef database update --project ClinicBooking.Infrastructure --startup-project ClinicBooking.Web
```

Luu y: phuong an nay tao DB moi tren LocalDB, du lieu se khac voi DB `ClinicBooking` tren `POTATO`.

### Phuong an C (de giam loi startup trong tuong lai)

Bo sung xu ly loi an toan quanh startup seeding (try/catch + log canh bao), de khi loi ket noi DB thi app khong crash dot ngot trong moi truong dev.

## 6) Kien nghi thuc thi ngay

1. Chon 1 nguon DB duy nhat cho local dev (de xuat: `POTATO/ClinicBooking` neu team dang dung chung).
2. Dong bo `DefaultConnection` giua `ClinicBooking.Api` va `ClinicBooking.Web` theo nguon DB da chon.
3. Chay lai:

```bash
dotnet run --project ClinicBooking.Web
```

4. Neu can, bo sung huong dan cau hinh user-secrets/env trong tai lieu dev de tranh lech cau hinh giua cac project.

## 6.1) Checklist xac nhan sau khi sua

1. In gia tri connection string ma `ClinicBooking.Web` dang nap (chi in server + database, khong in thong tin nhay cam).
2. Chay `dotnet run --project ClinicBooking.Web` va xac nhan app len o `http://localhost:5181`.
3. Dang nhap bang tai khoan dev de xac nhan truy van DB thanh cong.

## 7) Ket luan

Loi hien tai la loi cau hinh ket noi CSDL cua `ClinicBooking.Web` (runtime startup), khong phai loi build.
DB `ClinicBooking` da ton tai, nhung `ClinicBooking.Web` dang tro sai dich (`(localdb)\\mssqllocaldb` + `ClinicBookingDb`).
Khi tro dung ve DB `ClinicBooking` tren `POTATO`, ung dung chay binh thuong.
