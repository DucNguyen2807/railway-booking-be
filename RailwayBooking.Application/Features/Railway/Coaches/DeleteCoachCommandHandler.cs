using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Application.Features.Railway.Coaches
{
    public class DeleteCoachCommandHandler : IRequestHandler<DeleteCoachCommand, BaseResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCoachCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<bool>> Handle(DeleteCoachCommand request, CancellationToken cancellationToken)
        {
            var coach = await _unitOfWork.CoachRepository.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (coach == null)
                return BaseResponse<bool>.FailureResponse("Not found");

            _unitOfWork.CoachRepository.Delete(coach);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BaseResponse<bool>.SuccessResponse(true, "Deleted");
        }
    }
}
