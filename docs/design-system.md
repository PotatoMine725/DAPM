# Tài liệu Design System — Hệ thống đặt lịch khám bệnh

> Phiên bản 1.0 — Áp dụng cho toàn bộ giao diện HTML+CSS+JS của dự án.  
> Mọi thành viên **bắt buộc** đọc và tuân thủ trước khi bắt đầu code giao diện.

---

## Mục lục

1. [Nguyên tắc chung](#1-nguyên-tắc-chung)
2. [Bảng màu](#2-bảng-màu)
3. [Typography](#3-typography)
4. [Spacing & Border Radius](#4-spacing--border-radius)
5. [Layout](#5-layout)
6. [Components](#6-components)
7. [Trạng thái & Badge](#7-trạng-thái--badge)
8. [Quy tắc đặt tên & cấu trúc file](#8-quy-tắc-đặt-tên--cấu-trúc-file)

---

## 1. Nguyên tắc chung

- Phong cách: **chuyên nghiệp, tối giản, y tế** — nền trắng, màu chủ đạo xanh dương, không dùng màu trang trí tuỳ hứng.
- Nền tảng: **Web desktop**, độ rộng tối thiểu 1280px, không cần responsive mobile.
- Mỗi trang chỉ có **một nút Primary** duy nhất (hành động chính). Các hành động phụ dùng Secondary hoặc Ghost.
- Không dùng `box-shadow` trang trí — chỉ dùng cho focus ring (`0 0 0 3px`).
- Không dùng `!important` trong CSS.
- Không dùng gradient hoặc hiệu ứng blur làm nền.
- Thông báo lỗi form đặt **ngay dưới input** liên quan — không dùng alert popup cho lỗi validation.
- Mọi ảnh bác sĩ/bệnh nhân phải có fallback về **avatar chữ cái (initials)** nếu không load được.
- Ngày giờ thống nhất định dạng `DD/MM/YYYY` và `HH:mm` trên toàn hệ thống.
- Mã lịch hẹn, CCCD, số điện thoại luôn dùng font `monospace`.
- Trường bắt buộc đánh dấu `*` màu đỏ (`#A32D2D`) cạnh label — không viết chữ "(bắt buộc)".
- Icon dùng thống nhất bộ **Feather Icons** hoặc **Heroicons** (SVG inline) — không dùng font icon.

---

## 2. Bảng màu

> Chỉ dùng các giá trị màu trong bảng này. Không tự ý thêm màu ngoài danh sách.  
> Khai báo tất cả màu dưới dạng biến CSS trong `:root` của file `common.css`.

### 2.1 Màu chính (Primary)

| Tên biến               | Hex       | Mô tả sử dụng                          |
|------------------------|-----------|----------------------------------------|
| `--color-primary`      | `#1a5fa8` | Nút chính, sidebar, accent chủ đạo     |
| `--color-primary-dark` | `#155190` | Hover state trên nền sáng              |
| `--color-primary-mid`  | `#378ADD` | Link, icon nhấn mạnh, đường viền focus |
| `--color-primary-light`| `#E6F1FB` | Nền badge info, focus ring, highlight  |

### 2.2 Màu nền (Background)

| Tên biến              | Hex       | Mô tả sử dụng                            |
|-----------------------|-----------|------------------------------------------|
| `--color-bg-white`    | `#ffffff` | Nền card, modal, input, sidebar trắng    |
| `--color-bg-page`     | `#f5f6f8` | Nền trang chính, nền bảng dữ liệu        |
| `--color-bg-subtle`   | `#eef1f5` | Hover row bảng, nền section phụ          |

### 2.3 Màu ngữ nghĩa (Semantic)

| Tên biến                  | Hex (nền) | Hex (chữ) | Ý nghĩa                        |
|---------------------------|-----------|-----------|--------------------------------|
| `--color-success-bg`      | `#EAF3DE` | `#27500A` | Đã xác nhận, hoàn thành        |
| `--color-success-border`  | `#639922` | —         | Viền thành công                |
| `--color-warning-bg`      | `#FAEEDA` | `#633806` | Chờ xác nhận, sắp hết slot     |
| `--color-warning-border`  | `#BA7517` | —         | Viền cảnh báo                  |
| `--color-danger-bg`       | `#FCEBEB` | `#791F1F` | Đã hủy, lỗi, hạn chế          |
| `--color-danger-border`   | `#E24B4A` | —         | Viền lỗi/nguy hiểm             |
| `--color-neutral-bg`      | `#F1EFE8` | `#444441` | Trung lập (không đến, chờ khám)|
| `--color-neutral-border`  | `#888780` | —         | Viền trung lập                 |
| `--color-active-bg`       | `#E1F5EE` | `#085041` | Đang thực hiện (đang khám...)  |
| `--color-active-border`   | `#1D9E75` | —         | Viền đang hoạt động            |

### 2.4 Màu chữ (Text)

| Tên biến                  | Hex       | Mô tả sử dụng                             |
|---------------------------|-----------|-------------------------------------------|
| `--color-text-primary`    | `#1a1a1a` | Tiêu đề, nội dung chính                   |
| `--color-text-secondary`  | `#5f6368` | Nhãn, mô tả phụ, placeholder              |
| `--color-text-disabled`   | `#9aa0a6` | Disabled, hint, caption                   |
| `--color-text-link`       | `#1a5fa8` | Link, ghost button                        |
| `--color-text-error`      | `#A32D2D` | Thông báo lỗi, dấu * bắt buộc            |

### 2.5 Màu viền (Border)

| Tên biến                 | Hex       | Mô tả sử dụng                       |
|--------------------------|-----------|-------------------------------------|
| `--color-border-light`   | `#e8eaed` | Viền card, bảng, divider            |
| `--color-border-medium`  | `#dadce0` | Viền input, button secondary        |
| `--color-border-focus`   | `#378ADD` | Viền input khi focus                |

### 2.6 Khai báo biến CSS mẫu (`common.css`)

```css
:root {
  /* Primary */
  --color-primary:       #1a5fa8;
  --color-primary-dark:  #155190;
  --color-primary-mid:   #378ADD;
  --color-primary-light: #E6F1FB;

  /* Background */
  --color-bg-white:  #ffffff;
  --color-bg-page:   #f5f6f8;
  --color-bg-subtle: #eef1f5;

  /* Semantic — nền */
  --color-success-bg:     #EAF3DE;
  --color-warning-bg:     #FAEEDA;
  --color-danger-bg:      #FCEBEB;
  --color-neutral-bg:     #F1EFE8;
  --color-active-bg:      #E1F5EE;

  /* Semantic — chữ */
  --color-success-text:   #27500A;
  --color-warning-text:   #633806;
  --color-danger-text:    #791F1F;
  --color-neutral-text:   #444441;
  --color-active-text:    #085041;

  /* Semantic — viền */
  --color-success-border: #639922;
  --color-warning-border: #BA7517;
  --color-danger-border:  #E24B4A;
  --color-neutral-border: #888780;
  --color-active-border:  #1D9E75;

  /* Text */
  --color-text-primary:   #1a1a1a;
  --color-text-secondary: #5f6368;
  --color-text-disabled:  #9aa0a6;
  --color-text-link:      #1a5fa8;
  --color-text-error:     #A32D2D;

  /* Border */
  --color-border-light:   #e8eaed;
  --color-border-medium:  #dadce0;
  --color-border-focus:   #378ADD;
}
```

---

## 3. Typography

### 3.1 Font

```css
font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif;
```

Không import font ngoài (Google Fonts, v.v.) — dùng font hệ thống để đảm bảo nhất quán và tốc độ tải.

### 3.2 Thang chữ

| Vai trò           | Size  | Weight | Color                   | Sử dụng                              |
|-------------------|-------|--------|-------------------------|--------------------------------------|
| Page title (H1)   | 24px  | 600    | `--color-text-primary`  | Tiêu đề trang, tiêu đề modal lớn     |
| Section title (H2)| 18px  | 600    | `--color-text-primary`  | Tiêu đề section, tiêu đề card lớn   |
| Card title (H3)   | 15px  | 500    | `--color-text-primary`  | Tiêu đề card, tiêu đề panel         |
| Body              | 14px  | 400    | `--color-text-primary`  | Nội dung chính, dữ liệu bảng        |
| Label / Small     | 13px  | 400    | `--color-text-secondary`| Nhãn form, mô tả phụ, badge text    |
| Caption / Hint    | 12px  | 400    | `--color-text-disabled` | Gợi ý input, timestamp, chú thích   |
| Monospace         | 13px  | 400    | `--color-text-link`     | Mã lịch hẹn, CCCD, số điện thoại    |

### 3.3 Quy tắc

- Chỉ dùng **2 font-weight chính**: 400 (regular) và 600 (bold). Thêm 500 nếu cần tiêu đề card.
- Tiêu đề trang **căn trái** — không căn giữa trừ modal xác nhận.
- **Không viết hoa toàn bộ (ALL CAPS)** trừ nhãn cột bảng dữ liệu (`text-transform: uppercase`, `font-size: 12px`).
- `line-height` mặc định: `1.6` cho body, `1.4` cho tiêu đề, `1.5` cho label.
- Placeholder input dùng `--color-text-disabled`, không dùng màu primary.

---

## 4. Spacing & Border Radius

### 4.1 Hệ thống spacing

Chỉ dùng các giá trị sau. Không dùng giá trị tuỳ hứng như 7px, 11px, 17px.

| Token      | Giá trị | Sử dụng điển hình                                       |
|------------|---------|---------------------------------------------------------|
| `--space-1`| 4px     | Gap giữa icon và text, giữa các badge nhỏ              |
| `--space-2`| 8px     | Gap trong form inline, giữa các nút hành động           |
| `--space-3`| 12px    | Padding trong badge/chip, gap giữa card nhỏ            |
| `--space-4`| 16px    | Padding trong card, gap giữa section trong form        |
| `--space-5`| 20px    | Padding ngang nội dung chính                           |
| `--space-6`| 24px    | Gap giữa card lớn, margin giữa các section             |
| `--space-8`| 32px    | Khoảng cách giữa các block lớn trên trang              |

```css
:root {
  --space-1: 4px;
  --space-2: 8px;
  --space-3: 12px;
  --space-4: 16px;
  --space-5: 20px;
  --space-6: 24px;
  --space-8: 32px;
}
```

### 4.2 Border Radius

| Token        | Giá trị | Sử dụng                              |
|--------------|---------|--------------------------------------|
| `--radius-sm`| 4px     | Input, badge, tag nhỏ, tooltip       |
| `--radius-md`| 8px     | Nút bấm, dropdown, select            |
| `--radius-lg`| 12px    | Card, panel, modal                   |
| `--radius-xl`| 20px    | Badge pill (kiểu viên thuốc)         |
| `--radius-full`| 50%   | Avatar, icon button tròn             |

```css
:root {
  --radius-sm:   4px;
  --radius-md:   8px;
  --radius-lg:   12px;
  --radius-xl:   20px;
  --radius-full: 50%;
}
```

---

## 5. Layout

### 5.1 Cấu trúc tổng thể

```
┌─────────────────────────────────────────────────┐
│  Sidebar (220px)  │  Main Content Area           │
│  bg: #1a5fa8      │  bg: #f5f6f8                 │
│                   │  ┌─────────────────────────┐ │
│  Logo             │  │ Topbar (52px, bg: white) │ │
│  Nav items        │  └─────────────────────────┘ │
│                   │  ┌─────────────────────────┐ │
│                   │  │ Content (padding: 24px)  │ │
│                   │  └─────────────────────────┘ │
└─────────────────────────────────────────────────┘
```

- **Sidebar**: rộng `220px`, nền `#1a5fa8`, text trắng, cố định (position fixed hoặc sticky).
- **Main content**: chiếm phần còn lại, nền `#f5f6f8`, padding ngang `24px`.
- **Topbar**: cao `52px`, nền `#ffffff`, `border-bottom: 1px solid var(--color-border-light)`.
- **Tổng chiều rộng tối thiểu**: `1280px`.

### 5.2 Sidebar chi tiết

```css
.sidebar {
  width: 220px;
  background: var(--color-primary);
  height: 100vh;
  position: fixed;
  left: 0; top: 0;
  padding: 20px 12px;
}

.sidebar-nav-item {
  color: rgba(255, 255, 255, 0.75);
  font-size: 13px;
  padding: 8px 10px;
  border-radius: var(--radius-md);
  margin-bottom: 2px;
  cursor: pointer;
}

.sidebar-nav-item:hover,
.sidebar-nav-item.active {
  background: rgba(255, 255, 255, 0.15);
  color: #ffffff;
}
```

### 5.3 Grid nội dung

| Trường hợp                  | Grid / Width                        |
|-----------------------------|-------------------------------------|
| Metric cards tổng quan      | `repeat(4, 1fr)`, gap 16px          |
| Danh sách + chi tiết        | `2fr 1fr`, gap 20px                 |
| Form đơn (đặt lịch, hồ sơ) | `max-width: 640px`, căn giữa        |
| Bảng dữ liệu                | Full width trong content area       |
| Form 2 cột (thông tin cá nhân) | `repeat(2, 1fr)`, gap 16px       |

---

## 6. Components

### 6.1 Nút bấm (Button)

| Loại      | Dùng khi                               | Style                                               |
|-----------|----------------------------------------|-----------------------------------------------------|
| Primary   | Hành động chính duy nhất của trang     | `bg: #1a5fa8`, text trắng, không có viền            |
| Secondary | Hành động phụ (quay lại, đóng...)     | `bg: white`, `border: 1px solid #dadce0`, text đen  |
| Danger    | Hủy/xóa không thể phục hồi            | `bg: white`, `border: 1px solid #F09595`, text đỏ  |
| Ghost     | Điều hướng, link hành động            | Không nền, không viền, text `#1a5fa8`, underline    |

```css
.btn {
  padding: 8px 18px;
  border-radius: var(--radius-md);
  font-size: 13px;
  font-weight: 500;
  cursor: pointer;
  transition: background 0.15s, opacity 0.15s;
}

.btn-primary   { background: var(--color-primary); color: #fff; border: none; }
.btn-primary:hover { background: var(--color-primary-dark); }

.btn-secondary { background: var(--color-bg-white); color: var(--color-text-primary); border: 1px solid var(--color-border-medium); }
.btn-secondary:hover { background: var(--color-bg-subtle); }

.btn-danger    { background: var(--color-bg-white); color: var(--color-danger-text); border: 1px solid #F09595; }
.btn-danger:hover { background: var(--color-danger-bg); }

.btn-ghost     { background: transparent; color: var(--color-text-link); border: none; text-decoration: underline; padding: 8px 4px; }

.btn:disabled  { opacity: 0.5; cursor: not-allowed; }
```

### 6.2 Input & Form

```css
.form-label {
  display: block;
  font-size: 13px;
  font-weight: 500;
  color: var(--color-text-primary);
  margin-bottom: 5px;
}

.form-input,
.form-select,
.form-textarea {
  width: 100%;
  padding: 8px 12px;
  border: 1px solid var(--color-border-medium);
  border-radius: var(--radius-md);
  font-size: 14px;
  color: var(--color-text-primary);
  background: var(--color-bg-white);
  outline: none;
  transition: border-color 0.15s, box-shadow 0.15s;
}

.form-input:focus {
  border-color: var(--color-border-focus);
  box-shadow: 0 0 0 3px var(--color-primary-light);
}

.form-input.error {
  border-color: var(--color-danger-border);
  box-shadow: 0 0 0 3px var(--color-danger-bg);
}

.form-hint  { font-size: 12px; color: var(--color-text-disabled); margin-top: 4px; }
.form-error { font-size: 12px; color: var(--color-text-error);    margin-top: 4px; }

.form-group { margin-bottom: 16px; }

.required-mark { color: var(--color-text-error); margin-left: 2px; }
```

### 6.3 Card

```css
.card {
  background: var(--color-bg-white);
  border: 1px solid var(--color-border-light);
  border-radius: var(--radius-lg);
  padding: 16px 20px;
}

.card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding-bottom: 12px;
  margin-bottom: 14px;
  border-bottom: 1px solid var(--color-border-light);
}

.card-title {
  font-size: 15px;
  font-weight: 500;
  color: var(--color-text-primary);
}
```

### 6.4 Bảng dữ liệu (Table)

```css
.data-table { width: 100%; border-collapse: collapse; font-size: 14px; }

.data-table th {
  background: var(--color-bg-subtle);
  padding: 10px 14px;
  text-align: left;
  font-size: 12px;
  font-weight: 600;
  color: var(--color-text-secondary);
  text-transform: uppercase;
  letter-spacing: 0.04em;
  border-bottom: 1px solid var(--color-border-medium);
}

.data-table td {
  padding: 11px 14px;
  border-bottom: 1px solid var(--color-border-light);
  color: var(--color-text-primary);
  vertical-align: middle;
}

.data-table tr:last-child td { border-bottom: none; }
.data-table tr:hover td { background: var(--color-bg-subtle); }

.table-code { font-family: monospace; font-size: 13px; color: var(--color-text-link); }
```

### 6.5 Modal

- Overlay: `rgba(0, 0, 0, 0.4)`, toàn màn hình.
- Modal box: nền trắng, `border-radius: var(--radius-lg)`, `max-width: 480px`, căn giữa màn hình.
- Tiêu đề modal căn giữa nếu là modal xác nhận đơn giản; căn trái nếu là modal form phức tạp.
- Nút hành động: Secondary bên trái (Quay lại), Primary/Danger bên phải (Xác nhận).

```css
.modal-overlay {
  position: fixed;
  inset: 0;
  background: rgba(0, 0, 0, 0.4);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
}

.modal-box {
  background: var(--color-bg-white);
  border-radius: var(--radius-lg);
  padding: 24px;
  width: 100%;
  max-width: 480px;
  border: 1px solid var(--color-border-light);
}

.modal-title   { font-size: 16px; font-weight: 600; margin-bottom: 10px; }
.modal-body    { font-size: 14px; color: var(--color-text-secondary); line-height: 1.6; margin-bottom: 20px; }
.modal-actions { display: flex; gap: 10px; justify-content: flex-end; }
```

### 6.6 Badge / Trạng thái

```css
.badge {
  display: inline-block;
  padding: 3px 10px;
  border-radius: var(--radius-xl);
  font-size: 12px;
  font-weight: 500;
  white-space: nowrap;
}
```

Xem đầy đủ giá trị màu từng badge ở [Mục 7](#7-trạng-thái--badge).

### 6.7 Alert / Thông báo hệ thống

```css
.alert {
  padding: 10px 14px;
  border-radius: 0 var(--radius-md) var(--radius-md) 0;
  font-size: 13px;
  line-height: 1.6;
  border-left: 3px solid;
  margin-bottom: 10px;
}

.alert-success { background: var(--color-success-bg); color: var(--color-success-text); border-color: var(--color-success-border); }
.alert-warning { background: var(--color-warning-bg); color: var(--color-warning-text); border-color: var(--color-warning-border); }
.alert-danger  { background: var(--color-danger-bg);  color: var(--color-danger-text);  border-color: var(--color-danger-border);  }
.alert-info    { background: var(--color-primary-light); color: #0C447C; border-color: var(--color-primary-mid); }
```

### 6.8 Avatar

```css
.avatar {
  width: 36px; height: 36px;
  border-radius: var(--radius-full);
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 13px;
  font-weight: 600;
}

.avatar-blue { background: var(--color-primary-light); color: #0C447C; }
.avatar-teal { background: #E1F5EE; color: #085041; }
.avatar-gray { background: var(--color-bg-subtle);   color: var(--color-text-secondary); }
```

### 6.9 Tabs

```css
.tab-bar {
  display: flex;
  border-bottom: 2px solid var(--color-border-light);
  margin-bottom: 16px;
}

.tab {
  padding: 9px 18px;
  font-size: 14px;
  cursor: pointer;
  color: var(--color-text-secondary);
  border-bottom: 2px solid transparent;
  margin-bottom: -2px;
  transition: color 0.15s, border-color 0.15s;
}

.tab.active {
  color: var(--color-primary);
  border-bottom-color: var(--color-primary);
  font-weight: 500;
}

.tab:hover:not(.active) { color: var(--color-text-primary); }
```

### 6.10 Pagination

```css
.pagination { display: flex; gap: 4px; align-items: center; margin-top: 14px; }

.page-btn {
  width: 32px; height: 32px;
  border-radius: var(--radius-sm);
  border: 1px solid var(--color-border-medium);
  background: var(--color-bg-white);
  font-size: 13px;
  cursor: pointer;
  color: var(--color-text-secondary);
  display: flex; align-items: center; justify-content: center;
}

.page-btn.active { background: var(--color-primary); color: #fff; border-color: var(--color-primary); }
.page-btn:hover:not(.active) { background: var(--color-bg-subtle); }
```

### 6.11 Breadcrumb

```css
.breadcrumb { display: flex; gap: 6px; align-items: center; font-size: 13px; }
.breadcrumb-item { color: var(--color-text-secondary); }
.breadcrumb-item.active { color: var(--color-text-primary); font-weight: 500; }
.breadcrumb-sep { color: var(--color-text-disabled); }
```

---

## 7. Trạng thái & Badge

> Dùng đúng màu theo ngữ nghĩa. Không hoán đổi màu giữa các trạng thái.

### 7.1 Trạng thái lịch hẹn (`lich_hen.trang_thai`)

| Giá trị DB         | Hiển thị          | Nền badge | Chữ badge |
|--------------------|-------------------|-----------|-----------|
| `cho_xac_nhan`     | Chờ xác nhận      | `#FAEEDA` | `#633806` |
| `da_xac_nhan`      | Đã xác nhận       | `#EAF3DE` | `#27500A` |
| `dang_kham`        | Đang khám         | `#E1F5EE` | `#085041` |
| `hoan_thanh`       | Hoàn thành        | `#E6F1FB` | `#0C447C` |
| `benh_nhan_huy`    | Bệnh nhân hủy     | `#FCEBEB` | `#791F1F` |
| `phong_kham_huy`   | Phòng khám hủy    | `#FCEBEB` | `#791F1F` |
| `khong_den`        | Không đến         | `#F1EFE8` | `#444441` |

### 7.2 Trạng thái ca làm việc (`ca_lam_viec.trang_thai_duyet`)

| Giá trị DB   | Hiển thị    | Nền badge | Chữ badge |
|--------------|-------------|-----------|-----------|
| `cho_duyet`  | Chờ duyệt   | `#FAEEDA` | `#633806` |
| `da_duyet`   | Đã duyệt    | `#EAF3DE` | `#27500A` |
| `da_huy`     | Đã hủy      | `#FCEBEB` | `#791F1F` |
| `hoan_thanh` | Hoàn thành  | `#E6F1FB` | `#0C447C` |

### 7.3 Trạng thái hàng chờ (`hang_cho.trang_thai_cho`)

| Giá trị DB   | Hiển thị    | Nền badge | Chữ badge |
|--------------|-------------|-----------|-----------|
| `cho_kham`   | Chờ khám    | `#F1EFE8` | `#444441` |
| `dang_kham`  | Đang khám   | `#E1F5EE` | `#085041` |
| `hoan_thanh` | Hoàn thành  | `#E6F1FB` | `#0C447C` |
| `bo_qua`     | Bỏ qua      | `#FCEBEB` | `#791F1F` |

### 7.4 Trạng thái đặt cọc (`dat_coc.trang_thai`)

| Giá trị DB       | Hiển thị          | Nền badge | Chữ badge |
|------------------|-------------------|-----------|-----------|
| `cho_thanh_toan` | Chờ thanh toán    | `#FAEEDA` | `#633806` |
| `da_thanh_toan`  | Đã thanh toán     | `#EAF3DE` | `#27500A` |
| `da_hoan_tien`   | Đã hoàn tiền      | `#E6F1FB` | `#0C447C` |
| `that_bai`       | Thất bại          | `#FCEBEB` | `#791F1F` |

---

## 8. Quy tắc đặt tên & cấu trúc file

### 8.1 Đặt tên file

| Loại file | Quy tắc              | Ví dụ                    |
|-----------|----------------------|--------------------------|
| HTML      | `kebab-case.html`    | `dat-lich.html`          |
| CSS riêng | `kebab-case.css`     | `quan-ly-lich.css`       |
| JS        | `kebab-case.js`      | `mock-data.js`           |

### 8.2 Đặt tên class CSS

- Dùng `kebab-case` cho tất cả class: `.form-group`, `.card-header`, `.btn-primary`.
- Không dùng `camelCase` hay `PascalCase` cho class CSS.
- Tên class mô tả **chức năng**, không mô tả màu sắc: dùng `.btn-danger` thay vì `.btn-red`.

### 8.3 Cấu trúc thư mục

```
project/
├── css/
│   ├── common.css          ← biến CSS, reset, typography, layout chung
│   ├── components.css      ← button, badge, card, table, form, modal, alert
│   ├── dat-lich.css
│   ├── quan-ly-lich.css
│   ├── ho-so-benh-nhan.css
│   └── ...
├── js/
│   ├── mock-data.js        ← toàn bộ dữ liệu mẫu dùng chung
│   ├── dat-lich.js
│   └── ...
├── assets/
│   └── icons/              ← SVG icon files
└── pages/
    ├── dat-lich.html
    ├── quan-ly-lich.html
    ├── ho-so-benh-nhan.html
    └── ...
```

### 8.4 Thứ tự import trong HTML

```html
<link rel="stylesheet" href="../css/common.css">
<link rel="stylesheet" href="../css/components.css">
<link rel="stylesheet" href="../css/ten-trang.css">
<!-- ... nội dung ... -->
<script src="../js/mock-data.js"></script>
<script src="../js/ten-trang.js"></script>
```

### 8.5 Cấu trúc mock-data.js

```js
// mock-data.js — dữ liệu mẫu dùng chung toàn dự án
// Không tạo dữ liệu mẫu riêng trong từng file JS

const MOCK_BENH_NHAN = [ /* ... */ ];
const MOCK_BAC_SI    = [ /* ... */ ];
const MOCK_LICH_HEN  = [ /* ... */ ];
const MOCK_CA_LAM_VIEC = [ /* ... */ ];
```

---

*Tài liệu này do nhóm 17 biên soạn. Mọi thay đổi cần được thống nhất toàn nhóm trước khi áp dụng.*
