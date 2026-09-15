using MediatR;
using Microsoft.EntityFrameworkCore;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Booking;
using RailwayBooking.Domain.Enums.Status;
using RailwayBooking.Domain.Interfaces;
using RailwayBooking.Infrastructure.Persistence;

namespace RailwayBooking.Application.Features.BookingFeat.ConfirmBookingOrder
{
    public class ConfirmBookingOrderCommandHandler : IRequestHandler<ConfirmBookingOrderCommand, BaseResponse<ConfirmBookingOrderResponseDto>>
    {
        private readonly BookingDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;

        public ConfirmBookingOrderCommandHandler(BookingDbContext dbContext, ICurrentUserService currentUserService)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
        }

        public async Task<BaseResponse<ConfirmBookingOrderResponseDto>> Handle(ConfirmBookingOrderCommand request, CancellationToken cancellationToken)
        {
            var userIdValue = _currentUserService.GetUserId();
            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return BaseResponse<ConfirmBookingOrderResponseDto>.FailureResponse("Unauthorized user");
            }

            var order = await _dbContext.BookingOrders
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.Id == request.OrderId && x.UserId == userId, cancellationToken);

            if (order == null)
            {
                return BaseResponse<ConfirmBookingOrderResponseDto>.FailureResponse("Order not found");
            }

            if (order.Status == BookingOrderStatus.Confirmed)
            {
                return BaseResponse<ConfirmBookingOrderResponseDto>.SuccessResponse(new ConfirmBookingOrderResponseDto
                {
                    OrderId = order.Id,
                    OrderCode = order.OrderCode,
                    Status = order.Status.ToString(),
                    ConfirmedAt = order.ConfirmedAt,
                    TotalAmount = order.TotalAmount,
                    Currency = order.Currency
                }, "Order already confirmed");
            }

            if (order.Status != BookingOrderStatus.Pending && order.Status != BookingOrderStatus.PaymentProcessing)
            {
                return BaseResponse<ConfirmBookingOrderResponseDto>.FailureResponse("Order is not payable");
            }

            if (!order.HoldToken.HasValue)
            {
                return BaseResponse<ConfirmBookingOrderResponseDto>.FailureResponse("Order has no hold token");
            }

            var hold = await _dbContext.SeatHolds
                .Include(x => x.Items)
                .FirstOrDefaultAsync(x => x.HoldToken == order.HoldToken.Value && x.UserId == userId, cancellationToken);

            if (hold == null)
            {
                return BaseResponse<ConfirmBookingOrderResponseDto>.FailureResponse("Hold not found");
            }

            if (hold.Status != HoldStatus.Active)
            {
                return BaseResponse<ConfirmBookingOrderResponseDto>.FailureResponse("Hold is no longer active");
            }

            if (hold.ExpiredAt <= DateTime.UtcNow)
            {
                return BaseResponse<ConfirmBookingOrderResponseDto>.FailureResponse("Hold has expired");
            }

            var orderItems = await _dbContext.BookingOrderItems
                .Where(x => x.OrderId == order.Id)
                .Include(x => x.Inventory)
                .ToListAsync(cancellationToken);

            if (orderItems.Count == 0)
            {
                return BaseResponse<ConfirmBookingOrderResponseDto>.FailureResponse("Order items not found");
            }

            var now = DateTime.UtcNow;

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                foreach (var item in orderItems)
                {
                    var affectedRows = await _dbContext.Database.ExecuteSqlInterpolatedAsync($@"
UPDATE trip_seat_inventory
SET
    status = {(int)InventoryStatus.Booked},
    booked_order_id = {order.Id},
    hold_token = NULL,
    hold_expired_at = NULL,
    version = version + 1,
    last_reserved_at = {now}
WHERE
    id = {item.InventoryId}
    AND status = {(int)InventoryStatus.Hold}
    AND hold_token = {order.HoldToken.Value}", cancellationToken);

                    if (affectedRows == 0)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return BaseResponse<ConfirmBookingOrderResponseDto>.FailureResponse($"Seat inventory {item.InventoryId} is no longer available");
                    }
                }

                order.Status = BookingOrderStatus.Confirmed;
                order.ConfirmedAt = now;
                _dbContext.BookingOrders.Update(order);

                hold.Status = HoldStatus.Completed;
                _dbContext.SeatHolds.Update(hold);

                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return BaseResponse<ConfirmBookingOrderResponseDto>.SuccessResponse(new ConfirmBookingOrderResponseDto
                {
                    OrderId = order.Id,
                    OrderCode = order.OrderCode,
                    Status = order.Status.ToString(),
                    ConfirmedAt = order.ConfirmedAt,
                    TotalAmount = order.TotalAmount,
                    Currency = order.Currency
                }, "Order confirmed and seats booked");
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
