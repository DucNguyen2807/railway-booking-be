using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Application.DTOs.Station
{
    public class StationDto
    {
        public long Id { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string? Address { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public short Status { get; set; }
    }
}
