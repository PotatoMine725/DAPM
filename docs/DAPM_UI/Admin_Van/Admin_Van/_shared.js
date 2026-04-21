// _shared.js — Sidebar + Header + Modal helpers
// Include this file in every page

const CURRENT_PAGE = document.body.dataset.page || '';

function renderShell(pageTitle, pageCrumb) {
  const navItems = [
    { id: 'dashboard',     href: 'index.html',         icon: 'fa-chart-pie',       label: 'Dashboard',               section: 'Tổng quan' },
    { id: 'accounts',      href: 'accounts.html',      icon: 'fa-users-gear',      label: 'Tài khoản người dùng',    section: 'Quản lý' },
    { id: 'chuyen-khoa',   href: 'chuyen-khoa.html',   icon: 'fa-stethoscope',     label: 'Chuyên khoa' },
    { id: 'bac-si',        href: 'bac-si.html',        icon: 'fa-user-doctor',     label: 'Bác sĩ' },
    { id: 'dich-vu',       href: 'dich-vu.html',       icon: 'fa-syringe',         label: 'Dịch vụ' },
    { id: 'phong',         href: 'phong.html',         icon: 'fa-door-open',       label: 'Phòng khám' },
    { id: 'lich-noi-tru',  href: 'lich-noi-tru.html',  icon: 'fa-calendar-days',   label: 'Lịch bác sĩ nội trú',     section: 'Ca làm việc' },
    { id: 'duyet-ca',      href: 'duyet-ca.html',      icon: 'fa-clipboard-check', label: 'Duyệt ca hợp đồng',       badge: '5' },
    { id: 'ca-lam-viec',   href: 'ca-lam-viec.html',   icon: 'fa-calendar-week',   label: 'Ca làm việc' },
    { id: 'thong-ke',      href: 'thong-ke.html',      icon: 'fa-chart-bar',       label: 'Thống kê bệnh nhân',      section: 'Báo cáo' },
    { id: 'thong-bao',     href: 'thong-bao.html',     icon: 'fa-bell',            label: 'Thông báo hệ thống',      badge: '12' },
  ];

  let navHtml = '';
  let lastSection = '';
  navItems.forEach(item => {
    if (item.section && item.section !== lastSection) {
      navHtml += `<div class="sidebar-section">${item.section}</div>`;
      lastSection = item.section;
    }
    const active = item.id === CURRENT_PAGE ? 'active' : '';
    const badge = item.badge ? `<span class="badge">${item.badge}</span>` : '';
    navHtml += `<a href="${item.href}" class="nav-item ${active}">
      <i class="fa-solid ${item.icon} nav-icon"></i> ${item.label}${badge}
    </a>`;
  });

  const sidebar = `
  <aside class="sidebar">
    <div class="sidebar-logo">
      <div class="logo-icon"><i class="fa-solid fa-hospital"></i></div>
      <div class="logo-text">
        <div class="brand">MedAdmin</div>
        <div class="sub">Phòng Khám Đa Khoa</div>
      </div>
    </div>
    ${navHtml}
    <div class="sidebar-footer">
      <div class="avatar">A</div>
      <div class="info">
        <div class="name">Admin Hệ thống</div>
        <div class="role">Quản trị viên</div>
      </div>
      <i class="fa-solid fa-right-from-bracket logout" title="Đăng xuất" onclick="confirmLogout()"></i>
    </div>
  </aside>`;

  const header = `
  <header class="header">
    <div class="header-breadcrumb">
      <span class="page-title">${pageTitle}</span>
      <span class="sep"><i class="fa-solid fa-chevron-right" style="font-size:10px"></i></span>
      <span class="crumb">${pageCrumb}</span>
    </div>
    <div class="header-actions">
      <div class="header-search">
        <i class="fa-solid fa-magnifying-glass"></i>
        <input type="text" placeholder="Tìm kiếm nhanh..." id="globalSearch">
      </div>
      <a href="thong-bao.html" class="header-btn has-notif" title="Thông báo" id="notifBtn">
        <i class="fa-solid fa-bell"></i>
        <span class="notif-dot"></span>
      </a>
      <div class="header-btn" title="Cài đặt" id="settingsBtn" onclick="toggleSettings(event)">
        <i class="fa-solid fa-gear"></i>
      </div>
      <div class="header-avatar-wrap" id="avatarWrap">
        <div class="header-avatar" onclick="toggleAvatarMenu(event)" title="Tài khoản">A</div>
        <!-- Avatar dropdown -->
        <div class="avatar-dropdown" id="avatarDropdown">
          <div class="avatar-dropdown-header">
            <div class="avd-avatar">A</div>
            <div>
              <div class="avd-name">Admin Hệ thống</div>
              <div class="avd-email">admin@medadmin.vn</div>
            </div>
          </div>
          <div class="avd-divider"></div>
          <a href="accounts.html" class="avd-item"><i class="fa-solid fa-user-circle"></i> Hồ sơ cá nhân</a>
          <a href="#" class="avd-item" onclick="toggleSettings(event); closeAvatarMenu()"><i class="fa-solid fa-gear"></i> Cài đặt hệ thống</a>
          <a href="thong-bao.html" class="avd-item"><i class="fa-solid fa-bell"></i> Thông báo <span class="avd-badge">12</span></a>
          <div class="avd-divider"></div>
          <div class="avd-item avd-item-danger" onclick="confirmLogout()"><i class="fa-solid fa-right-from-bracket"></i> Đăng xuất</div>
        </div>
      </div>
    </div>
  </header>

  <!-- Settings panel -->
  <div class="settings-overlay" id="settingsOverlay" onclick="closeSettings()"></div>
  <div class="settings-panel" id="settingsPanel">
    <div class="sp-header">
      <div class="sp-title"><i class="fa-solid fa-gear" style="color:var(--primary);margin-right:8px"></i>Cài đặt hệ thống</div>
      <div class="sp-close" onclick="closeSettings()"><i class="fa-solid fa-times"></i></div>
    </div>
    <div class="sp-body">
      <div class="sp-section">Giao diện</div>
      <div class="sp-item">
        <div>
          <div class="sp-item-label">Chủ đề màu sắc</div>
          <div class="sp-item-sub">Chọn chủ đề cho giao diện quản trị</div>
        </div>
        <div class="sp-theme-btns">
          <div class="sp-theme active" style="background:#1a5fa8" title="Xanh dương" onclick="setTheme('blue',this)"></div>
          <div class="sp-theme" style="background:#0f766e" title="Xanh ngọc" onclick="setTheme('teal',this)"></div>
          <div class="sp-theme" style="background:#7c3aed" title="Tím" onclick="setTheme('violet',this)"></div>
          <div class="sp-theme" style="background:#dc2626" title="Đỏ" onclick="setTheme('red',this)"></div>
          <div class="sp-theme" style="background:#ea580c" title="Cam" onclick="setTheme('orange',this)"></div>
        </div>
      </div>
      <div class="sp-item">
        <div>
          <div class="sp-item-label">Cỡ chữ giao diện</div>
          <div class="sp-item-sub">Điều chỉnh kích thước hiển thị</div>
        </div>
        <select class="sp-select" id="fontSizeSel" onchange="setFontSize(this.value)">
          <option value="14">Nhỏ (14px)</option>
          <option value="15" selected>Mặc định (15px)</option>
          <option value="16">Lớn (16px)</option>
        </select>
      </div>
      <div class="sp-item">
        <div>
          <div class="sp-item-label">Thu gọn sidebar</div>
          <div class="sp-item-sub">Hiện icon, ẩn nhãn điều hướng</div>
        </div>
        <label class="toggle"><input type="checkbox" id="sidebarCollapse" onchange="toggleSidebarCollapse(this)"><span class="toggle-slider"></span></label>
      </div>

      <div class="sp-section">Thông báo</div>
      <div class="sp-item">
        <div>
          <div class="sp-item-label">Thông báo âm thanh</div>
          <div class="sp-item-sub">Phát tiếng khi có thông báo mới</div>
        </div>
        <label class="toggle"><input type="checkbox" checked><span class="toggle-slider"></span></label>
      </div>
      <div class="sp-item">
        <div>
          <div class="sp-item-label">Thông báo ca chờ duyệt</div>
          <div class="sp-item-sub">Nhắc khi có ca hợp đồng mới</div>
        </div>
        <label class="toggle"><input type="checkbox" checked><span class="toggle-slider"></span></label>
      </div>
      <div class="sp-item">
        <div>
          <div class="sp-item-label">Thông báo lịch hủy</div>
          <div class="sp-item-sub">Cảnh báo khi bệnh nhân hủy lịch</div>
        </div>
        <label class="toggle"><input type="checkbox"><span class="toggle-slider"></span></label>
      </div>

      <div class="sp-section">Tài khoản</div>
      <div class="sp-item">
        <div>
          <div class="sp-item-label">Phiên đăng nhập</div>
          <div class="sp-item-sub">Tự động đăng xuất sau bao lâu</div>
        </div>
        <select class="sp-select">
          <option>30 phút</option>
          <option selected>1 giờ</option>
          <option>4 giờ</option>
          <option>Không giới hạn</option>
        </select>
      </div>
      <div class="sp-item">
        <div>
          <div class="sp-item-label">Xác thực 2 bước</div>
          <div class="sp-item-sub">Bảo mật bổ sung khi đăng nhập</div>
        </div>
        <label class="toggle"><input type="checkbox"><span class="toggle-slider"></span></label>
      </div>
    </div>
    <div class="sp-footer">
      <button class="btn btn-ghost btn-sm" onclick="closeSettings()">Đóng</button>
      <button class="btn btn-primary btn-sm" onclick="saveSettings()"><i class="fa-solid fa-floppy-disk"></i> Lưu cài đặt</button>
    </div>
  </div>

  <!-- Logout confirm modal -->
  <div class="modal-overlay" id="modal-logout">
    <div class="modal" style="max-width:420px">
      <div class="modal-header">
        <div class="modal-title" style="display:flex;align-items:center;gap:10px">
          <div style="width:34px;height:34px;border-radius:50%;background:var(--danger-bg);display:flex;align-items:center;justify-content:center;color:var(--danger)">
            <i class="fa-solid fa-right-from-bracket"></i>
          </div>
          Xác nhận đăng xuất
        </div>
        <div class="modal-close" onclick="closeModal('modal-logout')"><i class="fa-solid fa-times"></i></div>
      </div>
      <div class="modal-body" style="text-align:center;padding:32px 24px">
        <div style="width:64px;height:64px;border-radius:50%;background:var(--danger-bg);display:flex;align-items:center;justify-content:center;margin:0 auto 16px;font-size:26px;color:var(--danger)">
          <i class="fa-solid fa-door-open"></i>
        </div>
        <div style="font-size:15px;font-weight:600;margin-bottom:8px">Bạn muốn đăng xuất?</div>
        <div style="font-size:13px;color:var(--text-secondary)">Phiên làm việc hiện tại sẽ kết thúc. Bạn cần đăng nhập lại để tiếp tục sử dụng hệ thống.</div>
      </div>
      <div class="modal-footer" style="justify-content:center;gap:12px">
        <button class="btn btn-ghost" onclick="closeModal('modal-logout')" style="min-width:110px">Ở lại</button>
        <button class="btn btn-danger" onclick="doLogout()" style="min-width:110px;background:var(--danger);color:#fff;border-color:var(--danger)">
          <i class="fa-solid fa-right-from-bracket"></i> Đăng xuất
        </button>
      </div>
    </div>
  </div>

  <!-- Toast container -->
  <div id="toast-container"></div>
  `;

  document.getElementById('sidebar-slot').innerHTML = sidebar;
  document.getElementById('header-slot').innerHTML = header;

  // Restore saved settings
  _restoreSettings();
}

// ===== AVATAR MENU =====
function toggleAvatarMenu(e) {
  e.stopPropagation();
  const dd = document.getElementById('avatarDropdown');
  const isOpen = dd.classList.contains('open');
  closeAllDropdowns();
  if (!isOpen) dd.classList.add('open');
}
function closeAvatarMenu() {
  const dd = document.getElementById('avatarDropdown');
  if (dd) dd.classList.remove('open');
}

// ===== SETTINGS PANEL =====
function toggleSettings(e) {
  if (e) e.stopPropagation();
  const panel = document.getElementById('settingsPanel');
  const overlay = document.getElementById('settingsOverlay');
  const isOpen = panel.classList.contains('open');
  closeAllDropdowns();
  if (!isOpen) {
    panel.classList.add('open');
    overlay.classList.add('open');
  }
}
function closeSettings() {
  const panel = document.getElementById('settingsPanel');
  const overlay = document.getElementById('settingsOverlay');
  if (panel) panel.classList.remove('open');
  if (overlay) overlay.classList.remove('open');
}
function saveSettings() {
  closeSettings();
  showToast('Cài đặt đã được lưu!', 'success');
}

function setTheme(name, el) {
  const themes = {
    blue:   { primary: '#1a5fa8', dark: '#155190', mid: '#378add', light: '#e6f1fb' },
    teal:   { primary: '#0f766e', dark: '#0d6460', mid: '#14b8a6', light: '#e0f7f5' },
    violet: { primary: '#7c3aed', dark: '#6d28d9', mid: '#a78bfa', light: '#f3e8ff' },
    red:    { primary: '#dc2626', dark: '#b91c1c', mid: '#f87171', light: '#fee2e2' },
    orange: { primary: '#ea580c', dark: '#c2410c', mid: '#fb923c', light: '#fff3e0' },
  };
  const t = themes[name];
  if (!t) return;
  document.documentElement.style.setProperty('--primary', t.primary);
  document.documentElement.style.setProperty('--primary-dark', t.dark);
  document.documentElement.style.setProperty('--primary-mid', t.mid);
  document.documentElement.style.setProperty('--primary-light', t.light);
  document.querySelectorAll('.sp-theme').forEach(b => b.classList.remove('active'));
  el.classList.add('active');
  localStorage.setItem('medadmin_theme', name);
}

function setFontSize(val) {
  document.documentElement.style.fontSize = val + 'px';
  localStorage.setItem('medadmin_fontsize', val);
}

function toggleSidebarCollapse(cb) {
  document.querySelector('.sidebar').classList.toggle('collapsed', cb.checked);
  document.querySelector('.main').classList.toggle('sidebar-collapsed', cb.checked);
  localStorage.setItem('medadmin_sidebar_collapsed', cb.checked ? '1' : '0');
}

function _restoreSettings() {
  const theme = localStorage.getItem('medadmin_theme');
  if (theme) {
    const btn = document.querySelector(`.sp-theme[title]`);
    const themeMap = { blue:'Xanh dương', teal:'Xanh ngọc', violet:'Tím', red:'Đỏ', orange:'Cam' };
    document.querySelectorAll('.sp-theme').forEach(el => {
      if (el.title === themeMap[theme]) setTheme(theme, el);
    });
  }
  const fs = localStorage.getItem('medadmin_fontsize');
  if (fs) {
    document.documentElement.style.fontSize = fs + 'px';
    const sel = document.getElementById('fontSizeSel');
    if (sel) sel.value = fs;
  }
  const sc = localStorage.getItem('medadmin_sidebar_collapsed');
  if (sc === '1') {
    const cb = document.getElementById('sidebarCollapse');
    if (cb) { cb.checked = true; toggleSidebarCollapse(cb); }
  }
}

// ===== LOGOUT =====
function confirmLogout() {
  closeAllDropdowns();
  openModal('modal-logout');
}
function doLogout() {
  closeModal('modal-logout');
  showToast('Đang đăng xuất...', 'info');
  setTimeout(() => { window.location.href = 'login.html'; }, 1200);
}

// ===== CLOSE ALL =====
function closeAllDropdowns() {
  closeAvatarMenu();
  closeSettings();
}
document.addEventListener('click', () => closeAllDropdowns());

// ===== TOAST =====
function showToast(msg, type = 'success') {
  const icons = { success: 'fa-circle-check', danger: 'fa-circle-xmark', info: 'fa-circle-info', warning: 'fa-triangle-exclamation' };
  const colors = { success: 'var(--success)', danger: 'var(--danger)', info: 'var(--primary)', warning: 'var(--warning)' };
  const container = document.getElementById('toast-container');
  if (!container) return;
  const t = document.createElement('div');
  t.className = 'toast toast-' + type;
  t.innerHTML = `<i class="fa-solid ${icons[type]||icons.info}" style="color:${colors[type]||colors.info};font-size:16px"></i><span>${msg}</span>`;
  container.appendChild(t);
  setTimeout(() => t.classList.add('show'), 10);
  setTimeout(() => { t.classList.remove('show'); setTimeout(() => t.remove(), 300); }, 3000);
}

// ===== MODAL HELPERS =====
function openModal(id) { document.getElementById(id).classList.add('open'); }
function closeModal(id) { document.getElementById(id).classList.remove('open'); }
document.addEventListener('click', e => {
  if (e.target.classList.contains('modal-overlay')) e.target.classList.remove('open');
});
document.addEventListener('keydown', e => {
  if (e.key === 'Escape') {
    document.querySelectorAll('.modal-overlay.open').forEach(o => o.classList.remove('open'));
    closeSettings();
  }
});

// ===== TAB SWITCHER =====
function switchTab(btn, paneId) {
  const modal = btn.closest('.modal') || btn.closest('.page-content');
  modal.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
  modal.querySelectorAll('.tab-pane').forEach(p => p.classList.remove('active'));
  btn.classList.add('active');
  document.getElementById(paneId).classList.add('active');
}

// ===== ANIMATE NUMBERS =====
function animateNum(el, target) {
  let cur = 0; const step = target / 40;
  const id = setInterval(() => {
    cur = Math.min(cur + step, target);
    el.textContent = Math.round(cur).toLocaleString('vi-VN');
    if (cur >= target) clearInterval(id);
  }, 20);
}
