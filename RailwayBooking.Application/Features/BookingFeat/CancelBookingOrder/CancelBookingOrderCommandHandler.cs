using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Booking;
using RailwayBooking.Application.Services;
using RailwayBooking.Domain.Enums.Status;
using RailwayBooking.Domain.Interfaces;

namespace RailwayBooking.Application.Features.BookingFeat.CancelBookingOrder
{
    public class CancelBookingOrderCommandHandler : IRequestHandler<CancelBookingOrderCommand, BaseResponse<CancelBookingOrderResponseDto>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IBookingOrderCancellationService _bookingOrderCancellationService;

        public CancelBookingOrderCommandHandler(ICurrentUserService currentUserService, IBookingOrderCancellationService bookingOrderCancellationService)
        {
            _currentUserService = currentUserService;
            _bookingOrderCancellationService = bookingOrderCancellationService;
        }

        public async Task<BaseResponse<CancelBookingOrderResponseDto>> Handle(CancelBookingOrderCommand request, CancellationToken cancellationToken)
        {
            var userIdValue = _currentUserService.GetUserId();
            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return BaseResponse<CancelBookingOrderResponseDto>.FailureResponse("Unauthorized user");
            }

            var cancelResult = await _bookingOrderCancellationService.CancelAndReleaseAsync(request.OrderId, userId, cancellationToken);

            if (cancelResult.IsNotFound)
            {
                return BaseResponse<CancelBookingOrderResponseDto>.FailureResponse(cancelResult.Message);
            }

            if (cancelResult.IsAlreadyCancelled)
            {
                return BaseResponse<CancelBookingOrderResponseDto>.SuccessResponse(new CancelBookingOrderResponseDto
                {
                    OrderId = cancelResult.OrderId,
                    OrderCode = cancelResult.OrderCode,
                    Status = cancelResult.Status ?? BookingOrderStatus.Cancelled.ToString(),
                    CancelledAt = cancelResult.CancelledAt,
                    ReleasedSeatCount = 0
                }, "Order already cancelled");
            }

            if (cancelResult.IsConfirmed)
            {
                return BaseResponse<CancelBookingOrderResponseDto>.FailureResponse(cancelResult.Message);
            }

            if (!cancelResult.IsSuccess)
            {
                return BaseResponse<CancelBookingOrderResponseDto>.FailureResponse(cancelResult.Message);
            }

            return BaseResponse<CancelBookingOrderResponseDto>.SuccessResponse(new CancelBookingOrderResponseDto
            {
                OrderId = cancelResult.OrderId,
                OrderCode = cancelResult.OrderCode,
                Status = cancelResult.Status ?? BookingOrderStatus.Cancelled.ToString(),
                CancelledAt = cancelResult.CancelledAt,
                ReleasedSeatCount = cancelResult.ReleasedSeatCount
            }, cancelResult.Message);
        }
    }
}
