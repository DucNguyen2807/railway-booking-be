using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Booking;
using System.ComponentModel.DataAnnotations;

namespace RailwayBooking.Application.Features.BookingFeat.CreateBookingOrder
{
    public class CreateBookingOrderCommand : IRequest<BaseResponse<CreateBookingOrderResponseDto>>
    {
        [Required]
        public Guid HoldToken { get; set; }

        [Required]
        [MinLength(1)]
        public List<CreateBookingOrderItemDto> Items { get; set; } = new List<CreateBookingOrderItemDto>();
    }
}
