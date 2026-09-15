using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RailwayBooking.Infrastructure.Entities.Inventory;

namespace RailwayBooking.Infrastructure.Persistence.Configurations.Inventory
{

    public class TripSeatInventoryConfiguration
        : IEntityTypeConfiguration<TripSeatInventory>
    {
        public void Configure(
            EntityTypeBuilder<TripSeatInventory> builder)
        {
            builder.ToTable("trip_seat_inventory");

            builder.HasKey(x => x.Id);

            //
            // CRITICAL UNIQUE CONSTRAINT
            // Anti-oversell guardrail
            //
            builder.HasIndex(x =>
                new { x.TripId, x.SeatId })
                .IsUnique();

            //
            // SEARCH PERFORMANCE
            //
            builder.HasIndex(x => x.TripId);

            builder.HasIndex(x => x.Status);

            builder.HasIndex(x => x.HoldExpiredAt);

            builder.HasIndex(x =>
                new { x.TripId, x.Status });

            //
            // ENUM CONVERSION
            //
            builder.Property(x => x.Status)
                .HasConversion<int>();

            //
            // CONCURRENCY TOKEN
            //
            builder.Property(x => x.Version)
                .IsConcurrencyToken();

            //
            // RELATIONS
            //
            builder.HasOne(x => x.Trip)
                .WithMany()
                .HasForeignKey(x => x.TripId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Seat)
                .WithMany()
                .HasForeignKey(x => x.SeatId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}