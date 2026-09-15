using RailwayBooking.Domain.Enums.Status;
using RailwayBooking.Infrastructure.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Entities.Payments
{
    public class RefundTransaction : BaseEntity
    {
        public long PaymentTransactionId { get; set; }

        public decimal Amount { get; set; }

        public string Reason { get; set; }

        public PaymentStatus Status { get; set; }

        public DateTime? RefundedAt { get; set; }

        public PaymentTransaction PaymentTransaction { get; set; }
    }
}
