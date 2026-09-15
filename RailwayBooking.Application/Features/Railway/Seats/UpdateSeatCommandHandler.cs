using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Application.Features.Railway.Seats
{
    public class UpdateSeatCommandHandler : IRequestHandler<UpdateSeatCommand, BaseResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateSeatCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<bool>> Handle(UpdateSeatCommand request, CancellationToken cancellationToken)
        {
            var seat = await _unitOfWork.SeatRepository.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (seat == null)
                return BaseResponse<bool>.FailureResponse("Not found");

            seat.CoachId = request.CoachId;
            seat.SeatNumber = request.SeatNumber;
            seat.SeatType = request.SeatType;
            seat.RowNumber = request.RowNumber;
            seat.ColumnNumber = request.ColumnNumber;

            _unitOfWork.SeatRepository.Update(seat);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BaseResponse<bool>.SuccessResponse(true, "Updated");
        }
    }
}
