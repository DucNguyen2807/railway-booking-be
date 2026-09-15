using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Domain.Interfaces;
using RailwayBooking.Infrastructure.Entities.Trips;

namespace RailwayBooking.Application.Features.Trips.TrainTrips
{
    public class CreateTrainTripCommandHandler : IRequestHandler<CreateTrainTripCommand, BaseResponse<long>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateTrainTripCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<long>> Handle(CreateTrainTripCommand request, CancellationToken cancellationToken)
        {
            var trip = new TrainTrip
            {
                TrainId = request.TrainId,
                TripCode = request.TripCode,
                DepartureStation = request.DepartureStation,
                ArrivalStation = request.ArrivalStation,
                DepartureTime = request.DepartureTime,
                ArrivalTime = request.ArrivalTime,
                SalesOpenAt = request.SalesOpenAt,
                SalesCloseAt = request.SalesCloseAt,
                Status = 1,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.TrainTripRepository.InsertAsync(trip);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BaseResponse<long>.SuccessResponse(trip.Id, "Created");
        }
    }
}
