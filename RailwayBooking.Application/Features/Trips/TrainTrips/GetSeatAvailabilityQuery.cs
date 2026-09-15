using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Railway;

namespace RailwayBooking.Application.Features.Trips.TrainTrips
{
    public class GetSeatAvailabilityQuery : IRequest<BaseResponse<List<SeatAvailabilityDto>>>
    {
        public long TripId { get; set; }
        public string? FromStation { get; set; }
        public string? ToStation { get; set; }
    }
}
