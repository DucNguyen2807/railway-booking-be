using System;

namespace RailwayBooking.Application.DTOs.Booking
{
    public class HoldSeatResponseDto
    {
        public string HoldToken { get; set; }
        public DateTime ExpiredAt { get; set; }
    }
}
