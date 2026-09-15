using MediatR;
using RailwayBooking.Application.Common;

namespace RailwayBooking.Application.Features.Stations
{
    public class DeleteStationCommand : IRequest<BaseResponse<bool>>
    {
        public long Id { get; set; }
    }
}
