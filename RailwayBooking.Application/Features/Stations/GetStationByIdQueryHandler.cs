using AutoMapper;
using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Station;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Application.Features.Stations
{
    public class GetStationByIdQueryHandler : IRequestHandler<GetStationByIdQuery, BaseResponse<StationDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetStationByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse<StationDto?>> Handle(GetStationByIdQuery request, CancellationToken cancellationToken)
        {
            var station = await _unitOfWork.StationRepository.FirstOrDefaultAsync(x => x.Id == request.Id);

            if (station == null)
                return BaseResponse<StationDto?>.FailureResponse("Not found");

            var dto = _mapper.Map<StationDto>(station);

            return BaseResponse<StationDto?>.SuccessResponse(dto);
        }
    }
}
