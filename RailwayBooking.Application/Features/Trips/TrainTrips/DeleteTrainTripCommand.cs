using MediatR;
using RailwayBooking.Application.Common;

namespace RailwayBooking.Application.Features.Trips.TrainTrips
{
    public class DeleteTrainTripCommand : IRequest<BaseResponse<bool>>
    {
        public long Id { get; set; }
    }
}
