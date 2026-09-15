using Google.Apis.Auth;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Auth;
using RailwayBooking.Application.Interfaces;
using RailwayBooking.Domain.Entities.Users;
using RailwayBooking.Domain.Enums.Auth;
using RailwayBooking.Domain.Enums.Roles;
using RailwayBooking.Domain.Enums.Status;
using RailwayBooking.Domain.Interfaces;
using RailwayBooking.Infrastructure.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Application.Features.UserFeat.Auth.SignIn
{
    public class SignInWithGoogleCommandHandler : IRequestHandler<SignInWithGoogleCommand, BaseResponse<AuthResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilityService _utilityService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SignInWithGoogleCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService, IUtilityService utilityService, IConfiguration configuration, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _utilityService = utilityService;
            _configuration = configuration;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BaseResponse<AuthResponse>> Handle(SignInWithGoogleCommand request, CancellationToken cancellationToken)
        {
            var storedState = _httpContextAccessor.HttpContext.Request.Cookies["oauth_state"];
            if (storedState != request.State)
            {
                return BaseResponse<AuthResponse>.FailureResponse("Trạng thái xác thực không hợp lệ");
            }

            if (string.IsNullOrEmpty(request.AuthorizationCode))
                return BaseResponse<AuthResponse>.FailureResponse("Thiếu mã xác thực");

            var tokenRequest = new Dictionary<string, string>
        {
            { "code", request.AuthorizationCode },
            { "client_id", _configuration["Authentication:Google:ClientId"] },
            { "client_secret", _configuration["Authentication:Google:ClientSecret"] },
            { "redirect_uri", _configuration["Authentication:Google:RedirectUri"] },
            { "grant_type", "authorization_code" }
        };

            var tokenResponse = await _httpClient.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(tokenRequest));
            if (!tokenResponse.IsSuccessStatusCode)
                return BaseResponse<AuthResponse>.FailureResponse("Thất bại trong việc chuyển đổi mã xác thực sang token");

            var tokenData = JsonConvert.DeserializeObject<GoogleTokenResponse>(await tokenResponse.Content.ReadAsStringAsync());

            var googleClientId = _configuration["Authentication:Google:ClientId"];
            var payload = await GoogleJsonWebSignature.ValidateAsync(tokenData.IdToken,
                new GoogleJsonWebSignature.ValidationSettings { Audience = new[] { googleClientId } });

            if (payload == null)
                return BaseResponse<AuthResponse>.FailureResponse("Token của google không hợp lệ");

            var user = _unitOfWork.UserRepository.Get(filter: u => u.GoogleId.Equals(payload.Subject)).FirstOrDefault();

            var authResponse = await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                if (user == null)
                {
                    user = new User
                    {
                        Email = payload.Email,
                        FullName = payload.Name,
                        Avatar = payload.Picture,
                        Status = (int)UserStatus.ACTIVE_STATUS,
                        Role = (int)GeneralRole.USER_ROLE,
                        GoogleId = payload.Subject
                    };

                    _unitOfWork.UserRepository.Insert(user);
                    await _unitOfWork.SaveChangesAsync();

                    var refreshToken = _tokenService.GenerateRefreshToken(user);
                    var revokedToken = new RevokedToken
                    {
                        Token = refreshToken,
                        TokenType = (int)TokenType.REFRESH_TOKEN,
                        UserId = user.Id,
                        ExpiryDate = _tokenService.GetExpiryDate(refreshToken)
                    };

                    _unitOfWork.RevokedTokenRepository.Insert(revokedToken);
                    await _unitOfWork.SaveChangesAsync();
                }

                var accessToken = _tokenService.GenerateAccessToken(user);
                var existRevokedToken = _unitOfWork.RevokedTokenRepository.Get(
                    filter: x => x.UserId == user.Id
                    && x.RevokedAt == null)
                    .FirstOrDefault();

                if (existRevokedToken == null)
                {
                    var refreshToken = _tokenService.GenerateRefreshToken(user);
                    existRevokedToken = new RevokedToken
                    {
                        Token = refreshToken,
                        TokenType = (int)TokenType.REFRESH_TOKEN,
                        UserId = user.Id,
                        ExpiryDate = _tokenService.GetExpiryDate(refreshToken)
                    };
                    _unitOfWork.RevokedTokenRepository.Insert(existRevokedToken);
                    await _unitOfWork.SaveChangesAsync();
                }
                return new AuthResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = existRevokedToken.Token
                };
            });

            return BaseResponse<AuthResponse>.SuccessResponse(authResponse, "Đăng nhập thành công");
        }
    }

}
