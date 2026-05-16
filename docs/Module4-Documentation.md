# Module 4 - Thông báo & Vận hành Documentation

## Tổng quan

Module 4 chịu trách nhiệm về tầng thông báo, email, OTP và các tác vụ nền vận hành của hệ thống ClinicBooking. Module này đảm bảo người dùng nhận được thông tin kịp thời về các hoạt động liên quan đến lịch hẹn khám bệnh.

## Các thành phần đã triển khai

### 1. OTP Service
**File:** `ClinicBooking.Infrastructure/Services/Security/OtpService.cs`

**Chức năng:**
- Tạo và gửi OTP xác nhận đặt lịch qua email
- Xác thực OTP có hiệu lực
- Lưu log OTP vào database để theo dõi
- Tự động cleanup OTP hết hạn

**Features:**
- OTP 6 số ngẫu nhiên
- Thời hạn OTP cấu hình được (mặc định 15 phút)
- Gửi email với template tùy chỉnh
- Logging đầy đủ cho audit trail

### 2. Notification Service
**File:** `ClinicBooking.Infrastructure/Services/Notifications/NotificationService.cs`

**Chức năng:**
- Gửi thông báo in-app và email cho các sự kiện lịch hẹn
- Hỗ trợ đa kênh (in-app + email)

**Các loại thông báo:**
- **Tạo lịch hẹn:** Xác nhận đặt lịch thành công
- **Xác nhận lịch hẹn:** Lễ tân xác nhận lịch
- **Hủy lịch hẹn:** Thông báo hủy kèm lý do
- **Đổi lịch hẹn:** Thông báo thay đổi lịch
- **Check-in:** Xác nhận check-in thành công
- **Gọi bệnh nhân:** Mời vào phòng khám

### 3. Email Service
**File:** `ClinicBooking.Infrastructure/Services/Notifications/EmailService.cs`

**Chức năng:**
- Gửi email qua SMTP (Gmail)
- Hỗ trợ HTML email
- Xử lý lỗi và retry logic
- Logging chi tiết

### 4. Background Jobs
**Files:**
- `ClinicBooking.Infrastructure/BackgroundJobs/QuetGiuChoHetHanJob.cs`
- `ClinicBooking.Infrastructure/BackgroundJobs/ChuyenLichHenDaQuaHanJob.cs`

**Chức năng:**
- **Quét giữ chỗ hết hạn:** Chạy mỗi phút, hủy các giữ chỗ quá thời hạn
- **Chuyển lịch hẹn quá hạn:** Chạy mỗi 30 phút, tự động chuyển lịch hẹn đã quá giờ

### 5. Configuration & Options
**Files:**
- `ClinicBooking.Application/Common/Options/OtpOptions.cs`
- `ClinicBooking.Application/Common/Options/EmailSettings.cs`
- Configuration trong `appsettings.json`

**Cấu hình:**
- OTP thời hạn, độ dài
- SMTP settings (host, port, credentials)
- Background job schedules

## Dependency Injection

Các services đã được đăng ký trong `ClinicBooking.Infrastructure/DependencyInjection.cs`:

```csharp
// OTP Services
services.AddScoped<IOtpService, OtpService>();

// Notification Services  
services.AddScoped<INotificationService, NotificationService>();
services.AddScoped<IEmailService, EmailService>();

// Background Jobs
services.AddHostedService<QuetGiuChoHetHanJob>();
services.AddHostedService<ChuyenLichHenDaQuaHanJob>();
```

## Database Schema

### OtpLog
- Lưu trữ lịch sử OTP
- Theo dõi trạng thái sử dụng
- Hỗ trợ cleanup tự động

### ThongBao
- Lưu trữ thông báo in-app
- Liên kết với các entity (LichHen, HangCho)
- Hỗ trợ đa kênh gửi

## Integration Points

### Với Module 1 (Lịch hẹn & Hàng chờ)
- Gửi OTP khi đặt lịch
- Thông báo các trạng thái lịch hẹn
- Thông báo check-in và gọi bệnh nhân

### Với Module 2 (Bác sĩ & Lịch làm việc)
- Thông báo thay đổi lịch làm việc
- Email xác nhận duyệt ca

### Với Module 3 (Hồ sơ bệnh nhân)
- Thông báo kết quả khám
- Gửi toa thuốc email

## Cấu hình môi trường

### Development
- Sử dụng `appsettings.Development.json`
- SMTP Gmail với app password
- Background jobs enabled

### Production
- Cần cấu hình SMTP production
- Configure appropriate job schedules
- Enable proper logging

## Logging & Monitoring

- Structured logging với Serilog
- Log levels: Information, Warning, Error
- Correlation IDs cho tracking
- Performance metrics cho email sending

## Security Considerations

- OTP rate limiting (cần implement)
- Email spoofing protection
- Sensitive data logging filtered
- SMTP authentication

## Future Enhancements

### Priority 1
- Rate limiting cho OTP
- SMS notification fallback
- Push notifications mobile

### Priority 2
- Email template engine
- Multi-language support
- Notification preferences

### Priority 3
- Real-time notifications (SignalR)
- Analytics dashboard
- A/B testing cho subject lines

## Testing

### Unit Tests
- Mock email service
- Test OTP generation/validation
- Background job scheduling tests

### Integration Tests
- End-to-end email sending
- Database transaction testing
- Job execution verification

## Troubleshooting

### Common Issues
1. **SMTP Authentication:** Kiểmtra app password Gmail
2. **OTP Not Sending:** Verify email configuration
3. **Background Jobs Not Running:** Check service registration
4. **Database Connection:** Verify connection string

### Debug Tips
- Enable verbose logging
- Check SMTP logs
- Monitor database for OTP records
- Verify job execution in logs

## Performance Considerations

- Email sending async
- Batch processing cho notifications
- Database indexing cho queries
- Memory optimization cho background jobs

## Conclusion

Module 4 đã cung cấp nền tảng thông báo vững chắc cho hệ thống:
- ✅ OTP verification system
- ✅ Multi-channel notifications  
- ✅ Background job processing
- ✅ Email service integration
- ✅ Comprehensive logging

Module sẵn sàng tích hợp với các module khác và có khả năng mở rộng cao cho các yêu cầu tương lai.
