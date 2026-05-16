using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicBooking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedCoDau : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ChuyenKhoa",
                keyColumn: "IdChuyenKhoa",
                keyValue: 1,
                columns: new[] { "MoTa", "TenChuyenKhoa" },
                values: new object[] { "Khám và điều trị các bệnh lý về tim và mạch máu", "Tim Mạch" });

            migrationBuilder.UpdateData(
                table: "ChuyenKhoa",
                keyColumn: "IdChuyenKhoa",
                keyValue: 2,
                column: "MoTa",
                value: "Khám và điều trị cho trẻ em dưới 16 tuổi");

            migrationBuilder.UpdateData(
                table: "ChuyenKhoa",
                keyColumn: "IdChuyenKhoa",
                keyValue: 3,
                columns: new[] { "MoTa", "TenChuyenKhoa" },
                values: new object[] { "Khám tổng quát các bệnh lý nội khoa", "Nội Tổng Quát" });

            migrationBuilder.UpdateData(
                table: "ChuyenKhoa",
                keyColumn: "IdChuyenKhoa",
                keyValue: 4,
                columns: new[] { "MoTa", "TenChuyenKhoa" },
                values: new object[] { "Khám và điều trị các bệnh lý ngoại khoa", "Ngoại Tổng Quát" });

            migrationBuilder.UpdateData(
                table: "ChuyenKhoa",
                keyColumn: "IdChuyenKhoa",
                keyValue: 5,
                columns: new[] { "MoTa", "TenChuyenKhoa" },
                values: new object[] { "Khám và điều trị các bệnh lý tai, mũi, họng", "Tai Mũi Họng" });

            migrationBuilder.UpdateData(
                table: "ChuyenKhoa",
                keyColumn: "IdChuyenKhoa",
                keyValue: 6,
                columns: new[] { "MoTa", "TenChuyenKhoa" },
                values: new object[] { "Khám và điều trị các bệnh lý về da", "Da Liễu" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 1,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Khám lâm sàng tim mạch tổng quát", "Khám Tim Mạch Tổng Quát" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 2,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Đo điện tim 12 chuyển đạo", "Điện Tim (ECG)" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 3,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Siêu âm tim qua thành ngực", "Siêu Âm Tim" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 4,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Khám tổng quát cho trẻ em", "Khám Nhi Tổng Quát" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 5,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Tiêm vaccine theo lịch", "Tiêm Chủng" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 6,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Khám tổng quát các bệnh lý nội khoa", "Khám Nội Tổng Quát" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 7,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Khám lâm sàng ngoại khoa", "Khám Ngoại Tổng Quát" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 8,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Tiểu phẫu các ca đơn giản", "Tiểu Phẫu" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 9,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Khám lâm sàng tai, mũi, họng", "Khám Tai Mũi Họng" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 10,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Nội soi chẩn đoán tai, mũi, họng", "Nội Soi Tai Mũi Họng" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 11,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Khám các bệnh lý về da", "Khám Da Liễu" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 12,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Soi da chẩn đoán bằng dermoscope", "Soi Da Bằng Dermoscope" });

            migrationBuilder.UpdateData(
                table: "DinhNghiaCa",
                keyColumn: "IdDinhNghiaCa",
                keyValue: 1,
                column: "MoTa",
                value: "Ca sáng: 07:00 - 12:00");

            migrationBuilder.UpdateData(
                table: "DinhNghiaCa",
                keyColumn: "IdDinhNghiaCa",
                keyValue: 2,
                column: "MoTa",
                value: "Ca chiều: 13:00 - 17:00");

            migrationBuilder.UpdateData(
                table: "DinhNghiaCa",
                keyColumn: "IdDinhNghiaCa",
                keyValue: 3,
                column: "MoTa",
                value: "Ca tối: 17:00 - 21:00");

            migrationBuilder.UpdateData(
                table: "DinhNghiaCa",
                keyColumn: "IdDinhNghiaCa",
                keyValue: 4,
                column: "MoTa",
                value: "Ca gộp sáng + chiều: 07:00 - 17:00");

            migrationBuilder.UpdateData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 1,
                columns: new[] { "NoiDungMau", "TieuDeMau" },
                values: new object[] { "Xin chào {ten_benh_nhan}, lịch hẹn của bạn mã {ma_lich_hen} vào ngày {ngay_kham} đã được xác nhận. Vui lòng đến trước giờ hẹn 15 phút.", "Xác nhận lịch hẹn {ma_lich_hen}" });

            migrationBuilder.UpdateData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 2,
                columns: new[] { "NoiDungMau", "TieuDeMau" },
                values: new object[] { "Xin chào {ten_benh_nhan}, bạn có lịch hẹn khám vào ngày mai {ngay_kham} lúc {gio_kham}. Mã lịch hẹn: {ma_lich_hen}.", "Nhắc lịch khám ngày mai" });

            migrationBuilder.UpdateData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 3,
                columns: new[] { "NoiDungMau", "TieuDeMau" },
                values: new object[] { "{ten_benh_nhan}, bạn có lịch khám lúc {gio_kham} hôm nay. Mã: {ma_lich_hen}.", "Nhắc lịch khám 2 giờ tới" });

            migrationBuilder.UpdateData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 4,
                columns: new[] { "NoiDungMau", "TieuDeMau" },
                values: new object[] { "Xin chào {ten_benh_nhan}, lịch hẹn {ma_lich_hen} vào ngày {ngay_kham} đã được hủy. Lý do: {ly_do}.", "Hủy lịch hẹn {ma_lich_hen}" });

            migrationBuilder.UpdateData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 5,
                columns: new[] { "NoiDungMau", "TieuDeMau" },
                values: new object[] { "Bạn đã check-in thành công. Số thứ tự của bạn là {so_thu_tu}. Vui lòng chờ gọi tên.", "Check-in thành công" });

            migrationBuilder.UpdateData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 6,
                columns: new[] { "NoiDungMau", "TieuDeMau" },
                values: new object[] { "Ca làm việc ngày {ngay_lam_viec} của bạn đã được admin duyệt.", "Ca làm việc đã được duyệt" });

            migrationBuilder.UpdateData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 7,
                columns: new[] { "NoiDungMau", "TieuDeMau" },
                values: new object[] { "Ca làm việc ngày {ngay_lam_viec} của bạn đã bị admin từ chối. Lý do: {ly_do}.", "Ca làm việc bị từ chối" });

            migrationBuilder.UpdateData(
                table: "Phong",
                keyColumn: "IdPhong",
                keyValue: 1,
                columns: new[] { "TenPhong", "TrangBi" },
                values: new object[] { "Phòng Khám 101", "Giường khám, máy đo huyết áp, ống nghe" });

            migrationBuilder.UpdateData(
                table: "Phong",
                keyColumn: "IdPhong",
                keyValue: 2,
                columns: new[] { "TenPhong", "TrangBi" },
                values: new object[] { "Phòng Khám 102", "Giường khám, máy đo huyết áp, ống nghe" });

            migrationBuilder.UpdateData(
                table: "Phong",
                keyColumn: "IdPhong",
                keyValue: 3,
                columns: new[] { "TenPhong", "TrangBi" },
                values: new object[] { "Phòng Khám Nhi 201", "Giường khám trẻ em, cân đo chuyên dụng" });

            migrationBuilder.UpdateData(
                table: "Phong",
                keyColumn: "IdPhong",
                keyValue: 4,
                columns: new[] { "TenPhong", "TrangBi" },
                values: new object[] { "Phòng Khám 202", "Giường khám, máy đo huyết áp" });

            migrationBuilder.UpdateData(
                table: "Phong",
                keyColumn: "IdPhong",
                keyValue: 5,
                columns: new[] { "TenPhong", "TrangBi" },
                values: new object[] { "Phòng Siêu Âm", "Máy siêu âm 4D" });

            migrationBuilder.UpdateData(
                table: "Phong",
                keyColumn: "IdPhong",
                keyValue: 6,
                columns: new[] { "TenPhong", "TrangBi" },
                values: new object[] { "Phòng X-Quang", "Máy X-quang kỹ thuật số" });

            migrationBuilder.UpdateData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 1,
                columns: new[] { "DonVi", "GhiChu" },
                values: new object[] { "Viên", "Giảm đau, hạ sốt" });

            migrationBuilder.UpdateData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 2,
                columns: new[] { "DonVi", "GhiChu" },
                values: new object[] { "Viên", "Kháng sinh nhóm Beta-lactam" });

            migrationBuilder.UpdateData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 3,
                columns: new[] { "DonVi", "GhiChu" },
                values: new object[] { "Viên", "Kháng viêm, giảm đau NSAID" });

            migrationBuilder.UpdateData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 4,
                columns: new[] { "DonVi", "GhiChu" },
                values: new object[] { "Viên", "Ức chế bơm proton, điều trị viêm loét dạ dày" });

            migrationBuilder.UpdateData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 5,
                columns: new[] { "DonVi", "GhiChu" },
                values: new object[] { "Viên", "Kháng histamin H1, dị ứng" });

            migrationBuilder.UpdateData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 6,
                columns: new[] { "DonVi", "GhiChu" },
                values: new object[] { "Viên", "Bổ sung vitamin C" });

            migrationBuilder.UpdateData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 7,
                columns: new[] { "GhiChu", "TenThuoc" },
                values: new object[] { "Rửa mắt, rửa mũi", "Nước Muối Sinh Lý 0.9%" });

            migrationBuilder.UpdateData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 8,
                columns: new[] { "DonVi", "GhiChu" },
                values: new object[] { "Viên", "Kháng histamin H1 thế hệ 2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ChuyenKhoa",
                keyColumn: "IdChuyenKhoa",
                keyValue: 1,
                columns: new[] { "MoTa", "TenChuyenKhoa" },
                values: new object[] { "Kham va dieu tri cac benh ly ve tim va mach mau", "Tim Mach" });

            migrationBuilder.UpdateData(
                table: "ChuyenKhoa",
                keyColumn: "IdChuyenKhoa",
                keyValue: 2,
                column: "MoTa",
                value: "Kham va dieu tri cho tre em duoi 16 tuoi");

            migrationBuilder.UpdateData(
                table: "ChuyenKhoa",
                keyColumn: "IdChuyenKhoa",
                keyValue: 3,
                columns: new[] { "MoTa", "TenChuyenKhoa" },
                values: new object[] { "Kham tong quat cac benh ly noi khoa", "Noi Tong Quat" });

            migrationBuilder.UpdateData(
                table: "ChuyenKhoa",
                keyColumn: "IdChuyenKhoa",
                keyValue: 4,
                columns: new[] { "MoTa", "TenChuyenKhoa" },
                values: new object[] { "Kham va dieu tri cac benh ly ngoai khoa", "Ngoai Tong Quat" });

            migrationBuilder.UpdateData(
                table: "ChuyenKhoa",
                keyColumn: "IdChuyenKhoa",
                keyValue: 5,
                columns: new[] { "MoTa", "TenChuyenKhoa" },
                values: new object[] { "Kham va dieu tri cac benh ly tai, mui, hong", "Tai Mui Hong" });

            migrationBuilder.UpdateData(
                table: "ChuyenKhoa",
                keyColumn: "IdChuyenKhoa",
                keyValue: 6,
                columns: new[] { "MoTa", "TenChuyenKhoa" },
                values: new object[] { "Kham va dieu tri cac benh ly ve da", "Da Lieu" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 1,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Kham lam sang tim mach tong quat", "Kham Tim Mach Tong Quat" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 2,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Do dien tim 12 chuyen dao", "Dien Tim (ECG)" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 3,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Sieu am tim qua thanh nguc", "Sieu Am Tim" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 4,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Kham tong quat cho tre em", "Kham Nhi Tong Quat" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 5,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Tiem vaccine theo lich", "Tiem Chung" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 6,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Kham tong quat cac benh ly noi khoa", "Kham Noi Tong Quat" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 7,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Kham lam sang ngoai khoa", "Kham Ngoai Tong Quat" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 8,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Tieu phau cac ca don gian", "Tieu Phau" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 9,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Kham lam sang tai, mui, hong", "Kham Tai Mui Hong" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 10,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Noi soi chan doan tai, mui, hong", "Noi Soi Tai Mui Hong" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 11,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Kham cac benh ly ve da", "Kham Da Lieu" });

            migrationBuilder.UpdateData(
                table: "DichVu",
                keyColumn: "IdDichVu",
                keyValue: 12,
                columns: new[] { "MoTa", "TenDichVu" },
                values: new object[] { "Soi da chan doan bang dermoscope", "Soi Da Bang Dermoscope" });

            migrationBuilder.UpdateData(
                table: "DinhNghiaCa",
                keyColumn: "IdDinhNghiaCa",
                keyValue: 1,
                column: "MoTa",
                value: "Ca sang: 07:00 - 12:00");

            migrationBuilder.UpdateData(
                table: "DinhNghiaCa",
                keyColumn: "IdDinhNghiaCa",
                keyValue: 2,
                column: "MoTa",
                value: "Ca chieu: 13:00 - 17:00");

            migrationBuilder.UpdateData(
                table: "DinhNghiaCa",
                keyColumn: "IdDinhNghiaCa",
                keyValue: 3,
                column: "MoTa",
                value: "Ca toi: 17:00 - 21:00");

            migrationBuilder.UpdateData(
                table: "DinhNghiaCa",
                keyColumn: "IdDinhNghiaCa",
                keyValue: 4,
                column: "MoTa",
                value: "Ca gop sang + chieu: 07:00 - 17:00");

            migrationBuilder.UpdateData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 1,
                columns: new[] { "NoiDungMau", "TieuDeMau" },
                values: new object[] { "Xin chao {ten_benh_nhan}, lich hen cua ban ma {ma_lich_hen} vao ngay {ngay_kham} da duoc xac nhan. Vui long den truoc gio hen 15 phut.", "Xac nhan lich hen {ma_lich_hen}" });

            migrationBuilder.UpdateData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 2,
                columns: new[] { "NoiDungMau", "TieuDeMau" },
                values: new object[] { "Xin chao {ten_benh_nhan}, ban co lich hen kham vao ngay mai {ngay_kham} luc {gio_kham}. Ma lich hen: {ma_lich_hen}.", "Nhac lich kham ngay mai" });

            migrationBuilder.UpdateData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 3,
                columns: new[] { "NoiDungMau", "TieuDeMau" },
                values: new object[] { "{ten_benh_nhan}, ban co lich kham luc {gio_kham} hom nay. Ma: {ma_lich_hen}.", "Nhac lich kham 2 gio toi" });

            migrationBuilder.UpdateData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 4,
                columns: new[] { "NoiDungMau", "TieuDeMau" },
                values: new object[] { "Xin chao {ten_benh_nhan}, lich hen {ma_lich_hen} vao ngay {ngay_kham} da duoc huy. Ly do: {ly_do}.", "Huy lich hen {ma_lich_hen}" });

            migrationBuilder.UpdateData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 5,
                columns: new[] { "NoiDungMau", "TieuDeMau" },
                values: new object[] { "Ban da check-in thanh cong. So thu tu cua ban la {so_thu_tu}. Vui long cho goi ten.", "Check-in thanh cong" });

            migrationBuilder.UpdateData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 6,
                columns: new[] { "NoiDungMau", "TieuDeMau" },
                values: new object[] { "Ca lam viec ngay {ngay_lam_viec} cua ban da duoc admin duyet.", "Ca lam viec da duoc duyet" });

            migrationBuilder.UpdateData(
                table: "MauThongBao",
                keyColumn: "IdMau",
                keyValue: 7,
                columns: new[] { "NoiDungMau", "TieuDeMau" },
                values: new object[] { "Ca lam viec ngay {ngay_lam_viec} cua ban da bi admin tu choi. Ly do: {ly_do}.", "Ca lam viec bi tu choi" });

            migrationBuilder.UpdateData(
                table: "Phong",
                keyColumn: "IdPhong",
                keyValue: 1,
                columns: new[] { "TenPhong", "TrangBi" },
                values: new object[] { "Phong Kham 101", "Giuong kham, may do huyet ap, ong nghe" });

            migrationBuilder.UpdateData(
                table: "Phong",
                keyColumn: "IdPhong",
                keyValue: 2,
                columns: new[] { "TenPhong", "TrangBi" },
                values: new object[] { "Phong Kham 102", "Giuong kham, may do huyet ap, ong nghe" });

            migrationBuilder.UpdateData(
                table: "Phong",
                keyColumn: "IdPhong",
                keyValue: 3,
                columns: new[] { "TenPhong", "TrangBi" },
                values: new object[] { "Phong Kham Nhi 201", "Giuong kham tre em, can do chuyen dung" });

            migrationBuilder.UpdateData(
                table: "Phong",
                keyColumn: "IdPhong",
                keyValue: 4,
                columns: new[] { "TenPhong", "TrangBi" },
                values: new object[] { "Phong Kham 202", "Giuong kham, may do huyet ap" });

            migrationBuilder.UpdateData(
                table: "Phong",
                keyColumn: "IdPhong",
                keyValue: 5,
                columns: new[] { "TenPhong", "TrangBi" },
                values: new object[] { "Phong Sieu Am", "May sieu am 4D" });

            migrationBuilder.UpdateData(
                table: "Phong",
                keyColumn: "IdPhong",
                keyValue: 6,
                columns: new[] { "TenPhong", "TrangBi" },
                values: new object[] { "Phong X-quang", "May X-quang ky thuat so" });

            migrationBuilder.UpdateData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 1,
                columns: new[] { "DonVi", "GhiChu" },
                values: new object[] { "Vien", "Giam dau, ha sot" });

            migrationBuilder.UpdateData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 2,
                columns: new[] { "DonVi", "GhiChu" },
                values: new object[] { "Vien", "Khang sinh nhom Beta-lactam" });

            migrationBuilder.UpdateData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 3,
                columns: new[] { "DonVi", "GhiChu" },
                values: new object[] { "Vien", "Khang viem, giam dau NSAID" });

            migrationBuilder.UpdateData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 4,
                columns: new[] { "DonVi", "GhiChu" },
                values: new object[] { "Vien", "Uc che bom proton, dieu tri viem loet da day" });

            migrationBuilder.UpdateData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 5,
                columns: new[] { "DonVi", "GhiChu" },
                values: new object[] { "Vien", "Khang histamin H1, di ung" });

            migrationBuilder.UpdateData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 6,
                columns: new[] { "DonVi", "GhiChu" },
                values: new object[] { "Vien", "Bo sung vitamin C" });

            migrationBuilder.UpdateData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 7,
                columns: new[] { "GhiChu", "TenThuoc" },
                values: new object[] { "Rua mat, rua mui", "Nuoc Muoi Sinh Ly 0.9%" });

            migrationBuilder.UpdateData(
                table: "Thuoc",
                keyColumn: "IdThuoc",
                keyValue: 8,
                columns: new[] { "DonVi", "GhiChu" },
                values: new object[] { "Vien", "Khang histamin H1 the he 2" });
        }
    }
}
