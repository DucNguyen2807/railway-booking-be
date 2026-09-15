using MediatR;
using RailwayBooking.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Application.Features.UserFeat.Auth.SignIn
{
    public class GoogleAuthProfileQuery : IRequest<BaseResponse<string>>
    {
        public GoogleAuthProfileQuery()
        {
        }
    }
}
