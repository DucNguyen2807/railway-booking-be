using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Application.Features.Railway.Seats
{
    public class DeleteSeatCommandHandler : IRequestHandler<DeleteSeatCommand, BaseResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteSeatCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<bool>> Handle(DeleteSeatCommand request, CancellationToken cancellationToken)
        {
            var seat = await _unitOfWork.SeatRepository.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (seat == null)
                return BaseResponse<bool>.FailureResponse("Not found");

            _unitOfWork.SeatRepository.Delete(seat);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BaseResponse<bool>.SuccessResponse(true, "Deleted");
        }
    }
}
