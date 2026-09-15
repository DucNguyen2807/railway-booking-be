using Microsoft.EntityFrameworkCore;
using RailwayBooking.Domain.Entities.Users;
using RailwayBooking.Infrastructure.Entities.Base;
using RailwayBooking.Infrastructure.Entities.Booking;
using RailwayBooking.Infrastructure.Entities.Infrastructure;
using RailwayBooking.Infrastructure.Entities.Inventory;
using RailwayBooking.Infrastructure.Entities.Payments;
using RailwayBooking.Infrastructure.Entities.Railway;
using RailwayBooking.Infrastructure.Entities.Trips;
using RailwayBooking.Infrastructure.Entities.Users;
using System.Net.Sockets;

namespace RailwayBooking.Infrastructure.Persistence;

public class BookingDbContext : DbContext
{
    public BookingDbContext(
        DbContextOptions<BookingDbContext> options)
        : base(options)
    {
    }

    //
    // Railway
    //
    public DbSet<Train> Trains => Set<Train>();

    public DbSet<Coach> Coaches => Set<Coach>();

    public DbSet<Seat> Seats => Set<Seat>();

    //
    // Trips
    //
    public DbSet<TrainTrip> TrainTrips => Set<TrainTrip>();

    public DbSet<TripStation> TripStations => Set<TripStation>();

    //
    // Inventory
    //
    public DbSet<TripSeatInventory> TripSeatInventories
        => Set<TripSeatInventory>();

    public DbSet<SeatHold> SeatHolds
        => Set<SeatHold>();

    public DbSet<HoldItem> HoldItems
        => Set<HoldItem>();

    //
    // Booking
    //
    public DbSet<BookingOrder> BookingOrders
        => Set<BookingOrder>();

    public DbSet<BookingOrderItem> BookingOrderItems
        => Set<BookingOrderItem>();

    public DbSet<Ticket> Tickets
        => Set<Ticket>();

    //
    // Payments
    //
    public DbSet<PaymentTransaction> PaymentTransactions
        => Set<PaymentTransaction>();

    public DbSet<RefundTransaction> RefundTransactions
        => Set<RefundTransaction>();

    //
    // User
    //
    public DbSet<User> Users => Set<User>();

    public DbSet<RevokedToken>
        RevokedTokens => Set<RevokedToken>();

    //
    // Infrastructure
    //
    public DbSet<OutboxEvent> OutboxEvents
        => Set<OutboxEvent>();

    public DbSet<AuditLog> AuditLogs
        => Set<AuditLog>();

    public DbSet<ProcessedEvent> ProcessedEvents
        => Set<ProcessedEvent>();

    public DbSet<IdempotencyRecord> IdempotencyRecords
        => Set<IdempotencyRecord>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //
        // Auto scan all configurations
        //
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(BookingDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        ApplyAuditTimestamps();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private void ApplyAuditTimestamps()
    {
        var entries = ChangeTracker
            .Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }
}