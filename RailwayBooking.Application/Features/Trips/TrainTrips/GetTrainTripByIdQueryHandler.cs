using AutoMapper;
using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Application.Features.Trips.TrainTrips
{
    public class GetTrainTripByIdQueryHandler : IRequestHandler<GetTrainTripByIdQuery, BaseResponse<RailwayBooking.Application.DTOs.Trips.TrainTripDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTrainTripByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse<RailwayBooking.Application.DTOs.Trips.TrainTripDto?>> Handle(GetTrainTripByIdQuery request, CancellationToken cancellationToken)
        {
            var trip = await _unitOfWork.TrainTripRepository.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (trip == null)
                return BaseResponse<RailwayBooking.Application.DTOs.Trips.TrainTripDto?>.FailureResponse("Not found");

            var dto = _mapper.Map<RailwayBooking.Application.DTOs.Trips.TrainTripDto>(trip);
            return BaseResponse<RailwayBooking.Application.DTOs.Trips.TrainTripDto?>.SuccessResponse(dto);
        }
    }
}
