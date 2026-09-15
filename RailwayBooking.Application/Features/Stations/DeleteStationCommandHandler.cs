using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Application.Features.Stations
{
    public class DeleteStationCommandHandler : IRequestHandler<DeleteStationCommand, BaseResponse<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteStationCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseResponse<bool>> Handle(DeleteStationCommand request, CancellationToken cancellationToken)
        {
            var station = await _unitOfWork.StationRepository.FirstOrDefaultAsync(x => x.Id == request.Id);

            if (station == null)
                return BaseResponse<bool>.FailureResponse("Not found");

            _unitOfWork.StationRepository.Delete(station);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BaseResponse<bool>.SuccessResponse(true, "Deleted");
        }
    }
}
