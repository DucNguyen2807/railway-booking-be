using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Trips;

namespace RailwayBooking.Application.Features.Trips.TrainTrips
{
    public class GetTrainTripByIdQuery : IRequest<BaseResponse<TrainTripDto?>>
    {
        public long Id { get; set; }
    }
}
