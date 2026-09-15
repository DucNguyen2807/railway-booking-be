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

    public class TicketConfiguration
        : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("tickets");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TicketCode)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasConversion<int>();

            builder.HasIndex(x => x.TicketCode)
                .IsUnique();

            builder.HasIndex(x => x.Status);
        }
    }
}