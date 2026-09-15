using RailwayBooking.Infrastructure.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Entities.Inventory
{
    public class HoldItem : BaseEntity
    {
        public long HoldId { get; set; }

        public long InventoryId { get; set; }

        public SeatHold Hold { get; set; }

        public TripSeatInventory Inventory { get; set; }
    }
}
