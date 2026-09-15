using RailwayBooking.Infrastructure.Entities.Base;
using RailwayBooking.Infrastructure.Entities.Railway;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Entities.Trips
{
    public class TrainTrip : BaseEntity
    {
        public long TrainId { get; set; }

        public string TripCode { get; set; }

        public string DepartureStation { get; set; }

        public string ArrivalStation { get; set; }

        public DateTime DepartureTime { get; set; }

        public DateTime ArrivalTime { get; set; }

        public DateTime? SalesOpenAt { get; set; }

        public DateTime? SalesCloseAt { get; set; }

        public short Status { get; set; }

        public Train Train { get; set; }

        public ICollection<TripStation> Stations { get; set; }
            = new List<TripStation>();
    }
}
