using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Application.Features.Stations
{
    public class UpdateStationCommandHandler : IRequestHandler<UpdateStationCommand, BaseResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateStationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<bool>> Handle(UpdateStationCommand request, CancellationToken cancellationToken)
        {
            var station = await _unitOfWork.StationRepository.FirstOrDefaultAsync(x => x.Id == request.Id);

            if (station == null)
                return BaseResponse<bool>.FailureResponse("Not found");

            station.Code = request.Code;
            station.Name = request.Name;
            station.Address = request.Address;
            station.Latitude = request.Latitude;
            station.Longitude = request.Longitude;

            _unitOfWork.StationRepository.Update(station);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BaseResponse<bool>.SuccessResponse(true, "Updated");
        }
    }
}
