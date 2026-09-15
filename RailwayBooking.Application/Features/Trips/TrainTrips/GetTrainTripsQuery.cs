using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Trips;
using System.Collections.Generic;

namespace RailwayBooking.Application.Features.Trips.TrainTrips
{
    public class GetTrainTripsQuery : IRequest<BaseResponse<PagedResult<TrainTripDto>>>
    {
        public long? TrainId { get; set; }
        public string? FromStation { get; set; }
        public string? ToStation { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
