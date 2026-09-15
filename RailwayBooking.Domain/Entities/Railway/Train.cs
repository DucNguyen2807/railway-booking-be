using RailwayBooking.Infrastructure.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Entities.Railway
{
    public class Train : BaseEntity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public short Status { get; set; }

        public ICollection<Coach> Coaches { get; set; }
            = new List<Coach>();
    }
}
