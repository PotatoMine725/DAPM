# Mo ta giao dien tong hop

Tai lieu nay tong hop toan bo cac file giao dien trong workspace de mo ta:
- Kieu giao dien va luong su dung
- Cau truc layout va cach bo tri element
- Bang mau dang su dung
- Cac style va pattern can luu y khi mo rong/chinh sua

## 1) Tong quan kien truc UI

Du an hien co 4 cum giao dien lon:
- Cum Le tan: `index.html`
- Cum Benh nhan (portal): `benhnhan.html`
- Cum Bac si (portal): `doctor-portal.html`
- Cum Quan tri Admin Van: toan bo trong `Admin_Van/Admin_Van/`

Dac diem chung:
- Huong toi dashboard y te: card-based, table-heavy, badge/trang thai ro rang.
- Chuan bo cuc pho bien: `sidebar + header(topbar) + content`.
- Nhieu thao tac trong modal (them/sua/duyet/huy).
- Tap trung vao nghiep vu dat lich, lich lam viec, hang cho, ho so, thong bao.

## 2) He thong layout va bo cuc element

### 2.1 Pattern bo cuc chung
- Sidebar ben trai: dieu huong module, icon + label, co trang thai active.
- Header tren cung: tieu de trang, thong tin user, thong bao, search nhanh.
- Content chinh:
- Khoi KPI/Stats o tren.
- Khoi table/list/card o giua.
- Modal cho create/update/detail/confirm.

### 2.2 Cac thanh phan lap lai
- Card: nen trang, bo goc vua/lon, border nhe + shadow nhe.
- Table: header mau subtle, hover row, cot trang thai dung badge.
- Form: label ro, `input/select/textarea` bo goc, focus ring xanh.
- Button:
- Primary: xanh duong (hanh dong chinh).
- Ghost/Outline: thao tac phu.
- Danger: canh bao/huy/xoa.
- Badge/Status:
- Success cho thanh cong/hoat dong.
- Warning cho cho duyet/nhac.
- Danger cho huy/loi.
- Neutral cho trang thai trung gian.

## 3) Bang mau su dung

### 3.1 Cum Le tan (`index.html`)
- Primary: `#1a5fa8`
- Primary dark: `#155190`
- Primary light: `#E6F1FB`
- Primary mid: `#378ADD`
- Page bg: `#f5f6f8`
- Subtle: `#eef1f5`
- Success bg/text: `#EAF3DE` / `#2e6930`
- Warning bg/text: `#FAEEDA` / `#8c6a15`
- Danger bg/text: `#FCEBEB` / `#a12b2b`
- Neutral bg/text: `#F1EFE8` / `#5f6368`

### 3.2 Cum Benh nhan (`benhnhan.html`)
- Primary: `#3B6BA8`
- Primary dark: `#2E5C99`
- Primary light: `#E8F0F9`
- Primary mid: `#5A8BC4`
- Page bg: `#F8F9FC`
- Text primary/secondary: `#2C3E50` / `#5A6C7D`
- Dung them gradient nen + glassmorphism nhe cho card/auth shell.

### 3.3 Cum Bac si (`doctor-portal.html`)
- Primary: `#1a5fa8`
- Primary dark: `#155190`
- Primary light: `#E6F1FB`
- Primary mid: `#378ADD`
- Border: `#d8dde6`
- Success/warning/danger theo nhom mau y te nhe.
- Sidebar mau dam, noi dung mau sang de nhan cap dieu huong.

### 3.4 Cum Admin Van (`Admin_Van/Admin_Van/_shared.css`)
- Primary: `#1a5fa8`
- Primary dark: `#155190`
- Primary light: `#e6f1fb`
- Primary mid: `#378add`
- Page bg: `#f5f6f8`
- Border: `#dde3ed`
- Trang thai su dung mau nhat quan voi 3 cum con lai.

## 4) Typography, icon, motion, responsive

Typography:
- `index.html`: Inter
- Admin Van: Be Vietnam Pro + Lexend
- `benhnhan.html`: system stack
- `doctor-portal.html`: Inter/Segoe/UI stack

Icon:
- `index.html`: Phosphor Icons
- Admin Van: Font Awesome 6.5
- `doctor-portal.html` va `benhnhan.html`: chu yeu SVG inline + mot so icon style utility

Animation/motion:
- Fade in cho view (`fadeIn`, `fade-up`).
- Hover transition nhe cho card/button/row.
- Dropdown/modal co animation vao/ra.
- Counter animate cho KPI o mot so trang.

Responsive:
- `benhnhan.html`: media query `max-width: 1080px`, `max-width: 720px`.
- `doctor-portal.html`: media query `max-width: 1100px`.
- Admin Van: responsive muc vua-phai, table va grid co co gian, nhung desktop-first ro rang.

## 5) Mo ta theo tung file giao dien

### 5.1 `index.html` (Le tan)
- Layout: sidebar trai co menu 4 module + logout, header tren cung, content chia theo view an/hien.
- View chinh:
- Dashboard (tong lich hen, pending online, hang cho).
- Quan ly lich hen (tim kiem, loc trang thai, tao lich tai quay).
- Hang cho kham (STT, phong, bac si, goi ten).
- Ho so benh nhan (tra cuu, cap nhat, tao moi).
- Modal:
- Lap/cap nhat ho so benh an.
- Tao lich tai quay.
- In phieu tiep nhan check-in.
- Doi lich.
- Style can chu y:
- Table nghiep vu day dac, button hanh dong theo ngu canh.
- Trang thai dung chip mau ro rang (`da_xac_nhan`, `cho_xac_nhan`, `da_check_in`).
- Form 2 cot o modal de toi uu nhap lieu.

### 5.2 `benhnhan.html` (Portal benh nhan)
- Layout tong:
- `auth-shell` cho dang nhap/dang ky + OTP.
- `system-shell` cho he thong sau dang nhap: menu + topbar + view.
- Menu chinh:
- Dashboard
- Danh sach bac si
- Dat lich
- Lich kham
- Thong bao
- Ho so ca nhan
- Luong chinh:
- Tim bac si theo chuyen khoa.
- Chon dich vu, ngay, slot, trieu chung, ghi chu.
- Theo doi lich kham da tao va trang thai.
- Quan ly thong bao va cap nhat profile.
- Style can chu y:
- Mau nen co gradient, card co blur/backdrop nhe.
- Bo cuc dang dashboard hien dai: grid tong hop + detail panel.
- Dieu huong theo state render JS, khong tach trang html.

### 5.3 `doctor-portal.html` (Portal bac si)
- Layout: sidebar collapse duoc, header co notification dropdown, content theo page state.
- Page chinh:
- `page-dashboard`: KPI, lich hom nay/sap toi, mini week schedule.
- `page-workschedule`: lich tuan dang calendar grid + thong ke.
- `page-appointments`: table lich hen + chip loc + pagination.
- `page-waiting`: danh sach cho + panel chi tiet benh nhan.
- `page-schedule`: dang ky lich tuan, chon phong/ca, tong hop ca da chon.
- Modal/chuc nang:
- Chi tiet lich hen, confirm thao tac, quan ly submit dang ky.
- Style can chu y:
- Kien truc data-driven tu JS render tung page.
- Nhieu chip/filter/trang thai, hop voi nghiep vu van hanh thoi gian thuc.

### 5.4 `Admin_Van/Admin_Van/_shared.css`
- File style nen tang cho toan bo cum admin.
- Dinh nghia token mau, radius, shadow, typography, button, table, form, modal, tabs, pagination, toast.
- Ho tro sidebar collapsed + header + dropdown avatar + settings panel.

### 5.5 `Admin_Van/Admin_Van/_shared.js`
- Dung shell chung: render sidebar + header + panel settings + avatar dropdown + modal logout.
- Helper chung:
- Modal open/close.
- Toast.
- Switch tab.
- Animate number.
- Luu/restore setting UI qua localStorage (theme/font-size/sidebar).

### 5.6 `Admin_Van/Admin_Van/index.html` (Dashboard admin)
- KPI cards tong quan.
- Bang lich hen gan day.
- Bieu do mini 7 ngay + panel ca cho duyet.
- Dashboard theo huong quan sat nhanh + dieu huong sang trang nghiep vu.

### 5.7 `Admin_Van/Admin_Van/accounts.html`
- Quan ly tai khoan: tim, loc role/status, table + pagination.
- Tab phan quyen vai tro (Admin, Le tan, Bac si, Benh nhan).
- Modal: them, sua, vo hieu hoa tai khoan.

### 5.8 `Admin_Van/Admin_Van/bac-si.html`
- Dang card theo bac si, filter theo chuyen khoa/loai hop dong/trang thai.
- Modal da tab: thong tin ca nhan + hanh nghe.
- Modal xem chi tiet va sua ho so.

### 5.9 `Admin_Van/Admin_Van/chuyen-khoa.html`
- KPI nho + table danh muc chuyen khoa.
- Cau hinh slot mac dinh, booking window, trang thai hien thi.
- Modal: them/sua/xem chi tiet chuyen khoa.

### 5.10 `Admin_Van/Admin_Van/dich-vu.html`
- KPI + table dich vu theo chuyen khoa.
- Co progress mini cho luot su dung.
- Modal them/sua dich vu va tuy chon hien thi cho benh nhan.

### 5.11 `Admin_Van/Admin_Van/ca-lam-viec.html`
- Man hinh phuc tap nhat cum admin.
- Co 2 view: week calendar va list.
- Co legend mau cho ca sang/chieu/toi va trang thai duyet.
- Modal chi tiet ca, tao/sua ca, huy ca.

### 5.12 `Admin_Van/Admin_Van/duyet-ca.html`
- Tap trung xu ly workflow duyet ca hop dong.
- Bang pending + panel lich su quyet dinh + luu y van hanh.
- Modal: xac nhan duyet, tu choi, xem chi tiet yeu cau.

### 5.13 `Admin_Van/Admin_Van/lich-noi-tru.html`
- Bo cuc 2 cot: danh sach bac si noi tru va bang toggle lich tuan.
- Co phan cau hinh ca mac dinh (phong, slot, so slot).
- Co bang don xin nghi phep va xu ly duyet/tu choi.

### 5.14 `Admin_Van/Admin_Van/phong.html`
- Co 2 cach xem: card view va table view.
- Quan ly trang thai phong (hoat dong/bao tri), trang bi, suc chua.
- Modal: them/sua phong, chuyen bao tri.

### 5.15 `Admin_Van/Admin_Van/thong-bao.html`
- Bo cuc 2 cot: bo loc ben trai + danh sach thong bao ben phai.
- Co trang thai unread, filter theo loai va kenh (SMS/Email/App).
- Modal chi tiet thong bao va modal gui thong bao thu cong (ca nhan/nhom).

### 5.16 `Admin_Van/Admin_Van/thong-ke.html`
- KPI + nhieu khoi chart:
- Bar chart theo ngay.
- Donut trang thai lich hen.
- Horizontal bar theo chuyen khoa.
- Bang top bac si.
- Khoi benh nhan moi theo tuan + phan tich huy lich.
- Toan bo chart dung HTML/CSS/JS render thu cong (khong dung thu vien chart ngoai).

## 6) Cac style can dac biet luu y khi phat trien tiep

- Tinh nhat quan mau:
- Cum admin + le tan + bac si da dong bo palette xanh y te.
- Cum benh nhan dung bien the nhe hon va gradient, can giu tinh than than thien.

- Tinh nhat quan thanh phan:
- Nen tai su dung class utility co san trong `_shared.css` thay vi viet inline style qua nhieu.
- Badge/trang thai nen map 1-1 voi nghiep vu, tranh dung mau khong nhat quan.

- Tranh vo layout:
- Cac trang dang desktop-first. Khi them cot table hoac card, can test o breakpoint nho.
- Modal co nhieu form dai, can giu max-height + overflow-y nhu hien tai.

- Tuong tac quan trong:
- Kiem tra hover/focus/disabled state cho input/button.
- Giu ro role cua button chinh/phu/nguy hiem (primary/ghost/danger).

- Tinh de doc du lieu y te:
- Typography, spacing, contrast hien tai kha tot cho nghiep vu.
- Uu tien thong tin chinh o cot dau (ma lich, ten BN, bac si, trang thai).

## 7) Danh sach file da duoc tong hop

- `index.html`
- `benhnhan.html`
- `doctor-portal.html`
- `Admin_Van/Admin_Van/_shared.css`
- `Admin_Van/Admin_Van/_shared.js`
- `Admin_Van/Admin_Van/index.html`
- `Admin_Van/Admin_Van/accounts.html`
- `Admin_Van/Admin_Van/bac-si.html`
- `Admin_Van/Admin_Van/chuyen-khoa.html`
- `Admin_Van/Admin_Van/dich-vu.html`
- `Admin_Van/Admin_Van/ca-lam-viec.html`
- `Admin_Van/Admin_Van/duyet-ca.html`
- `Admin_Van/Admin_Van/lich-noi-tru.html`
- `Admin_Van/Admin_Van/phong.html`
- `Admin_Van/Admin_Van/thong-bao.html`
- `Admin_Van/Admin_Van/thong-ke.html`
