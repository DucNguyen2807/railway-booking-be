using Microsoft.EntityFrameworkCore;
using RailwayBooking.Domain.Enums.Status;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Application.Services.Impl
{
    public class BookingOrderCancellationService : IBookingOrderCancellationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingOrderCancellationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BookingOrderCancellationResult> CancelAndReleaseAsync(long orderId, Guid? requestedByUserId, CancellationToken cancellationToken)
        {
            return await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var order = await _unitOfWork.BookingOrderRepository
                    .GetIncludeMultiLayer(
                        x => x.Id == orderId,
                        include: query => query.Include(x => x.Items))
                    .FirstOrDefaultAsync(cancellationToken);

                if (order == null || (requestedByUserId.HasValue && order.UserId != requestedByUserId.Value))
                {
                    return new BookingOrderCancellationResult
                    {
                        IsNotFound = true,
                        Message = "Order not found"
                    };
                }

                if (order.Status == BookingOrderStatus.Cancelled)
                {
                    return new BookingOrderCancellationResult
                    {
                        IsSuccess = true,
                        IsAlreadyCancelled = true,
                        Message = "Order already cancelled",
                        OrderId = order.Id,
                        OrderCode = order.OrderCode,
                        CancelledAt = order.CancelledAt,
                        ReleasedSeatCount = 0,
                        Status = order.Status.ToString()
                    };
                }

                if (order.Status == BookingOrderStatus.Confirmed)
                {
                    return new BookingOrderCancellationResult
                    {
                        IsConfirmed = true,
                        Message = "Confirmed order cannot be cancelled"
                    };
                }

                if (order.Items.Count == 0)
                {
                    return new BookingOrderCancellationResult
                    {
                        Message = "Order items not found"
                    };
                }

                var now = DateTime.UtcNow;
                var holdToken = order.HoldToken;
                var releasedCount = 0;

                if (holdToken.HasValue)
                {
                    foreach (var item in order.Items)
                    {
                        var inventory = await _unitOfWork.TripSeatInventoryRepository
                            .FirstOrDefaultAsync(x => x.Id == item.InventoryId
                                && x.Status == InventoryStatus.Hold
                                && x.HoldToken == holdToken.Value);

                        if (inventory == null)
                        {
                            return new BookingOrderCancellationResult
                            {
                                Message = $"Seat inventory {item.InventoryId} is no longer on hold"
                            };
                        }

                        inventory.Status = InventoryStatus.Available;
                        inventory.BookedOrderId = null;
                        inventory.HoldToken = null;
                        inventory.HoldExpiredAt = null;
                        inventory.Version += 1;
                        inventory.LastReservedAt = now;
                        _unitOfWork.TripSeatInventoryRepository.Update(inventory);
                        releasedCount++;
                    }

                    var hold = await _unitOfWork.SeatHoldRepository
                        .FirstOrDefaultAsync(x => x.HoldToken == holdToken.Value);

                    if (hold != null)
                    {
                        hold.Status = HoldStatus.Cancelled;
                        _unitOfWork.SeatHoldRepository.Update(hold);
                    }
                }

                order.Status = BookingOrderStatus.Cancelled;
                order.CancelledAt = now;
                _unitOfWork.BookingOrderRepository.Update(order);

                return new BookingOrderCancellationResult
                {
                    IsSuccess = true,
                    Message = "Order cancelled and seats released",
                    OrderId = order.Id,
                    OrderCode = order.OrderCode,
                    CancelledAt = order.CancelledAt,
                    ReleasedSeatCount = releasedCount,
                    Status = order.Status.ToString()
                };
            });
        }
    }
}
