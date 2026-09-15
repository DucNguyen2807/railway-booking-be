using MediatR;
using RailwayBooking.Application.Common;
using System.ComponentModel.DataAnnotations;

namespace RailwayBooking.Application.Features.Railway.Coaches
{
    public class UpdateCoachCommand : IRequest<BaseResponse<bool>>
    {
        public long Id { get; set; }
        [Range(1, long.MaxValue)]
        public long TrainId { get; set; }

        [Required]
        [MaxLength(20)]
        public string CoachNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string ClassType { get; set; }

        [Range(1, 1000)]
        public int SeatCapacity { get; set; }
    }
}
