using RailwayBooking.Application.DTOs.Auth;
using RailwayBooking.Domain.Entities.Users;
using RailwayBooking.Infrastructure.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);

        string GenerateRefreshToken(User user);

        Task<AuthResponse?>
            RefreshAccessTokenAsync(
                RevokedToken token);

        bool IsTokenRevoked(
            RevokedToken token);

        DateTime? GetExpiryDate(
            string token);

        bool IsTokenExpired(
            string token);

        Task RevokeRefreshTokenAsync(
            RevokedToken token);

        Task<string>
            GenerateJwtTokenGoogle(
                ClaimsPrincipal user);
    }
}
