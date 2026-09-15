using RailwayBooking.Infrastructure.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Entities.Booking
{
    public class BookingOrderItem : BaseEntity
    {
        public long OrderId { get; set; }

        public long InventoryId { get; set; }

        public string PassengerName { get; set; }

        public string PassengerType { get; set; }

        public decimal TicketPrice { get; set; }

        public BookingOrder Order { get; set; }

        public Inventory.TripSeatInventory Inventory { get; set; }
    }
}
