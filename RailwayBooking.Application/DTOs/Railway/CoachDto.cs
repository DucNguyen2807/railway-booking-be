namespace RailwayBooking.Application.DTOs.Railway
{
    public class CoachDto
    {
        public long Id { get; set; }
        public long TrainId { get; set; }
        public string CoachNumber { get; set; }
        public string ClassType { get; set; }
        public int SeatCapacity { get; set; }
    }
}
