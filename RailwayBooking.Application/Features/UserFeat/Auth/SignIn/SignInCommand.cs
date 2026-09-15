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
    public class SignInCommand : IRequest<BaseResponse<AuthResponse>>
    {
        public string Mail { get; set; }
        public string Password { get; set; }
    }
}
