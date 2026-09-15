using MediatR;
using RailwayBooking.Application.Common;

namespace RailwayBooking.Application.Features.UserFeat.Auth.SignUp
{
    public class SignUpCommand : IRequest<BaseResponse<string>>
    {
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }
    }
}
