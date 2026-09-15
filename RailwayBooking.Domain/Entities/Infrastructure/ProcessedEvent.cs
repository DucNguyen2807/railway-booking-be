using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Entities.Infrastructure
{
    public class ProcessedEvent
    {
        public Guid EventId { get; set; }

        public string ConsumerName { get; set; }

        public DateTime ProcessedAt { get; set; }
    }
}
