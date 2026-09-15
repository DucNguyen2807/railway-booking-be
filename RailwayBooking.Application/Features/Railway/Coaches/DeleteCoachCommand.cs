using MediatR;
using RailwayBooking.Application.Common;

namespace RailwayBooking.Application.Features.Railway.Coaches
{
    public class DeleteCoachCommand : IRequest<BaseResponse<bool>>
    {
        public long Id { get; set; }
    }
}
