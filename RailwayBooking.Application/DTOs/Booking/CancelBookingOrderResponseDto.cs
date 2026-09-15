using System;

namespace RailwayBooking.Application.DTOs.Booking
{
    public class CancelBookingOrderResponseDto
    {
        public long OrderId { get; set; }
        public string OrderCode { get; set; }
        public string Status { get; set; }
        public DateTime? CancelledAt { get; set; }
        public int ReleasedSeatCount { get; set; }
    }
}
