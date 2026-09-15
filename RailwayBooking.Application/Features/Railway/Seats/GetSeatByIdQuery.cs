using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Railway;

namespace RailwayBooking.Application.Features.Railway.Seats
{
    public class GetSeatByIdQuery : IRequest<BaseResponse<SeatDto?>>
    {
        public long Id { get; set; }
    }
}
