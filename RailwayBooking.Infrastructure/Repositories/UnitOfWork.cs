using Microsoft.EntityFrameworkCore;
using RailwayBooking.Domain.Entities.Users;
using RailwayBooking.Domain.Interfaces;
using RailwayBooking.Infrastructure.Entities.Users;
using RailwayBooking.Infrastructure.Entities.Railway;
using RailwayBooking.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RailwayBooking.Infrastructure.Entities.Trips;
using RailwayBooking.Infrastructure.Entities.Booking;
using RailwayBooking.Infrastructure.Entities.Inventory;
using RailwayBooking.Infrastructure.Entities.Payments;

namespace RailwayBooking.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly BookingDbContext context;

        private GenericRepository<RevokedToken> revokedTokenRepository;
        private GenericRepository<User> userRepository;
        private GenericRepository<Station> stationRepository;
        private GenericRepository<Train> trainRepository;
        private GenericRepository<Coach> coachRepository;
        private GenericRepository<Seat> seatRepository;
        private GenericRepository<TrainTrip> trainTripRepository;
        private GenericRepository<TripSeatInventory> tripSeatInventoryRepository;
        private GenericRepository<BookingOrder> bookingOrderRepository;
        private GenericRepository<SeatHold> seatHoldRepository;
        private GenericRepository<PaymentTransaction> paymentTransactionRepository;

        public UnitOfWork(BookingDbContext _context)
        {
            context = _context;
        }

        public IGenericRepository<User> UserRepository => userRepository ??= new GenericRepository<User>(context);
        public IGenericRepository<RevokedToken> RevokedTokenRepository => revokedTokenRepository ??= new GenericRepository<RevokedToken>(context);
        public IGenericRepository<Station> StationRepository => stationRepository ??= new GenericRepository<Station>(context);
        public IGenericRepository<Train> TrainRepository => trainRepository ??= new GenericRepository<Train>(context);
        public IGenericRepository<Coach> CoachRepository => coachRepository ??= new GenericRepository<Coach>(context);
        public IGenericRepository<Seat> SeatRepository => seatRepository ??= new GenericRepository<Seat>(context);
        public IGenericRepository<TrainTrip> TrainTripRepository => trainTripRepository ??= new GenericRepository<TrainTrip>(context);
        public IGenericRepository<TripSeatInventory> TripSeatInventoryRepository => tripSeatInventoryRepository ??= new GenericRepository<TripSeatInventory>(context);
        public IGenericRepository<BookingOrder> BookingOrderRepository => bookingOrderRepository ??= new GenericRepository<BookingOrder>(context);
        public IGenericRepository<SeatHold> SeatHoldRepository => seatHoldRepository ??= new GenericRepository<SeatHold>(context);
        public IGenericRepository<PaymentTransaction> PaymentTransactionRepository => paymentTransactionRepository ??= new GenericRepository<PaymentTransaction>(context);


        public void Save()
        {
            var validationErrors = context.ChangeTracker.Entries<IValidatableObject>()
                .SelectMany(e => e.Entity.Validate(null))
                .Where(e => e != ValidationResult.Success)
                .ToArray();
            if (validationErrors.Any())
            {
                var exceptionMessage = string.Join(Environment.NewLine,
                    validationErrors.Select(error => $"Properties {error.MemberNames} Error: {error.ErrorMessage}"));
                throw new Exception(exceptionMessage);
            }
            context.SaveChanges();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var validationErrors = context.ChangeTracker.Entries<IValidatableObject>()
                .SelectMany(e => e.Entity.Validate(null))
                .Where(e => e != ValidationResult.Success)
                .ToArray();
            if (validationErrors.Any())
            {
                var exceptionMessage = string.Join(Environment.NewLine,
                    validationErrors.Select(error => $"Properties {error.MemberNames} Error: {error.ErrorMessage}"));
                throw new Exception(exceptionMessage);
            }

            return context.SaveChangesAsync(cancellationToken);
        }

        public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            var strategy = context.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await context.Database.BeginTransactionAsync();

                try
                {
                    T result = await action();

                    await context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return result;
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
