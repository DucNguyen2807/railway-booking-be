using Microsoft.AspNetCore.Http;
using RailwayBooking.Domain.Enums.Roles;
using RailwayBooking.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Application.Services.Impl
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        private Guid? _systemId;

        public bool IsSystem => _systemId.HasValue;

        public Guid? SystemId => _systemId;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal? GetCurrentUser()
        {
            return _httpContextAccessor.HttpContext?.User;
        }

        public string? GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public string? GetUserRole()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Role)?.Value;
        }

        public string? GetUserEmail()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value;
        }

        public bool? IsAdmin()
        {
            return _httpContextAccessor.HttpContext?.User.IsInRole(GeneralRole.ADMIN_ROLE.ToString());
        }

        public bool? IsUser()
        {
            return _httpContextAccessor.HttpContext?.User.IsInRole(GeneralRole.USER_ROLE.ToString());
        }

        public void SetSystemId(Guid systemId)
        {
            _systemId = systemId;
        }
    }
}
