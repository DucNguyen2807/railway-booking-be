using MediatR;
using RailwayBooking.Application.Common;

namespace RailwayBooking.Application.Features.Railway.Trains
{
    public class DeleteTrainCommand : IRequest<BaseResponse<bool>>
    {
        public long Id { get; set; }
    }
}
