using MediatR;
using Microsoft.EntityFrameworkCore;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Booking;
using RailwayBooking.Domain.Enums.Status;
using RailwayBooking.Domain.Interfaces;
using RailwayBooking.Infrastructure.Entities.Booking;
using RailwayBooking.Infrastructure.Entities.Inventory;
using RailwayBooking.Infrastructure.Persistence;

namespace RailwayBooking.Application.Features.BookingFeat.CreateBookingOrder
{
    public class CreateBookingOrderCommandHandler : IRequestHandler<CreateBookingOrderCommand, BaseResponse<CreateBookingOrderResponseDto>>
    {
        private readonly BookingDbContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public CreateBookingOrderCommandHandler(BookingDbContext dbContext, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<BaseResponse<CreateBookingOrderResponseDto>> Handle(CreateBookingOrderCommand request, CancellationToken cancellationToken)
        {
            var userIdValue = _currentUserService.GetUserId();
            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return BaseResponse<CreateBookingOrderResponseDto>.FailureResponse("Unauthorized user");
            }

            var existingOrder = await _dbContext.BookingOrders
                .FirstOrDefaultAsync(x => x.HoldToken == request.HoldToken && x.UserId == userId, cancellationToken);

            if (existingOrder != null)
            {
                return BaseResponse<CreateBookingOrderResponseDto>.SuccessResponse(new CreateBookingOrderResponseDto
                {
                    OrderId = existingOrder.Id,
                    OrderCode = existingOrder.OrderCode,
                    HoldToken = existingOrder.HoldToken ?? request.HoldToken,
                    Status = existingOrder.Status.ToString(),
                    TotalAmount = existingOrder.TotalAmount,
                    Currency = existingOrder.Currency,
                    CreatedAt = existingOrder.CreatedAt
                }, "Order already created");
            }

            var hold = await _dbContext.SeatHolds
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.HoldToken == request.HoldToken && x.UserId == userId, cancellationToken);

            if (hold == null)
            {
                return BaseResponse<CreateBookingOrderResponseDto>.FailureResponse("Hold token not found");
            }

            if (hold.Status != HoldStatus.Active)
            {
                return BaseResponse<CreateBookingOrderResponseDto>.FailureResponse("Hold is no longer active");
            }

            if (hold.ExpiredAt <= DateTime.UtcNow)
            {
                return BaseResponse<CreateBookingOrderResponseDto>.FailureResponse("Hold has expired");
            }

            var holdInventoryIds = hold.Items.Select(x => x.InventoryId).ToHashSet();
            var itemsBySeatId = request.Items.ToDictionary(x => x.SeatId);
            var requestedSeatIds = itemsBySeatId.Keys.ToHashSet();

            var holdInventories = await _dbContext.TripSeatInventories
                .Include(x => x.Seat)
                .Where(x => x.TripId == hold.TripId && holdInventoryIds.Contains(x.Id))
                .ToListAsync(cancellationToken);

            if (holdInventories.Count != hold.Items.Count)
            {
                return BaseResponse<CreateBookingOrderResponseDto>.FailureResponse("Hold inventory not found");
            }

            var holdSeatIds = holdInventories.Select(x => x.SeatId).ToHashSet();
            if (!requestedSeatIds.SetEquals(holdSeatIds))
            {
                return BaseResponse<CreateBookingOrderResponseDto>.FailureResponse("Seat list does not match the hold");
            }

            var inventoryMap = holdInventories.ToDictionary(x => x.SeatId, x => x);
            var now = DateTime.UtcNow;
            var orderCode = $"ORD-{now:yyyyMMddHHmmss}-{Guid.NewGuid():N}";

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var totalAmount = request.Items.Sum(x => x.TicketPrice);

                var order = new BookingOrder
                {
                    OrderCode = orderCode,
                    UserId = userId,
                    TripId = hold.TripId,
                    HoldToken = request.HoldToken,
                    Status = BookingOrderStatus.Pending,
                    TotalAmount = totalAmount,
                    Currency = "VND"
                };

                await _dbContext.BookingOrders.AddAsync(order, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                var orderItems = request.Items.Select(item =>
                {
                    var inventory = inventoryMap[item.SeatId];
                    return new BookingOrderItem
                    {
                        OrderId = order.Id,
                        InventoryId = inventory.Id,
                        PassengerName = item.PassengerName,
                        PassengerType = item.PassengerType,
                        TicketPrice = item.TicketPrice
                    };
                }).ToList();

                await _dbContext.BookingOrderItems.AddRangeAsync(orderItems, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return BaseResponse<CreateBookingOrderResponseDto>.SuccessResponse(new CreateBookingOrderResponseDto
                {
                    OrderId = order.Id,
                    OrderCode = order.OrderCode,
                    HoldToken = request.HoldToken,
                    Status = order.Status.ToString(),
                    TotalAmount = order.TotalAmount,
                    Currency = order.Currency,
                    CreatedAt = order.CreatedAt
                }, "Order created");
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
