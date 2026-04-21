using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Security;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.BenhNhan.Commands.TaoBenhNhanWalkIn;

public sealed class TaoBenhNhanWalkInHandler : IRequestHandler<TaoBenhNhanWalkInCommand, TaoBenhNhanWalkInResult>
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TaoBenhNhanWalkInHandler(
        IAppDbContext db,
        IPasswordHasher passwordHasher,
        IDateTimeProvider dateTimeProvider)
    {
        _db = db;
        _passwordHasher = passwordHasher;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<TaoBenhNhanWalkInResult> Handle(TaoBenhNhanWalkInCommand request, CancellationToken cancellationToken)
    {
        var soDienThoai = request.SoDienThoai.Trim();
        var cccd = string.IsNullOrWhiteSpace(request.Cccd) ? null : request.Cccd.Trim();

        var soDienThoaiTrung = await _db.TaiKhoan
            .AnyAsync(x => x.SoDienThoai == soDienThoai, cancellationToken);
        if (soDienThoaiTrung)
        {
            throw new ConflictException("So dien thoai da duoc su dung.");
        }

        if (!string.IsNullOrWhiteSpace(cccd))
        {
            var cccdTrung = await _db.BenhNhan
                .AnyAsync(x => x.Cccd == cccd, cancellationToken);
            if (cccdTrung)
            {
                throw new ConflictException("CCCD da duoc su dung.");
            }
        }

        var now = _dateTimeProvider.UtcNow;
        var taiKhoan = await TaoTaiKhoanWalkInAsync(soDienThoai, now, cancellationToken);

        var benhNhan = new ClinicBooking.Domain.Entities.BenhNhan
        {
            TaiKhoan = taiKhoan,
            HoTen = request.HoTen.Trim(),
            Cccd = cccd,
            NgaySinh = request.NgaySinh,
            GioiTinh = request.GioiTinh,
            DiaChi = string.IsNullOrWhiteSpace(request.DiaChi) ? null : request.DiaChi.Trim(),
            SoLanHuyMuon = 0,
            BiHanChe = false,
            NgayTao = now
        };

        _db.TaiKhoan.Add(taiKhoan);
        _db.BenhNhan.Add(benhNhan);

        await _db.SaveChangesAsync(cancellationToken);
        return new TaoBenhNhanWalkInResult(benhNhan.IdBenhNhan, taiKhoan.IdTaiKhoan);
    }

    private async Task<TaiKhoan> TaoTaiKhoanWalkInAsync(
        string soDienThoai,
        DateTime now,
        CancellationToken cancellationToken)
    {
        for (var lanThu = 0; lanThu < 5; lanThu++)
        {
            var suffix = $"{now:yyyyMMddHHmmssfff}{lanThu}{Random.Shared.Next(100, 999)}";
            var tenDangNhap = $"walkin_{suffix}";
            var email = $"walkin_{suffix}@local.invalid";

            var trung = await _db.TaiKhoan.AnyAsync(
                x => x.TenDangNhap == tenDangNhap || x.Email == email,
                cancellationToken);
            if (trung)
            {
                continue;
            }

            return new TaiKhoan
            {
                TenDangNhap = tenDangNhap,
                Email = email,
                SoDienThoai = soDienThoai,
                MatKhau = _passwordHasher.HashPassword($"WalkIn@{suffix}"),
                VaiTro = VaiTro.BenhNhan,
                TrangThai = false,
                NgayTao = now
            };
        }

        throw new ConflictException("Khong the tao tai khoan walk-in. Vui long thu lai.");
    }
}
