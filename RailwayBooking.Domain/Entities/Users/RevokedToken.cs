using RailwayBooking.Infrastructure.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Domain.Entities.Users
{
    public class RevokedToken
    {
        public long Id { get; set; }

        public string Token { get; set; }

        public int TokenType { get; set; }

        public DateTime? RevokedAt { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }
    }
}
