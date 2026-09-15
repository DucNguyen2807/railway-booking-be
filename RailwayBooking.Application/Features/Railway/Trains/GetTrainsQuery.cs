using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Railway;
using System.Collections.Generic;

namespace RailwayBooking.Application.Features.Railway.Trains
{
    public class GetTrainsQuery : IRequest<BaseResponse<PagedResult<TrainDto>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}

// moved into namespace above
