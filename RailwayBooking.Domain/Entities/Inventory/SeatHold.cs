using RailwayBooking.Domain.Enums.Status;
using RailwayBooking.Infrastructure.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Entities.Inventory
{
    public class SeatHold : BaseEntity
    {
        public Guid HoldToken { get; set; }

        public Guid UserId { get; set; }

        public long TripId { get; set; }

        public HoldStatus Status { get; set; }

        public DateTime ExpiredAt { get; set; }

        public ICollection<HoldItem> Items { get; set; }
            = new List<HoldItem>();
    }
}
