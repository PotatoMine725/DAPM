using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Infrastructure.Persistence;

public class AppDbContext : DbContext, IAppDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<TaiKhoan> TaiKhoan => Set<TaiKhoan>();
    public DbSet<BenhNhan> BenhNhan => Set<BenhNhan>();
    public DbSet<BacSi> BacSi => Set<BacSi>();
    public DbSet<LeTan> LeTan => Set<LeTan>();
    public DbSet<ChuyenKhoa> ChuyenKhoa => Set<ChuyenKhoa>();
    public DbSet<DichVu> DichVu => Set<DichVu>();
    public DbSet<Phong> Phong => Set<Phong>();
    public DbSet<DinhNghiaCa> DinhNghiaCa => Set<DinhNghiaCa>();
    public DbSet<LichNoiTru> LichNoiTru => Set<LichNoiTru>();
    public DbSet<CaLamViec> CaLamViec => Set<CaLamViec>();
    public DbSet<DonNghiPhep> DonNghiPhep => Set<DonNghiPhep>();
    public DbSet<LichHen> LichHen => Set<LichHen>();
    public DbSet<LichSuLichHen> LichSuLichHen => Set<LichSuLichHen>();
    public DbSet<GiuCho> GiuCho => Set<GiuCho>();
    public DbSet<HangCho> HangCho => Set<HangCho>();
    public DbSet<HoSoKham> HoSoKham => Set<HoSoKham>();
    public DbSet<Thuoc> Thuoc => Set<Thuoc>();
    public DbSet<ToaThuoc> ToaThuoc => Set<ToaThuoc>();
    public DbSet<MauThongBao> MauThongBao => Set<MauThongBao>();
    public DbSet<ThongBao> ThongBao => Set<ThongBao>();
    public DbSet<OtpLog> OtpLog => Set<OtpLog>();
    public DbSet<RefreshToken> RefreshToken => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TaiKhoan>(e =>
        {
            e.HasKey(x => x.IdTaiKhoan);
            e.HasIndex(x => x.TenDangNhap).IsUnique();
            e.HasIndex(x => x.Email).IsUnique();
            e.HasIndex(x => x.SoDienThoai).IsUnique();
            e.Property(x => x.VaiTro).HasConversion<string>();
            e.Property(x => x.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<BenhNhan>(e =>
        {
            e.HasKey(x => x.IdBenhNhan);
            e.HasIndex(x => x.Cccd).IsUnique();
            e.Property(x => x.GioiTinh).HasConversion<string>();
            e.Property(x => x.SoLanHuyMuon).HasDefaultValue(0);
            e.Property(x => x.BiHanChe).HasDefaultValue(false);

            e.HasOne(x => x.TaiKhoan)
                .WithOne(x => x.BenhNhan)
                .HasForeignKey<BenhNhan>(x => x.IdTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BacSi>(e =>
        {
            e.HasKey(x => x.IdBacSi);
            e.Property(x => x.LoaiHopDong).HasConversion<string>();
            e.Property(x => x.TrangThai).HasConversion<string>();

            e.HasOne(x => x.TaiKhoan)
                .WithOne(x => x.BacSi)
                .HasForeignKey<BacSi>(x => x.IdTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.ChuyenKhoa)
                .WithMany(x => x.DanhSachBacSi)
                .HasForeignKey(x => x.IdChuyenKhoa)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LeTan>(e =>
        {
            e.HasKey(x => x.IdLeTan);

            e.HasOne(x => x.TaiKhoan)
                .WithOne(x => x.LeTan)
                .HasForeignKey<LeTan>(x => x.IdTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<ChuyenKhoa>(e =>
        {
            e.HasKey(x => x.IdChuyenKhoa);
            e.HasIndex(x => x.TenChuyenKhoa).IsUnique();
            e.Property(x => x.HienThi).HasDefaultValue(true);
        });

        modelBuilder.Entity<DichVu>(e =>
        {
            e.HasKey(x => x.IdDichVu);
            e.Property(x => x.HienThi).HasDefaultValue(true);

            e.HasOne(x => x.ChuyenKhoa)
                .WithMany(x => x.DanhSachDichVu)
                .HasForeignKey(x => x.IdChuyenKhoa)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Phong>(e =>
        {
            e.HasKey(x => x.IdPhong);
            e.HasIndex(x => x.MaPhong).IsUnique();
            e.Property(x => x.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<DinhNghiaCa>(e =>
        {
            e.HasKey(x => x.IdDinhNghiaCa);
            e.HasIndex(x => x.TenCa).IsUnique();
            e.Property(x => x.TrangThai).HasDefaultValue(true);
        });

        modelBuilder.Entity<LichNoiTru>(e =>
        {
            e.HasKey(x => x.IdLichNoiTru);
            e.Property(x => x.TrangThai).HasDefaultValue(true);

            e.HasOne(x => x.BacSi)
                .WithMany(x => x.DanhSachLichNoiTru)
                .HasForeignKey(x => x.IdBacSi)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Phong)
                .WithMany(x => x.DanhSachLichNoiTru)
                .HasForeignKey(x => x.IdPhong)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.DinhNghiaCa)
                .WithMany(x => x.DanhSachLichNoiTru)
                .HasForeignKey(x => x.IdDinhNghiaCa)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CaLamViec>(e =>
        {
            e.HasKey(x => x.IdCaLamViec);
            e.Property(x => x.TrangThaiDuyet).HasConversion<string>();
            e.Property(x => x.NguonTaoCa).HasConversion<string>();
            e.Property(x => x.SoSlotDaDat).HasDefaultValue(0);

            e.HasOne(x => x.BacSi)
                .WithMany(x => x.DanhSachCaLamViec)
                .HasForeignKey(x => x.IdBacSi)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.Phong)
                .WithMany(x => x.DanhSachCaLamViec)
                .HasForeignKey(x => x.IdPhong)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.ChuyenKhoa)
                .WithMany(x => x.DanhSachCaLamViec)
                .HasForeignKey(x => x.IdChuyenKhoa)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.DinhNghiaCa)
                .WithMany(x => x.DanhSachCaLamViec)
                .HasForeignKey(x => x.IdDinhNghiaCa)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.BacSiYeuCau)
                .WithMany()
                .HasForeignKey(x => x.IdBacSiYeuCau)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.AdminDuyet)
                .WithMany()
                .HasForeignKey(x => x.IdAdminDuyet)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<DonNghiPhep>(e =>
        {
            e.HasKey(x => x.IdDonNghiPhep);
            e.Property(x => x.LoaiNghiPhep).HasConversion<string>();
            e.Property(x => x.TrangThaiDuyet).HasConversion<string>();

            e.HasOne(x => x.BacSi)
                .WithMany(x => x.DanhSachDonNghiPhep)
                .HasForeignKey(x => x.IdBacSi)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.CaLamViec)
                .WithMany(x => x.DanhSachDonNghiPhep)
                .HasForeignKey(x => x.IdCaLamViec)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.NguoiDuyet)
                .WithMany()
                .HasForeignKey(x => x.IdNguoiDuyet)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LichHen>(e =>
        {
            e.HasKey(x => x.IdLichHen);
            e.HasIndex(x => x.MaLichHen).IsUnique();
            e.Property(x => x.MaLichHen).HasMaxLength(32).IsRequired();
            e.Property(x => x.HinhThucDat).HasConversion<string>();
            e.Property(x => x.TrangThai).HasConversion<string>();

            // Module 1: concurrency token bao ve race condition huy/doi lich dong thoi.
            // SQLite khong ho tro rowversion (se fail NOT NULL khi test) -> skip IsRowVersion khi test.
            // Production dung SQL Server nen van duoc bat.
            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
            {
                e.Property(x => x.RowVersion).IsRowVersion();
            }
            else
            {
                e.Property(x => x.RowVersion).IsConcurrencyToken();
            }

            // Module 1: unique (IdCaLamViec, SoSlot) chong double-book
            // Xac nhan tu clinic.dbml dong 203: SoSlot la ordinal position trong ca
            e.HasIndex(x => new { x.IdCaLamViec, x.SoSlot }).IsUnique();

            // Module 1: index phuc vu query "danh sach lich hen cua toi" (benh nhan)
            e.HasIndex(x => new { x.IdBenhNhan, x.TrangThai });

            // Module 1: index phuc vu query "danh sach lich hen theo ngay" (le tan/admin)
            e.HasIndex(x => new { x.IdCaLamViec, x.TrangThai });

            e.HasOne(x => x.BenhNhan)
                .WithMany(x => x.DanhSachLichHen)
                .HasForeignKey(x => x.IdBenhNhan)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.CaLamViec)
                .WithMany(x => x.DanhSachLichHen)
                .HasForeignKey(x => x.IdCaLamViec)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.DichVu)
                .WithMany(x => x.DanhSachLichHen)
                .HasForeignKey(x => x.IdDichVu)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.BacSiMongMuon)
                .WithMany()
                .HasForeignKey(x => x.IdBacSiMongMuon)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<LichSuLichHen>(e =>
        {
            e.HasKey(x => x.IdLichSu);
            e.Property(x => x.HanhDong).HasConversion<string>();
            e.Property(x => x.DanhDauHuyMuon).HasDefaultValue(false);

            // Module 1: index phuc vu tab "lich su" cua mot lich hen
            e.HasIndex(x => x.IdLichHen);

            e.HasOne(x => x.LichHen)
                .WithMany(x => x.DanhSachLichSu)
                .HasForeignKey(x => x.IdLichHen)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.NguoiThucHien)
                .WithMany()
                .HasForeignKey(x => x.IdNguoiThucHien)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.LichHenTruoc)
                .WithMany()
                .HasForeignKey(x => x.IdLichHenTruoc)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<GiuCho>(e =>
        {
            e.HasKey(x => x.IdGiuCho);
            e.Property(x => x.DaGiaiPhong).HasDefaultValue(false);

            // Module 1: index phuc vu lookup giu cho con hieu luc (chua giai phong, chua het han)
            e.HasIndex(x => new { x.IdCaLamViec, x.DaGiaiPhong, x.GioHetHan });

            e.HasOne(x => x.CaLamViec)
                .WithMany(x => x.DanhSachGiuCho)
                .HasForeignKey(x => x.IdCaLamViec)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.BenhNhan)
                .WithMany(x => x.DanhSachGiuCho)
                .HasForeignKey(x => x.IdBenhNhan)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<HangCho>(e =>
        {
            e.HasKey(x => x.IdHangCho);
            e.Property(x => x.TrangThai).HasConversion<string>();

            // Module 1: unique (IdCaLamViec, SoThuTu) dam bao khong trung so thu tu trong 1 ca
            e.HasIndex(x => new { x.IdCaLamViec, x.SoThuTu }).IsUnique();

            // Module 1: index phuc vu query "goi benh nhan ke tiep" + "xem hang cho theo ca"
            e.HasIndex(x => new { x.IdCaLamViec, x.TrangThai, x.SoThuTu });

            e.HasOne(x => x.CaLamViec)
                .WithMany(x => x.DanhSachHangCho)
                .HasForeignKey(x => x.IdCaLamViec)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.LichHen)
                .WithOne(x => x.HangCho)
                .HasForeignKey<HangCho>(x => x.IdLichHen)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<HoSoKham>(e =>
        {
            e.HasKey(x => x.IdHoSoKham);

            e.HasOne(x => x.LichHen)
                .WithOne(x => x.HoSoKham)
                .HasForeignKey<HoSoKham>(x => x.IdLichHen)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.BacSi)
                .WithMany(x => x.DanhSachHoSoKham)
                .HasForeignKey(x => x.IdBacSi)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Thuoc>(e =>
        {
            e.HasKey(x => x.IdThuoc);
            e.HasIndex(x => x.TenThuoc).IsUnique();
        });

        modelBuilder.Entity<ToaThuoc>(e =>
        {
            e.HasKey(x => x.IdToaThuoc);

            e.HasOne(x => x.HoSoKham)
                .WithMany(x => x.DanhSachToaThuoc)
                .HasForeignKey(x => x.IdHoSoKham)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Thuoc)
                .WithMany(x => x.DanhSachToaThuoc)
                .HasForeignKey(x => x.IdThuoc)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<MauThongBao>(e =>
        {
            e.HasKey(x => x.IdMau);
            e.Property(x => x.LoaiThongBao).HasConversion<string>();
            e.Property(x => x.KenhGui).HasConversion<string>();
            e.HasIndex(x => x.LoaiThongBao).IsUnique();
        });

        modelBuilder.Entity<ThongBao>(e =>
        {
            e.HasKey(x => x.IdThongBao);
            e.Property(x => x.KenhGui).HasConversion<string>();
            e.Property(x => x.LoaiThamChieu).HasConversion<string>();
            e.Property(x => x.DaDoc).HasDefaultValue(false);

            e.HasOne(x => x.TaiKhoan)
                .WithMany(x => x.DanhSachThongBao)
                .HasForeignKey(x => x.IdTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(x => x.MauThongBao)
                .WithMany(x => x.DanhSachThongBao)
                .HasForeignKey(x => x.IdMau)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OtpLog>(e =>
        {
            e.HasKey(x => x.IdOtpLog);
            e.Property(x => x.MucDich).HasConversion<string>();
            e.Property(x => x.DaSuDung).HasDefaultValue(false);
            e.Property(x => x.SoLanThu).HasDefaultValue(0);
            e.HasIndex(x => new { x.IdTaiKhoan, x.MucDich, x.NgayTao });

            e.HasOne(x => x.TaiKhoan)
                .WithMany(x => x.DanhSachOtpLog)
                .HasForeignKey(x => x.IdTaiKhoan)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.HasKey(x => x.IdRefreshToken);
            e.HasIndex(x => x.Token).IsUnique();
            e.HasIndex(x => x.IdTaiKhoan);
            e.Property(x => x.Token).HasMaxLength(256).IsRequired();
            e.Property(x => x.LyDoThuHoi).HasMaxLength(256);
            e.Property(x => x.ThayTheBangToken).HasMaxLength(256);
            e.Property(x => x.DaThuHoi).HasDefaultValue(false);

            e.HasOne(x => x.TaiKhoan)
                .WithMany(x => x.DanhSachRefreshToken)
                .HasForeignKey(x => x.IdTaiKhoan)
                .OnDelete(DeleteBehavior.Cascade);
        });

        SeedData.Apply(modelBuilder);
    }
}
