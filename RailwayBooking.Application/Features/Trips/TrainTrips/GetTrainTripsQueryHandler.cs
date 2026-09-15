using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Trips;
using RailwayBooking.Domain.Interfaces;
using RailwayBooking.Domain.Enums.Status;
using System.Linq;
using System.Collections.Generic;

namespace RailwayBooking.Application.Features.Trips.TrainTrips
{
    public class GetTrainTripsQueryHandler : IRequestHandler<GetTrainTripsQuery, BaseResponse<PagedResult<TrainTripDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetTrainTripsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse<PagedResult<TrainTripDto>>> Handle(GetTrainTripsQuery request, CancellationToken cancellationToken)
        {
            System.Linq.Expressions.Expression<Func<RailwayBooking.Infrastructure.Entities.Trips.TrainTrip, bool>> baseFilter = null;
            if (request.TrainId.HasValue)
            {
                baseFilter = t => t.TrainId == request.TrainId.Value;
            }

            var query = _unitOfWork.TrainTripRepository.Get(filter: baseFilter,
                orderBy: q => q.OrderBy(x => x.DepartureTime));

            // filter by stations order if provided
            if (!string.IsNullOrWhiteSpace(request.FromStation) && !string.IsNullOrWhiteSpace(request.ToStation))
            {
                var from = request.FromStation.Trim();
                var to = request.ToStation.Trim();

                query = query.Where(t => t.Stations.Any(s => s.StationCode == from)
                    && t.Stations.Any(s => s.StationCode == to)
                    && t.Stations.Where(s => s.StationCode == from).Min(s => s.StationOrder) < t.Stations.Where(s => s.StationCode == to).Min(s => s.StationOrder));

                // ensure available inventory exists for the trip
                var invQuery = _unitOfWork.TripSeatInventoryRepository.Get(filter: i => i.Status == InventoryStatus.Available);
                query = query.Where(t => invQuery.Any(i => i.TripId == t.Id));
            }

            var total = await query.CountAsync(cancellationToken);
            var items = await query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
            var dtos = _mapper.Map<List<TrainTripDto>>(items);

            var paged = new PagedResult<TrainTripDto>
            {
                Items = dtos,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = total
            };

            return BaseResponse<PagedResult<TrainTripDto>>.SuccessResponse(paged);
        }
    }
}
