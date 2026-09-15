using RailwayBooking.Domain.Enums.Status;
using RailwayBooking.Infrastructure.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Entities.Booking
{
    public class Ticket : BaseEntity
    {
        public string TicketCode { get; set; }

        public long OrderItemId { get; set; }

        public string QrCode { get; set; }

        public DateTime? IssuedAt { get; set; }

        public TicketStatus Status { get; set; }

        public BookingOrderItem OrderItem { get; set; }
    }
}
