using RailwayBooking.Infrastructure.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Entities.Trips
{
    public class TripStation : BaseEntity
    {
        public long TripId { get; set; }

        public string StationCode { get; set; }

        public int StationOrder { get; set; }

        public DateTime? ArrivalTime { get; set; }

        public DateTime? DepartureTime { get; set; }

        public TrainTrip Trip { get; set; }
    }
}
