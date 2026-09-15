using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Domain.Enums.Status
{
    public enum OutboxStatus
    {
        Pending = 1,
        Published = 2,
        Failed = 3
    }
}
