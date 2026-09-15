using AutoMapper;
using RailwayBooking.Application.DTOs.Railway;
using RailwayBooking.Infrastructure.Entities.Railway;

namespace RailwayBooking.Application.Mappings
{
    public class TrainProfile : Profile
    {
        public TrainProfile()
        {
            CreateMap<Train, TrainDto>().ReverseMap();
        }
    }
}
