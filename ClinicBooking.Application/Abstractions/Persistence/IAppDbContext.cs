using ClinicBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace ClinicBooking.Application.Abstractions.Persistence;

public interface IAppDbContext
{
    DatabaseFacade Database { get; }

    DbSet<TaiKhoan> TaiKhoan { get; }
    DbSet<BenhNhan> BenhNhan { get; }
    DbSet<BacSi> BacSi { get; }
    DbSet<LeTan> LeTan { get; }
    DbSet<ChuyenKhoa> ChuyenKhoa { get; }
    DbSet<DichVu> DichVu { get; }
    DbSet<Phong> Phong { get; }
    DbSet<DinhNghiaCa> DinhNghiaCa { get; }
    DbSet<LichNoiTru> LichNoiTru { get; }
    DbSet<CaLamViec> CaLamViec { get; }
    DbSet<DonNghiPhep> DonNghiPhep { get; }
    DbSet<LichHen> LichHen { get; }
    DbSet<LichSuLichHen> LichSuLichHen { get; }
    DbSet<GiuCho> GiuCho { get; }
    DbSet<HangCho> HangCho { get; }
    DbSet<HoSoKham> HoSoKham { get; }
    DbSet<Thuoc> Thuoc { get; }
    DbSet<ToaThuoc> ToaThuoc { get; }
    DbSet<MauThongBao> MauThongBao { get; }
    DbSet<ThongBao> ThongBao { get; }
    DbSet<OtpLog> OtpLog { get; }
    DbSet<RefreshToken> RefreshToken { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
