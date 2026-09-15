namespace RailwayBooking.Application.DTOs.Booking
{
    public class PaymentCallbackResponseDto
    {
        public long OrderId { get; set; }

        public string? Provider { get; set; }

        public string? PaymentStatus { get; set; }

        public string? OrderStatus { get; set; }

        public string? Message { get; set; }
    }
}
