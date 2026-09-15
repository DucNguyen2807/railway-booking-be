using RailwayBooking.Infrastructure.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Entities.Infrastructure
{
    public class IdempotencyRecord : BaseEntity
    {
        public string IdempotencyKey { get; set; }

        public string RequestHash { get; set; }

        public string ResponseBody { get; set; }

        public int StatusCode { get; set; }

        public DateTime? ExpiredAt { get; set; }
    }
}
