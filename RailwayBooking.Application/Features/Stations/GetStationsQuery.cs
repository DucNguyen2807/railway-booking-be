using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Station;
using System.Collections.Generic;

namespace RailwayBooking.Application.Features.Stations
{
    public class GetStationsQuery : IRequest<BaseResponse<PagedResult<StationDto>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
