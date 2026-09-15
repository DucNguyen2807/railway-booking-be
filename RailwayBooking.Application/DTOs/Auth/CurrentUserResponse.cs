using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Application.DTOs.Auth
{
    public class CurrentUserResponse
    {
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public int Status { get; set; }

        public int Role { get; set; }

        public string? Avatar { get; set; }
    }
}
