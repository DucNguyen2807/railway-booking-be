using RailwayBooking.Infrastructure.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Entities.Railway
{
    public class Seat : BaseEntity
    {
        public long CoachId { get; set; }

        public string SeatNumber { get; set; }

        public string SeatType { get; set; }

        public int RowNumber { get; set; }

        public int ColumnNumber { get; set; }

        public Coach Coach { get; set; }
    }
}
