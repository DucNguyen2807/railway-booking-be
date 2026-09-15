using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Railway;
using RailwayBooking.Domain.Interfaces;
using RailwayBooking.Domain.Enums.Status;
using System.Collections.Generic;
using System.Linq;

namespace RailwayBooking.Application.Features.Trips.TrainTrips
{
    public class GetSeatAvailabilityQueryHandler : IRequestHandler<GetSeatAvailabilityQuery, BaseResponse<List<SeatAvailabilityDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetSeatAvailabilityQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<BaseResponse<List<SeatAvailabilityDto>>> Handle(GetSeatAvailabilityQuery request, CancellationToken cancellationToken)
        {
            // verify trip exists
            var trip = await _unitOfWork.TrainTripRepository.FirstOrDefaultAsync(t => t.Id == request.TripId);
            if (trip == null)
            {
                return BaseResponse<List<SeatAvailabilityDto>>.FailureResponse("Trip not found");
            }

            // if provided, ensure fromStation occurs before toStation
            if (!string.IsNullOrWhiteSpace(request.FromStation) && !string.IsNullOrWhiteSpace(request.ToStation))
            {
                var from = request.FromStation.Trim();
                var to = request.ToStation.Trim();

                var stations = _unitOfWork.TrainTripRepository.GetIncludeMultiLayer(filter: t => t.Id == request.TripId,
                    include: q => q.Include(x => x.Stations));

                var tripWithStations = await stations.FirstOrDefaultAsync(cancellationToken);
                if (tripWithStations == null)
                {
                    return BaseResponse<List<SeatAvailabilityDto>>.FailureResponse("Trip stations not found");
                }

                var fromOrder = tripWithStations.Stations.Where(s => s.StationCode == from).Select(s => (int?)s.StationOrder).Min();
                var toOrder = tripWithStations.Stations.Where(s => s.StationCode == to).Select(s => (int?)s.StationOrder).Min();

                if (fromOrder == null || toOrder == null || fromOrder >= toOrder)
                {
                    return BaseResponse<List<SeatAvailabilityDto>>.FailureResponse("Invalid from/to station sequence");
                }
            }

            // load inventory for trip including seat and coach
            var invQuery = _unitOfWork.TripSeatInventoryRepository.GetIncludeMultiLayer(filter: i => i.TripId == request.TripId,
                include: q => q.Include(i => i.Seat).ThenInclude(s => s.Coach));

            var inventories = await invQuery.ToListAsync(cancellationToken);

            var result = inventories.Select(i => new SeatAvailabilityDto
            {
                InventoryId = i.Id,
                SeatId = i.SeatId,
                SeatNumber = i.Seat?.SeatNumber ?? string.Empty,
                CoachId = i.Seat?.CoachId ?? 0,
                CoachNumber = i.Seat?.Coach?.CoachNumber ?? string.Empty,
                Status = i.Status == InventoryStatus.Available ? "Available" : (i.Status == InventoryStatus.Hold ? "Hold" : "Booked"),
                HoldToken = i.HoldToken,
                HoldExpiredAt = i.HoldExpiredAt,
                BookedOrderId = i.BookedOrderId
            }).ToList();

            return BaseResponse<List<SeatAvailabilityDto>>.SuccessResponse(result);
        }
    }
}
