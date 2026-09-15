using RailwayBooking.Infrastructure.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Entities.Infrastructure
{
    public class AuditLog : BaseEntity
    {
        public string EntityType { get; set; }

        public long EntityId { get; set; }

        public string Action { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public Guid? RequestId { get; set; }

        public long? UserId { get; set; }
    }
}
