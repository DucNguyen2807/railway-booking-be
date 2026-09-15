using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Railway;

namespace RailwayBooking.Application.Features.Railway.Coaches
{
    public class GetCoachByIdQuery : IRequest<BaseResponse<CoachDto?>>
    {
        public long Id { get; set; }
    }
}
