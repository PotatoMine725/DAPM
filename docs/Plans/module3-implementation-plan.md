# Module 3 — Kế hoạch thực hiện chi tiết
> Bệnh nhân, Hồ sơ khám & Kê đơn  
> **Owner:** Member C  
> **Branch:** `feature/module3`  
> **Ngày lập:** 2026-05-05  
> **Effort:** 14–18 ngày (10–14 dev + 4 QA)

---

## 📋 I. Scope & Yêu cầu

### Công việc chính Module 3
1. **BenhNhan** — Quản lý dữ liệu bệnh nhân
2. **HoSoKham** — Hồ sơ khám tạo từ LichHen (sau check-in)
3. **ToaThuoc** — Kê đơn thuốc
4. **Thuoc** — Danh mục thuốc (admin quản lý)

### Điểm phối hợp Module 1
| Trường | Hoạt động | Xác nhận cần |
|---|---|---|
| `BenhNhan.SoLanHuyMuon` | Module 1 increment khi bệnh nhân hủy lịch | **Ngưỡng cấm?** Reset khi nào? (BLOCKING) |

### Ràng buộc bắt buộc
- ✅ Module 3 **chỉ đọc** `LichHen` (không update `TrangThai`)
- ✅ `HoSoKham` phải trỏ về `IdLichHen` hợp lệ với `TrangThai = DangKham | HoanThanh`
- ✅ Không break **241 unit tests + 11 integration tests** Module 1 hiện tại

---

## ⚠️ II. Blocking Points & Critical Path

### 🔴 CRITICAL: Xác nhận `SoLanHuyMuon` logic (Task 1.1)
**Tác động:** Nếu không xác nhận, không thể merge Phase 2  
**Cần xác nhận với Module 1 owner:**
- Ngưỡng tối đa hủy mấy lần? (ví dụ: 5 lần/tháng thì bị cấm?)
- Reset counter khi nào? (hàng tháng? hàng năm? từng lần?)
- Có rule nào phía Module 3 không? (ví dụ: bệnh nhân bị cấm có tạo HoSoKham được không?)
- Lưu trữ log hủy (link BenhNhan + LichHen) để kiểm toán?

**→ IMMEDIATE ACTION:** Tạo issue / PR nhỏ tag Module 1 owner, xác nhận document

---

## 📅 III. Kế hoạch 5 Phase

### **PHASE 1: Xác nhận & Setup** (Dự kiến: 1–2 ngày)
🎯 **Mục đích:** Confirm yêu cầu, verify DB schema, chuẩn bị DTOs & Validators

#### Task 1.1: ⚠️ Confirm `SoLanHuyMuon` logic [BLOCKING]
**Deliverable:** Document xác nhận từ Module 1 owner hoặc PR merged  
**Checklist:**
- [ ] Đọc code `HuyLichHenHandler` (Module 1) xem cách dùng `SoLanHuyMuon`
- [ ] Liên hệ Module 1 owner, gửi list câu hỏi (xem phía trên)
- [ ] Document confirm + PR nhỏ nếu cần thay đổi logic
- [ ] Merge xác nhận vào `develop` trước Phase 2

#### Task 1.2: Verify DB schema & Entities
**Location:** `/database/clinic.dbml` + `ClinicBooking.Domain/Entities/`  
**Checklist:**
- [ ] Kiểm tra schema `BenhNhan` (có `SoLanHuyMuon`? kiểu dữ liệu?)
- [ ] Kiểm tra schema `HoSoKham` (foreign key tới `LichHen`? `BacSi`?)
- [ ] Kiểm tra schema `ToaThuoc` + `ChiTietToaThuoc`
- [ ] Kiểm tra schema `Thuoc` (status, giá, v.v.)
- [ ] Verify các Entity class trong `Domain/Entities/` có đầy đủ chưa
- [ ] **Deliverable:** Checklist + note schema hiểu được

#### Task 1.3: Tạo DTOs & Validators
**Location:** `ClinicBooking.Application/Features/{Entity}/` + `Common/`  
**DTOs cần:**
```
TaoHoSoKhamRequest
CapNhatHoSoKhamRequest
HoSoKhamResponse
LichSuKhamResponse (paginated)

ToaThuocRequest (kèm chi tiết thuốc)
ToaThuocResponse

ThuocRequest
ThuocResponse
```

**Validators:** Dùng FluentValidation cho mỗi DTO  
**Deliverable:** PR nhỏ chứa DTOs + Validators, pass unit tests

---

### **PHASE 2: CRUD HoSoKham** (Dự kiến: 3–5 ngày)
🎯 **Mục đích:** Implement create/read/update cho hồ sơ khám

#### Task 2.1: `TaoHoSoKham` Command & Handler
**Endpoint:** `POST /api/ho-so-kham`  
**Request body:**
```json
{
  "idLichHen": 1,
  "chanDoan": "Cảm cúm thường",
  "loiDan": "Nghỉ ngơi, uống nước ấm",
  "ghiChu": "Tái khám sau 1 tuần"
}
```

**Command logic:**
1. Validate `idLichHen` tồn tại & fetch `LichHen` + `BenhNhan` + `CaLamViec`
2. Validate `LichHen.TrangThai = DangKham` (đang khám)
3. Validate bệnh nhân không bị cấm (check `SoLanHuyMuon` < ngưỡng)
4. Create `HoSoKham` record mới
5. Ghi audit log (user, timestamp, action)
6. Return `HoSoKhamResponse` (với dữ liệu vừa tạo)

**Error handling:**
- `404 Not Found` — LichHen không tồn tại
- `409 Conflict` — LichHen trạng thái sai
- `400 Bad Request` — Bệnh nhân bị cấm, validation failed
- `401 Unauthorized` — Chỉ bác sĩ / admin được tạo

**Tests (unit):**
- ✅ Success case
- ✅ LichHen not found
- ✅ LichHen wrong status
- ✅ Bệnh nhân bị cấm
- ✅ Validation failed (chanDoan empty, v.v.)
- ✅ Authorization failed

**Deliverable:** Handler + 6 test cases

#### Task 2.2: `CapNhatHoSoKham` Command & Handler
**Endpoint:** `PUT /api/ho-so-kham/{id}`  
**Request body:** (tương tự TaoHoSoKham, có thể có thêm trạng thái)

**Logic:**
1. Validate `HoSoKham` tồn tại
2. Validate authorization (chỉ bác sĩ chủ trị hoặc admin được update)
3. Update `ChanDoan`, `LoiDan`, `GhiChu`, v.v.
4. **Không update** `IdLichHen` (immutable)
5. Ghi audit log
6. Return updated `HoSoKhamResponse`

**Tests:**
- ✅ Success case
- ✅ Not found
- ✅ Unauthorized (bác sĩ khác)

**Deliverable:** Handler + 3 test cases

#### Task 2.3: Queries — Lấy HoSoKham
**Endpoint 1:** `GET /api/ho-so-kham/{id}`
- Lấy chi tiết 1 hồ sơ
- Include: `LichHen`, `BacSi`, `BenhNhan`, v.v.
- Authorization: bệnh nhân xem của mình, bác sĩ xem của mình, admin xem hết

**Endpoint 2:** `GET /api/benh-nhan/{idBenhNhan}/lich-su-kham`
- Lấy tất cả hồ sơ khám của 1 bệnh nhân (paginated)
- Query params: `page=1&pageSize=10&sortBy=ngayKham&sortDirection=desc`
- Filter: theo ngày, trạng thái (nếu có)
- Authorization: bệnh nhân xem của mình, bác sĩ xem bệnh nhân được giao phó, admin xem hết

**Endpoint 3 (optional):** `GET /api/bac-si/{idBacSi}/ho-so-kham`
- Lấy tất cả hồ sơ của 1 bác sĩ
- Paginated, filter

**Tests:**
- ✅ Pagination works
- ✅ Filter works
- ✅ Authorization enforced
- ✅ Data complete (kèm navigation properties)

**Deliverable:** 3 Query handlers + Integration tests

#### Task 2.4: HoSoKhamController
**File:** `ClinicBooking.Api/Controllers/HoSoKhamController.cs`  
**Endpoints:**
```csharp
[HttpPost] 
[Authorize(Roles = "bac_si,admin")]
public async Task<IActionResult> TaoHoSoKham([FromBody] TaoHoSoKhamRequest request)

[HttpPut("{id}")]
[Authorize(Roles = "bac_si,admin")]
public async Task<IActionResult> CapNhatHoSoKham(int id, [FromBody] CapNhatHoSoKhamRequest request)

[HttpGet("{id}")]
[Authorize]
public async Task<IActionResult> LayChiTiet(int id)

[HttpGet("benh-nhan/{idBenhNhan}/lich-su-kham")]
[Authorize]
public async Task<IActionResult> LayLichSuKham(int idBenhNhan, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
```

**Checklist:**
- [ ] Thin controller (delegate to MediatR)
- [ ] Swagger attributes (`[HttpPost]`, `[FromBody]`, etc.)
- [ ] GlobalExceptionHandler sẽ wrap errors
- [ ] Swagger documentation auto-generated

**Deliverable:** Controller code

#### Task 2.5: Data seeding (optional, dev/test)
- [ ] Seed 2–3 `HoSoKham` records nếu cần demo

---

### **PHASE 3: CRUD ToaThuoc** (Dự kiến: 3–5 ngày)
🎯 **Mục đích:** Kê đơn thuốc tại `HoSoKham`

#### Task 3.1: `KeToa` Command & Handler
**Endpoint:** `POST /api/ho-so-kham/{idHoSoKham}/toa-thuoc`  
**Request body:**
```json
{
  "idHoSoKham": 5,
  "ghiChu": "Dùng sau ăn",
  "chiTiet": [
    {
      "idThuoc": 10,
      "lieuLuong": "1 viên",
      "soLan": "3 lần/ngày",
      "thoiGianSuDung": 7,
      "ghiChu": "Sau ăn"
    },
    {
      "idThuoc": 12,
      "lieuLuong": "2 viên",
      "soLan": "2 lần/ngày",
      "thoiGianSuDung": 5
    }
  ]
}
```

**Logic:**
1. Validate `HoSoKham` tồn tại & belong to `LichHen` được check-in
2. Validate từng `idThuoc` tồn tại
3. Validate từng thuốc không ngừng bán (`Thuoc.TrangThai = Hoat động`)
4. Create `ToaThuoc` record
5. Create `ChiTietToaThuoc` cho từng thuốc
6. Ghi audit log
7. Return `ToaThuocResponse` (kèm chi tiết)

**Error handling:**
- `404 Not Found` — HoSoKham hoặc Thuoc không tồn tại
- `409 Conflict` — Thuoc ngừng bán
- `400 Bad Request` — Validation failed

**Tests:**
- ✅ Success case (1 thuốc)
- ✅ Success case (2+ thuốc)
- ✅ Thuoc not found
- ✅ Thuoc discontinued
- ✅ Invalid lieuLuong, v.v.

**Deliverable:** Handler + 5 test cases

#### Task 3.2: Queries — Lấy ToaThuoc
**Endpoint 1:** `GET /api/ho-so-kham/{idHoSoKham}/toa-thuoc`
- Lấy đơn thuốc của 1 hồ sơ
- Include: `ChiTietToaThuoc` + `Thuoc` info (tên, giá, v.v.)

**Endpoint 2:** `GET /api/benh-nhan/{idBenhNhan}/toa-thuoc`
- Lấy tất cả đơn của 1 bệnh nhân (paginated)
- Filter: theo ngày, trạng thái (nếu có)
- Authorization: bệnh nhân xem của mình, bác sĩ xem bệnh nhân được giao, admin xem hết

**Tests:**
- ✅ Data complete (kèm chi tiết thuốc)
- ✅ Pagination
- ✅ Authorization

**Deliverable:** 2 Query handlers + Integration tests

#### Task 3.3: `HuyToaThuoc` Command (soft-delete)
**Endpoint:** `DELETE /api/ho-so-kham/{idHoSoKham}/toa-thuoc/{idToaThuoc}`

**Logic:**
- Validate quyền (bác sĩ chủ trị hoặc admin)
- Soft-delete (`ToaThuoc.DaXoa = true`) hoặc hard-delete (tuỳ yêu cầu)
- Ghi audit log

**Tests:**
- ✅ Success
- ✅ Unauthorized

**Deliverable:** Handler + tests

#### Task 3.4: ToaThuocController
**File:** `ClinicBooking.Api/Controllers/ToaThuocController.cs`

---

### **PHASE 4: CRUD Danh mục Thuốc** (Dự kiến: 1–2 ngày)
🎯 **Mục đích:** Admin quản lý kho thuốc

#### Task 4.1: CRUD Thuoc
**Endpoint 1:** `POST /api/danh-muc/thuoc` — Tạo thuốc
- Admin only
- Request: `TenThuoc`, `MaThuoc` (unique), `DonVi`, `GiaNhap`, `GiaXuat`, v.v.
- Validate trùng `MaThuoc`

**Endpoint 2:** `PUT /api/danh-muc/thuoc/{id}` — Cập nhật
- Admin only
- Validate `MaThuoc` không trùng (exclude current record)

**Endpoint 3:** `GET /api/danh-muc/thuoc` — Danh sách
- Pagination, search by name, filter by status
- Public read (bác sĩ xem để kê đơn)

**Endpoint 4:** `DELETE /api/danh-muc/thuoc/{id}` — Xóa
- Admin only
- Soft-delete nếu có chi tiết ToaThuoc tham chiếu
- Hard-delete nếu không sử dụng

**Tests:**
- ✅ CRUD all operations
- ✅ Unique MaThuoc validation
- ✅ Authorization admin-only
- ✅ Search & pagination

**Deliverable:** Commands + Queries + Controller + tests

#### Task 4.2: Seed danh mục thuốc
- [ ] Seed 5–10 thuốc sample (phổ biến: paracetamol, ibuprofen, v.v.)

---

### **PHASE 5: Web UI nối Backend** (Dự kiến: 2–3 ngày)
🎯 **Mục đích:** Nối Razor pages với API backend

#### Task 5.1: Trang `/BenhNhan/ThongBao` (update)
**Hiện tại:** Razor page tồn tại nhưng chưa nối data  
**Cần làm:**
- [ ] JavaScript gọi `GET /api/benh-nhan/{idBenhNhan}/lich-su-kham`
- [ ] Render danh sách HoSoKham (ngày khám, bác sĩ, chẩn đoán)
- [ ] Link tới chi tiết hồ sơ hoặc đơn thuốc
- [ ] Styling responsive

**Deliverable:** Razor page updated + tested

#### Task 5.2: Trang Bác sĩ "Danh sách khám" (mới)
**Route:** `/BacSi/QuanLyKham` (hoặc `/BacSi/HoSoKham`)  
**Features:**
- [ ] Tab: "Chờ khám", "Đang khám", "Hoàn thành"
- [ ] Danh sách HoSoKham (gọi API phía trên)
- [ ] Form tạo HoSoKham (modal hoặc form inline)
- [ ] Form kê đơn (modal hoặc trong chi tiết)
- [ ] Validate phía client (JS)

**Deliverable:** Razor pages (2–3 trang) + JS + CSS

#### Task 5.3: Trang Admin "Quản lý thuốc" (mới)
**Route:** `/Admin/QuanLyThuoc`  
**Features:**
- [ ] Danh sách thuốc (table paginated)
- [ ] Nút "Thêm", "Sửa", "Xóa"
- [ ] Modal form CRUD
- [ ] Search by name

**Deliverable:** Razor pages + JS + CSS

---

## 🔗 IV. Dependencies & External Points

### Phụ thuộc Module 1
| Điểm | Chi tiết | Ảnh hưởng |
|---|---|---|
| `LichHen.TrangThai = DangKham` | Module 1 set khi check-in | Module 3 validate dựa vào enum này |
| `BenhNhan.SoLanHuyMuon` | Module 1 increment | **Task 1.1 MUST xác nhận** |
| Notification `GuiThongBaoTaoHoSoKhamAsync` (Module 4) | Optional, sau | Phase 3+ nếu Module 4 ready |

### Phụ thuộc Module 2
| Điểm | Chi tiết | Ảnh hưởng |
|---|---|---|
| `ChuyenKhoa` CRUD | Dropdown form | Optional; có thể hardcode temp hoặc fetch từ static list |

---

## 🧪 V. Testing Strategy

### Unit Tests (bắt buộc mỗi Handler)
- ✅ Success case
- ✅ Validation failure
- ✅ Not found (404)
- ✅ Unauthorized (403)
- ✅ Business rule violation

**Ví dụ:** TaoHoSoKham
```csharp
[Test] public async Task TaoHoSoKham_Success() { }
[Test] public async Task TaoHoSoKham_LichHenNotFound() { }
[Test] public async Task TaoHoSoKham_WrongStatus() { }
[Test] public async Task TaoHoSoKham_BenhNhanBiCam() { }
[Test] public async Task TaoHoSoKham_ValidationFailed() { }
[Test] public async Task TaoHoSoKham_Unauthorized() { }
```

### Integration Tests (1–2 per Phase)
- ✅ E2E: Create LichHen (M1) → Check-in → Create HoSoKham → Kê Toa
- ✅ Authorization enforcement
- ✅ Data consistency (FK integrity)

### Regression Tests
- ✅ Chạy `dotnet test` sau mỗi Phase
- ✅ **Không break 241 unit tests + 11 integration tests Module 1**

### UI Tests (manual)
- ✅ Form submission works
- ✅ Data displays correctly
- ✅ Error messages clear

---

## ⏱️ VI. Effort Estimate

| Phase | Dev Days | QA Days | Total | Status |
|---|---|---|---|---|
| **1. Setup & Confirm** | 1–2 | 0.5 | **2.5 days** | Not started |
| **2. HoSoKham CRUD** | 3–4 | 1 | **5 days** | Waiting Phase 1 |
| **3. ToaThuoc CRUD** | 3–4 | 1 | **5 days** | Waiting Phase 2 |
| **4. Thuoc CRUD** | 1 | 0.5 | **1.5 days** | Waiting Phase 3 |
| **5. Web UI** | 2–3 | 1 | **3–4 days** | Waiting Phase 4 |
| **📊 TOTAL** | **10–14 days** | **4 days** | **14–18 days** | |

---

## 📌 VII. Risks & Mitigation

### 🔴 High Severity

| Risk | Cause | Mitigation |
|---|---|---|
| `SoLanHuyMuon` logic unclear | Module 1 không document rõ | **Immediate:** Sync meeting ngay, xác nhận document (Task 1.1) |
| DB schema missing fields | Schema chưa update | Verify `/database/clinic.dbml` first (Task 1.2) |

### 🟠 Medium Severity

| Risk | Cause | Mitigation |
|---|---|---|
| LichHen `DangKham` status không exist | Module 1 code khác expectation | Review Module 1 code, verify enum |
| Race condition HoSoKham update | Concurrent updates | Use EF locking hoặc atomic DB ops |
| Module 2 ChuyenKhoa delay | Module 2 behind schedule | Hardcode dropdown temp, refactor sau |

---

## ✅ VIII. Done Criteria & Merge Checklist

### Trước khi merge vào `develop`:
- [ ] **Tất cả tasks Phase 1–5 đạt "Done"**
- [ ] **Task 1.1 confirm với Module 1 owner** (CRITICAL)
- [ ] **`dotnet test` pass:** 241 + 11 unit/integration tests vẫn green
- [ ] **Code review approved** (ít nhất 1 reviewer)
- [ ] **No TODOs/FIXMEs trong production code**

### Trước khi merge vào `main`:
- [ ] **PR approved & squash-merged vào develop**
- [ ] **`gitnexus_detect_changes()` confirm scope hợp lý** (chỉ Module 3 files)
- [ ] **Swagger contract stable** (không breaking API changes)
- [ ] **Manual UI test** — flow đặt lịch → khám → kê đơn hoạt động
- [ ] **Integration test pass** — E2E Module 1 + 3 flow
- [ ] **Tag version** (ví dụ: `v0.3.0`)

---

## 🚀 IX. Timeline & Milestones

### Week 1 (5–9 May)
- **Mon–Tue:** Phase 1 (Setup, Confirm) — BLOCKING POINT
- **Wed–Fri:** Phase 2 (HoSoKham CRUD) — Start if Phase 1 unblocked

### Week 2 (12–16 May)
- **Mon–Tue:** Phase 2 finish + QA
- **Wed–Thu:** Phase 3 (ToaThuoc CRUD)
- **Fri:** Phase 3 QA

### Week 3 (19–23 May)
- **Mon:** Phase 4 (Thuoc CRUD)
- **Tue–Thu:** Phase 5 (Web UI)
- **Fri:** Final QA + Merge

---

## 📞 X. Communication Plan

### Internal (Team)
- **Sync daily standup:** 9:00 AM
- **Async update:** End-of-day Slack/email

### Cross-module
- **Module 1 owner:** Task 1.1 xác nhận (immediate)
- **Module 2 owner:** ChuyenKhoa dependency (notify by Wed)
- **Module 4 owner:** Notification integration (Phase 3+)

### Escalation
- **Blocking point (Task 1.1):** Escalate to PM if Module 1 owner unresponsive > 1 day

---

## 📚 XI. Reference & Resources

### Code Structure
```
ClinicBooking.Application/
├── Features/
│   └── HoSoKham/
│       ├── Commands/
│       │   ├── TaoHoSoKham/
│       │   ├── CapNhatHoSoKham/
│       │   └── HuyHoSoKham/ (soft-delete)
│       └── Queries/
│           ├── LayChiTiet/
│           └── LayLichSuKham/
│   └── ToaThuoc/
│       ├── Commands/
│       │   ├── KeToa/
│       │   └── HuyToa/
│       └── Queries/
│   └── DanhMuc/
│       └── Thuoc/
│
ClinicBooking.Api/
└── Controllers/
    ├── HoSoKhamController.cs
    ├── ToaThuocController.cs
    └── DanhMucController.cs

ClinicBooking.Domain/
└── Entities/
    ├── HoSoKham.cs
    ├── ToaThuoc.cs
    ├── ChiTietToaThuoc.cs
    └── Thuoc.cs
```

### Related Files
- `/database/clinic.dbml` — DB schema source of truth
- `ClinicBooking.Infrastructure/Persistence/ClinicBookingDbContext.cs` — DbContext
- `ClinicBooking.Application/Abstractions/` — Interfaces
- `.claude/CLAUDE.md` — Architecture guidelines

---

## 🎯 XII. Key Success Metrics

By end of Module 3:
- ✅ **252 tests pass** (241 M1 + 11 M1 integration + new M3 tests)
- ✅ **0 breaking changes** to Module 1 API
- ✅ **E2E flow works:** Đặt lịch → Check-in → Khám → Kê đơn → Xem đơn
- ✅ **Code coverage:** >80% for CRUD logic
- ✅ **Zero TODOs** in production code
- ✅ **Swagger contract stable** for deployment

---

**Prepared by:** Claude Code  
**Status:** Ready for Phase 1 (Confirm blocking point Task 1.1)  
**Next Action:** Schedule sync với Module 1 owner → xác nhận `SoLanHuyMuon` logic
