using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Auth;
using RailwayBooking.Application.Interfaces;
using RailwayBooking.Domain.Entities.Users;
using RailwayBooking.Domain.Enums.Auth;
using RailwayBooking.Domain.Enums.Status;
using RailwayBooking.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Application.Features.UserFeat.Auth.SignIn
{
    public class SignInCommandHandler : IRequestHandler<SignInCommand, BaseResponse<AuthResponse>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUtilityService _utilityService;
        private readonly ITokenService _tokenService;

        public SignInCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService, IUtilityService utilityService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _utilityService = utilityService;
        }

        public async Task<BaseResponse<AuthResponse>> Handle(SignInCommand request, CancellationToken cancellationToken)
        {
            var user = _unitOfWork.UserRepository.Get(filter: x => x.Email == request.Mail && x.GoogleId == null)
                .FirstOrDefault();

            if (user == null)
            {
                return BaseResponse<AuthResponse>.FailureResponse("Mail hoặc mật khẩu không hợp lệ");
            }

            if (user.Status != (int)UserStatus.ACTIVE_STATUS)
            {
                return BaseResponse<AuthResponse>.FailureResponse("Tài khoản chưa được xác minh OTP");
            }

            bool isPasswordValid = _utilityService.VerifyPassword(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return BaseResponse<AuthResponse>.FailureResponse("Mail hoặc mật khẩu không hợp lệ");
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var revokedToken = _unitOfWork.RevokedTokenRepository.Get(
                filter: x => x.UserId == user.Id
                && x.RevokedAt == null)
                .FirstOrDefault();

            if (revokedToken == null)
            {
                var refreshToken = _tokenService.GenerateRefreshToken(user);
                var expiryDateUtc = _tokenService.GetExpiryDate(refreshToken);
                revokedToken = new RevokedToken
                {
                    Token = refreshToken,
                    TokenType = (int)TokenType.REFRESH_TOKEN,
                    UserId = user.Id,
                    ExpiryDate = expiryDateUtc
                };
                _unitOfWork.RevokedTokenRepository.Insert(revokedToken);
                await _unitOfWork.SaveChangesAsync();
            }

            var authResponse = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = revokedToken.Token
            };

            return BaseResponse<AuthResponse>.SuccessResponse(authResponse, "Đăng nhập thành công");
        }

    }

}
