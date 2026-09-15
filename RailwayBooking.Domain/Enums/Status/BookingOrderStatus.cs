using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Domain.Enums.Status
{
    public enum BookingOrderStatus
    {
        Pending = 1,
        PaymentProcessing = 2,
        Confirmed = 3,
        Failed = 4,
        Cancelled = 5
    }
}
