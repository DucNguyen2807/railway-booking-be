using MediatR;
using RailwayBooking.Application.Common;

namespace RailwayBooking.Application.Features.UserFeat.Auth.VerifyOtp
{
    public class VerifyOtpCommand : IRequest<BaseResponse<string>>
    {
        public string Mail { get; set; }
        public string Otp { get; set; }
    }
}
