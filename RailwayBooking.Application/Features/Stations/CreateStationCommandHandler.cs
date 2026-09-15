using AutoMapper;
using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Domain.Interfaces;
using RailwayBooking.Infrastructure.Entities.Railway;

namespace RailwayBooking.Application.Features.Stations
{
    public class CreateStationCommandHandler : IRequestHandler<CreateStationCommand, BaseResponse<long>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateStationCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse<long>> Handle(CreateStationCommand request, CancellationToken cancellationToken)
        {
            var station = new Station
            {
                Code = request.Code,
                Name = request.Name,
                Address = request.Address,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Status = 1,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.StationRepository.InsertAsync(station);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return BaseResponse<long>.SuccessResponse(station.Id, "Station created");
        }
    }
}
