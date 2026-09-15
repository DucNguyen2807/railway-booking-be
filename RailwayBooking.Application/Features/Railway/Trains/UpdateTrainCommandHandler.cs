using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Application.Features.Railway.Trains
{
    public class UpdateTrainCommandHandler : IRequestHandler<UpdateTrainCommand, BaseResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTrainCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<bool>> Handle(UpdateTrainCommand request, CancellationToken cancellationToken)
        {
            var train = await _unitOfWork.TrainRepository.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (train == null)
                return BaseResponse<bool>.FailureResponse("Not found");

            train.Code = request.Code;
            train.Name = request.Name;
            train.Status = request.Status;

            _unitOfWork.TrainRepository.Update(train);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BaseResponse<bool>.SuccessResponse(true, "Updated");
        }
    }
}
