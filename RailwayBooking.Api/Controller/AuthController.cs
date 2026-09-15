using MediatR;
using Microsoft.AspNetCore.Mvc;
using RailwayBooking.Application.Features.UserFeat.Auth.CurrentUser;
using RailwayBooking.Application.Features.UserFeat.Auth.ResendOtp;
using RailwayBooking.Application.Features.UserFeat.Auth.SignIn;
using RailwayBooking.Application.Features.UserFeat.Auth.SignOut;
using RailwayBooking.Application.Features.UserFeat.Auth.SignUp;
using RailwayBooking.Application.Features.UserFeat.Auth.VerifyOtp;
using RailwayBooking.Application.Interfaces;

namespace RailwayBooking.Api.Controller
{
    public class AuthController : BaseController
    {
        private readonly ITokenService _tokenService;

        public AuthController(IMediator mediator, ITokenService tokenService) : base(mediator)
        {
            _tokenService = tokenService;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInCommand command)
        {
            var response = await Send(command);
            return response;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpCommand command)
        {
            var response = await Send(command);
            return response;
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpCommand command)
        {
            var response = await Send(command);
            return response;
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpCommand command)
        {
            var response = await Send(command);
            return response;
        }

        [HttpGet("google-signin")]
        public async Task<IActionResult> GoogleLogin()
        {
            var googleAuthUrl = await _mediator.Send(new GoogleAuthProfileQuery());
            return Redirect(googleAuthUrl.Data);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("google-callback")]
        public async Task<IActionResult> SignInWithGoogle([FromQuery] string code, [FromQuery] string state)
        {
            var response = await _mediator.Send(new SignInWithGoogleCommand
            {
                AuthorizationCode = code,
                State = state
            });

            if (!response.Success)
            {
                return Redirect($"https://cfms.site/auth/error?message={Uri.EscapeDataString(response.Message)}");
            }

            var token = response.Data;

            return Redirect($"https://cfms.site/check-login?token={Uri.EscapeDataString(token?.AccessToken)}&refreshToken={Uri.EscapeDataString(token?.RefreshToken)}");
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var response = await Send(new GetCurrentUserQuery());
            return response;
        }

        [HttpPost("signout")]
        public async Task<IActionResult> SignOut()
        {
            var response = await Send(new SignOutQuery());
            return response;
        }
    }
}
