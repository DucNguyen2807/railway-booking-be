using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Railway;

namespace RailwayBooking.Application.Features.Railway.Trains
{
    public class GetTrainByIdQuery : IRequest<BaseResponse<TrainDto?>>
    {
        public long Id { get; set; }
    }
}
