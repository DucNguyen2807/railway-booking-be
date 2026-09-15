using AutoMapper;
using RailwayBooking.Application.DTOs.Railway;
using RailwayBooking.Infrastructure.Entities.Railway;

namespace RailwayBooking.Application.Mappings
{
    public class SeatProfile : Profile
    {
        public SeatProfile()
        {
            CreateMap<Seat, SeatDto>().ReverseMap();
        }
    }
}
