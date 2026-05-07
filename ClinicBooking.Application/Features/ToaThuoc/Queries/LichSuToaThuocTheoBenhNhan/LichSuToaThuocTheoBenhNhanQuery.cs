using ClinicBooking.Application.Features.ToaThuoc.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.ToaThuoc.Queries.LichSuToaThuocTheoBenhNhan;

public record LichSuToaThuocTheoBenhNhanQuery(int IdBenhNhan, int SoTrang = 1, int KichThuocTrang = 20) 
    : IRequest<IReadOnlyList<ToaThuocResponse>>;
