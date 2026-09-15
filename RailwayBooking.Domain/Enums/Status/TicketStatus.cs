using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Domain.Enums.Status
{
    public enum TicketStatus
    {
        Active = 1,
        Used = 2,
        Cancelled = 3,
        Refunded = 4
    }
}
