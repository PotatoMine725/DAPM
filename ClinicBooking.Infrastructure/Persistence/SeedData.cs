using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Infrastructure.Persistence;

public static class SeedData
{
    private static readonly DateTime NgayTaoMacDinh =
        new(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // Hash gia lap cho mat khau admin. Khong verify duoc voi bat ky mat khau nao.
    // DatabaseSeeder se phat hien chuoi nay khi khoi dong app va thay bang hash
    // BCrypt that dua tren cau hinh Admin:MatKhauMacDinh.
    internal const string HashMatKhauAdminGiaLap =
        "$2a$11$GiaLapHashThayDoiTruocKhiDeployProdXXXXXXXXXXXXXXXXXXXX";

    public static void Apply(ModelBuilder modelBuilder)
    {
        SeedTaiKhoanAdmin(modelBuilder);
        SeedChuyenKhoa(modelBuilder);
        SeedPhong(modelBuilder);
        SeedDinhNghiaCa(modelBuilder);
        SeedDichVu(modelBuilder);
        SeedThuoc(modelBuilder);
        SeedMauThongBao(modelBuilder);
    }

    private static void SeedTaiKhoanAdmin(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TaiKhoan>().HasData(
            new TaiKhoan
            {
                IdTaiKhoan = 1,
                TenDangNhap = "admin",
                Email = "admin@phongkham.local",
                SoDienThoai = "0900000000",
                MatKhau = HashMatKhauAdminGiaLap,
                VaiTro = VaiTro.Admin,
                TrangThai = true,
                LanDangNhapCuoi = null,
                NgayTao = NgayTaoMacDinh
            }
        );
    }

    private static void SeedChuyenKhoa(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChuyenKhoa>().HasData(
            new ChuyenKhoa
            {
                IdChuyenKhoa = 1,
                TenChuyenKhoa = "Tim Mach",
                MoTa = "Kham va dieu tri cac benh ly ve tim va mach mau",
                ThoiGianSlotMacDinh = 20,
                GioMoDatLich = new TimeOnly(7, 0),
                GioDongDatLich = new TimeOnly(17, 0),
                HienThi = true
            },
            new ChuyenKhoa
            {
                IdChuyenKhoa = 2,
                TenChuyenKhoa = "Nhi Khoa",
                MoTa = "Kham va dieu tri cho tre em duoi 16 tuoi",
                ThoiGianSlotMacDinh = 15,
                GioMoDatLich = new TimeOnly(7, 0),
                GioDongDatLich = new TimeOnly(17, 0),
                HienThi = true
            },
            new ChuyenKhoa
            {
                IdChuyenKhoa = 3,
                TenChuyenKhoa = "Noi Tong Quat",
                MoTa = "Kham tong quat cac benh ly noi khoa",
                ThoiGianSlotMacDinh = 20,
                GioMoDatLich = new TimeOnly(7, 0),
                GioDongDatLich = new TimeOnly(17, 0),
                HienThi = true
            },
            new ChuyenKhoa
            {
                IdChuyenKhoa = 4,
                TenChuyenKhoa = "Ngoai Tong Quat",
                MoTa = "Kham va dieu tri cac benh ly ngoai khoa",
                ThoiGianSlotMacDinh = 30,
                GioMoDatLich = new TimeOnly(7, 0),
                GioDongDatLich = new TimeOnly(17, 0),
                HienThi = true
            },
            new ChuyenKhoa
            {
                IdChuyenKhoa = 5,
                TenChuyenKhoa = "Tai Mui Hong",
                MoTa = "Kham va dieu tri cac benh ly tai, mui, hong",
                ThoiGianSlotMacDinh = 15,
                GioMoDatLich = new TimeOnly(7, 0),
                GioDongDatLich = new TimeOnly(17, 0),
                HienThi = true
            },
            new ChuyenKhoa
            {
                IdChuyenKhoa = 6,
                TenChuyenKhoa = "Da Lieu",
                MoTa = "Kham va dieu tri cac benh ly ve da",
                ThoiGianSlotMacDinh = 20,
                GioMoDatLich = new TimeOnly(7, 0),
                GioDongDatLich = new TimeOnly(17, 0),
                HienThi = true
            }
        );
    }

    private static void SeedPhong(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Phong>().HasData(
            new Phong
            {
                IdPhong = 1,
                MaPhong = "P101",
                TenPhong = "Phong Kham 101",
                SucChua = 10,
                TrangBi = "Giuong kham, may do huyet ap, ong nghe",
                TrangThai = true
            },
            new Phong
            {
                IdPhong = 2,
                MaPhong = "P102",
                TenPhong = "Phong Kham 102",
                SucChua = 10,
                TrangBi = "Giuong kham, may do huyet ap, ong nghe",
                TrangThai = true
            },
            new Phong
            {
                IdPhong = 3,
                MaPhong = "P201",
                TenPhong = "Phong Kham Nhi 201",
                SucChua = 8,
                TrangBi = "Giuong kham tre em, can do chuyen dung",
                TrangThai = true
            },
            new Phong
            {
                IdPhong = 4,
                MaPhong = "P202",
                TenPhong = "Phong Kham 202",
                SucChua = 8,
                TrangBi = "Giuong kham, may do huyet ap",
                TrangThai = true
            },
            new Phong
            {
                IdPhong = 5,
                MaPhong = "P301",
                TenPhong = "Phong Sieu Am",
                SucChua = 5,
                TrangBi = "May sieu am 4D",
                TrangThai = true
            },
            new Phong
            {
                IdPhong = 6,
                MaPhong = "P302",
                TenPhong = "Phong X-quang",
                SucChua = 5,
                TrangBi = "May X-quang ky thuat so",
                TrangThai = true
            }
        );
    }

    private static void SeedDinhNghiaCa(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DinhNghiaCa>().HasData(
            new DinhNghiaCa
            {
                IdDinhNghiaCa = 1,
                TenCa = "sang",
                GioBatDauMacDinh = new TimeOnly(7, 0),
                GioKetThucMacDinh = new TimeOnly(12, 0),
                MoTa = "Ca sang: 07:00 - 12:00",
                TrangThai = true
            },
            new DinhNghiaCa
            {
                IdDinhNghiaCa = 2,
                TenCa = "chieu",
                GioBatDauMacDinh = new TimeOnly(13, 0),
                GioKetThucMacDinh = new TimeOnly(17, 0),
                MoTa = "Ca chieu: 13:00 - 17:00",
                TrangThai = true
            },
            new DinhNghiaCa
            {
                IdDinhNghiaCa = 3,
                TenCa = "toi",
                GioBatDauMacDinh = new TimeOnly(17, 0),
                GioKetThucMacDinh = new TimeOnly(21, 0),
                MoTa = "Ca toi: 17:00 - 21:00",
                TrangThai = true
            },
            new DinhNghiaCa
            {
                IdDinhNghiaCa = 4,
                TenCa = "sang_chieu",
                GioBatDauMacDinh = new TimeOnly(7, 0),
                GioKetThucMacDinh = new TimeOnly(17, 0),
                MoTa = "Ca gop sang + chieu: 07:00 - 17:00",
                TrangThai = true
            }
        );
    }

    private static void SeedDichVu(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DichVu>().HasData(
            new DichVu
            {
                IdDichVu = 1,
                IdChuyenKhoa = 1,
                TenDichVu = "Kham Tim Mach Tong Quat",
                MoTa = "Kham lam sang tim mach tong quat",
                ThoiGianUocTinh = 20,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 2,
                IdChuyenKhoa = 1,
                TenDichVu = "Dien Tim (ECG)",
                MoTa = "Do dien tim 12 chuyen dao",
                ThoiGianUocTinh = 15,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 3,
                IdChuyenKhoa = 1,
                TenDichVu = "Sieu Am Tim",
                MoTa = "Sieu am tim qua thanh nguc",
                ThoiGianUocTinh = 30,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 4,
                IdChuyenKhoa = 2,
                TenDichVu = "Kham Nhi Tong Quat",
                MoTa = "Kham tong quat cho tre em",
                ThoiGianUocTinh = 15,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 5,
                IdChuyenKhoa = 2,
                TenDichVu = "Tiem Chung",
                MoTa = "Tiem vaccine theo lich",
                ThoiGianUocTinh = 10,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 6,
                IdChuyenKhoa = 3,
                TenDichVu = "Kham Noi Tong Quat",
                MoTa = "Kham tong quat cac benh ly noi khoa",
                ThoiGianUocTinh = 20,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 7,
                IdChuyenKhoa = 4,
                TenDichVu = "Kham Ngoai Tong Quat",
                MoTa = "Kham lam sang ngoai khoa",
                ThoiGianUocTinh = 20,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 8,
                IdChuyenKhoa = 4,
                TenDichVu = "Tieu Phau",
                MoTa = "Tieu phau cac ca don gian",
                ThoiGianUocTinh = 45,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 9,
                IdChuyenKhoa = 5,
                TenDichVu = "Kham Tai Mui Hong",
                MoTa = "Kham lam sang tai, mui, hong",
                ThoiGianUocTinh = 15,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 10,
                IdChuyenKhoa = 5,
                TenDichVu = "Noi Soi Tai Mui Hong",
                MoTa = "Noi soi chan doan tai, mui, hong",
                ThoiGianUocTinh = 30,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 11,
                IdChuyenKhoa = 6,
                TenDichVu = "Kham Da Lieu",
                MoTa = "Kham cac benh ly ve da",
                ThoiGianUocTinh = 20,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 12,
                IdChuyenKhoa = 6,
                TenDichVu = "Soi Da Bang Dermoscope",
                MoTa = "Soi da chan doan bang dermoscope",
                ThoiGianUocTinh = 20,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            }
        );
    }

    private static void SeedThuoc(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Thuoc>().HasData(
            new Thuoc
            {
                IdThuoc = 1,
                TenThuoc = "Paracetamol 500mg",
                HoatChat = "Paracetamol",
                DonVi = "Vien",
                GhiChu = "Giam dau, ha sot"
            },
            new Thuoc
            {
                IdThuoc = 2,
                TenThuoc = "Amoxicillin 500mg",
                HoatChat = "Amoxicillin",
                DonVi = "Vien",
                GhiChu = "Khang sinh nhom Beta-lactam"
            },
            new Thuoc
            {
                IdThuoc = 3,
                TenThuoc = "Ibuprofen 400mg",
                HoatChat = "Ibuprofen",
                DonVi = "Vien",
                GhiChu = "Khang viem, giam dau NSAID"
            },
            new Thuoc
            {
                IdThuoc = 4,
                TenThuoc = "Omeprazole 20mg",
                HoatChat = "Omeprazole",
                DonVi = "Vien",
                GhiChu = "Uc che bom proton, dieu tri viem loet da day"
            },
            new Thuoc
            {
                IdThuoc = 5,
                TenThuoc = "Cetirizine 10mg",
                HoatChat = "Cetirizine",
                DonVi = "Vien",
                GhiChu = "Khang histamin H1, di ung"
            },
            new Thuoc
            {
                IdThuoc = 6,
                TenThuoc = "Vitamin C 500mg",
                HoatChat = "Ascorbic Acid",
                DonVi = "Vien",
                GhiChu = "Bo sung vitamin C"
            },
            new Thuoc
            {
                IdThuoc = 7,
                TenThuoc = "Nuoc Muoi Sinh Ly 0.9%",
                HoatChat = "NaCl 0.9%",
                DonVi = "Chai",
                GhiChu = "Rua mat, rua mui"
            },
            new Thuoc
            {
                IdThuoc = 8,
                TenThuoc = "Loratadine 10mg",
                HoatChat = "Loratadine",
                DonVi = "Vien",
                GhiChu = "Khang histamin H1 the he 2"
            }
        );
    }

    private static void SeedMauThongBao(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MauThongBao>().HasData(
            new MauThongBao
            {
                IdMau = 1,
                LoaiThongBao = LoaiThongBao.XacNhanLich,
                TieuDeMau = "Xac nhan lich hen {ma_lich_hen}",
                NoiDungMau = "Xin chao {ten_benh_nhan}, lich hen cua ban ma {ma_lich_hen} vao ngay {ngay_kham} da duoc xac nhan. Vui long den truoc gio hen 15 phut.",
                KenhGui = KenhGui.Email
            },
            new MauThongBao
            {
                IdMau = 2,
                LoaiThongBao = LoaiThongBao.Nhac1Ngay,
                TieuDeMau = "Nhac lich kham ngay mai",
                NoiDungMau = "Xin chao {ten_benh_nhan}, ban co lich hen kham vao ngay mai {ngay_kham} luc {gio_kham}. Ma lich hen: {ma_lich_hen}.",
                KenhGui = KenhGui.Email
            },
            new MauThongBao
            {
                IdMau = 3,
                LoaiThongBao = LoaiThongBao.Nhac2Gio,
                TieuDeMau = "Nhac lich kham 2 gio toi",
                NoiDungMau = "{ten_benh_nhan}, ban co lich kham luc {gio_kham} hom nay. Ma: {ma_lich_hen}.",
                KenhGui = KenhGui.Sms
            },
            new MauThongBao
            {
                IdMau = 4,
                LoaiThongBao = LoaiThongBao.HuyLich,
                TieuDeMau = "Huy lich hen {ma_lich_hen}",
                NoiDungMau = "Xin chao {ten_benh_nhan}, lich hen {ma_lich_hen} vao ngay {ngay_kham} da duoc huy. Ly do: {ly_do}.",
                KenhGui = KenhGui.Email
            },
            new MauThongBao
            {
                IdMau = 5,
                LoaiThongBao = LoaiThongBao.CheckIn,
                TieuDeMau = "Check-in thanh cong",
                NoiDungMau = "Ban da check-in thanh cong. So thu tu cua ban la {so_thu_tu}. Vui long cho goi ten.",
                KenhGui = KenhGui.TrongApp
            },
            new MauThongBao
            {
                IdMau = 6,
                LoaiThongBao = LoaiThongBao.DuyetCa,
                TieuDeMau = "Ca lam viec da duoc duyet",
                NoiDungMau = "Ca lam viec ngay {ngay_lam_viec} cua ban da duoc admin duyet.",
                KenhGui = KenhGui.TrongApp
            },
            new MauThongBao
            {
                IdMau = 7,
                LoaiThongBao = LoaiThongBao.TuChoiCa,
                TieuDeMau = "Ca lam viec bi tu choi",
                NoiDungMau = "Ca lam viec ngay {ngay_lam_viec} cua ban da bi admin tu choi. Ly do: {ly_do}.",
                KenhGui = KenhGui.TrongApp
            }
        );
    }
}
