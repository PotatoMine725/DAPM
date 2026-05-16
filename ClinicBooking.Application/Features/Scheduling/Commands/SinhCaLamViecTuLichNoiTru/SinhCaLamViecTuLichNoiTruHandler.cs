using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Abstractions.Scheduling;
using ClinicBooking.Domain.Entities;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.Scheduling.Commands.SinhCaLamViecTuLichNoiTru;

public sealed class SinhCaLamViecTuLichNoiTruHandler : IRequestHandler<SinhCaLamViecTuLichNoiTruCommand, KetQuaSinhCaLamViecTuLichNoiTruResponse>
{
    private readonly IAppDbContext _db;
    private readonly ICaLamViecConflictChecker _conflictChecker;

    public SinhCaLamViecTuLichNoiTruHandler(IAppDbContext db, ICaLamViecConflictChecker conflictChecker)
    {
        _db = db;
        _conflictChecker = conflictChecker;
    }

    public async Task<KetQuaSinhCaLamViecTuLichNoiTruResponse> Handle(SinhCaLamViecTuLichNoiTruCommand request, CancellationToken cancellationToken)
    {
        var homNay = DateOnly.FromDateTime(DateTime.UtcNow);
        var ngayKetThuc = homNay.AddDays(request.SoNgaySinhTruoc);
        var lichs = await _db.LichNoiTru
            .AsNoTracking()
            .Include(x => x.BacSi)
            .Include(x => x.Phong)
            .Include(x => x.DinhNghiaCa)
            .Where(x => x.TrangThai)
            .ToListAsync(cancellationToken);

        var soCaSinh = 0;
        var soCaBoQua = 0;
        var conflicts = new List<XungDotSinhCaDto>();

        foreach (var lich in lichs)
        {
            for (var ngay = homNay; ngay <= ngayKetThuc; ngay = ngay.AddDays(1))
            {
                if ((int)ngay.DayOfWeek != lich.NgayTrongTuan)
                {
                    continue;
                }

                var exists = await _db.CaLamViec.AnyAsync(x =>
                    x.IdBacSi == lich.IdBacSi
                    && x.NgayLamViec == ngay
                    && x.IdDinhNghiaCa == lich.IdDinhNghiaCa, cancellationToken);
                if (exists)
                {
                    soCaBoQua++;
                    conflicts.Add(new XungDotSinhCaDto(
                        ngay,
                        lich.IdBacSi,
                        lich.IdPhong,
                        LoaiXungDotSinhCa.CaDaTonTai,
                        $"Bác sĩ {lich.BacSi.HoTen} đã có ca {lich.DinhNghiaCa.TenCa} vào ngày {ngay:dd/MM/yyyy}."));
                    continue;
                }

                var conflictMessage = await KiemTraXungDotNghiepVuAsync(lich, ngay, cancellationToken);
                if (conflictMessage is not null)
                {
                    soCaBoQua++;
                    conflicts.Add(conflictMessage with { NgayLamViec = ngay, IdBacSi = lich.IdBacSi, IdPhong = lich.IdPhong });
                    continue;
                }

                _db.CaLamViec.Add(new CaLamViec
                {
                    IdBacSi = lich.IdBacSi,
                    IdPhong = lich.IdPhong,
                    IdChuyenKhoa = lich.BacSi.IdChuyenKhoa,
                    IdDinhNghiaCa = lich.IdDinhNghiaCa,
                    NgayLamViec = ngay,
                    GioBatDau = lich.DinhNghiaCa.GioBatDauMacDinh,
                    GioKetThuc = lich.DinhNghiaCa.GioKetThucMacDinh,
                    ThoiGianSlot = 15,
                    SoSlotToiDa = 1,
                    SoSlotDaDat = 0,
                    TrangThaiDuyet = TrangThaiDuyetCa.DaDuyet,
                    NguonTaoCa = NguonTaoCa.TuDong,
                    NgayTao = DateTime.UtcNow
                });
                soCaSinh++;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return new KetQuaSinhCaLamViecTuLichNoiTruResponse(soCaSinh, soCaBoQua, conflicts);
    }

    private async Task<XungDotSinhCaDto?> KiemTraXungDotNghiepVuAsync(LichNoiTru lich, DateOnly ngay, CancellationToken cancellationToken)
    {
        var existingCa = await _db.CaLamViec
            .AsNoTracking()
            .Include(x => x.BacSi)
            .Include(x => x.Phong)
            .Where(x => x.NgayLamViec == ngay)
            .Where(x => x.TrangThaiDuyet != TrangThaiDuyetCa.DaHuy)
            .Where(x => x.GioBatDau < lich.DinhNghiaCa.GioKetThucMacDinh && x.GioKetThuc > lich.DinhNghiaCa.GioBatDauMacDinh)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingCa is not null)
        {
            if (existingCa.IdBacSi == lich.IdBacSi)
            {
                return new XungDotSinhCaDto(
                    ngay,
                    lich.IdBacSi,
                    lich.IdPhong,
                    LoaiXungDotSinhCa.TrungLichBacSi,
                    $"Bác sĩ {existingCa.BacSi.HoTen} đã có ca khác lúc {existingCa.GioBatDau:HH\:mm}-{existingCa.GioKetThuc:HH\:mm} ngày {ngay:dd/MM/yyyy}.");
            }

            if (existingCa.IdPhong == lich.IdPhong)
            {
                return new XungDotSinhCaDto(
                    ngay,
                    lich.IdBacSi,
                    lich.IdPhong,
                    LoaiXungDotSinhCa.TrungLichPhong,
                    $"Phòng {existingCa.Phong.MaPhong} đã được đặt lúc {existingCa.GioBatDau:HH\:mm}-{existingCa.GioKetThuc:HH\:mm} ngày {ngay:dd/MM/yyyy}.");
            }
        }

        var nghiPhep = await _db.DonNghiPhep
            .AsNoTracking()
            .Where(x => x.IdBacSi == lich.IdBacSi)
            .Where(x => x.TrangThaiDuyet == TrangThaiDuyetDon.DaDuyet)
            .Where(x => x.CaLamViec.NgayLamViec <= ngay && x.CaLamViec.NgayLamViec >= ngay)
            .Select(x => new { x.BacSi.HoTen, x.CaLamViec.NgayLamViec, x.CaLamViec.GioBatDau, x.CaLamViec.GioKetThuc })
            .FirstOrDefaultAsync(cancellationToken);

        if (nghiPhep is not null)
        {
            return new XungDotSinhCaDto(
                ngay,
                lich.IdBacSi,
                lich.IdPhong,
                LoaiXungDotSinhCa.DonNghiPhepDaDuyet,
                $"Bác sĩ {nghiPhep.HoTen} đang nghỉ phép từ {nghiPhep.NgayLamViec:dd/MM/yyyy} trong khung giờ {nghiPhep.GioBatDau:HH\:mm}-{nghiPhep.GioKetThuc:HH\:mm}.");
        }

        return null;
    }
}