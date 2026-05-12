# Yeu cau Module 3 — UI Tra cuu Ho so Benh nhan (Cho Le tan)

Nguoi gui: Module 1 (Le tan portal)
Ngay gui: 2026-05-12

---

## Boi canh

Le tan can xem thong tin ho so benh nhan (lich su kham, bao hiem) de ho tro tiep nhan va xu ly lich hen.
Hien tai trong LeTan/QuanLyLichHen khi xem chi tiet lich hen, khong co thong tin lich su kham cua benh nhan.

## Yeu cau

### Trang moi (Module 3 implement)

**Trang**: `LeTan/HoSoBenhNhan` hoac `BenhNhan/XemHoSo` (Module 3 quyet dinh ten)

**Quyen truy cap**: `le_tan` (chi xem, khong sua)

**Noi dung can hien thi**:
1. Thong tin co ban: ten, ngay sinh, gioi tinh, CCCD, so dien thoai
2. Lich su kham: danh sach cac lan kham (ngay, bac si, chan doan, ket qua) — phan trang, moi lan kham 1 row
3. Thong tin bao hiem (neu co): so the, noi cap, han su dung

**API / Query can co (Module 3 cung cap)**:
- `GET /api/benh-nhan/{idBenhNhan}/ho-so` hoac MediatR query tuong duong

**Cach Module 1 su dung**:
- Trong modal chi tiet lich hen cua LeTan/QuanLyLichHen, them nut "Xem ho so" link sang trang tren
- URL dang: `/BenhNhan/XemHoSo?idBenhNhan={idBenhNhan}`

## Rang buoc

- Le tan chi duoc XEM, khong cap nhat ho so
- Can kiem tra quyen: chi role `le_tan` va `admin` moi truy cap duoc
- Co the hien thi toi da 10 lan kham gan nhat

## Do uu tien

TRUNG BINH — can truoc khi hoan thien portal le tan (Module 1 Phase 2)
