using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Application.Features.Trips.TrainTrips
{
    public class UpdateTrainTripCommandHandler : IRequestHandler<UpdateTrainTripCommand, BaseResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTrainTripCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<bool>> Handle(UpdateTrainTripCommand request, CancellationToken cancellationToken)
        {
            var trip = await _unitOfWork.TrainTripRepository.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (trip == null)
                return BaseResponse<bool>.FailureResponse("Not found");

            trip.TrainId = request.TrainId;
            trip.TripCode = request.TripCode;
            trip.DepartureStation = request.DepartureStation;
            trip.ArrivalStation = request.ArrivalStation;
            trip.DepartureTime = request.DepartureTime;
            trip.ArrivalTime = request.ArrivalTime;
            trip.SalesOpenAt = request.SalesOpenAt;
            trip.SalesCloseAt = request.SalesCloseAt;
            trip.Status = request.Status;

            _unitOfWork.TrainTripRepository.Update(trip);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BaseResponse<bool>.SuccessResponse(true, "Updated");
        }
    }
}
