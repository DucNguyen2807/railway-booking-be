using AutoMapper;
using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Application.Features.Railway.Trains
{
    public class GetTrainByIdQueryHandler : IRequestHandler<GetTrainByIdQuery, BaseResponse<RailwayBooking.Application.DTOs.Railway.TrainDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTrainByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse<RailwayBooking.Application.DTOs.Railway.TrainDto?>> Handle(GetTrainByIdQuery request, CancellationToken cancellationToken)
        {
            var train = await _unitOfWork.TrainRepository.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (train == null)
                return BaseResponse<RailwayBooking.Application.DTOs.Railway.TrainDto?>.FailureResponse("Not found");

            var dto = _mapper.Map<RailwayBooking.Application.DTOs.Railway.TrainDto>(train);
            return BaseResponse<RailwayBooking.Application.DTOs.Railway.TrainDto?>.SuccessResponse(dto);
        }
    }
}
