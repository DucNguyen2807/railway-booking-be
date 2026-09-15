using RailwayBooking.Domain.Enums.Status;
using RailwayBooking.Infrastructure.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Entities.Payments
{
    public class PaymentTransaction : BaseEntity
    {
        public long OrderId { get; set; }

        public string PaymentProvider { get; set; }

        public string ProviderTransactionId { get; set; }

        public string IdempotencyKey { get; set; }

        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public PaymentStatus Status { get; set; }

        public string RawResponse { get; set; }

        public DateTime? PaidAt { get; set; }

        public Booking.BookingOrder Order { get; set; }
    }
}
