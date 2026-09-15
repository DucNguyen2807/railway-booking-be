using AutoMapper;
using RailwayBooking.Application.DTOs.Auth;
using RailwayBooking.Domain.Enums.Roles;
using RailwayBooking.Domain.Enums.Status;
using RailwayBooking.Infrastructure.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, CurrentUserResponse>();
        }
    }

}
