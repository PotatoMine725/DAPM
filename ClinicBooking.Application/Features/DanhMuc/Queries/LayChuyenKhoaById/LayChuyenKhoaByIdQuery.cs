using ClinicBooking.Application.Features.DanhMuc.Dtos;
using MediatR;

namespace ClinicBooking.Application.Features.DanhMuc.Queries.LayChuyenKhoaById;

public sealed record LayChuyenKhoaByIdQuery(int IdChuyenKhoa) : IRequest<ChuyenKhoaResponse>;
