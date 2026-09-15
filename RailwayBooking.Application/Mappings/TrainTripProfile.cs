using AutoMapper;
using RailwayBooking.Application.DTOs.Trips;
using RailwayBooking.Infrastructure.Entities.Trips;

namespace RailwayBooking.Application.Mappings
{
    public class TrainTripProfile : Profile
    {
        public TrainTripProfile()
        {
            CreateMap<TrainTrip, TrainTripDto>().ReverseMap();
        }
    }
}
