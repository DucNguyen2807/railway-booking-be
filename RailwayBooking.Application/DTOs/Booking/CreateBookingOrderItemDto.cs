using System.ComponentModel.DataAnnotations;

namespace RailwayBooking.Application.DTOs.Booking
{
    public class CreateBookingOrderItemDto
    {
        [Range(1, long.MaxValue)]
        public long SeatId { get; set; }

        [Required]
        [MaxLength(200)]
        public string PassengerName { get; set; }

        [Required]
        [MaxLength(30)]
        public string PassengerType { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TicketPrice { get; set; }
    }
}
