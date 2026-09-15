using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using RailwayBooking.Infrastructure.Entities.Booking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailwayBooking.Infrastructure.Persistence.Configurations.Booking
{

    public class BookingOrderItemConfiguration
        : IEntityTypeConfiguration<BookingOrderItem>
    {
        public void Configure(
            EntityTypeBuilder<BookingOrderItem> builder)
        {
            builder.ToTable("booking_order_items");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PassengerName)
                .HasMaxLength(200);

            builder.Property(x => x.PassengerType)
                .HasMaxLength(30);

            builder.Property(x => x.TicketPrice)
                .HasPrecision(12, 2);

            builder.HasOne(x => x.Order)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.OrderId);

            builder.HasOne(x => x.Inventory)
                .WithMany()
                .HasForeignKey(x => x.InventoryId);
        }
    }
}