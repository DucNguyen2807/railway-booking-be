using RailwayBooking.Domain.Entities.Users;
using RailwayBooking.Infrastructure.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Entities.Users
{
    public class User : BaseEntity
    {
        public Guid Id { get; set; }

        public string? FullName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public string? PasswordHash { get; set; }

        public int? Status { get; set; }

        public string? OtpCodeHash { get; set; }

        public DateTime? OtpCodeExpiryUtc { get; set; }

        public int? Role { get; set; }

        public string? GoogleId { get; set; }

        public string? Avatar { get; set; }

        public ICollection<RevokedToken>
            RevokedTokens
        { get; set; }
                = new List<RevokedToken>();
    }
}
