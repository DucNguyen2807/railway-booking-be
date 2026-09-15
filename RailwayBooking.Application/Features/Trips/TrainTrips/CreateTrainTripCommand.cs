using MediatR;
using RailwayBooking.Application.Common;
using System.ComponentModel.DataAnnotations;

namespace RailwayBooking.Application.Features.Trips.TrainTrips
{
    public class CreateTrainTripCommand : IRequest<BaseResponse<long>>
    {
        [Range(1, long.MaxValue)]
        public long TrainId { get; set; }

        [Required]
        [MaxLength(50)]
        public string TripCode { get; set; }

        [Required]
        [MaxLength(100)]
        public string DepartureStation { get; set; }

        [Required]
        [MaxLength(100)]
        public string ArrivalStation { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        public DateTime? SalesOpenAt { get; set; }
        public DateTime? SalesCloseAt { get; set; }
    }
}
