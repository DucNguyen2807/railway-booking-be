using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Booking;
using System.ComponentModel.DataAnnotations;

namespace RailwayBooking.Application.Features.BookingFeat.ConfirmBookingOrder
{
    public class ConfirmBookingOrderCommand : IRequest<BaseResponse<ConfirmBookingOrderResponseDto>>
    {
        [Range(1, long.MaxValue)]
        public long OrderId { get; set; }
    }
}
