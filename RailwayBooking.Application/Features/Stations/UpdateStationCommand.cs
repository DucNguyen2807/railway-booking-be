using MediatR;
using RailwayBooking.Application.Common;
using System.ComponentModel.DataAnnotations;

namespace RailwayBooking.Application.Features.Stations
{
    public class UpdateStationCommand : IRequest<BaseResponse<bool>>
    {
        public long Id { get; set; }
        [Required]
        [MaxLength(10)]
        public string Code { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }
    }
}
