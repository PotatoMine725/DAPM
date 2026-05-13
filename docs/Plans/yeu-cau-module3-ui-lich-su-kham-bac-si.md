# Yeu cau Module 3 — Panel Lich su kham tom tat (Cho BacSi/QuanLyKham)

Nguoi gui: Module 1 (BacSi portal)
Ngay gui: 2026-05-12

---

## Boi canh

Trong trang `BacSi/QuanLyKham`, bac si can xem lich su kham gan nhat cua benh nhan truoc khi nhap chan doan.
Hien tai trang chi co form nhap ho so va ke toa, khong co panel lich su.

## Yeu cau

### API / Query can co (Module 3 implement)

**Query**: `LayLichSuKhamGanNhat(idBenhNhan, soLan = 5)`

**Response**:
```
record LichSuKhamTomTat(
    int IdHoSoKham,
    DateTime NgayKham,
    string HoTenBacSi,
    string? ChanDoan,
    string? KetQuaKham,
    bool CoToaThuoc
)
```

**Endpoint goi y**: `GET /api/benh-nhan/{idBenhNhan}/lich-su-kham?soLan=5`
hoac MediatR query `LayLichSuKhamGanNhatQuery(IdBenhNhan, SoLan)`

### UI Module 1 se implement sau khi co API

Trong `BacSi/QuanLyKham`, them panel ben trai (hoac phia duoi thong tin ho so hien tai):

```
[ Lich su kham ]
| Ngay      | Bac si    | Chan doan     | Co toa |
| 10/05/26  | BS Nguyen | Viem hong cap | Co     |
| 02/04/26  | BS Tran   | Cam cum        | Co     |
| ...       |           |               |        |
```

- Hien thi toi da 5 lan kham gan nhat
- Click vao dong → mo modal xem chi tiet ho so (Module 3 cung cap)
- Neu benh nhan chua co lich su → hien thi "Chua co lich su kham"

### Cach goi data trong BacSi/QuanLyKham

```csharp
// Them vao DashboardModel.OnGetAsync sau khi co IdBenhNhan tu HoSoHienTai
if (Model.HoSoHienTai is not null)
{
    var idBenhNhan = Model.HoSoHienTai.IdBenhNhan; // can bo sung field nay vao HoSoKhamResponse
    LichSuKham = await _mediator.Send(new LayLichSuKhamGanNhatQuery(idBenhNhan, 5));
}
```

## Rang buoc

- Chi bac si dang dang nhap moi goi duoc — khong lo data cua benh nhan khac
- Module 3 responsible cho phan quyen va privacy
- Module 1 chi hien thi, khong chinh sua du lieu Module 3

## Do uu tien

TRUNG BINH — can truoc khi hoan thien portal bac si (Module 1 Phase 2)
