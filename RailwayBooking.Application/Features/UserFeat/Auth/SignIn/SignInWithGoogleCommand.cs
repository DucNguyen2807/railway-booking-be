using MediatR;
using RailwayBooking.Application.Common;
using RailwayBooking.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Application.Features.UserFeat.Auth.SignIn
{
    public class SignInWithGoogleCommand : IRequest<BaseResponse<AuthResponse>>
    {
        public string State { get; set; }
        public string AuthorizationCode { get; set; }
        //public string IdToken { get; set; }
    }
}
