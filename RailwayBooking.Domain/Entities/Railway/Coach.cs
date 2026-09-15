using RailwayBooking.Infrastructure.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Entities.Railway
{
    public class Coach : BaseEntity
    {
        public long TrainId { get; set; }

        public string CoachNumber { get; set; }

        public string ClassType { get; set; }

        public int SeatCapacity { get; set; }

        public Train Train { get; set; }

        public ICollection<Seat> Seats { get; set; }
            = new List<Seat>();
    }
}
