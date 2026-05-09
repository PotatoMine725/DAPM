using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using ClinicBooking.Infrastructure.Persistence.Migrations;
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
        Module1TestDataSeeder.SeedModule1TestData(modelBuilder);
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
                TenChuyenKhoa = "Tim Mạch",
                MoTa = "Khám và điều trị các bệnh lý về tim và mạch máu",
                ThoiGianSlotMacDinh = 20,
                GioMoDatLich = new TimeOnly(7, 0),
                GioDongDatLich = new TimeOnly(17, 0),
                HienThi = true
            },
            new ChuyenKhoa
            {
                IdChuyenKhoa = 2,
                TenChuyenKhoa = "Nhi Khoa",
                MoTa = "Khám và điều trị cho trẻ em dưới 16 tuổi",
                ThoiGianSlotMacDinh = 15,
                GioMoDatLich = new TimeOnly(7, 0),
                GioDongDatLich = new TimeOnly(17, 0),
                HienThi = true
            },
            new ChuyenKhoa
            {
                IdChuyenKhoa = 3,
                TenChuyenKhoa = "Nội Tổng Quát",
                MoTa = "Khám tổng quát các bệnh lý nội khoa",
                ThoiGianSlotMacDinh = 20,
                GioMoDatLich = new TimeOnly(7, 0),
                GioDongDatLich = new TimeOnly(17, 0),
                HienThi = true
            },
            new ChuyenKhoa
            {
                IdChuyenKhoa = 4,
                TenChuyenKhoa = "Ngoại Tổng Quát",
                MoTa = "Khám và điều trị các bệnh lý ngoại khoa",
                ThoiGianSlotMacDinh = 30,
                GioMoDatLich = new TimeOnly(7, 0),
                GioDongDatLich = new TimeOnly(17, 0),
                HienThi = true
            },
            new ChuyenKhoa
            {
                IdChuyenKhoa = 5,
                TenChuyenKhoa = "Tai Mũi Họng",
                MoTa = "Khám và điều trị các bệnh lý tai, mũi, họng",
                ThoiGianSlotMacDinh = 15,
                GioMoDatLich = new TimeOnly(7, 0),
                GioDongDatLich = new TimeOnly(17, 0),
                HienThi = true
            },
            new ChuyenKhoa
            {
                IdChuyenKhoa = 6,
                TenChuyenKhoa = "Da Liễu",
                MoTa = "Khám và điều trị các bệnh lý về da",
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
                TenPhong = "Phòng Khám 101",
                SucChua = 10,
                TrangBi = "Giường khám, máy đo huyết áp, ống nghe",
                TrangThai = true
            },
            new Phong
            {
                IdPhong = 2,
                MaPhong = "P102",
                TenPhong = "Phòng Khám 102",
                SucChua = 10,
                TrangBi = "Giường khám, máy đo huyết áp, ống nghe",
                TrangThai = true
            },
            new Phong
            {
                IdPhong = 3,
                MaPhong = "P201",
                TenPhong = "Phòng Khám Nhi 201",
                SucChua = 8,
                TrangBi = "Giường khám trẻ em, cân đo chuyên dụng",
                TrangThai = true
            },
            new Phong
            {
                IdPhong = 4,
                MaPhong = "P202",
                TenPhong = "Phòng Khám 202",
                SucChua = 8,
                TrangBi = "Giường khám, máy đo huyết áp",
                TrangThai = true
            },
            new Phong
            {
                IdPhong = 5,
                MaPhong = "P301",
                TenPhong = "Phòng Siêu Âm",
                SucChua = 5,
                TrangBi = "Máy siêu âm 4D",
                TrangThai = true
            },
            new Phong
            {
                IdPhong = 6,
                MaPhong = "P302",
                TenPhong = "Phòng X-Quang",
                SucChua = 5,
                TrangBi = "Máy X-quang kỹ thuật số",
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
                MoTa = "Ca sáng: 07:00 - 12:00",
                TrangThai = true
            },
            new DinhNghiaCa
            {
                IdDinhNghiaCa = 2,
                TenCa = "chieu",
                GioBatDauMacDinh = new TimeOnly(13, 0),
                GioKetThucMacDinh = new TimeOnly(17, 0),
                MoTa = "Ca chiều: 13:00 - 17:00",
                TrangThai = true
            },
            new DinhNghiaCa
            {
                IdDinhNghiaCa = 3,
                TenCa = "toi",
                GioBatDauMacDinh = new TimeOnly(17, 0),
                GioKetThucMacDinh = new TimeOnly(21, 0),
                MoTa = "Ca tối: 17:00 - 21:00",
                TrangThai = true
            },
            new DinhNghiaCa
            {
                IdDinhNghiaCa = 4,
                TenCa = "sang_chieu",
                GioBatDauMacDinh = new TimeOnly(7, 0),
                GioKetThucMacDinh = new TimeOnly(17, 0),
                MoTa = "Ca gộp sáng + chiều: 07:00 - 17:00",
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
                TenDichVu = "Khám Tim Mạch Tổng Quát",
                MoTa = "Khám lâm sàng tim mạch tổng quát",
                ThoiGianUocTinh = 20,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 2,
                IdChuyenKhoa = 1,
                TenDichVu = "Điện Tim (ECG)",
                MoTa = "Đo điện tim 12 chuyển đạo",
                ThoiGianUocTinh = 15,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 3,
                IdChuyenKhoa = 1,
                TenDichVu = "Siêu Âm Tim",
                MoTa = "Siêu âm tim qua thành ngực",
                ThoiGianUocTinh = 30,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 4,
                IdChuyenKhoa = 2,
                TenDichVu = "Khám Nhi Tổng Quát",
                MoTa = "Khám tổng quát cho trẻ em",
                ThoiGianUocTinh = 15,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 5,
                IdChuyenKhoa = 2,
                TenDichVu = "Tiêm Chủng",
                MoTa = "Tiêm vaccine theo lịch",
                ThoiGianUocTinh = 10,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 6,
                IdChuyenKhoa = 3,
                TenDichVu = "Khám Nội Tổng Quát",
                MoTa = "Khám tổng quát các bệnh lý nội khoa",
                ThoiGianUocTinh = 20,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 7,
                IdChuyenKhoa = 4,
                TenDichVu = "Khám Ngoại Tổng Quát",
                MoTa = "Khám lâm sàng ngoại khoa",
                ThoiGianUocTinh = 20,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 8,
                IdChuyenKhoa = 4,
                TenDichVu = "Tiểu Phẫu",
                MoTa = "Tiểu phẫu các ca đơn giản",
                ThoiGianUocTinh = 45,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 9,
                IdChuyenKhoa = 5,
                TenDichVu = "Khám Tai Mũi Họng",
                MoTa = "Khám lâm sàng tai, mũi, họng",
                ThoiGianUocTinh = 15,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 10,
                IdChuyenKhoa = 5,
                TenDichVu = "Nội Soi Tai Mũi Họng",
                MoTa = "Nội soi chẩn đoán tai, mũi, họng",
                ThoiGianUocTinh = 30,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 11,
                IdChuyenKhoa = 6,
                TenDichVu = "Khám Da Liễu",
                MoTa = "Khám các bệnh lý về da",
                ThoiGianUocTinh = 20,
                HienThi = true,
                NgayTao = NgayTaoMacDinh
            },
            new DichVu
            {
                IdDichVu = 12,
                IdChuyenKhoa = 6,
                TenDichVu = "Soi Da Bằng Dermoscope",
                MoTa = "Soi da chẩn đoán bằng dermoscope",
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
                DonVi = "Viên",
                GhiChu = "Giảm đau, hạ sốt"
            },
            new Thuoc
            {
                IdThuoc = 2,
                TenThuoc = "Amoxicillin 500mg",
                HoatChat = "Amoxicillin",
                DonVi = "Viên",
                GhiChu = "Kháng sinh nhóm Beta-lactam"
            },
            new Thuoc
            {
                IdThuoc = 3,
                TenThuoc = "Ibuprofen 400mg",
                HoatChat = "Ibuprofen",
                DonVi = "Viên",
                GhiChu = "Kháng viêm, giảm đau NSAID"
            },
            new Thuoc
            {
                IdThuoc = 4,
                TenThuoc = "Omeprazole 20mg",
                HoatChat = "Omeprazole",
                DonVi = "Viên",
                GhiChu = "Ức chế bơm proton, điều trị viêm loét dạ dày"
            },
            new Thuoc
            {
                IdThuoc = 5,
                TenThuoc = "Cetirizine 10mg",
                HoatChat = "Cetirizine",
                DonVi = "Viên",
                GhiChu = "Kháng histamin H1, dị ứng"
            },
            new Thuoc
            {
                IdThuoc = 6,
                TenThuoc = "Vitamin C 500mg",
                HoatChat = "Ascorbic Acid",
                DonVi = "Viên",
                GhiChu = "Bổ sung vitamin C"
            },
            new Thuoc
            {
                IdThuoc = 7,
                TenThuoc = "Nước Muối Sinh Lý 0.9%",
                HoatChat = "NaCl 0.9%",
                DonVi = "Chai",
                GhiChu = "Rửa mắt, rửa mũi"
            },
            new Thuoc
            {
                IdThuoc = 8,
                TenThuoc = "Loratadine 10mg",
                HoatChat = "Loratadine",
                DonVi = "Viên",
                GhiChu = "Kháng histamin H1 thế hệ 2"
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
                TieuDeMau = "Xác nhận lịch hẹn {ma_lich_hen}",
                NoiDungMau = "Xin chào {ten_benh_nhan}, lịch hẹn của bạn mã {ma_lich_hen} vào ngày {ngay_kham} đã được xác nhận. Vui lòng đến trước giờ hẹn 15 phút.",
                KenhGui = KenhGui.Email
            },
            new MauThongBao
            {
                IdMau = 2,
                LoaiThongBao = LoaiThongBao.Nhac1Ngay,
                TieuDeMau = "Nhắc lịch khám ngày mai",
                NoiDungMau = "Xin chào {ten_benh_nhan}, bạn có lịch hẹn khám vào ngày mai {ngay_kham} lúc {gio_kham}. Mã lịch hẹn: {ma_lich_hen}.",
                KenhGui = KenhGui.Email
            },
            new MauThongBao
            {
                IdMau = 3,
                LoaiThongBao = LoaiThongBao.Nhac2Gio,
                TieuDeMau = "Nhắc lịch khám 2 giờ tới",
                NoiDungMau = "{ten_benh_nhan}, bạn có lịch khám lúc {gio_kham} hôm nay. Mã: {ma_lich_hen}.",
                KenhGui = KenhGui.Sms
            },
            new MauThongBao
            {
                IdMau = 4,
                LoaiThongBao = LoaiThongBao.HuyLich,
                TieuDeMau = "Hủy lịch hẹn {ma_lich_hen}",
                NoiDungMau = "Xin chào {ten_benh_nhan}, lịch hẹn {ma_lich_hen} vào ngày {ngay_kham} đã được hủy. Lý do: {ly_do}.",
                KenhGui = KenhGui.Email
            },
            new MauThongBao
            {
                IdMau = 5,
                LoaiThongBao = LoaiThongBao.CheckIn,
                TieuDeMau = "Check-in thành công",
                NoiDungMau = "Bạn đã check-in thành công. Số thứ tự của bạn là {so_thu_tu}. Vui lòng chờ gọi tên.",
                KenhGui = KenhGui.TrongApp
            },
            new MauThongBao
            {
                IdMau = 6,
                LoaiThongBao = LoaiThongBao.DuyetCa,
                TieuDeMau = "Ca làm việc đã được duyệt",
                NoiDungMau = "Ca làm việc ngày {ngay_lam_viec} của bạn đã được admin duyệt.",
                KenhGui = KenhGui.TrongApp
            },
            new MauThongBao
            {
                IdMau = 7,
                LoaiThongBao = LoaiThongBao.TuChoiCa,
                TieuDeMau = "Ca làm việc bị từ chối",
                NoiDungMau = "Ca làm việc ngày {ngay_lam_viec} của bạn đã bị admin từ chối. Lý do: {ly_do}.",
                KenhGui = KenhGui.TrongApp
            }
        );
    }
}
