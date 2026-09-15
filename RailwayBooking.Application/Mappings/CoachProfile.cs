using AutoMapper;
using RailwayBooking.Application.DTOs.Railway;
using RailwayBooking.Infrastructure.Entities.Railway;

namespace RailwayBooking.Application.Mappings
{
    public class CoachProfile : Profile
    {
        public CoachProfile()
        {
            CreateMap<Coach, CoachDto>().ReverseMap();
        }
    }
}
