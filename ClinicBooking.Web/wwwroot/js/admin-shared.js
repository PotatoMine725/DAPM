// Admin Shared JavaScript
(function() {
    'use strict';

    // Navigation handler
    function initNavigation() {
        const navItems = document.querySelectorAll('.nav-item[data-page]');
        navItems.forEach(item => {
            item.addEventListener('click', function() {
                const page = this.dataset.page;
                window.location.href = `/Admin/${page}`;
            });
        });
    }

    // Sidebar toggle (for mobile)
    function initSidebarToggle() {
        const sidebar = document.getElementById('sidebar');
        const toggleBtn = document.createElement('button');
        toggleBtn.className = 'sidebar-toggle';
        toggleBtn.innerHTML = '<i class="fa-solid fa-bars"></i>';
        toggleBtn.style.cssText = `
            position: fixed;
            top: 12px;
            left: 12px;
            z-index: 1001;
            width: 40px;
            height: 40px;
            background: var(--primary);
            color: white;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            display: none;
            align-items: center;
            justify-content: center;
            box-shadow: var(--shadow-md);
        `;

        document.body.appendChild(toggleBtn);

        toggleBtn.addEventListener('click', function() {
            sidebar.classList.toggle('open');
        });

        // Show toggle button on mobile
        function checkMobile() {
            if (window.innerWidth <= 768) {
                toggleBtn.style.display = 'flex';
            } else {
                toggleBtn.style.display = 'none';
                sidebar.classList.remove('open');
            }
        }

        window.addEventListener('resize', checkMobile);
        checkMobile();
    }

    // Search functionality
    function initSearch() {
        const searchInput = document.querySelector('.header-search input');
        if (searchInput) {
            searchInput.addEventListener('keypress', function(e) {
                if (e.key === 'Enter') {
                    const query = this.value.trim();
                    if (query) {
                        // Implement search functionality
                        console.log('Search for:', query);
                    }
                }
            });
        }
    }

    // Notification handling
    function initNotifications() {
        const notifBtn = document.querySelector('.header-btn');
        if (notifBtn) {
            notifBtn.addEventListener('click', function() {
                // Implement notification dropdown
                console.log('Show notifications');
            });
        }
    }

    // User avatar dropdown
    function initAvatarDropdown() {
        const avatarWrap = document.querySelector('.header-avatar-wrap');
        if (avatarWrap) {
            avatarWrap.addEventListener('click', function(e) {
                e.stopPropagation();
                // Toggle dropdown
                const existingDropdown = document.querySelector('.avatar-dropdown');
                if (existingDropdown) {
                    existingDropdown.remove();
                } else {
                    createAvatarDropdown();
                }
            });
        }

        // Close dropdown when clicking outside
        document.addEventListener('click', function() {
            const dropdown = document.querySelector('.avatar-dropdown');
            if (dropdown) {
                dropdown.remove();
            }
        });
    }

    function createAvatarDropdown() {
        const dropdown = document.createElement('div');
        dropdown.className = 'avatar-dropdown open';
        dropdown.innerHTML = `
            <div class="avatar-dropdown-header">
                <div class="avd-avatar">A</div>
                <div>
                    <div class="avd-name">Admin Demo</div>
                    <div class="avd-email">admin001@test.vn</div>
                </div>
            </div>
            <div class="avd-divider"></div>
            <div class="avd-item">
                <i class="fa-solid fa-user"></i>
                <span>Hồ sơ cá nhân</span>
            </div>
            <div class="avd-item">
                <i class="fa-solid fa-gear"></i>
                <span>Cài đặt</span>
            </div>
            <div class="avd-divider"></div>
            <div class="avd-item avd-item-danger" onclick="location.href='/Auth/DangXuat'">
                <i class="fa-solid fa-right-from-bracket"></i>
                <span>Đăng xuất</span>
            </div>
        `;

        const avatarWrap = document.querySelector('.header-avatar-wrap');
        avatarWrap.appendChild(dropdown);
    }

    // Toast notification system
    window.showToast = function(message, type = 'info') {
        const toast = document.createElement('div');
        toast.className = `toast toast-${type}`;
        toast.innerHTML = `
            <i class="fa-solid ${getToastIcon(type)}"></i>
            <span>${message}</span>
        `;

        const container = document.getElementById('toast-container') || createToastContainer();
        container.appendChild(toast);

        // Animate in
        setTimeout(() => toast.classList.add('show'), 10);

        // Remove after 4 seconds
        setTimeout(() => {
            toast.classList.remove('show');
            setTimeout(() => toast.remove(), 300);
        }, 4000);
    };

    function getToastIcon(type) {
        const icons = {
            success: 'fa-circle-check',
            error: 'fa-circle-xmark',
            warning: 'fa-triangle-exclamation',
            info: 'fa-circle-info'
        };
        return icons[type] || icons.info;
    }

    function createToastContainer() {
        const container = document.createElement('div');
        container.id = 'toast-container';
        container.style.cssText = `
            position: fixed;
            bottom: 24px;
            right: 24px;
            z-index: 9999;
            display: flex;
            flex-direction: column;
            gap: 10px;
        `;
        document.body.appendChild(container);
        return container;
    }

    // Modal helpers
    window.showModal = function(modalId) {
        const modal = document.getElementById(modalId);
        if (modal) {
            modal.classList.add('active');
            document.body.style.overflow = 'hidden';
        }
    };

    window.hideModal = function(modalId) {
        const modal = document.getElementById(modalId);
        if (modal) {
            modal.classList.remove('active');
            document.body.style.overflow = '';
        }
    };

    // Initialize everything
    document.addEventListener('DOMContentLoaded', function() {
        initNavigation();
        initSidebarToggle();
        initSearch();
        initNotifications();
        initAvatarDropdown();
    });

})();
