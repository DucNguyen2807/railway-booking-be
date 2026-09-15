using System;

namespace RailwayBooking.Application.DTOs.Booking
{
    public class CreateBookingOrderResponseDto
    {
        public long OrderId { get; set; }
        public string OrderCode { get; set; }
        public Guid HoldToken { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
