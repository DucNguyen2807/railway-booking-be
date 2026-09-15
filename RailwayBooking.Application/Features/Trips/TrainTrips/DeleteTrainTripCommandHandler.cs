using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Application.Features.Trips.TrainTrips
{
    public class DeleteTrainTripCommandHandler : IRequestHandler<DeleteTrainTripCommand, BaseResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTrainTripCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<bool>> Handle(DeleteTrainTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await _unitOfWork.TrainTripRepository.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (trip == null)
                return BaseResponse<bool>.FailureResponse("Not found");

            _unitOfWork.TrainTripRepository.Delete(trip);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BaseResponse<bool>.SuccessResponse(true, "Deleted");
        }
    }
}
