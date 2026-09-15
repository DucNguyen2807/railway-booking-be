using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Booking;
using System.ComponentModel.DataAnnotations;

namespace RailwayBooking.Application.Features.BookingFeat.CancelBookingOrder
{
    public class CancelBookingOrderCommand : IRequest<BaseResponse<CancelBookingOrderResponseDto>>
    {
        [Range(1, long.MaxValue)]
        public long OrderId { get; set; }
    }
}
