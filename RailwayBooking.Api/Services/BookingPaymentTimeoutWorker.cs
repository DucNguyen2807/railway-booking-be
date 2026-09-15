using Microsoft.EntityFrameworkCore;
using RailwayBooking.Application.Services;
using RailwayBooking.Domain.Enums.Status;
using RailwayBooking.Infrastructure.Persistence;

namespace RailwayBooking.Api.Services
{
    public class BookingPaymentTimeoutWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<BookingPaymentTimeoutWorker> _logger;
        private readonly TimeSpan _scanInterval;
        private readonly TimeSpan _paymentTimeout;

        public BookingPaymentTimeoutWorker(IServiceScopeFactory scopeFactory, IConfiguration configuration, ILogger<BookingPaymentTimeoutWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            var scanIntervalSeconds = configuration.GetValue<int?>("Booking:TimeoutScanIntervalSeconds") ?? 60;
            var paymentTimeoutMinutes = configuration.GetValue<int?>("Booking:PaymentTimeoutMinutes") ?? 15;

            _scanInterval = TimeSpan.FromSeconds(Math.Max(10, scanIntervalSeconds));
            _paymentTimeout = TimeSpan.FromMinutes(Math.Max(1, paymentTimeoutMinutes));
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(_scanInterval);

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    await ProcessExpiredOrdersAsync(stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process expired booking orders");
                }
            }
        }

        private async Task ProcessExpiredOrdersAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
            var bookingOrderCancellationService = scope.ServiceProvider.GetRequiredService<IBookingOrderCancellationService>();

            var cutoff = DateTime.UtcNow.Subtract(_paymentTimeout);

            var expiredOrders = await dbContext.BookingOrders
                .Where(x => x.Status == BookingOrderStatus.Pending || x.Status == BookingOrderStatus.PaymentProcessing)
                .Where(x => x.CreatedAt <= cutoff)
                .OrderBy(x => x.CreatedAt)
                .Select(x => x.Id)
                .ToListAsync(cancellationToken);

            foreach (var orderId in expiredOrders)
            {
                try
                {
                    var result = await bookingOrderCancellationService.CancelAndReleaseAsync(orderId, null, cancellationToken);
                    if (!result.IsSuccess && !result.IsAlreadyCancelled)
                    {
                        _logger.LogWarning("Failed to auto-cancel expired order {OrderId}: {Message}", orderId, result.Message);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to cancel expired order {OrderId}", orderId);
                }
            }
        }
    }
}
