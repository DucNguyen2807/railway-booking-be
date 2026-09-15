using MediatR;
using RailwayBooking.Application.Common;

namespace RailwayBooking.Application.Features.UserFeat.Auth.ResendOtp
{
    public class ResendOtpCommand : IRequest<BaseResponse<string>>
    {
        public string Mail { get; set; }
    }
}
