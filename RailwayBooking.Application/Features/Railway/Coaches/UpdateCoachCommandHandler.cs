using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Application.Features.Railway.Coaches
{
    public class UpdateCoachCommandHandler : IRequestHandler<UpdateCoachCommand, BaseResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCoachCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<bool>> Handle(UpdateCoachCommand request, CancellationToken cancellationToken)
        {
            var coach = await _unitOfWork.CoachRepository.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (coach == null)
                return BaseResponse<bool>.FailureResponse("Not found");

            coach.TrainId = request.TrainId;
            coach.CoachNumber = request.CoachNumber;
            coach.ClassType = request.ClassType;
            coach.SeatCapacity = request.SeatCapacity;

            _unitOfWork.CoachRepository.Update(coach);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BaseResponse<bool>.SuccessResponse(true, "Updated");
        }
    }
}
