using AutoMapper;
using RailwayBooking.Application.DTOs.Station;
using RailwayBooking.Infrastructure.Entities.Railway;

namespace RailwayBooking.Application.Mappings
{
    public class StationProfile : Profile
    {
        public StationProfile()
        {
            CreateMap<Station, StationDto>().ReverseMap();
        }
    }
}
