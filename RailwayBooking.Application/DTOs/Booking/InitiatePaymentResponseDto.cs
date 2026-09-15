namespace RailwayBooking.Application.DTOs.Booking
{
    public class InitiatePaymentResponseDto
    {
        public long OrderId { get; set; }

        public string? OrderCode { get; set; }

        public string? Provider { get; set; }

        public string? PaymentUrl { get; set; }

        public string? ProviderTransactionId { get; set; }

        public string? OrderStatus { get; set; }
    }
}
