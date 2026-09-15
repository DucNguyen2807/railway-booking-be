using RailwayBooking.Domain.Enums.Status;
using RailwayBooking.Infrastructure.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Entities.Inventory
{
    public class TripSeatInventory : BaseEntity
    {
        public long TripId { get; set; }

        public long SeatId { get; set; }

        public InventoryStatus Status { get; set; }

        public Guid? HoldToken { get; set; }

        public DateTime? HoldExpiredAt { get; set; }

        public long? BookedOrderId { get; set; }

        public int Version { get; set; }

        public DateTime? LastReservedAt { get; set; }

        public Trips.TrainTrip Trip { get; set; }

        public Railway.Seat Seat { get; set; }
    }
}
