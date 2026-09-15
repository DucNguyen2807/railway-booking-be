using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Booking;
using System.ComponentModel.DataAnnotations;

namespace RailwayBooking.Application.Features.BookingFeat.HoldSeat
{
    public class HoldSeatCommand : IRequest<BaseResponse<HoldSeatResponseDto>>
    {
        [Range(1, long.MaxValue)]
        public long TripId { get; set; }

        [Range(1, long.MaxValue)]
        public long FromStationId { get; set; }

        [Range(1, long.MaxValue)]
        public long ToStationId { get; set; }

        [Required]
        [MinLength(1)]
        public List<long> SeatIds { get; set; } = new List<long>();
    }
}
