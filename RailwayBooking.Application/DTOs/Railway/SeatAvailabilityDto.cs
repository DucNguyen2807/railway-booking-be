using System;

namespace RailwayBooking.Application.DTOs.Railway
{
    public class SeatAvailabilityDto
    {
        public long InventoryId { get; set; }
        public long SeatId { get; set; }
        public string SeatNumber { get; set; }
        public long CoachId { get; set; }
        public string CoachNumber { get; set; }
        public string Status { get; set; }
        public Guid? HoldToken { get; set; }
        public DateTime? HoldExpiredAt { get; set; }
        public long? BookedOrderId { get; set; }
    }
}
