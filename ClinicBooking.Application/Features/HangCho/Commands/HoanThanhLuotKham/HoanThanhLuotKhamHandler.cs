using ClinicBooking.Application.Abstractions.Persistence;
using ClinicBooking.Application.Common.Exceptions;
using ClinicBooking.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Application.Features.HangCho.Commands.HoanThanhLuotKham;

public class HoanThanhLuotKhamHandler : IRequestHandler<HoanThanhLuotKhamCommand, Unit>
{
    private readonly IAppDbContext _db;

    public HoanThanhLuotKhamHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(HoanThanhLuotKhamCommand request, CancellationToken cancellationToken)
    {
        var hangCho = await _db.HangCho
            .Include(x => x.LichHen)
            .FirstOrDefaultAsync(x => x.IdHangCho == request.IdHangCho, cancellationToken)
            ?? throw new NotFoundException("Khong tim thay ban ghi hang cho.");

        if (hangCho.TrangThai == TrangThaiHangCho.HoanThanh)
        {
            throw new ConflictException("Luot kham da duoc danh dau hoan thanh truoc do.");
        }

        if (hangCho.TrangThai != TrangThaiHangCho.DangKham)
        {
            throw new ConflictException("Chi co the hoan thanh luot dang kham.");
        }

        hangCho.TrangThai = TrangThaiHangCho.HoanThanh;
        hangCho.LichHen.TrangThai = TrangThaiLichHen.HoanThanh;

        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
