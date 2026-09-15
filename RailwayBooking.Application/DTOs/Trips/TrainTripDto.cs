namespace RailwayBooking.Application.DTOs.Trips
{
    public class TrainTripDto
    {
        public long Id { get; set; }
        public long TrainId { get; set; }
        public string TripCode { get; set; }
        public string DepartureStation { get; set; }
        public string ArrivalStation { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime? SalesOpenAt { get; set; }
        public DateTime? SalesCloseAt { get; set; }
        public short Status { get; set; }
    }
}
