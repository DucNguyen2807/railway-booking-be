using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Application.Features.Railway.Trains
{
    public class DeleteTrainCommandHandler : IRequestHandler<DeleteTrainCommand, BaseResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTrainCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<bool>> Handle(DeleteTrainCommand request, CancellationToken cancellationToken)
        {
            var train = await _unitOfWork.TrainRepository.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (train == null)
                return BaseResponse<bool>.FailureResponse("Not found");

            _unitOfWork.TrainRepository.Delete(train);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BaseResponse<bool>.SuccessResponse(true, "Deleted");
        }
    }
}
