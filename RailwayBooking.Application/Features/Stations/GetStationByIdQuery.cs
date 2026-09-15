using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Station;

namespace RailwayBooking.Application.Features.Stations
{
    public class GetStationByIdQuery : IRequest<BaseResponse<StationDto?>>
    {
        public long Id { get; set; }
    }
}
