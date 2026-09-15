using AutoMapper;
using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Application.Features.Railway.Coaches
{
    public class GetCoachByIdQueryHandler : IRequestHandler<GetCoachByIdQuery, BaseResponse<RailwayBooking.Application.DTOs.Railway.CoachDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetCoachByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse<RailwayBooking.Application.DTOs.Railway.CoachDto?>> Handle(GetCoachByIdQuery request, CancellationToken cancellationToken)
        {
            var coach = await _unitOfWork.CoachRepository.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (coach == null)
                return BaseResponse<RailwayBooking.Application.DTOs.Railway.CoachDto?>.FailureResponse("Not found");

            var dto = _mapper.Map<RailwayBooking.Application.DTOs.Railway.CoachDto>(coach);
            return BaseResponse<RailwayBooking.Application.DTOs.Railway.CoachDto?>.SuccessResponse(dto);
        }
    }
}
