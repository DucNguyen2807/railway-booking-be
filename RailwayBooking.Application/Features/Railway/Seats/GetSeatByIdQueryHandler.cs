using AutoMapper;
using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Application.Features.Railway.Seats
{
    public class GetSeatByIdQueryHandler : IRequestHandler<GetSeatByIdQuery, BaseResponse<RailwayBooking.Application.DTOs.Railway.SeatDto?>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSeatByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse<RailwayBooking.Application.DTOs.Railway.SeatDto?>> Handle(GetSeatByIdQuery request, CancellationToken cancellationToken)
        {
            var seat = await _unitOfWork.SeatRepository.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (seat == null)
                return BaseResponse<RailwayBooking.Application.DTOs.Railway.SeatDto?>.FailureResponse("Not found");

            var dto = _mapper.Map<RailwayBooking.Application.DTOs.Railway.SeatDto>(seat);
            return BaseResponse<RailwayBooking.Application.DTOs.Railway.SeatDto?>.SuccessResponse(dto);
        }
    }
}
