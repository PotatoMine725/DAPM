-- Fix mật khẩu cho patient001 để có thể đăng nhập
-- Hash thật cho "Patient@123456" với BCrypt work factor 11

UPDATE TaiKhoan 
SET MatKhau = '$2a$11$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewdBPj6ukx.LrUpm'
WHERE TenDangNhap = 'patient001';

-- Kiểm tra kết quả
SELECT TenDangNhap, Email, VaiTro, TrangThai 
FROM TaiKhoan 
WHERE TenDangNhap = 'patient001';
