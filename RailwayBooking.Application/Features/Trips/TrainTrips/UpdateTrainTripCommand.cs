using MediatR;
using RailwayBooking.Application.Common;
using System.ComponentModel.DataAnnotations;

namespace RailwayBooking.Application.Features.Trips.TrainTrips
{
    public class UpdateTrainTripCommand : IRequest<BaseResponse<bool>>
    {
        public long Id { get; set; }
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
        public short Status { get; set; }
    }
}
