using RailwayBooking.Domain.Entities.Users;
using RailwayBooking.Infrastructure.Entities.Booking;
using RailwayBooking.Infrastructure.Entities.Inventory;
using RailwayBooking.Infrastructure.Entities.Payments;
using RailwayBooking.Infrastructure.Entities.Railway;
using RailwayBooking.Infrastructure.Entities.Trips;
using RailwayBooking.Infrastructure.Entities.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<RevokedToken> RevokedTokenRepository { get; }
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<Station> StationRepository { get; }
        IGenericRepository<Train> TrainRepository { get; }
        IGenericRepository<Coach> CoachRepository { get; }
        IGenericRepository<Seat> SeatRepository { get; }
        IGenericRepository<TrainTrip> TrainTripRepository { get; }
        IGenericRepository<TripSeatInventory> TripSeatInventoryRepository { get; }
        IGenericRepository<BookingOrder> BookingOrderRepository { get; }
        IGenericRepository<SeatHold> SeatHoldRepository { get; }
        IGenericRepository<PaymentTransaction> PaymentTransactionRepository { get; }

        void Save();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action);
    }
}
