namespace RailwayBooking.Application.Services
{
    public interface IBookingOrderCancellationService
    {
        Task<BookingOrderCancellationResult> CancelAndReleaseAsync(long orderId, Guid? requestedByUserId, CancellationToken cancellationToken);
    }

    public class BookingOrderCancellationResult
    {
        public bool IsSuccess { get; init; }
        public bool IsNotFound { get; init; }
        public bool IsAlreadyCancelled { get; init; }
        public bool IsConfirmed { get; init; }
        public string Message { get; init; } = string.Empty;
        public long OrderId { get; init; }
        public string? OrderCode { get; init; }
        public DateTime? CancelledAt { get; init; }
        public int ReleasedSeatCount { get; init; }
        public string? Status { get; init; }
    }
}