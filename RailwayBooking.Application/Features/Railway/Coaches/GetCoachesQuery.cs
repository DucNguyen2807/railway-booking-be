using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Railway;
using System.Collections.Generic;

namespace RailwayBooking.Application.Features.Railway.Coaches
{
    public class GetCoachesQuery : IRequest<BaseResponse<PagedResult<CoachDto>>>
    {
        public long? TrainId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
