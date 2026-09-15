using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RailwayBooking.Infrastructure.Entities.Booking;

namespace RailwayBooking.Infrastructure.Persistence.Configurations.Booking
{

    public class BookingOrderConfiguration
        : IEntityTypeConfiguration<BookingOrder>
    {
        public void Configure(
            EntityTypeBuilder<BookingOrder> builder)
        {
            builder.ToTable("booking_orders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.OrderCode)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Currency)
                .HasMaxLength(10);

            builder.Property(x => x.TotalAmount)
                .HasPrecision(12, 2);

            builder.Property(x => x.Status)
                .HasConversion<int>();

            builder.HasIndex(x => x.OrderCode)
                .IsUnique();

            builder.HasIndex(x => x.HoldToken)
                .IsUnique();

            builder.HasIndex(x => x.UserId);

            builder.HasIndex(x => x.TripId);

            builder.HasIndex(x => x.Status);

            builder.HasIndex(x => x.CreatedAt);
        }
    }
}