using RailwayBooking.Domain.Enums.Status;
using RailwayBooking.Infrastructure.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Entities.Infrastructure
{
    public class OutboxEvent : BaseEntity
    {
        public Guid EventId { get; set; }

        public string EventType { get; set; }

        public string AggregateType { get; set; }

        public long AggregateId { get; set; }

        public string Payload { get; set; }

        public OutboxStatus Status { get; set; }

        public int RetryCount { get; set; }

        public DateTime? PublishedAt { get; set; }
    }
}
