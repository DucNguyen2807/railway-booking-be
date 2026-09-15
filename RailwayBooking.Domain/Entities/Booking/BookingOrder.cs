using RailwayBooking.Domain.Enums.Status;
using RailwayBooking.Infrastructure.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Entities.Booking
{
    public class BookingOrder : BaseEntity
    {
        public string OrderCode { get; set; }

        public Guid UserId { get; set; }

        public long TripId { get; set; }

        public Guid? HoldToken { get; set; }

        public BookingOrderStatus Status { get; set; }

        public decimal TotalAmount { get; set; }

        public string Currency { get; set; } = "VND";

        public DateTime? ConfirmedAt { get; set; }

        public DateTime? CancelledAt { get; set; }

        public ICollection<BookingOrderItem> Items { get; set; }
            = new List<BookingOrderItem>();
    }
}
