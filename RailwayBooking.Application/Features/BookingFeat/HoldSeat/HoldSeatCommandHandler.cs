using MediatR;
using Microsoft.EntityFrameworkCore;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Booking;
using RailwayBooking.Domain.Enums.Status;
using RailwayBooking.Domain.Interfaces;
using RailwayBooking.Infrastructure.Entities.Inventory;
using RailwayBooking.Infrastructure.Persistence;

namespace RailwayBooking.Application.Features.BookingFeat.HoldSeat
{
    public class HoldSeatCommandHandler : IRequestHandler<HoldSeatCommand, BaseResponse<HoldSeatResponseDto>>
    {
        private static readonly TimeSpan HoldDuration = TimeSpan.FromMinutes(5);

        private readonly BookingDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public HoldSeatCommandHandler(BookingDbContext dbContext, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<BaseResponse<HoldSeatResponseDto>> Handle(HoldSeatCommand request, CancellationToken cancellationToken)
        {
            var userIdValue = _currentUserService.GetUserId();
            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return BaseResponse<HoldSeatResponseDto>.FailureResponse("Unauthorized user");
            }

            var seatIds = request.SeatIds.Distinct().ToList();
            if (seatIds.Count == 0)
            {
                return BaseResponse<HoldSeatResponseDto>.FailureResponse("SeatIds is required");
            }

            var trip = await _unitOfWork.TrainTripRepository.GetIncludeMultiLayer(
                    filter: t => t.Id == request.TripId,
                    include: q => q.Include(x => x.Stations))
                .FirstOrDefaultAsync(cancellationToken);

            if (trip == null)
            {
                return BaseResponse<HoldSeatResponseDto>.FailureResponse("Trip not found");
            }

            var fromStation = await _unitOfWork.StationRepository.FirstOrDefaultAsync(s => s.Id == request.FromStationId);
            var toStation = await _unitOfWork.StationRepository.FirstOrDefaultAsync(s => s.Id == request.ToStationId);

            if (fromStation == null || toStation == null)
            {
                return BaseResponse<HoldSeatResponseDto>.FailureResponse("Station not found");
            }

            var fromOrder = trip.Stations.Where(s => s.StationCode == fromStation.Code).Select(s => (int?)s.StationOrder).Min();
            var toOrder = trip.Stations.Where(s => s.StationCode == toStation.Code).Select(s => (int?)s.StationOrder).Min();

            if (fromOrder == null || toOrder == null || fromOrder >= toOrder)
            {
                return BaseResponse<HoldSeatResponseDto>.FailureResponse("Invalid station segment");
            }

            var now = DateTime.UtcNow;
            var expiredAt = DateTime.SpecifyKind(now.Add(HoldDuration), DateTimeKind.Utc);

            var expiredHoldInventories = await _dbContext.TripSeatInventories
                .Where(x => x.TripId == request.TripId && x.Status == InventoryStatus.Hold && x.HoldExpiredAt != null && x.HoldExpiredAt < now)
                .ToListAsync(cancellationToken);

            foreach (var inventory in expiredHoldInventories)
            {
                inventory.Status = InventoryStatus.Available;
                inventory.HoldToken = null;
                inventory.HoldExpiredAt = null;
                inventory.LastReservedAt = null;
                inventory.Version += 1;
            }

            var inventories = await _dbContext.TripSeatInventories
                .Where(x => x.TripId == request.TripId && seatIds.Contains(x.SeatId))
                .ToListAsync(cancellationToken);

            if (inventories.Count != seatIds.Count)
            {
                return BaseResponse<HoldSeatResponseDto>.FailureResponse("One or more seats not found for this trip");
            }

            var holdToken = Guid.NewGuid();

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                foreach (var inventory in inventories)
                {
                    var affectedRows = await _dbContext.Database.ExecuteSqlInterpolatedAsync($@"
UPDATE trip_seat_inventory
SET
    status = {(int)InventoryStatus.Hold},
    hold_token = {holdToken},
    hold_expired_at = {expiredAt},
    last_reserved_at = {now},
    version = version + 1
WHERE
    id = {inventory.Id}
    AND status = {(int)InventoryStatus.Available}", cancellationToken);

                    if (affectedRows == 0)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return BaseResponse<HoldSeatResponseDto>.FailureResponse($"Seat {inventory.SeatId} already taken");
                    }
                }

                var hold = new SeatHold
                {
                    HoldToken = holdToken,
                    UserId = userId,
                    TripId = request.TripId,
                    Status = HoldStatus.Active,
                    ExpiredAt = expiredAt
                };

                await _dbContext.SeatHolds.AddAsync(hold, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                var holdItems = inventories.Select(x => new HoldItem
                {
                    HoldId = hold.Id,
                    InventoryId = x.Id
                }).ToList();

                await _dbContext.HoldItems.AddRangeAsync(holdItems, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return BaseResponse<HoldSeatResponseDto>.SuccessResponse(new HoldSeatResponseDto
                {
                    HoldToken = holdToken.ToString(),
                    ExpiredAt = expiredAt
                });
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}