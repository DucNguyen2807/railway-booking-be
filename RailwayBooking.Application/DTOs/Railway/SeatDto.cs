namespace RailwayBooking.Application.DTOs.Railway
{
    public class SeatDto
    {
        public long Id { get; set; }
        public long CoachId { get; set; }
        public string SeatNumber { get; set; }
        public string SeatType { get; set; }
        public int RowNumber { get; set; }
        public int ColumnNumber { get; set; }
    }
}
