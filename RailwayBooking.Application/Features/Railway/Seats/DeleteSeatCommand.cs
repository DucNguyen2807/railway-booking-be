using MediatR;
using RailwayBooking.Application.Common;

namespace RailwayBooking.Application.Features.Railway.Seats
{
    public class DeleteSeatCommand : IRequest<BaseResponse<bool>>
    {
        public long Id { get; set; }
    }
}
