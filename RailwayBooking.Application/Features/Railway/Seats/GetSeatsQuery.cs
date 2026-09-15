using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Railway;
using System.Collections.Generic;

namespace RailwayBooking.Application.Features.Railway.Seats
{
        public class GetSeatsQuery : IRequest<BaseResponse<PagedResult<SeatDto>>>
        {
            public long? CoachId { get; set; }
            public int Page { get; set; } = 1;
            public int PageSize { get; set; } = 20;
        }
}
