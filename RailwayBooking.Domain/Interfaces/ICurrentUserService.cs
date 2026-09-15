using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Domain.Interfaces
{
    public interface ICurrentUserService
    {
        public bool IsSystem { get; }
        public Guid? SystemId { get; }
        ClaimsPrincipal? GetCurrentUser();
        string? GetUserId();
        string? GetUserRole();
        string? GetUserEmail();
        bool? IsAdmin();
        bool? IsUser();
        void SetSystemId(Guid systemId);
    }
}
