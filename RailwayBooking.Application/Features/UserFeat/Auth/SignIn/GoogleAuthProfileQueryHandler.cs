using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RailwayBooking.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Application.Features.UserFeat.Auth.SignIn
{
    public class GoogleAuthProfileQueryHandler : IRequestHandler<GoogleAuthProfileQuery, BaseResponse<string>>
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GoogleAuthProfileQueryHandler(IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BaseResponse<string>> Handle(GoogleAuthProfileQuery request, CancellationToken cancellationToken)
        {
            var state = Guid.NewGuid().ToString("N");

            _httpContextAccessor.HttpContext.Response.Cookies.Append("oauth_state", state, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            });

            var googleAuthUrl = "https://accounts.google.com/o/oauth2/v2/auth" +
                "?client_id=" + _configuration["Authentication:Google:ClientId"] +
                "&redirect_uri=" + _configuration["Authentication:Google:RedirectUri"] +
                "&response_type=code" +
                "&scope=openid email profile" +
                "&state=" + state;

            return BaseResponse<string>.SuccessResponse(data: googleAuthUrl);
        }
    }

}
