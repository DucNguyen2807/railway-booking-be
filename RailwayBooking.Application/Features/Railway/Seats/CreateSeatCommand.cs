using MediatR;
using RailwayBooking.Application.Common;
using System.ComponentModel.DataAnnotations;

namespace RailwayBooking.Application.Features.Railway.Seats
{
    public class CreateSeatCommand : IRequest<BaseResponse<long>>
    {
        [Range(1, long.MaxValue)]
        public long CoachId { get; set; }

        [Required]
        [MaxLength(20)]
        public string SeatNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string SeatType { get; set; }

        [Range(0, 1000)]
        public int RowNumber { get; set; }

        [Range(0, 1000)]
        public int ColumnNumber { get; set; }
    }
}
